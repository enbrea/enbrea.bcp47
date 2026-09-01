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
/// Unit tests for <see cref="LanguageRangeExtensions"/>.
/// </summary>
public class LanguageRangeExtensionsTests
{
    [Fact]
    public void BasicFilter_ReturnsMatchingTagsInInputOrder()
    {
        var range = LanguageRangeParser.ParseBasic("de-DE");
        var tags = ParseTags(
            "de-Latn-DE",
            "de-DE-1996",
            "en-US",
            "de-DE");

        var result = range.BasicFilter(tags);

        Assert.Equal(2, result.Count);
        Assert.Equal("de-DE-1996", result[0].Value);
        Assert.Equal("de-DE", result[1].Value);
    }

    [Fact]
    public void ExtendedFilter_ReturnsMatchingTagsInInputOrder()
    {
        var range = LanguageRangeParser.Parse("de-*-DE");
        var tags = ParseTags(
            "de-DE",
            "de-Latn-DE",
            "de-DE-x-goethe",
            "de",
            "de-x-DE",
            "en-US");

        var result = range.ExtendedFilter(tags);

        Assert.Equal(3, result.Count);
        Assert.Equal("de-DE", result[0].Value);
        Assert.Equal("de-Latn-DE", result[1].Value);
        Assert.Equal("de-DE-x-goethe", result[2].Value);
    }

    [Theory]
    [InlineData("de", "de", true)]
    [InlineData("de", "de-DE", true)]
    [InlineData("de-DE", "de-DE-1996", true)]
    [InlineData("de-DE", "de-Latn-DE", false)]
    public void IsBasicMatch_ReturnsExpectedResult(string rangeValue, string tagValue, bool expected)
    {
        var range = LanguageRangeParser.ParseBasic(rangeValue);
        var tag = Bcp47Parser.Parse(tagValue);

        Assert.Equal(expected, range.IsBasicMatch(tag));
    }

    [Fact]
    public void IsBasicMatch_WithExtendedOnlyRange_ThrowsArgumentException()
    {
        var range = LanguageRangeParser.Parse("de-*-DE");
        var tag = Bcp47Parser.Parse("de-DE");

        Assert.Throws<ArgumentException>(() => range.IsBasicMatch(tag));
    }

    [Theory]
    [InlineData("de-*-DE", "de-DE", true)]
    [InlineData("de-*-DE", "de-Latn-DE", true)]
    [InlineData("de-*-DE", "de-DE-1996", true)]
    [InlineData("de-*-DE", "de-x-DE", false)]
    [InlineData("*-CH", "fr-Latn-CH", true)]
    public void IsExtendedMatch_ReturnsExpectedResult(string rangeValue, string tagValue, bool expected)
    {
        var range = LanguageRangeParser.Parse(rangeValue);
        var tag = Bcp47Parser.Parse(tagValue);

        Assert.Equal(expected, range.IsExtendedMatch(tag));
    }

    [Fact]
    public void Lookup_ReturnsBestMatchingTag()
    {
        var range = LanguageRangeParser.ParseBasic("de-DE-1996");
        var tags = ParseTags(
            "de",
            "de-DE",
            "en-US");

        var result = range.Lookup(tags);

        Assert.NotNull(result);
        Assert.Equal("de-DE", result.Value);
    }

    [Fact]
    public void Lookup_WhenNoMatchExists_ReturnsNull()
    {
        var range = LanguageRangeParser.ParseBasic("de-CH");
        var tags = ParseTags(
            "de-AT",
            "en-US");

        var result = range.Lookup(tags);

        Assert.Null(result);
    }

    [Fact]
    public void Lookup_WithExtendedOnlyRange_ThrowsArgumentException()
    {
        var range = LanguageRangeParser.Parse("de-*-DE");
        var tags = ParseTags("de-DE");

        Assert.Throws<ArgumentException>(() => range.Lookup(tags));
    }

    [Fact]
    public void Lookup_WithWildcard_ReturnsNull()
    {
        var range = LanguageRangeParser.ParseBasic("*");
        var tags = ParseTags(
            "de-DE",
            "en-US");

        var result = range.Lookup(tags);

        Assert.Null(result);
    }

    [Fact]
    public void Methods_WithNullArguments_ThrowArgumentNullException()
    {
        var range = LanguageRangeParser.ParseBasic("de");
        var tag = Bcp47Parser.Parse("de");

        Assert.Throws<ArgumentNullException>(
            () => LanguageRangeExtensions.IsBasicMatch(null, tag));

        Assert.Throws<ArgumentNullException>(
            () => range.IsBasicMatch(null));

        Assert.Throws<ArgumentNullException>(
            () => LanguageRangeExtensions.IsExtendedMatch(null, tag));

        Assert.Throws<ArgumentNullException>(
            () => range.IsExtendedMatch(null));

        Assert.Throws<ArgumentNullException>(
            () => LanguageRangeExtensions.BasicFilter(null, [tag]));

        Assert.Throws<ArgumentNullException>(
            () => range.BasicFilter(null));

        Assert.Throws<ArgumentNullException>(
            () => LanguageRangeExtensions.ExtendedFilter(null, [tag]));

        Assert.Throws<ArgumentNullException>(
            () => range.ExtendedFilter(null));

        Assert.Throws<ArgumentNullException>(
            () => LanguageRangeExtensions.Lookup(null, [tag]));

        Assert.Throws<ArgumentNullException>(
            () => range.Lookup(null));
    }

    private static LanguageTag[] ParseTags(params string[] values)
    {
        var result = new LanguageTag[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            result[i] = Bcp47Parser.Parse(values[i]);
        }

        return result;
    }
}