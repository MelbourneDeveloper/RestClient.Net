#!/usr/bin/env python3
"""
Generate comprehensive HTML statistics report for RestClient.Net.
Combines NuGet and GitHub data with Material Design styling.
"""

import json
import sys
from pathlib import Path
from datetime import datetime
from collections import defaultdict, Counter

# Paths
SCRIPTS_DIR = Path(__file__).parent


def load_json(filename, sourcedata_dir):
    """Load JSON file from sourcedata directory."""
    filepath = sourcedata_dir / filename
    if not filepath.exists():
        print(f"  ⚠ Warning: {filename} not found")
        return None

    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            return json.load(f)
    except Exception as e:
        print(f"  ✗ Error loading {filename}: {e}")
        return None


def format_number(n):
    """Format number with K/M suffix."""
    if n >= 1_000_000:
        return f"{n/1_000_000:.1f}M"
    elif n >= 1_000:
        return f"{n/1_000:.1f}K"
    return str(n)


def extract_nuget_data(prefix, sourcedata_dir):
    """Extract and process NuGet statistics."""
    print("\n📦 Processing NuGet data...")

    # Load package metadata
    metadata = load_json(f"{prefix}-stats.json", sourcedata_dir)
    downloads_data = load_json(f"{prefix}-downloads.json", sourcedata_dir)

    if not metadata:
        print("  ✗ No NuGet metadata found")
        return None

    # Extract versions
    versions = []
    for page in metadata.get('items', []):
        for item in page.get('items', []):
            catalog = item.get('catalogEntry', {})
            version = catalog.get('version', 'Unknown')
            published = catalog.get('published', '')
            listed = catalog.get('listed', False)

            pub_date = None
            if published and published != '1900-01-01T00:00:00+00:00':
                try:
                    pub_date = datetime.fromisoformat(published.replace('+00:00', ''))
                except:
                    pass

            versions.append({
                'version': version,
                'published': pub_date,
                'published_str': pub_date.strftime('%Y-%m-%d') if pub_date else 'Unknown',
                'listed': listed,
            })

    versions.sort(key=lambda x: x['published'] if x['published'] else datetime.min)

    # Get download stats
    total_downloads = 0
    version_dl_map = {}

    if downloads_data and downloads_data.get('data'):
        package_data = downloads_data['data'][0]
        total_downloads = package_data.get('totalDownloads', 0)
        version_downloads = package_data.get('versions', [])
        version_dl_map = {v.get('version', ''): v.get('downloads', 0) for v in version_downloads}

    # Add downloads to versions
    for v in versions:
        v['downloads'] = version_dl_map.get(v['version'], 0)

    # Statistics
    downloads_by_month = defaultdict(int)
    versions_by_year = defaultdict(int)

    for v in versions:
        if v['published']:
            year = v['published'].year
            month_key = v['published'].strftime('%Y-%m')
            downloads_by_month[month_key] += v['downloads']
            versions_by_year[year] += 1

    print(f"  ✓ Processed {len(versions)} versions, {total_downloads:,} total downloads")

    return {
        'versions': versions,
        'total_downloads': total_downloads,
        'downloads_by_month': downloads_by_month,
        'versions_by_year': versions_by_year,
        'listed_count': sum(1 for v in versions if v['listed']),
        'max_downloads': max(v['downloads'] for v in versions) if versions else 0,
    }


