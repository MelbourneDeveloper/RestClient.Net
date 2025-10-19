using System.Diagnostics;

#pragma warning disable CS8509 // C# compiler doesn't understand nested discriminated unions - use EXHAUSTION001 instead
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line


namespace NucliaDbClient.Tests;

public sealed class NucliaDbFixture : IDisposable
{
    private static readonly string DockerComposeDir = Path.Combine(
        Directory.GetCurrentDirectory(),
        "..",
        "..",
        "..",
        "..",
        "NucliaDbClient"
    );

    public NucliaDbFixture()
    {
        CleanDockerEnvironment();
        StartDockerEnvironment();
        WaitForNucliaDbReady();
    }

    private static void CleanDockerEnvironment()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "compose down -v --remove-orphans",
            WorkingDirectory = DockerComposeDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = Process.Start(startInfo);
        process?.WaitForExit();
    }

    private static void StartDockerEnvironment()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "compose up -d",
            WorkingDirectory = DockerComposeDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = Process.Start(startInfo);
        process?.WaitForExit();
    }

    private static void WaitForNucliaDbReady()
    {
        using var httpClient = new HttpClient();
        var maxAttempts = 30;
        var delayMs = 1000;
        var uri = new Uri("http://localhost:8080/");

        for (var i = 0; i < maxAttempts; i++)
        {
            try
            {
                var response = httpClient.GetAsync(uri).GetAwaiter().GetResult();
                if (
                    response.IsSuccessStatusCode
                    || response.StatusCode == System.Net.HttpStatusCode.NotFound
                )
                {
                    return;
                }
            }
            catch
            {
                // Ignore exceptions during startup
            }

            Thread.Sleep(delayMs);
        }

        throw new InvalidOperationException("NucliaDB failed to start within the expected time");
    }

    public void Dispose()
    {
        // Optionally clean up after tests
        // CleanDockerEnvironment();
    }
}
