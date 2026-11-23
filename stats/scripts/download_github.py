#!/usr/bin/env python3
"""
Download GitHub repository statistics.
Uses GitHub CLI (gh) and git commands to collect comprehensive metrics.
"""

import json
import subprocess
import sys
import argparse
from pathlib import Path


def run_gh_command(cmd, parse_json=True):
    """Execute gh CLI command and return result."""
    try:
        result = subprocess.run(
            cmd,
            shell=True,
            capture_output=True,
            text=True,
            check=True
        )
        if parse_json and result.stdout.strip():
            return json.loads(result.stdout)
        return result.stdout.strip()
    except subprocess.CalledProcessError as e:
        print(f"  ✗ Command failed: {e.stderr[:100]}")
        return None
    except json.JSONDecodeError as e:
        print(f"  ✗ Invalid JSON: {e}")
        return None


def save_json(data, filename, output_dir):
    """Save data to JSON file."""
    if data is None:
        return None

    filepath = output_dir / filename
    with open(filepath, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=2, ensure_ascii=False)

    file_size = filepath.stat().st_size
    print(f"  ✓ Saved: {filename} ({file_size:,} bytes)")
    return filepath


class GitHubCollector:
    """Collects GitHub repository statistics."""

    def __init__(self, repo, output_dir, prefix):
        self.repo = repo
        self.output_dir = output_dir
        self.prefix = prefix

    def save(self, data, suffix):
        """Save data with prefix."""
        return save_json(data, f'{self.prefix}-{suffix}', self.output_dir)

    def collect_repo_info(self):
        """Get basic repository information."""
        print("\n📊 Collecting repository info...")
        data = run_gh_command(
            f'gh repo view {self.repo} --json name,description,stargazerCount,forkCount,'
            f'watchers,createdAt,updatedAt,pushedAt,isPrivate,isArchived,visibility,'
            f'primaryLanguage,languages,licenseInfo,url,homepageUrl,defaultBranchRef,'
            f'diskUsage,hasIssuesEnabled,hasWikiEnabled,issues'
        )
        if data:
            self.save(data, 'repo_info.json')
        return data

    def collect_issues(self):
        """Get issues statistics."""
        print("\n🐛 Collecting issues...")

        all_issues = run_gh_command(
            f'gh issue list --repo {self.repo} --state all --limit 1000 '
            f'--json number,title,state,createdAt,closedAt,author,labels,comments'
        )
        if all_issues:
            self.save(all_issues, 'issues_all.json')

        open_issues = run_gh_command(
            f'gh issue list --repo {self.repo} --state open --limit 500 '
            f'--json number,title,createdAt,author,labels,comments'
        )
        if open_issues:
            self.save(open_issues, 'issues_open.json')

        closed_issues = run_gh_command(
            f'gh issue list --repo {self.repo} --state closed --limit 500 '
            f'--json number,title,createdAt,closedAt,author,labels'
        )
        if closed_issues:
            self.save(closed_issues, 'issues_closed.json')

        return all_issues

    def collect_pull_requests(self):
        """Get pull request statistics."""
        print("\n🔀 Collecting pull requests...")

        all_prs = run_gh_command(
            f'gh pr list --repo {self.repo} --state all --limit 1000 '
            f'--json number,title,state,createdAt,closedAt,mergedAt,author,'
            f'additions,deletions,changedFiles,reviews,labels'
        )
        if all_prs:
            self.save(all_prs, 'pull_requests_all.json')

        merged_prs = run_gh_command(
            f'gh pr list --repo {self.repo} --state merged --limit 500 '
            f'--json number,title,createdAt,mergedAt,author,additions,deletions,changedFiles'
        )
        if merged_prs:
            self.save(merged_prs, 'pull_requests_merged.json')

        return all_prs

    def collect_releases(self):
        """Get release information."""
        print("\n🚀 Collecting releases...")
        releases = run_gh_command(
            f'gh release list --repo {self.repo} --limit 100 '
            f'--json tagName,name,createdAt,publishedAt,isPrerelease,isDraft'
        )
        if releases:
            self.save(releases, 'releases.json')
        return releases

    def collect_tags(self):
        """Get tags."""
        print("\n🏷️  Collecting tags...")
        tags = run_gh_command(f'gh api repos/{self.repo}/tags --paginate')
        if tags:
            self.save(tags, 'tags.json')
        return tags

    def collect_contributors(self):
        """Get contributor statistics."""
        print("\n👥 Collecting contributors...")
        contributors = run_gh_command(f'gh api repos/{self.repo}/contributors --paginate')
        if contributors:
            self.save(contributors, 'contributors.json')
        return contributors

    def collect_commits(self):
        """Get recent commit history via git log."""
        print("\n📝 Collecting commits from git log...")
        try:
            cmd = (
                'git log --all --pretty=format:\''
                '{"hash":"%H","author":"%an","email":"%ae","date":"%ai","message":"%s"}\' '
                '--date=iso -n 1000'
            )
            result = subprocess.run(cmd, shell=True, capture_output=True, text=True, check=True)

            commits = []
            for line in result.stdout.strip().split('\n'):
                if line:
                    try:
                        commits.append(json.loads(line))
                    except:
                        pass

            if commits:
                self.save(commits, 'commits.json')
            return commits
        except Exception as e:
            print(f"  ✗ Error: {e}")
            return None

    def collect_languages(self):
        """Get language statistics."""
        print("\n💻 Collecting language statistics...")
        languages = run_gh_command(f'gh api repos/{self.repo}/languages')
        if languages:
            self.save(languages, 'languages.json')
        return languages

    def collect_traffic(self):
        """Get traffic statistics (requires repo owner permissions)."""
        print("\n📈 Collecting traffic stats...")

        views = run_gh_command(f'gh api repos/{self.repo}/traffic/views')
        if views:
            self.save(views, 'traffic_views.json')

        clones = run_gh_command(f'gh api repos/{self.repo}/traffic/clones')
        if clones:
            self.save(clones, 'traffic_clones.json')

        paths = run_gh_command(f'gh api repos/{self.repo}/traffic/popular/paths')
        if paths:
            self.save(paths, 'traffic_paths.json')

        referrers = run_gh_command(f'gh api repos/{self.repo}/traffic/popular/referrers')
        if referrers:
            self.save(referrers, 'traffic_referrers.json')

    def collect_participation(self):
        """Get participation statistics."""
        print("\n📅 Collecting participation stats...")
        participation = run_gh_command(f'gh api repos/{self.repo}/stats/participation')
        if participation:
            self.save(participation, 'participation.json')
        return participation

    def collect_forks(self):
        """Get fork information."""
        print("\n🍴 Collecting forks...")
        forks = run_gh_command(f'gh api repos/{self.repo}/forks --paginate -F per_page=100')
        if forks:
            self.save(forks, 'forks.json')
        return forks

    def collect_workflows(self):
        """Get GitHub Actions workflows."""
        print("\n⚙️  Collecting workflows...")
        workflows = run_gh_command(f'gh api repos/{self.repo}/actions/workflows')
        if workflows:
            self.save(workflows, 'workflows.json')
        return workflows

    def collect_workflow_runs(self):
        """Get recent workflow runs."""
        print("\n🏃 Collecting workflow runs...")
        runs = run_gh_command(
            f'gh run list --repo {self.repo} --limit 100 '
            f'--json databaseId,name,headBranch,event,status,conclusion,createdAt,updatedAt'
        )
        if runs:
            self.save(runs, 'workflow_runs.json')
        return runs

    def collect_all(self):
        """Run all collectors."""
        collectors = [
            self.collect_repo_info,
            self.collect_issues,
            self.collect_pull_requests,
            self.collect_releases,
            self.collect_tags,
            self.collect_contributors,
            self.collect_commits,
            self.collect_languages,
            self.collect_traffic,
            self.collect_participation,
            self.collect_forks,
            self.collect_workflows,
            self.collect_workflow_runs,
        ]

        for collector in collectors:
            try:
                collector()
            except Exception as e:
                print(f"  ⚠ Error in {collector.__name__}: {e}")
                continue