def extract_github_data(prefix, sourcedata_dir):
    """Extract and process GitHub statistics."""
    print("\n🐙 Processing GitHub data...")

    # Load GitHub files
    repo_info = load_json(f"{prefix}-repo_info.json", sourcedata_dir)
    contributors = load_json(f"{prefix}-contributors.json", sourcedata_dir) or []
    commits = load_json(f"{prefix}-commits.json", sourcedata_dir) or []
    issues_all = load_json(f"{prefix}-issues_all.json", sourcedata_dir) or []
    prs_all = load_json(f"{prefix}-pull_requests_all.json", sourcedata_dir) or []
    traffic_views = load_json(f"{prefix}-traffic_views.json", sourcedata_dir) or {}
    traffic_clones = load_json(f"{prefix}-traffic_clones.json", sourcedata_dir) or {}
    forks = load_json(f"{prefix}-forks.json", sourcedata_dir) or []
    releases = load_json(f"{prefix}-releases.json", sourcedata_dir) or []

    if not repo_info:
        print("  ✗ No GitHub repo info found")
        return None

    # Extract key metrics
    stars = repo_info.get('stargazerCount', 0)
    fork_count = repo_info.get('forkCount', 0)
    watchers = repo_info.get('watchers', {}).get('totalCount', 0)
    created_at = repo_info.get('createdAt', '')

    # Traffic stats
    total_views = traffic_views.get('count', 0)
    unique_visitors = traffic_views.get('uniques', 0)
    total_clones = traffic_clones.get('count', 0)

    # Commit activity by month
    commits_by_month = defaultdict(int)
    for commit in commits:
        try:
            date = datetime.fromisoformat(commit['date'].replace('+00:00', ''))
            month_key = date.strftime('%Y-%m')
            commits_by_month[month_key] += 1
        except:
            pass

    # Top contributors
    top_contributors = sorted(contributors, key=lambda x: x.get('contributions', 0), reverse=True)[:10]

    # Issue stats
    open_issues = sum(1 for i in issues_all if i.get('state') == 'open')
    closed_issues = sum(1 for i in issues_all if i.get('state') == 'closed')

    # PR stats
    merged_prs = sum(1 for pr in prs_all if pr.get('mergedAt'))

    print(f"  ✓ {stars} stars, {fork_count} forks, {len(commits)} commits")

    return {
        'stars': stars,
        'forks': fork_count,
        'watchers': watchers,
        'created_at': created_at,
        'contributors_count': len(contributors),
        'top_contributors': top_contributors,
        'commits_count': len(commits),
        'commits_by_month': commits_by_month,
        'total_views': total_views,
        'unique_visitors': unique_visitors,
        'total_clones': total_clones,
        'issues_total': len(issues_all),
        'issues_open': open_issues,
        'issues_closed': closed_issues,
        'prs_total': len(prs_all),
        'prs_merged': merged_prs,
        'releases_count': len(releases),
    }


