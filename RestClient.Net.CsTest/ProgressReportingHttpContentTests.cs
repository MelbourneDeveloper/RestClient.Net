using System.Text;
using RestClient.Net.Utilities;

namespace RestClient.Net.CsTest;

#pragma warning disable CA1812

[TestClass]
public sealed class ProgressReportingHttpContentTests
{
    [TestMethod]
    [DataRow("concrete")]
    [DataRow("HttpContent")]
    [DataRow("IDisposable")]
    [DataRow("HttpRequestMessage")]
    public async Task Dispose_DisposesContentStream(string disposalRoute)
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("test content");
        using var stream = new MemoryStream(bytes);
        using var content = new ProgressReportingHttpContent(stream);
        using var request = new HttpRequestMessage { Content = content };
        using var destination = new MemoryStream();
        Assert.IsTrue(stream.CanRead);
        Assert.IsTrue(stream.CanSeek);
        Assert.AreEqual(12L, content.Headers.ContentLength);
        await content.CopyToAsync(destination).ConfigureAwait(false);
        CollectionAssert.AreEqual(bytes, destination.ToArray());
        HttpContent baseContent = content;
        IDisposable disposableContent = content;
        Action dispose = disposalRoute switch
        {
            "concrete" => content.Dispose,
            "HttpContent" => baseContent.Dispose,
            "IDisposable" => disposableContent.Dispose,
            "HttpRequestMessage" => request.Dispose,
            _ => throw new ArgumentOutOfRangeException(nameof(disposalRoute)),
        };

        // Act
        dispose();

        // Assert - attempting to access disposed stream should throw
        Assert.IsFalse(
            stream.CanRead,
            $"Disposal through {disposalRoute} must close the owned source stream."
        );
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.ReadByte());
        Assert.IsFalse(stream.CanWrite);
        Assert.IsFalse(stream.CanSeek);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.WriteByte(1));
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Seek(0, SeekOrigin.Begin));
        dispose();
        dispose();
        Assert.IsFalse(stream.CanRead, "Repeated disposal must not reopen the source.");
        Assert.IsTrue(
            destination.CanRead,
            "Disposal must preserve the caller's destination stream."
        );
        Assert.IsTrue(destination.CanWrite);
        Assert.AreEqual(bytes.Length, destination.Length);
        CollectionAssert.AreEqual(bytes, destination.ToArray());
        _ = await Assert
            .ThrowsExceptionAsync<ObjectDisposedException>(() => content.CopyToAsync(destination))
            .ConfigureAwait(false);
        CollectionAssert.AreEqual(
            bytes,
            destination.ToArray(),
            "Rejected writes after disposal must preserve the copied data."
        );
    }

    [TestMethod]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        using var content = new ProgressReportingHttpContent(stream);
        Assert.IsTrue(stream.CanRead);
        Assert.AreEqual(12L, content.Headers.ContentLength);

        // Act & Assert - multiple dispose calls should be safe
        content.Dispose();
        Assert.IsFalse(stream.CanRead);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.ReadByte());
        content.Dispose();
        Assert.IsFalse(stream.CanWrite);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.WriteByte(0));
        content.Dispose();
        Assert.IsFalse(stream.CanSeek);
    }

    [TestMethod]
    public async Task Dispose_DisposesBaseHttpContent()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
#pragma warning disable CA2000 // Dispose objects before losing scope - we're testing disposal
        var content = new ProgressReportingHttpContent(stream);
