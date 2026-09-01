#region Enbrea.Bcp47 - Copyright (c) STÜBER SYSTEMS GmbH
/*
 *    Enbrea.Bcp47
 *
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License.
 *
 */
#endregion

using System;
using Xunit;

namespace Enbrea.Bcp47.XUnit;

/// <summary>
/// Unit tests for <see cref="LanguageRangeParser"/>.
/// </summary>
public class LanguageRangeParserTests
{
    [Theory]
    [InlineData("*-DE")]
    [InlineData("de-*")]
    [InlineData("de-*-DE")]
    [InlineData("*-*-*")]
    public void IsBasic_WithExtendedOnlyRange_ReturnsFalse(string value)
    {
        Assert.False(LanguageRangeParser.IsBasic(value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData("de-")]
    [InlineData("-de")]
    [InlineData("de--DE")]
    [InlineData("1")]
    [InlineData("abcdefghi")]
    [InlineData("de-abcdefghi")]
    [InlineData("de_DE")]
    [InlineData("de DE")]
    [InlineData("de.DE")]
    [InlineData("ä")]
    public void IsBasic_WithInvalidRange_ReturnsFalse(string value)
    {
        Assert.False(LanguageRangeParser.IsBasic(value));
    }

    [Fact]
    public void IsBasic_WithNull_ReturnsFalse()
    {
        Assert.False(LanguageRangeParser.IsBasic(null));
    }

    [Theory]
    [InlineData("*")]
    [InlineData("a")]
    [InlineData("abcdefgh")]
    [InlineData("de")]
    [InlineData("de-DE")]
    [InlineData("de-1")]
    [InlineData("de-abcdefgh")]
    public void IsBasic_WithValidBasicRange_ReturnsTrue(string value)
    {
        Assert.True(LanguageRangeParser.IsBasic(value));
    }
    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData("de-")]
    [InlineData("-de")]
    [InlineData("de--DE")]
    [InlineData("1-DE")]
    [InlineData("abcdefghi-DE")]
    [InlineData("de-abcdefghi")]
    [InlineData("de-ä")]
    public void IsExtended_WithInvalidRange_ReturnsFalse(string value)
    {
        Assert.False(LanguageRangeParser.IsExtended(value));
    }

    [Fact]
    public void IsExtended_WithNull_ReturnsFalse()
    {
        Assert.False(LanguageRangeParser.IsExtended(null));
    }

    [Theory]
    [InlineData("*")]
    [InlineData("de")]
    [InlineData("de-DE")]
    [InlineData("*-DE")]
    [InlineData("de-*")]
    [InlineData("de-*-DE")]
    [InlineData("*-*-*")]
    public void IsExtended_WithValidRange_ReturnsTrue(string value)
    {
        Assert.True(LanguageRangeParser.IsExtended(value));
    }

    [Fact]
    public void Parse_WithBasicRange_ReturnsNormalizedComponents()
    {
        var range = LanguageRangeParser.Parse("DE-de-1996");

        Assert.Equal("DE-de-1996", range.Value);
        Assert.Equal("de-de-1996", range.ToString());
        Assert.Equal(["de", "de", "1996"], range.Subtags);
        Assert.True(range.IsBasic);
        Assert.False(range.IsWildcard);
    }

    [Fact]
    public void Parse_WithExtendedRange_ReturnsNormalizedComponents()
    {
        var range = LanguageRangeParser.Parse("DE-*-Ch");

        Assert.Equal("DE-*-Ch", range.Value);
        Assert.Equal("de-*-ch", range.ToString());
        Assert.Equal(["de", "*", "ch"], range.Subtags);
        Assert.False(range.IsBasic);
        Assert.False(range.IsWildcard);
    }

    [Fact]
    public void Parse_WithInvalidRange_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => LanguageRangeParser.Parse("de--DE"));
    }

    [Fact]
    public void Parse_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => LanguageRangeParser.Parse(null));
    }

    [Fact]
    public void Parse_WithSingleLetterPrimaryRange_IsValid()
    {
        var range = LanguageRangeParser.Parse("A-12345678");

        Assert.Equal("a-12345678", range.ToString());
        Assert.True(range.IsBasic);
    }

    [Fact]
    public void Parse_WithWildcard_ReturnsWildcardRange()
    {
        var range = LanguageRangeParser.Parse("*");

        Assert.Equal("*", range.Value);
        Assert.Equal("*", range.ToString());
        Assert.Equal(["*"], range.Subtags);
        Assert.True(range.IsBasic);
        Assert.True(range.IsWildcard);
    }
    [Fact]
    public void ParseBasic_WithExtendedOnlyRange_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => LanguageRangeParser.ParseBasic("de-*-DE"));
    }

    [Fact]
    public void ParseBasic_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => LanguageRangeParser.ParseBasic(null));
    }

    [Fact]
    public void ParseBasic_WithWildcard_ReturnsWildcardRange()
    {
        var range = LanguageRangeParser.ParseBasic("*");

        Assert.True(range.IsBasic);
        Assert.True(range.IsWildcard);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("de--DE")]
    public void TryParse_WithInvalidRange_ReturnsFalse(string value)
    {
        var result = LanguageRangeParser.TryParse(value, out var range);

        Assert.False(result);
        Assert.Null(range);
    }

    [Fact]
    public void TryParse_WithValidExtendedRange_ReturnsTrue()
    {
        var result = LanguageRangeParser.TryParse("DE-*-Ch", out var range);

        Assert.True(result);
        Assert.NotNull(range);
        Assert.Equal("de-*-ch", range.ToString());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("de-*")]
    public void TryParseBasic_WithInvalidOrExtendedOnlyRange_ReturnsFalse(string value)
    {
        var result = LanguageRangeParser.TryParseBasic(value, out var range);

        Assert.False(result);
        Assert.Null(range);
    }

    [Fact]
    public void TryParseBasic_WithValidBasicRange_ReturnsTrue()
    {
        var result = LanguageRangeParser.TryParseBasic("DE-de", out var range);

        Assert.True(result);
        Assert.NotNull(range);
        Assert.Equal("de-de", range.ToString());
        Assert.True(range.IsBasic);
    }
}