def generate_html(nuget_data, github_data):
    """Generate HTML report."""
    print("\n📝 Generating HTML report...")

    # Prepare chart data
    month_labels = sorted(nuget_data['downloads_by_month'].keys())
    month_downloads = [nuget_data['downloads_by_month'][m] for m in month_labels]

    year_labels = sorted(nuget_data['versions_by_year'].keys())
    year_versions = [nuget_data['versions_by_year'][y] for y in year_labels]

    commit_months = sorted(github_data['commits_by_month'].keys())
    commit_counts = [github_data['commits_by_month'][m] for m in commit_months]

    contributor_names = [c.get('login', 'Unknown') for c in github_data['top_contributors']]
    contributor_commits = [c.get('contributions', 0) for c in github_data['top_contributors']]

    # Generate HTML
    html = f'''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>RestClient.Net - Statistics Report</title>
    <link rel="stylesheet" href="styles.css">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</head>
<body>
    <div class="container">
        <!-- HEADER -->
        <header>
            <h1>RestClient.Net</h1>
            <p>The safest way to make REST calls in C#. Functional HTTP client library with Result types, exhaustiveness checking, and zero exceptions.</p>
            <div class="hero-stat">
                <h2>{format_number(nuget_data['total_downloads'])}</h2>
                <p>Total Downloads</p>
            </div>
        </header>

        <!-- KEY METRICS -->
        <section class="gray-bg">
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number">{format_number(nuget_data['total_downloads'])}</div>
                    <div class="stat-label">NuGet Downloads</div>
                    <div class="stat-sublabel">All-time</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['stars']}</div>
                    <div class="stat-label">GitHub Stars</div>
                    <div class="stat-sublabel">Community love</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{len(nuget_data['versions'])}</div>
                    <div class="stat-label">Total Versions</div>
                    <div class="stat-sublabel">{nuget_data['listed_count']} listed</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['contributors_count']}</div>
                    <div class="stat-label">Contributors</div>
                    <div class="stat-sublabel">{github_data['commits_count']} commits</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['forks']}</div>
                    <div class="stat-label">Repository Forks</div>
                    <div class="stat-sublabel">{github_data['watchers']} watchers</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{format_number(github_data['unique_visitors'])}</div>
                    <div class="stat-label">Unique Visitors</div>
                    <div class="stat-sublabel">Last 14 days</div>
                </div>
            </div>
        </section>

        <!-- POPULARITY & GROWTH -->
        <section>
            <h2>📈 Popularity & Growth</h2>

            <div class="chart-container">
                <h3>NuGet Downloads by Month</h3>
                <div class="chart-wrapper">
                    <canvas id="downloadsChart"></canvas>
                </div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number">{format_number(github_data['total_views'])}</div>
                    <div class="stat-label">Repository Views</div>
                    <div class="stat-sublabel">Last 14 days</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{format_number(github_data['total_clones'])}</div>
                    <div class="stat-label">Repository Clones</div>
                    <div class="stat-sublabel">Last 14 days</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{format_number(nuget_data['max_downloads'])}</div>
                    <div class="stat-label">Most Downloaded</div>
                    <div class="stat-sublabel">Single version</div>
                </div>
            </div>
        </section>

        <!-- RELEASE ACTIVITY -->
        <section class="gray-bg">
            <h2>🚀 Release Activity</h2>

            <div class="chart-container">
                <h3>Versions Released per Year</h3>
                <div class="chart-wrapper">
                    <canvas id="versionsChart"></canvas>
                </div>
            </div>

            <div class="chart-container">
                <h3>Commit Activity by Month</h3>
                <div class="chart-wrapper">
                    <canvas id="commitsChart"></canvas>
                </div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number">{github_data['releases_count']}</div>
                    <div class="stat-label">GitHub Releases</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['prs_merged']}</div>
                    <div class="stat-label">Merged PRs</div>
                    <div class="stat-sublabel">{github_data['prs_total']} total</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['issues_open']}</div>
                    <div class="stat-label">Open Issues</div>
                    <div class="stat-sublabel">{github_data['issues_total']} total</div>
                </div>
            </div>
        </section>

        <!-- COMMUNITY ENGAGEMENT -->
        <section>
            <h2>👥 Community Engagement</h2>

            <div class="chart-container">
                <h3>Contributors</h3>
                <div class="chart-wrapper">
                    <canvas id="contributorsChart"></canvas>
                </div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number">{github_data['stars']}</div>
                    <div class="stat-label">⭐ Stars</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['forks']}</div>
                    <div class="stat-label">🍴 Forks</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['watchers']}</div>
                    <div class="stat-label">👁 Watchers</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">{github_data['contributors_count']}</div>
                    <div class="stat-label">👨‍💻 Contributors</div>
                </div>
            </div>
        </section>

        <!-- VERSION HISTORY -->
        <section class="gray-bg">
            <h2>📋 Complete Version History</h2>
            <div style="max-height: 600px; overflow-y: auto; border-radius: 4px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
                <table>
                    <thead style="position: sticky; top: 0; z-index: 10;">
                        <tr>
                            <th>Version</th>
                            <th>Published Date</th>
                            <th>Downloads</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
'''

    # Add version rows (latest first)
    for version in reversed(nuget_data['versions']):
        status_class = 'badge-success' if version['listed'] else 'badge-warning'
        status_text = 'Listed' if version['listed'] else 'Unlisted'

        html += f'''
                    <tr>
                        <td><strong>{version['version']}</strong></td>
                        <td>{version['published_str']}</td>
                        <td><span class="badge badge-primary">{format_number(version['downloads'])}</span></td>
                        <td><span class="badge {status_class}">{status_text}</span></td>
                    </tr>
'''

    html += f'''
                </tbody>
            </table>
            </div>
        </section>

        <!-- FOOTER -->
        <footer>
            <p>Generated on {datetime.now().strftime('%B %d, %Y at %I:%M %p')}</p>
            <p>Data sources: <a href="https://www.nuget.org/packages/RestClient.Net">NuGet.org</a> ·
               <a href="https://github.com/MelbourneDeveloper/RestClient.Net">GitHub</a></p>
        </footer>
    </div>

    <script>
        // Chart.js Material Design Configuration
        Chart.defaults.font.family = 'Roboto, sans-serif';
        Chart.defaults.color = '#616161';

        const primaryColor = '#1976D2';
        const accentColor = '#FFC107';
        const grayColor = '#9E9E9E';

        // Downloads Chart
        new Chart(document.getElementById('downloadsChart'), {{
            type: 'bar',
            data: {{
                labels: {json.dumps(month_labels)},
                datasets: [{{
                    label: 'Downloads',
                    data: {json.dumps(month_downloads)},
                    backgroundColor: primaryColor,
                    borderRadius: 4
                }}]
            }},
            options: {{
                responsive: true,
                maintainAspectRatio: false,
                plugins: {{ legend: {{ display: false }} }},
                scales: {{
                    y: {{ beginAtZero: true }}
                }}
            }}
        }});

        // Versions Chart
        new Chart(document.getElementById('versionsChart'), {{
            type: 'bar',
            data: {{
                labels: {json.dumps(year_labels)},
                datasets: [{{
                    label: 'Versions Released',
                    data: {json.dumps(year_versions)},
                    backgroundColor: accentColor,
                    borderRadius: 4
                }}]
            }},
            options: {{
                responsive: true,
                maintainAspectRatio: false,
                plugins: {{ legend: {{ display: false }} }},
                scales: {{
                    y: {{
                        beginAtZero: true,
                        ticks: {{ stepSize: 1 }}
                    }}
                }}
            }}
        }});

        // Commits Chart
        new Chart(document.getElementById('commitsChart'), {{
            type: 'line',
            data: {{
                labels: {json.dumps(commit_months)},
                datasets: [{{
                    label: 'Commits',
                    data: {json.dumps(commit_counts)},
                    borderColor: primaryColor,
                    backgroundColor: 'rgba(25, 118, 210, 0.1)',
                    fill: true,
                    tension: 0.4
                }}]
            }},
            options: {{
                responsive: true,
                maintainAspectRatio: false,
                plugins: {{ legend: {{ display: false }} }},
                scales: {{
                    y: {{ beginAtZero: true }}
                }}
            }}
        }});

        // Contributors Chart (Pie)
        new Chart(document.getElementById('contributorsChart'), {{
            type: 'pie',
            data: {{
                labels: {json.dumps(contributor_names)},
                datasets: [{{
                    label: 'Commits',
                    data: {json.dumps(contributor_commits)},
                    backgroundColor: [
                        'rgba(25, 118, 210, 0.8)',
                        'rgba(255, 193, 7, 0.8)',
                        'rgba(76, 175, 80, 0.8)',
                        'rgba(244, 67, 54, 0.8)',
                        'rgba(156, 39, 176, 0.8)',
                        'rgba(255, 152, 0, 0.8)',
                        'rgba(0, 188, 212, 0.8)',
                        'rgba(233, 30, 99, 0.8)',
                        'rgba(63, 81, 181, 0.8)',
                        'rgba(139, 195, 74, 0.8)'
                    ],
                    borderColor: '#fff',
                    borderWidth: 2
                }}]
            }},
            options: {{
                responsive: true,
                maintainAspectRatio: false,
                plugins: {{
                    legend: {{
                        display: true,
                        position: 'right'
                    }}
                }}
            }}
        }});
    </script>
</body>
</html>
'''

    return html


