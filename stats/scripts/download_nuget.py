#!/usr/bin/env python3
"""
Download NuGet package statistics.
Uses curl to fetch data from NuGet API endpoints.
"""

import subprocess
import json
import sys
import argparse
from pathlib import Path


def run_curl(url, output_file):
    """Execute curl command to download JSON data."""
    print(f"  Downloading: {url}")

    try:
        result = subprocess.run(
            ["curl", "-s", "-L", url, "-o", str(output_file)],
            capture_output=True,
            text=True,
            check=True
        )

        # Verify the file was created and contains valid JSON
        if output_file.exists():
            with open(output_file, 'r') as f:
                json.load(f)  # Validate JSON

            file_size = output_file.stat().st_size
            print(f"  ✓ Saved: {output_file.name} ({file_size:,} bytes)")
            return True
        else:
            print(f"  ✗ Failed: File not created")
            return False

    except subprocess.CalledProcessError as e:
        print(f"  ✗ Curl error: {e}")
        return False
    except json.JSONDecodeError as e:
        print(f"  ✗ Invalid JSON: {e}")
        return False
    except Exception as e:
        print(f"  ✗ Unexpected error: {e}")
        return False


def download_nuget_stats(package_id, output_dir):
    """Download all NuGet statistics."""
    print("=" * 70)
    print(f"NuGet Statistics Downloader - {package_id}")
    print("=" * 70)

    # Ensure sourcedata directory exists
    output_dir.mkdir(parents=True, exist_ok=True)
    print(f"\n📁 Output directory: {output_dir}")

    # NuGet API endpoints
    metadata_url = f"https://api.nuget.org/v3/registration5-semver1/{package_id.lower()}/index.json"
    downloads_url = f"https://azuresearch-usnc.nuget.org/query?q=packageid:{package_id}&prerelease=true"

    # Safe filename
    safe_name = package_id.lower().replace('.', '-')

    success_count = 0
    total_count = 2

    # Download package metadata (registration index)
    print("\n📦 Downloading package metadata...")
    metadata_file = output_dir / f"{safe_name}-stats.json"
    if run_curl(metadata_url, metadata_file):
        success_count += 1

    # Download download statistics
    print("\n📊 Downloading download statistics...")
    downloads_file = output_dir / f"{safe_name}-downloads.json"
    if run_curl(downloads_url, downloads_file):
        success_count += 1

    # Summary
    print("\n" + "=" * 70)
    if success_count == total_count:
        print(f"✓ Success! Downloaded {success_count}/{total_count} files")
        print(f"📁 Data saved to: {output_dir}")
        return 0
    else:
        print(f"⚠ Partial success: {success_count}/{total_count} files downloaded")
        return 1


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description='Download NuGet package statistics',
        formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument(
        'package_id',
        nargs='?',
        default='RestClient.Net',
        help='NuGet package ID (default: RestClient.Net)'
    )
    parser.add_argument(
        '--output-dir',
        type=Path,
        default=Path(__file__).parent.parent / "sourcedata",
        help='Output directory for JSON files'
    )

    args = parser.parse_args()

    try:
        exit_code = download_nuget_stats(args.package_id, args.output_dir)
        print("=" * 70)
        sys.exit(exit_code)
    except KeyboardInterrupt:
        print("\n\n⚠ Download interrupted by user")
        sys.exit(130)
    except Exception as e:
        print(f"\n✗ Fatal error: {e}")
        sys.exit(1)


if __name__ == '__main__':
    main()