def download_github_stats(repo, output_dir, prefix):
    """Main collection function."""
    print("=" * 70)
    print(f"GitHub Statistics Downloader - {repo}")
    print("=" * 70)

    # Ensure output directory exists
    output_dir.mkdir(parents=True, exist_ok=True)
    print(f"\n📁 Output directory: {output_dir}")

    # Collect data
    collector = GitHubCollector(repo, output_dir, prefix)
    collector.collect_all()

    # Count collected files
    collected_files = [f.name for f in output_dir.glob(f'{prefix}-*.json')]

    # Generate summary
    from datetime import datetime
    summary = {
        'collected_at': datetime.now().isoformat(),
        'repository': repo,
        'files_collected': sorted(collected_files),
        'total_files': len(collected_files)
    }
    save_json(summary, f'{prefix}-summary.json', output_dir)

    # Final summary
    print("\n" + "=" * 70)
    print(f"✓ Downloaded {len(collected_files)} GitHub data files")
    print(f"📁 Data saved to: {output_dir}")
    print("=" * 70)

    return 0


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description='Download GitHub repository statistics',
        formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument(
        'repo',
        nargs='?',
        default='MelbourneDeveloper/RestClient.Net',
        help='GitHub repository (owner/repo) (default: MelbourneDeveloper/RestClient.Net)'
    )
    parser.add_argument(
        '--output-dir',
        type=Path,
        default=Path(__file__).parent.parent / "sourcedata",
        help='Output directory for JSON files'
    )
    parser.add_argument(
        '--prefix',
        default=None,
        help='Filename prefix (default: derived from repo name)'
    )

    args = parser.parse_args()

    # Default prefix from repo name
    if not args.prefix:
        args.prefix = args.repo.split('/')[-1].lower().replace('.', '-')

    try:
        exit_code = download_github_stats(args.repo, args.output_dir, args.prefix)
        sys.exit(exit_code)
    except KeyboardInterrupt:
        print("\n\n⚠ Download interrupted by user")
        sys.exit(130)
    except Exception as e:
        print(f"\n✗ Fatal error: {e}")
        sys.exit(1)


if __name__ == '__main__':
    main()
