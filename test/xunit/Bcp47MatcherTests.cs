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
/// Unit tests for <see cref="Bcp47Matcher"/>.
/// </summary>
public class Bcp47MatcherTests
{
    [Theory]
    [InlineData("de", "de", true)]
    [InlineData("de", "de-DE", true)]
    [InlineData("de-DE", "de-DE", true)]
    [InlineData("de-DE", "de-DE-1996", true)]
    [InlineData("de-DE", "de-Deva", false)]
    [InlineData("de-DE", "de-Latn-DE", false)]
    [InlineData("DE-de", "de-DE-1996", true)]
    public void IsBasicMatch_WithRfcExamples_ReturnsExpectedResult(string rangeValue, string tagValue, bool expected)
    {
        var range = LanguageRangeParser.ParseBasic(rangeValue);
        var tag = Bcp47Parser.Parse(tagValue);

        Assert.Equal(expected, Bcp47Matcher.IsBasicMatch(range, tag));
    }

    [Fact]
    public void IsBasicMatch_WithWildcard_MatchesEveryTag()
    {
        var range = LanguageRangeParser.ParseBasic("*");

        Assert.True(Bcp47Matcher.IsBasicMatch(range, Bcp47Parser.Parse("de-DE")));
        Assert.True(Bcp47Matcher.IsBasicMatch(range, Bcp47Parser.Parse("x-private")));
        Assert.True(Bcp47Matcher.IsBasicMatch(range, Bcp47Parser.Parse("i-klingon")));
    }

    [Fact]
    public void IsBasicMatch_WithExtendedOnlyRange_ThrowsArgumentException()
    {
        var range = LanguageRangeParser.Parse("de-*-DE");
        var tag = Bcp47Parser.Parse("de-DE");

        Assert.Throws<ArgumentException>(() => Bcp47Matcher.IsBasicMatch(range, tag));
    }

    [Fact]
    public void BasicFilter_WithOneRange_ReturnsMatchingTagsInInputOrder()
    {
        var range = LanguageRangeParser.ParseBasic("de-DE");
        var tags = ParseTags(
            "de-Latn-DE",
            "de-DE-1996",
            "en-US",
            "de-DE");

        var result = Bcp47Matcher.BasicFilter(range, tags);

        Assert.Equal(2, result.Count);
        Assert.Equal("de-DE-1996", result[0].Value);
        Assert.Equal("de-DE", result[1].Value);
    }

    [Fact]
    public void BasicFilter_WithMultipleRanges_ReturnsEachTagOnlyOnce()
    {
        var ranges = new[]
        {
            LanguageRangeParser.ParseBasic("de"),
            LanguageRangeParser.ParseBasic("de-DE")
        };
        var tags = ParseTags("de-DE", "de-AT", "en-US");

        var result = Bcp47Matcher.BasicFilter(ranges, tags);

        Assert.Equal(2, result.Count);
        Assert.Equal("de-DE", result[0].Value);
        Assert.Equal("de-AT", result[1].Value);
    }

    [Theory]
    [InlineData("de-*-DE", "de-DE", true)]
    [InlineData("de-*-DE", "de-Latn-DE", true)]
    [InlineData("de-*-DE", "de-Latf-DE", true)]
    [InlineData("de-*-DE", "de-DE-x-goethe", true)]
    [InlineData("de-*-DE", "de-Latn-DE-1996", true)]
    [InlineData("de-*-DE", "de-Deva-DE", true)]
    [InlineData("de-*-DE", "de", false)]
    [InlineData("de-*-DE", "de-x-DE", false)]
    [InlineData("de-*-DE", "de-Deva", false)]
    public void IsExtendedMatch_WithRfcExamples_ReturnsExpectedResult(
        string rangeValue,
        string tagValue,
        bool expected)
    {
        var range = LanguageRangeParser.Parse(rangeValue);
        var tag = Bcp47Parser.Parse(tagValue);

        Assert.Equal(expected, Bcp47Matcher.IsExtendedMatch(range, tag));
    }

    [Theory]
    [InlineData("de-DE", "de-Latn-DE", true)]
    [InlineData("de-DE", "de-Deva-DE", true)]
    [InlineData("de-DE", "de-x-DE", false)]
    [InlineData("*-CH", "de-CH", true)]
    [InlineData("*-CH", "fr-Latn-CH", true)]
    [InlineData("*-CH", "it-IT", false)]
    public void IsExtendedMatch_WithImplicitWildcards_ReturnsExpectedResult(
        string rangeValue,
        string tagValue,
        bool expected)
    {
        var range = LanguageRangeParser.Parse(rangeValue);
        var tag = Bcp47Parser.Parse(tagValue);

        Assert.Equal(expected, Bcp47Matcher.IsExtendedMatch(range, tag));
    }

    [Fact]
    public void ExtendedFilter_WithRfcRange_ReturnsExpectedTags()
    {
        var range = LanguageRangeParser.Parse("de-*-DE");
        var tags = ParseTags(
            "de-DE",
            "de-Latn-DE",
            "de-DE-x-goethe",
            "de",
            "de-x-DE",
            "de-Deva");

        var result = Bcp47Matcher.ExtendedFilter(range, tags);

        Assert.Equal(3, result.Count);
        Assert.Equal("de-DE", result[0].Value);
        Assert.Equal("de-Latn-DE", result[1].Value);
        Assert.Equal("de-DE-x-goethe", result[2].Value);
    }

