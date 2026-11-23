#!/bin/bash
#
# Generate statistics reports for 5 popular .NET packages
#

PACKAGES=(
    "RestClient.Net"
    "Newtonsoft.Json"
    "Serilog"
    "Dapper"
    "Polly"
)

echo "======================================================================"
echo "Collecting Statistics for 5 Popular .NET Packages"
echo "======================================================================"
echo ""

for package in "${PACKAGES[@]}"; do
    echo "======================================================================"
    echo "Processing: $package"
    echo "======================================================================"

    # Collect data
    python3 "$(dirname "$0")/collect_all.py" "$package"

    if [ $? -ne 0 ]; then
        echo "⚠ Failed to collect data for $package"
        continue
    fi

    # Generate report (note: requires updated generate_report.py)
    # python3 "$(dirname "$0")/generate_report.py" "$package"

    echo ""
    echo "✓ Completed: $package"
    echo ""
done

echo "======================================================================"
echo "✓ All packages processed!"
echo "======================================================================"
