# RestClient.Net Statistics Dashboard

Professional statistics dashboard combining NuGet package analytics and GitHub repository metrics with Material Design aesthetics.

## 🎯 Quick Start

Generate the complete statistics report:

```bash
# 1. Collect all data (NuGet + GitHub in parallel)
python3 scripts/collect_all.py

# 2. Generate the HTML report
python3 scripts/generate_report.py

# 3. Open the report
open output/restclient-report.html
```

## 📁 Directory Structure

```
stats/
├── sourcedata/           # All collected JSON data
│   ├── restclient-stats.json    # NuGet package metadata
│   ├── nuget-downloads.json     # NuGet download statistics
│   ├── repo_info.json           # GitHub repository info
│   ├── contributors.json        # GitHub contributors
│   ├── commits.json             # Commit history
│   ├── issues_*.json            # Issues data
│   ├── pull_requests_*.json     # Pull request data
│   ├── traffic_*.json           # Traffic statistics
│   └── ... (more GitHub data files)
│
├── output/              # Generated reports
│   ├── restclient-report.html   # Main statistics report
│   └── styles.css               # Material Design stylesheet
│
├── scripts/             # Python data collection & generation scripts
│   ├── collect_all.py          # Master orchestrator (run this first)
│   ├── download_nuget.py       # NuGet statistics downloader
│   ├── download_github.py      # GitHub statistics downloader
│   └── generate_report.py      # HTML report generator
│
└── README.md            # This file
```

## 🔧 Scripts Overview

### `collect_all.py` - Master Orchestrator

Runs both data downloaders in parallel for maximum efficiency.

```bash
python3 scripts/collect_all.py
```

**Features:**
- Parallel execution of NuGet and GitHub downloaders
- Real-time progress monitoring
- Error handling and summary reporting
- Fastest way to collect all data

### `download_nuget.py` - NuGet Statistics

Downloads NuGet package data using curl.

```bash
python3 scripts/download_nuget.py
```

**Downloads:**
- Package metadata (all versions, publish dates, status)
- Download statistics per version
- Total download counts

**Output:**
- `sourcedata/restclient-stats.json` (248KB)
- `sourcedata/nuget-downloads.json`

### `download_github.py` - GitHub Statistics

Collects comprehensive GitHub repository metrics using `gh` CLI.

```bash
python3 scripts/download_github.py
```

**Requirements:**
- GitHub CLI (`gh`) installed and authenticated
- Git repository access

**Downloads:**
- Repository info (stars, forks, watchers)
- Contributors and commit history
- Issues and pull requests
- Traffic statistics (views, clones)
- Release information
- Workflow runs

**Output:** 15+ JSON files in `sourcedata/`

### `generate_report.py` - HTML Report Generator

Creates professional HTML report from collected data.

```bash
python3 scripts/generate_report.py
```

**Features:**
- Material Design styling
- Interactive Chart.js visualizations
- Responsive layout
- Combined NuGet + GitHub insights
- External CSS (no inline styles)

**Output:**
- `output/restclient-report.html` - Main report
- Uses `output/styles.css` for styling

## 📊 Report Sections

The generated report includes:

### 1. **Hero Section**
- Total NuGet downloads
- Project description

### 2. **Key Metrics Grid**
- NuGet downloads
- GitHub stars
- Total versions
- Contributors
- Forks
- Traffic statistics

### 3. **Popularity & Growth**
- Downloads by month chart
- Recent traffic statistics
- Most downloaded version

### 4. **Release Activity**
- Versions released per year chart
- Commit activity timeline
- GitHub releases count
- Merged PRs and issues

### 5. **Community Engagement**
- Top contributors chart
- Stars, forks, watchers breakdown
- Contributor statistics

### 6. **Complete Version History**
- Full table of all NuGet versions
- Download counts per version
- Published dates
- Listed/unlisted status

## 🎨 Styling

The report uses **Material Design** principles:

- **Color Scheme:**
  - Primary: Material Blue (#1976D2)
  - Accent: Amber (#FFC107)
  - Backgrounds: Light gray (#FAFAFA)

- **Typography:**
  - Roboto font family
  - Clear visual hierarchy
  - Readable body text (16px)

- **Components:**
  - Elevation shadows (2dp, 4dp, 8dp)
  - Card-based layout
  - Smooth transitions
  - Responsive grid system

- **NO gradients, NO purple** - Professional and clean

## 🔄 Updating Statistics

To refresh the statistics:

```bash
# Quick update (recommended)
python3 scripts/collect_all.py && python3 scripts/generate_report.py

# Manual step-by-step
python3 scripts/download_nuget.py    # Update NuGet data
python3 scripts/download_github.py   # Update GitHub data
python3 scripts/generate_report.py   # Regenerate report
```

## 📋 Requirements

### Python
- Python 3.7 or higher
- No external Python packages required (uses stdlib only)

### System Tools
- `curl` - For NuGet API calls
- `gh` - GitHub CLI for GitHub data
- `git` - For local commit history

### Installing GitHub CLI

```bash
# macOS
brew install gh

# Linux
sudo apt install gh  # Debian/Ubuntu
sudo dnf install gh  # Fedora

# Authenticate
gh auth login
```

## 🔍 Data Sources

### NuGet
- **Package Metadata:** `https://api.nuget.org/v3/registration5-semver1/restclient.net/index.json`
- **Download Stats:** `https://azuresearch-usnc.nuget.org/query?q=packageid:RestClient.Net`

### GitHub
- **Repository:** `MelbourneDeveloper/RestClient.Net`
- **API:** GitHub REST API via `gh` CLI
- **Commits:** Local git log

## 📈 Metrics Collected

### NuGet Metrics
- Total downloads (501K+)
- Downloads per version
- Downloads by month
- Version release timeline
- Listed/unlisted status

### GitHub Metrics
- ⭐ Stars (380)
- 🍴 Forks (49)
- 👁 Watchers (18)
- 👨‍💻 Contributors (7)
- 📝 Commits (1000+)
- 🐛 Issues (86 total, 8 open)
- 🔀 Pull Requests (53 total)
- 📦 Releases
- 📊 Traffic (views, clones)
- ⚙️ Workflow runs

## 🐛 Troubleshooting

### "gh: command not found"
Install GitHub CLI: `brew install gh` (macOS) or follow [official instructions](https://cli.github.com/)

### "Not Found (HTTP 404)" from GitHub API
Authenticate with GitHub CLI: `gh auth login`

### Missing data in report
1. Run `python3 scripts/collect_all.py` to collect all data
2. Check `sourcedata/` directory for JSON files
3. Ensure both NuGet and GitHub downloads completed successfully

### Traffic data shows zeros
Traffic statistics require repository owner permissions. Other metrics will still work.

## 📝 Customization

### Modify Report Layout
Edit `scripts/generate_report.py` to add/remove sections or change structure.

### Update Styling
Edit `output/styles.css` to customize colors, fonts, spacing, or layout.

### Change Data Collection
Edit individual downloader scripts to add/remove metrics:
- `scripts/download_nuget.py` - NuGet customization
- `scripts/download_github.py` - GitHub customization

## 📜 License

Part of RestClient.Net project - see main repository LICENSE file.

## 🔗 Links

- **NuGet Package:** https://www.nuget.org/packages/RestClient.Net
- **GitHub Repository:** https://github.com/MelbourneDeveloper/RestClient.Net
- **Documentation:** https://github.com/MelbourneDeveloper/RestClient.Net#readme

---

*Generated with Material Design principles - Professional statistics for professional developers*
