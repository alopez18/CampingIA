using FluentAssertions;
using FluentValidation.TestHelper;
using CampingAI.Application.Queries.Camping.SearchCampings;

namespace CampingAI.Application.Tests.Queries.Camping;
public class SearchCampingsQueryValidatorTests {

    private readonly SearchCampingsQueryValidator _validator = new();

    [Fact]
    public void Validate_Should_Pass_WhenQueryIsValid() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, 10m, 50m, null, null, null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Pass_WhenNoFiltersApplied() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, null, null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_Should_Fail_WhenPageIsLessThanOne(int page) {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, null, null, null, null, page, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(200)]
    public void Validate_Should_Fail_WhenPageSizeIsOutOfRange(int pageSize) {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, null, null, null, null, 1, pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_Should_Fail_WhenMaxPriceIsLessThanMinPrice() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, 100m, 50m, null, null, null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxPrice!.Value);
    }

    [Fact]
    public void Validate_Should_Pass_WhenMaxPriceEqualsMinPrice() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, 50m, 50m, null, null, null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenMinPriceIsNegative() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, -1m, null, null, null, null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Should_Fail_WhenMaxPriceIsNegative() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, -1m, null, null, null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_Should_Pass_WhenPageSizeIsInRange(int pageSize) {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, null, null, null, null, 1, pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Pass_WhenBoundingBoxIsValid() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, 40m, 41m, -4m, -3m, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Validate_Should_Fail_WhenLatitudeIsOutOfRange(decimal lat) {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, lat, null, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinLat!.Value);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Validate_Should_Fail_WhenLongitudeIsOutOfRange(decimal lng) {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, null, null, lng, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinLng!.Value);
    }

    [Fact]
    public void Validate_Should_Fail_WhenMaxLatIsLessThanMinLat() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, 42m, 40m, null, null, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxLat!.Value);
    }

    [Fact]
    public void Validate_Should_Fail_WhenMaxLngIsLessThanMinLng() {
        // Arrange
        var query = new SearchCampingsQuery(null, null, null, null, null, null, null, null, null, -3m, -5m, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxLng!.Value);
    }
}