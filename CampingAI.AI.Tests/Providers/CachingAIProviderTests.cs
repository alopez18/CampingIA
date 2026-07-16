using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using CampingAI.AI.Providers;
using CampingAI.AI.Settings;

namespace CampingAI.AI.Tests.Providers;
public class CachingAIProviderTests {

    static CachingAIProvider CreateSut(IAIProvider inner, AISettings? settings = null) {
        var cache = new MemoryCache(new MemoryCacheOptions());
        return new CachingAIProvider(
            inner,
            cache,
            NullLogger<CachingAIProvider>.Instance,
            settings ?? new AISettings());
    }

    [Fact]
    public void IsEnabled_Should_DelegateToInnerProvider() {
        // Arrange
        var inner = new Mock<IAIProvider>();
        inner.SetupGet(p => p.IsEnabled).Returns(true);
        var sut = CreateSut(inner.Object);

        // Act
        var isEnabled = sut.IsEnabled;

        // Assert
        isEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task GenerateTextAsync_Should_CallInnerOnce_WhenSameInputRepeated() {
        // Arrange
        var inner = new Mock<IAIProvider>();
        inner.Setup(p => p.GenerateTextAsync("sys", "user", It.IsAny<CancellationToken>()))
             .ReturnsAsync("respuesta");
        var sut = CreateSut(inner.Object);

        // Act
        var first = await sut.GenerateTextAsync("sys", "user");
        var second = await sut.GenerateTextAsync("sys", "user");

        // Assert
        first.Should().Be("respuesta");
        second.Should().Be("respuesta");
        inner.Verify(p => p.GenerateTextAsync("sys", "user", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateJsonAsync_Should_CallInnerOnce_WhenSameInputRepeated() {
        // Arrange
        var inner = new Mock<IAIProvider>();
        inner.Setup(p => p.GenerateJsonAsync("sys", "user", It.IsAny<CancellationToken>()))
             .ReturnsAsync("{\"ok\":true}");
        var sut = CreateSut(inner.Object);

        // Act
        var first = await sut.GenerateJsonAsync("sys", "user");
        var second = await sut.GenerateJsonAsync("sys", "user");

        // Assert
        first.Should().Be("{\"ok\":true}");
        second.Should().Be("{\"ok\":true}");
        inner.Verify(p => p.GenerateJsonAsync("sys", "user", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateTextAsync_Should_CallInnerTwice_WhenInputDiffers() {
        // Arrange
        var inner = new Mock<IAIProvider>();
        inner.Setup(p => p.GenerateTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync("respuesta");
        var sut = CreateSut(inner.Object);

        // Act
        await sut.GenerateTextAsync("sys", "user-a");
        await sut.GenerateTextAsync("sys", "user-b");

        // Assert
        inner.Verify(p => p.GenerateTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GenerateTextAndJson_Should_NotShareCache_ForSameInput() {
        // Arrange
        var inner = new Mock<IAIProvider>();
        inner.Setup(p => p.GenerateTextAsync("sys", "user", It.IsAny<CancellationToken>()))
             .ReturnsAsync("texto");
        inner.Setup(p => p.GenerateJsonAsync("sys", "user", It.IsAny<CancellationToken>()))
             .ReturnsAsync("json");
        var sut = CreateSut(inner.Object);

        // Act
        var text = await sut.GenerateTextAsync("sys", "user");
        var json = await sut.GenerateJsonAsync("sys", "user");

        // Assert
        text.Should().Be("texto");
        json.Should().Be("json");
        inner.Verify(p => p.GenerateTextAsync("sys", "user", It.IsAny<CancellationToken>()), Times.Once);
        inner.Verify(p => p.GenerateJsonAsync("sys", "user", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateTextAsync_Should_NotCache_WhenResultIsEmpty() {
        // Arrange
        var inner = new Mock<IAIProvider>();
        inner.Setup(p => p.GenerateTextAsync("sys", "user", It.IsAny<CancellationToken>()))
             .ReturnsAsync(string.Empty);
        var sut = CreateSut(inner.Object);

        // Act
        await sut.GenerateTextAsync("sys", "user");
        await sut.GenerateTextAsync("sys", "user");

        // Assert
        inner.Verify(p => p.GenerateTextAsync("sys", "user", It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
