#!/usr/bin/env python3
"""
Collect comprehensive GitHub statistics for RestClient.Net repository.
"""

import json
import subprocess
import sys
from pathlib import Path
from datetime import datetime, timedelta

REPO = "MelbourneDeveloper/RestClient.Net"
STATS_DIR = Path(__file__).parent / "stats"

def run_gh_command(cmd, parse_json=True):
    """Run gh CLI command and return result."""
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
        print(f"Error running command: {cmd}")
        print(f"Error: {e.stderr}")
        return None
    except json.JSONDecodeError as e:
        print(f"Error parsing JSON from: {cmd}")
        print(f"Output: {result.stdout}")
        return None

def save_json(data, filename):
    """Save data to JSON file."""
    filepath = STATS_DIR / filename
    with open(filepath, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=2, ensure_ascii=False)
    print(f"✓ Saved: {filename}")
    return filepath

def collect_repo_info():
    """Get basic repository information."""
    print("\n📊 Collecting repository info...")
    data = run_gh_command(f'gh repo view {REPO} --json name,description,stargazerCount,forkCount,watchers,createdAt,updatedAt,pushedAt,isPrivate,isArchived,visibility,primaryLanguage,languages,licenseInfo,url,homepageUrl,defaultBranchRef,diskUsage,hasIssuesEnabled,hasWikiEnabled,issues')
    if data:
        save_json(data, 'repo_info.json')
    return data

def collect_issues():
    """Get issues statistics."""
    print("\n🐛 Collecting issues...")

    # All issues
    all_issues = run_gh_command(f'gh issue list --repo {REPO} --state all --limit 1000 --json number,title,state,createdAt,closedAt,author,labels,comments')
    if all_issues:
        save_json(all_issues, 'issues_all.json')

    # Open issues
    open_issues = run_gh_command(f'gh issue list --repo {REPO} --state open --limit 500 --json number,title,createdAt,author,labels,comments')
    if open_issues:
        save_json(open_issues, 'issues_open.json')

    # Closed issues
    closed_issues = run_gh_command(f'gh issue list --repo {REPO} --state closed --limit 500 --json number,title,createdAt,closedAt,author,labels')
    if closed_issues:
        save_json(closed_issues, 'issues_closed.json')

    return all_issues

def collect_pull_requests():
    """Get pull request statistics."""
    print("\n🔀 Collecting pull requests...")

    # All PRs
    all_prs = run_gh_command(f'gh pr list --repo {REPO} --state all --limit 1000 --json number,title,state,createdAt,closedAt,mergedAt,author,additions,deletions,changedFiles,reviews,labels')
    if all_prs:
        save_json(all_prs, 'pull_requests_all.json')

    # Merged PRs
    merged_prs = run_gh_command(f'gh pr list --repo {REPO} --state merged --limit 500 --json number,title,createdAt,mergedAt,author,additions,deletions,changedFiles')
    if merged_prs:
        save_json(merged_prs, 'pull_requests_merged.json')

    return all_prs

def collect_releases():
    """Get release information."""
    print("\n🚀 Collecting releases...")
    releases = run_gh_command(f'gh release list --repo {REPO} --limit 100 --json tagName,name,createdAt,publishedAt,isPrerelease,isDraft')
    if releases:
        save_json(releases, 'releases.json')
    return releases

def collect_tags():
    """Get tags."""
    print("\n🏷️  Collecting tags...")
    tags = run_gh_command(f'gh api repos/{REPO}/tags --paginate')
    if tags:
        save_json(tags, 'tags.json')
    return tags

def collect_contributors():
    """Get contributor statistics."""
    print("\n👥 Collecting contributors...")
    contributors = run_gh_command(f'gh api repos/{REPO}/contributors --paginate')
    if contributors:
        save_json(contributors, 'contributors.json')
    return contributors

def collect_commits():
    """Get recent commit history via git log."""
    print("\n📝 Collecting commits from git log...")
    try:
        # Use git log to get commit data
        cmd = 'git log --all --pretty=format:\'{"hash":"%H","author":"%an","email":"%ae","date":"%ai","message":"%s"}\' --date=iso -n 1000'
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True, check=True)

        # Parse each line as JSON
        commits = []
        for line in result.stdout.strip().split('\n'):
            if line:
                try:
                    commits.append(json.loads(line))
                except:
                    pass

        if commits:
            save_json(commits, 'commits.json')
        return commits
    except Exception as e:
        print(f"Error collecting commits: {e}")
        return None

def collect_languages():
    """Get language statistics."""
    print("\n💻 Collecting language statistics...")
    languages = run_gh_command(f'gh api repos/{REPO}/languages')
    if languages:
        save_json(languages, 'languages.json')
    return languages