    [Fact]
    public void ExtendedFilter_WithMultipleRanges_ReturnsEachTagOnlyOnce()
    {
        var ranges = new[]
        {
            LanguageRangeParser.Parse("de"),
            LanguageRangeParser.Parse("de-*-DE")
        };
        var tags = ParseTags("de-DE", "de-AT", "en-US");

        var result = Bcp47Matcher.ExtendedFilter(ranges, tags);

        Assert.Equal(2, result.Count);
        Assert.Equal("de-DE", result[0].Value);
        Assert.Equal("de-AT", result[1].Value);
    }

    [Fact]
    public void Lookup_WithExactMatch_ReturnsExactTag()
    {
        var range = LanguageRangeParser.ParseBasic("de-CH");
        var tags = ParseTags("de", "de-CH", "de-CH-1996");

        var result = Bcp47Matcher.Lookup(range, tags);

        Assert.NotNull(result);
        Assert.Equal("de-CH", result.Value);
    }

    [Fact]
    public void Lookup_DoesNotReturnMoreSpecificTag()
    {
        var range = LanguageRangeParser.ParseBasic("de-CH");
        var tags = ParseTags("de-CH-1996", "de");

        var result = Bcp47Matcher.Lookup(range, tags);

        Assert.NotNull(result);
        Assert.Equal("de", result.Value);
    }

    [Fact]
    public void Lookup_WithPrivateUse_FollowsRfcFallbackPattern()
    {
        var range = LanguageRangeParser.ParseBasic("zh-Hant-CN-x-private1-private2");
        var tags = ParseTags(
            "zh",
            "zh-Hant",
            "zh-Hant-CN",
            "zh-Hant-CN-x-private1");

        var result = Bcp47Matcher.Lookup(range, tags);

        Assert.NotNull(result);
        Assert.Equal("zh-Hant-CN-x-private1", result.Value);
    }

    [Fact]
    public void Lookup_WithExtension_RemovesSingletonWithTrailingSubtag()
    {
        var range = LanguageRangeParser.ParseBasic("en-US-u-ca-gregory");
        var tags = ParseTags("en", "en-US", "en-US-u-ca");

        var result = Bcp47Matcher.Lookup(range, tags);

        Assert.NotNull(result);
        Assert.Equal("en-US-u-ca", result.Value);
    }

    [Fact]
    public void Lookup_WithOnlyMoreSpecificTag_ReturnsNull()
    {
        var range = LanguageRangeParser.ParseBasic("de-CH");
        var tags = ParseTags("de-CH-1996");

        var result = Bcp47Matcher.Lookup(range, tags);

        Assert.Null(result);
    }

    [Fact]
    public void Lookup_WithPriorityList_UsesFirstRangeThatProducesMatch()
    {
        var ranges = new[]
        {
            LanguageRangeParser.ParseBasic("de-CH"),
            LanguageRangeParser.ParseBasic("fr-FR"),
            LanguageRangeParser.ParseBasic("en")
        };
        var tags = ParseTags("de", "fr-FR", "en");

        var result = Bcp47Matcher.Lookup(ranges, tags);

        Assert.NotNull(result);
        Assert.Equal("de", result.Value);
    }

    [Fact]
    public void Lookup_WithWildcardBeforeConcreteRange_SkipsWildcard()
    {
        var ranges = new[]
        {
            LanguageRangeParser.ParseBasic("*"),
            LanguageRangeParser.ParseBasic("fr")
        };
        var tags = ParseTags("de", "fr");

        var result = Bcp47Matcher.Lookup(ranges, tags);

        Assert.NotNull(result);
        Assert.Equal("fr", result.Value);
    }

    [Fact]
    public void Lookup_WithOnlyWildcard_ReturnsNull()
    {
        var range = LanguageRangeParser.ParseBasic("*");
        var tags = ParseTags("de", "fr");

        var result = Bcp47Matcher.Lookup(range, tags);

        Assert.Null(result);
    }

    [Fact]
    public void Lookup_WithExtendedOnlyRange_ThrowsArgumentException()
    {
        var range = LanguageRangeParser.Parse("de-*-DE");
        var tags = ParseTags("de-DE");

        Assert.Throws<ArgumentException>(() => Bcp47Matcher.Lookup(range, tags));
    }

    [Fact]
    public void Lookup_IsCaseInsensitive()
    {
        var range = LanguageRangeParser.ParseBasic("DE-ch");
        var tags = ParseTags("de-CH");

        var result = Bcp47Matcher.Lookup(range, tags);

        Assert.NotNull(result);
        Assert.Equal("de-CH", result.Value);
    }

    [Fact]
    public void Methods_WithNullArguments_ThrowArgumentNullException()
    {
        var range = LanguageRangeParser.ParseBasic("de");
        var tag = Bcp47Parser.Parse("de");
        var tags = ParseTags("de");

        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.IsBasicMatch(null, tag));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.IsBasicMatch(range, null));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.IsExtendedMatch(null, tag));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.IsExtendedMatch(range, null));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.BasicFilter(range, null));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.ExtendedFilter(range, null));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.Lookup(range, null));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.BasicFilter((LanguageRange)null, tags));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.ExtendedFilter((LanguageRange)null, tags));
        Assert.Throws<ArgumentNullException>(() => Bcp47Matcher.Lookup((LanguageRange)null, tags));
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
