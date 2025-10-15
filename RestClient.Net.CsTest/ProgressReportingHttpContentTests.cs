using System.Text;
using RestClient.Net.Utilities;

namespace RestClient.Net.CsTest;

#pragma warning disable CA1812

[TestClass]
public sealed class ProgressReportingHttpContentTests
{
    [TestMethod]
    public void Dispose_DisposesContentStream()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        using var content = new ProgressReportingHttpContent(stream);

        // Act
        content.Dispose();

        // Assert - attempting to access disposed stream should throw
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.ReadByte());
    }

    [TestMethod]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        using var content = new ProgressReportingHttpContent(stream);

        // Act & Assert - multiple dispose calls should be safe
        content.Dispose();
        content.Dispose();
    }

    [TestMethod]
    public async Task Dispose_DisposesBaseHttpContent()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
#pragma warning disable CA2000 // Dispose objects before losing scope - we're testing disposal
        var content = new ProgressReportingHttpContent(stream);
#pragma warning restore CA2000

        // Act
        content.Dispose();

        // Assert - using disposed HttpContent should throw
        var destinationStream = new MemoryStream();
        _ = await Assert
            .ThrowsExceptionAsync<ObjectDisposedException>(
                async () => await content.CopyToAsync(destinationStream).ConfigureAwait(false)
            )
            .ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SerializeToStreamAsync_ReportsProgress()
    {
        // Arrange
        var progressReports = new List<(long current, long total)>();
        var testData = "This is test data for progress reporting";
        using var content = new ProgressReportingHttpContent(
            testData,
            bufferSize: 5,
            progress: (current, total) => progressReports.Add((current, total))
        );

        var destinationStream = new MemoryStream();

        // Act
        await content.CopyToAsync(destinationStream).ConfigureAwait(false);

        // Assert
        Assert.IsTrue(progressReports.Count > 0, "Progress should be reported");
        Assert.AreEqual(
            testData.Length,
            progressReports.Last().current,
            "Final progress should match content length"
        );
        Assert.AreEqual(
            testData.Length,
            progressReports.Last().total,
            "Total should match content length"
        );

        destinationStream.Position = 0;
        using var reader = new StreamReader(destinationStream);
        var result = await reader.ReadToEndAsync().ConfigureAwait(false);
        Assert.AreEqual(testData, result, "Content should be copied correctly");
    }

    [TestMethod]
    public void TryComputeLength_ReturnsTrue()
    {
        // Arrange
        var testData = "Test content";
        using var content = new ProgressReportingHttpContent(testData);

        // Assert
        Assert.IsTrue(content.Headers.ContentLength.HasValue, "Content length should be available");
        Assert.AreEqual(
            testData.Length,
            content.Headers.ContentLength.Value,
            "Content length should match data length"
        );
    }

    [TestMethod]
    public void Constructor_WithByteArray_SetsContentType()
    {
        // Arrange & Act
        using var content = new ProgressReportingHttpContent(
            Encoding.UTF8.GetBytes("test"),
            contentType: "application/json"
        );

        // Assert
        Assert.AreEqual("application/json", content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public void Constructor_WithString_SetsContentType()
    {
        // Arrange & Act
        using var content = new ProgressReportingHttpContent("test", contentType: "text/plain");

        // Assert
        Assert.AreEqual("text/plain", content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public void Constructor_WithStream_SetsContentType()
    {
        // Arrange & Act
        using var content = new ProgressReportingHttpContent(
            new MemoryStream(Encoding.UTF8.GetBytes("test")),
            contentType: "application/xml"
        );

        // Assert
        Assert.AreEqual("application/xml", content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task SerializeToStreamAsync_WithoutProgressCallback_DoesNotThrow()
    {
        // Arrange
        var testData = "Test data without progress callback";
        using var content = new ProgressReportingHttpContent(testData, progress: null);
        var destinationStream = new MemoryStream();

        // Act
        await content.CopyToAsync(destinationStream).ConfigureAwait(false);

        // Assert
        destinationStream.Position = 0;
        using var reader = new StreamReader(destinationStream);
        var result = await reader.ReadToEndAsync().ConfigureAwait(false);
        Assert.AreEqual(
            testData,
            result,
            "Content should be copied correctly without progress callback"
        );
    }
}