#pragma warning restore CA2000
        Assert.IsTrue(stream.CanRead);
        Assert.AreEqual(12L, content.Headers.ContentLength);

        // Act
        content.Dispose();

        // Assert - using disposed HttpContent should throw
        using var destinationStream = new MemoryStream();
        _ = await Assert
            .ThrowsExceptionAsync<ObjectDisposedException>(
                async () => await content.CopyToAsync(destinationStream).ConfigureAwait(false)
            )
            .ConfigureAwait(false);
        Assert.AreEqual(
            0L,
            destinationStream.Length,
            "Disposed content must not write partial data."
        );
        Assert.IsTrue(destinationStream.CanWrite, "The caller owns the destination stream.");
        Assert.IsFalse(stream.CanRead);
        content.Dispose();
        _ = await Assert
            .ThrowsExceptionAsync<ObjectDisposedException>(() => content.ReadAsStringAsync())
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
        Assert.AreEqual((testData.Length + 4) / 5, progressReports.Count);
        for (var index = 0; index < progressReports.Count; index++)
        {
            Assert.AreEqual(
                Math.Min((index + 1) * 5, testData.Length),
                progressReports[index].current
            );
            Assert.AreEqual(testData.Length, progressReports[index].total);
            Assert.IsTrue(progressReports[index].current > 0);
            Assert.IsTrue(progressReports[index].current <= progressReports[index].total);
        }
        Assert.AreEqual(testData.Length, destinationStream.Length);
        Assert.IsTrue(destinationStream.CanRead);
        Assert.IsTrue(destinationStream.CanWrite);
        Assert.AreEqual(testData.Length, content.Headers.ContentLength);
        Assert.AreEqual("application/octet-stream", content.Headers.ContentType?.MediaType);
        destinationStream.Position = 0;
        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(testData), destinationStream.ToArray());
    }

    [TestMethod]
    public async Task TryComputeLength_ReturnsTrue()
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
        Assert.AreEqual("application/octet-stream", content.Headers.ContentType?.MediaType);
        var firstRead = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(testData), firstRead);
        Assert.AreEqual(testData.Length, content.Headers.ContentLength);
        Assert.AreEqual(testData, await content.ReadAsStringAsync().ConfigureAwait(false));
        CollectionAssert.AreEqual(
            firstRead,
            await content.ReadAsByteArrayAsync().ConfigureAwait(false)
        );
    }

    [TestMethod]
    public async Task Constructor_WithByteArray_SetsContentType()
    {
        // Arrange & Act
        using var content = new ProgressReportingHttpContent(
            Encoding.UTF8.GetBytes("test"),
            contentType: "application/json"
        );

        // Assert
        Assert.AreEqual("application/json", content.Headers.ContentType?.MediaType);
        await AssertBufferedContentAsync(content, "test", "application/json").ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Constructor_WithString_SetsContentType()
    {
        // Arrange & Act
        using var content = new ProgressReportingHttpContent("test", contentType: "text/plain");

        // Assert
        Assert.AreEqual("text/plain", content.Headers.ContentType?.MediaType);
        await AssertBufferedContentAsync(content, "test", "text/plain").ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Constructor_WithStream_SetsContentType()
    {
        // Arrange & Act
        using var content = new ProgressReportingHttpContent(
            new MemoryStream(Encoding.UTF8.GetBytes("test")),
            contentType: "application/xml"
        );

        // Assert
        Assert.AreEqual("application/xml", content.Headers.ContentType?.MediaType);
        await AssertBufferedContentAsync(content, "test", "application/xml").ConfigureAwait(false);
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
        Assert.AreEqual(testData.Length, destinationStream.Length);
        Assert.IsTrue(destinationStream.CanWrite);
        Assert.AreEqual(testData.Length, content.Headers.ContentLength);
        Assert.AreEqual("application/octet-stream", content.Headers.ContentType?.MediaType);
        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(testData), destinationStream.ToArray());
        content.Dispose();
        _ = await Assert
            .ThrowsExceptionAsync<ObjectDisposedException>(
                () => content.CopyToAsync(destinationStream)
            )
            .ConfigureAwait(false);
        CollectionAssert.AreEqual(
            Encoding.UTF8.GetBytes(testData),
            destinationStream.ToArray(),
            "A rejected operation must preserve already-copied bytes."
        );
    }

    private static async Task AssertBufferedContentAsync(
        ProgressReportingHttpContent content,
        string expected,
        string mediaType
    )
    {
        var bytes = Encoding.UTF8.GetBytes(expected);
        Assert.AreEqual(bytes.Length, content.Headers.ContentLength);
        Assert.AreEqual(mediaType, content.Headers.ContentType?.MediaType);
        Assert.IsNull(content.Headers.ContentType?.CharSet);
        Assert.AreEqual(expected, await content.ReadAsStringAsync().ConfigureAwait(false));
        CollectionAssert.AreEqual(
            bytes,
            await content.ReadAsByteArrayAsync().ConfigureAwait(false)
        );
        using var destination = new MemoryStream();
        await content.CopyToAsync(destination).ConfigureAwait(false);
        CollectionAssert.AreEqual(bytes, destination.ToArray());
        Assert.AreEqual(bytes.Length, destination.Position);
        Assert.IsTrue(destination.CanWrite);
        Assert.AreEqual(mediaType, content.Headers.ContentType?.MediaType);
        Assert.AreEqual(bytes.Length, content.Headers.ContentLength);
        content.Dispose();
        _ = await Assert
            .ThrowsExceptionAsync<ObjectDisposedException>(() => content.ReadAsByteArrayAsync())
            .ConfigureAwait(false);
        CollectionAssert.AreEqual(
            bytes,
            destination.ToArray(),
            "Disposing content must preserve the caller's copy."
        );
    }
}
