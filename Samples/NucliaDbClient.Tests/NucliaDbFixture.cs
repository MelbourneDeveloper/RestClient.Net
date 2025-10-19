using System.Diagnostics;

#pragma warning disable CS8509 // C# compiler doesn't understand nested discriminated unions - use EXHAUSTION001 instead
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line


namespace NucliaDbClient.Tests;

public sealed class NucliaDbFixture : IDisposable
{
    private static readonly string DockerComposeDir = GetDockerComposeDirectory();

    private static string GetDockerComposeDirectory()
    {
        var dir = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            "NucliaDbClient"
        );
        var fullPath = Path.GetFullPath(dir);
        var composePath = Path.Combine(fullPath, "docker-compose.yml");

        if (!File.Exists(composePath))
        {
            throw new InvalidOperationException(
                $"docker-compose.yml not found at {composePath}. Current directory: {Directory.GetCurrentDirectory()}"
            );
        }

        Console.WriteLine($"Docker Compose directory: {fullPath}");
        return fullPath;
    }

    public NucliaDbFixture()
    {
        Console.WriteLine("Setting up NucliaDB test environment...");
        CleanDockerEnvironment();
        StartDockerEnvironment();
        WaitForNucliaDbReady();
        Console.WriteLine("NucliaDB test environment ready!");
    }

    private static void CleanDockerEnvironment()
    {
        Console.WriteLine("Cleaning Docker environment...");
        RunDockerCommand("compose down -v --remove-orphans");
    }

    private static void StartDockerEnvironment()
    {
        Console.WriteLine("Starting Docker Compose...");
        RunDockerCommand("compose up -d");
    }

    private static void RunDockerCommand(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = arguments,
            WorkingDirectory = DockerComposeDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process =
            Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                $"Failed to start docker process with arguments: {arguments}"
            );

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine($"Docker output: {output}");
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.WriteLine($"Docker stderr: {error}");
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Docker command failed with exit code {process.ExitCode}: {arguments}\nError: {error}"
            );
        }
    }

    private static void WaitForNucliaDbReady()
    {
        Console.WriteLine("Waiting for NucliaDB to be ready...");
        using var httpClient = new HttpClient();
        var maxAttempts = 60;
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
                    Console.WriteLine($"NucliaDB is ready! (attempt {i + 1}/{maxAttempts})");
                    return;
                }

                Console.WriteLine(
                    $"Attempt {i + 1}/{maxAttempts}: Status code {response.StatusCode}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attempt {i + 1}/{maxAttempts}: {ex.GetType().Name}");
            }

            Thread.Sleep(delayMs);
        }

        throw new InvalidOperationException(
            $"NucliaDB failed to start within {maxAttempts} seconds. Check Docker logs with: docker logs nucliadb-local"
        );
    }

    public void Dispose()
    {
        Console.WriteLine("Cleaning up NucliaDB test environment...");
        try
        {
            CleanDockerEnvironment();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to clean up Docker environment: {ex.Message}");
        }
    }
}
