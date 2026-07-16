using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using CampingAI.AI.Providers;

namespace CampingAI.AI.Tests.Providers;
public class NullAIProviderTests {

    [Fact]
    public void IsEnabled_Should_BeFalse() {
        // Arrange
        var provider = new NullAIProvider(NullLogger<NullAIProvider>.Instance);

        // Act
        var isEnabled = provider.IsEnabled;

        // Assert
        isEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task GenerateTextAsync_Should_Throw_WhenNotConfigured() {
        // Arrange
        var provider = new NullAIProvider(NullLogger<NullAIProvider>.Instance);

        // Act
        var act = async () => await provider.GenerateTextAsync("sys", "user");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GenerateJsonAsync_Should_Throw_WhenNotConfigured() {
        // Arrange
        var provider = new NullAIProvider(NullLogger<NullAIProvider>.Instance);

        // Act
        var act = async () => await provider.GenerateJsonAsync("sys", "user");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
