#!/usr/bin/env python3
"""
Master orchestrator for collecting package statistics.
Runs NuGet and GitHub downloaders in parallel for maximum efficiency.
"""

import subprocess
import sys
import argparse
from pathlib import Path
from concurrent.futures import ThreadPoolExecutor, as_completed
from datetime import datetime
import time

# Script paths
SCRIPTS_DIR = Path(__file__).parent
NUGET_SCRIPT = SCRIPTS_DIR / "download_nuget.py"
GITHUB_SCRIPT = SCRIPTS_DIR / "download_github.py"


def run_script(script_path, args_list, script_name):
    """Run a Python script and capture its output."""
    start_time = time.time()

    try:
        print(f"\n{'='*70}")
        print(f"Starting: {script_name}")
        print(f"{'='*70}")

        cmd = [sys.executable, str(script_path)] + args_list
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            check=True
        )

        elapsed = time.time() - start_time

        # Print the script's output
        if result.stdout:
            print(result.stdout)

        return {
            'name': script_name,
            'success': True,
            'exit_code': 0,
            'elapsed': elapsed,
            'error': None
        }

    except subprocess.CalledProcessError as e:
        elapsed = time.time() - start_time

        # Print output even on failure
        if e.stdout:
            print(e.stdout)
        if e.stderr:
            print(e.stderr)

        return {
            'name': script_name,
            'success': False,
            'exit_code': e.returncode,
            'elapsed': elapsed,
            'error': str(e)
        }

    except Exception as e:
        elapsed = time.time() - start_time

        return {
            'name': script_name,
            'success': False,
            'exit_code': 1,
            'elapsed': elapsed,
            'error': str(e)
        }


def collect_all_stats(package_id, github_repo, output_dir):
    """Run all data collection scripts in parallel."""
    print("=" * 70)
    print(f"Statistics Collection - {package_id}")
    print("=" * 70)
    print(f"\nPackage: {package_id}")
    print(f"GitHub: {github_repo}")
    print(f"Started: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print(f"\n📁 Output directory: {output_dir}")

    # Verify scripts exist
    if not NUGET_SCRIPT.exists():
        print(f"\n✗ Error: NuGet script not found: {NUGET_SCRIPT}")
        return 1

    if not GITHUB_SCRIPT.exists():
        print(f"\n✗ Error: GitHub script not found: {GITHUB_SCRIPT}")
        return 1

    # Define collection tasks
    prefix = package_id.lower().replace('.', '-')
    tasks = [
        (NUGET_SCRIPT, [package_id, '--output-dir', str(output_dir)], f"NuGet ({package_id})"),
        (GITHUB_SCRIPT, [github_repo, '--output-dir', str(output_dir), '--prefix', prefix], f"GitHub ({github_repo})"),
    ]

    print(f"\n🚀 Running {len(tasks)} collection tasks in parallel...")

    # Execute tasks in parallel
    results = []
    with ThreadPoolExecutor(max_workers=2) as executor:
        # Submit all tasks
        future_to_task = {
            executor.submit(run_script, script, args, name): (script, name)
            for script, args, name in tasks
        }

        # Process as they complete
        for future in as_completed(future_to_task):
            result = future.result()
            results.append(result)

    # Summary
    print("\n" + "=" * 70)
    print("Collection Summary")
    print("=" * 70)

    success_count = sum(1 for r in results if r['success'])
    total_count = len(results)

    for result in sorted(results, key=lambda x: x['name']):
        status = "✓ SUCCESS" if result['success'] else "✗ FAILED"
        elapsed = f"{result['elapsed']:.1f}s"
        print(f"{status:12} | {result['name']:30} | {elapsed:>8}")

        if not result['success'] and result['error']:
            print(f"             | Error: {result['error'][:50]}")

    print("=" * 70)

    if success_count == total_count:
        print(f"\n✓ All tasks completed successfully ({success_count}/{total_count})")
        return 0
    else:
        print(f"\n⚠ Partial success: {success_count}/{total_count} tasks completed")
        return 1


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description='Collect NuGet and GitHub statistics',
        formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument(
        'package_id',
        nargs='?',
        default='RestClient.Net',
        help='NuGet package ID (default: RestClient.Net)'
    )
    parser.add_argument(
        '--github-repo',
        default=None,
        help='GitHub repository (owner/repo). Default: derived from package name'
    )
    parser.add_argument(
        '--output-dir',
        type=Path,
        default=Path(__file__).parent.parent / "sourcedata",
        help='Output directory for data files'
    )

    args = parser.parse_args()

    # Default GitHub repo based on package
    github_mappings = {
        'RestClient.Net': 'MelbourneDeveloper/RestClient.Net',
        'Newtonsoft.Json': 'JamesNK/Newtonsoft.Json',
        'Serilog': 'serilog/serilog',
        'Dapper': 'DapperLib/Dapper',
        'Polly': 'App-vNext/Polly',
    }

    if not args.github_repo:
        args.github_repo = github_mappings.get(args.package_id)
        if not args.github_repo:
            print(f"✗ No GitHub repo mapping for {args.package_id}")
            print(f"  Please specify with --github-repo")
            sys.exit(1)

    try:
        start_time = time.time()
        exit_code = collect_all_stats(args.package_id, args.github_repo, args.output_dir)
        total_time = time.time() - start_time

        print(f"\nTotal execution time: {total_time:.1f}s")
        print(f"\n📊 Data collected in: {args.output_dir}")
        print(f"🎨 Next: python3 scripts/generate_report.py {args.package_id}")
        print("=" * 70)

        sys.exit(exit_code)

    except KeyboardInterrupt:
        print("\n\n⚠ Collection interrupted by user")
        sys.exit(130)

    except Exception as e:
        print(f"\n✗ Fatal error: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)


if __name__ == '__main__':
    main()