def collect_traffic():
    """Get traffic statistics (requires repo owner permissions)."""
    print("\n📈 Collecting traffic stats (may require owner permissions)...")

    # Views
    views = run_gh_command(f'gh api repos/{REPO}/traffic/views')
    if views:
        save_json(views, 'traffic_views.json')

    # Clones
    clones = run_gh_command(f'gh api repos/{REPO}/traffic/clones')
    if clones:
        save_json(clones, 'traffic_clones.json')

    # Popular paths
    paths = run_gh_command(f'gh api repos/{REPO}/traffic/popular/paths')
    if paths:
        save_json(paths, 'traffic_paths.json')

    # Referrers
    referrers = run_gh_command(f'gh api repos/{REPO}/traffic/popular/referrers')
    if referrers:
        save_json(referrers, 'traffic_referrers.json')

def collect_code_frequency():
    """Get code frequency statistics."""
    print("\n📊 Collecting code frequency...")
    frequency = run_gh_command(f'gh api repos/{REPO}/stats/code_frequency')
    if frequency:
        save_json(frequency, 'code_frequency.json')
    return frequency

def collect_commit_activity():
    """Get commit activity statistics."""
    print("\n📅 Collecting commit activity...")
    activity = run_gh_command(f'gh api repos/{REPO}/stats/commit_activity')
    if activity:
        save_json(activity, 'commit_activity.json')
    return activity

def collect_participation():
    """Get participation statistics."""
    print("\n📈 Collecting participation stats...")
    participation = run_gh_command(f'gh api repos/{REPO}/stats/participation')
    if participation:
        save_json(participation, 'participation.json')
    return participation

def collect_stargazers():
    """Get stargazer data (basic info from repo info)."""
    print("\n⭐ Collecting stargazer count (from repo info)...")
    # Stargazers endpoint requires auth, so we'll rely on repo_info data
    # Just log that this info is in repo_info.json
    print("  (Detailed stargazer timeline available in repo_info.json)")
    return None

def collect_forks():
    """Get fork information."""
    print("\n🍴 Collecting forks...")
    forks = run_gh_command(f'gh api repos/{REPO}/forks --paginate -F per_page=100')
    if forks:
        save_json(forks, 'forks.json')
    return forks

def collect_workflows():
    """Get GitHub Actions workflows."""
    print("\n⚙️  Collecting workflows...")
    workflows = run_gh_command(f'gh api repos/{REPO}/actions/workflows')
    if workflows:
        save_json(workflows, 'workflows.json')
    return workflows

def collect_workflow_runs():
    """Get recent workflow runs."""
    print("\n🏃 Collecting workflow runs...")
    runs = run_gh_command(f'gh run list --repo {REPO} --limit 100 --json databaseId,name,headBranch,event,status,conclusion,createdAt,updatedAt')
    if runs:
        save_json(runs, 'workflow_runs.json')
    return runs

def generate_summary():
    """Generate a summary of collected statistics."""
    print("\n📋 Generating summary...")

    summary = {
        'collected_at': datetime.now().isoformat(),
        'repository': REPO,
        'files_collected': []
    }

    # List all collected files
    for file in sorted(STATS_DIR.glob('*.json')):
        if file.name != 'summary.json':
            summary['files_collected'].append(file.name)

    save_json(summary, 'summary.json')
    return summary

def main():
    """Main collection process."""
    print("=" * 70)
    print(f"GitHub Statistics Collector for {REPO}")
    print("=" * 70)

    # Create stats directory
    STATS_DIR.mkdir(exist_ok=True)
    print(f"\n📁 Stats directory: {STATS_DIR}")

    # Collect all statistics
    collectors = [
        collect_repo_info,
        collect_issues,
        collect_pull_requests,
        collect_releases,
        collect_tags,
        collect_contributors,
        collect_commits,
        collect_languages,
        collect_traffic,
        collect_code_frequency,
        collect_commit_activity,
        collect_participation,
        collect_stargazers,
        collect_forks,
        collect_workflows,
        collect_workflow_runs,
    ]

    for collector in collectors:
        try:
            collector()
        except Exception as e:
            print(f"⚠ Error in {collector.__name__}: {e}")
            continue

    # Generate summary
    summary = generate_summary()

    print("\n" + "=" * 70)
    print("✓ Collection complete!")
    print(f"\n📊 Collected {len(summary['files_collected'])} data files")
    print(f"📁 Location: {STATS_DIR}")
    print("\nCollected files:")
    for filename in summary['files_collected']:
        filepath = STATS_DIR / filename
        size = filepath.stat().st_size
        print(f"  - {filename} ({size:,} bytes)")
    print("=" * 70)

if __name__ == '__main__':
    main()