def generate_report(package_id, sourcedata_dir, output_dir):
    """Main report generation function."""
    print("=" * 70)
    print(f"{package_id} - Statistics Report Generator")
    print("=" * 70)

    # Ensure output directory exists
    output_dir.mkdir(parents=True, exist_ok=True)

    # Generate prefix
    prefix = package_id.lower().replace('.', '-')

    # Load and process data
    nuget_data = extract_nuget_data(prefix, sourcedata_dir)
    github_data = extract_github_data(prefix, sourcedata_dir)

    if not nuget_data or not github_data:
        print("\n✗ Error: Missing required data files")
        return 1

    # Generate HTML
    html = generate_html(nuget_data, github_data)

    # Write report
    output_file = output_dir / f"{prefix}-report.html"
    print(f"\n📄 Writing report to: {output_file}")
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(html)

    file_size = output_file.stat().st_size

    # Summary
    print("\n" + "=" * 70)
    print("✓ Report generated successfully!")
    print(f"\n📊 Report: {output_file} ({file_size:,} bytes)")
    print(f"🎨 Styles: {output_dir / 'styles.css'}")
    print(f"\n🌐 Open: file://{output_file.absolute()}")
    print("=" * 70)

    return 0


def main():
    """Main entry point."""
    import argparse
    parser = argparse.ArgumentParser(
        description='Generate statistics report from collected data',
        formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument(
        'package_id',
        nargs='?',
        default='RestClient.Net',
        help='Package ID (default: RestClient.Net)'
    )
    parser.add_argument(
        '--sourcedata-dir',
        type=Path,
        default=Path(__file__).parent.parent / "sourcedata",
        help='Source data directory'
    )
    parser.add_argument(
        '--output-dir',
        type=Path,
        default=Path(__file__).parent.parent / "output",
        help='Output directory for HTML files'
    )

    args = parser.parse_args()

    try:
        exit_code = generate_report(args.package_id, args.sourcedata_dir, args.output_dir)
        sys.exit(exit_code)
    except KeyboardInterrupt:
        print("\n\n⚠ Generation interrupted by user")
        sys.exit(130)
    except Exception as e:
        print(f"\n✗ Fatal error: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)


if __name__ == '__main__':
    main()
