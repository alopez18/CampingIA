using CampingAI.DataImporter.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace CampingAI.DataImporter.Tests.Services;

public class ProvinceGeoResolverTests {
    private readonly ProvinceGeoResolver _sut = new(NullLogger<ProvinceGeoResolver>.Instance);

    [Theory]
    // Coordenadas representativas de cada provincia → PRV_Code esperado.
    [InlineData(41.3874, 2.1686, "BCN")]   // Barcelona
    [InlineData(40.4168, -3.7038, "MAD")]  // Madrid
    [InlineData(39.4699, -0.3763, "VLC")]  // Valencia
    [InlineData(37.3891, -5.9845, "SEV")]  // Sevilla
    [InlineData(43.2630, -2.9350, "VIZ")]  // Bilbao (Vizcaya)
    [InlineData(36.7213, -4.4213, "MAL")]  // Málaga
    [InlineData(39.5696, 2.6502, "BAL")]   // Palma (Illes Balears)
    [InlineData(28.4636, -16.2518, "TFE")] // Santa Cruz de Tenerife
    public void ResolveProvinceCode_Should_ReturnExpectedCode_WhenPointInsideProvince(
        double latitude, double longitude, string expectedCode) {
        // Act
        var code = _sut.ResolveProvinceCode(latitude, longitude);

        // Assert
        code.Should().Be(expectedCode);
    }

    [Fact]
    public void ResolveProvinceCode_Should_ReturnNull_WhenPointOutsideSpain() {
        // Arrange — punto en medio del Atlántico.
        // Act
        var code = _sut.ResolveProvinceCode(40.0, -30.0);

        // Assert
        code.Should().BeNull();
    }
}
