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
/// Unit tests for <see cref="Bcp47Parser"/>.
/// </summary>
public class Bcp47ParserTests
{
    [Theory]
    [InlineData("en-a-aaa-A-bbb")]
    [InlineData("en-U-ca-gregory-u-nu-latn")]
    public void IsStructurallyValid_WithCaseInsensitiveDuplicateExtensionSingleton_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("sl-rozaj-ROZAJ")]
    [InlineData("de-1901-1901")]
    public void IsStructurallyValid_WithCaseInsensitiveDuplicateVariant_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("en-a-aaa-a-bbb")]
    [InlineData("en-u-ca-gregory-u-nu-latn")]
    public void IsStructurallyValid_WithDuplicateExtensionSingleton_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("sl-rozaj-rozaj")]
    [InlineData("de-1901-1901")]
    public void IsStructurallyValid_WithDuplicateVariant_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("en-u-ca-gregory")]
    [InlineData("de-DE-u-co-phonebk")]
    [InlineData("en-a-aaa")]
    [InlineData("en-a-aaa-b-bbb")]
    [InlineData("zh-Hans-CN-u-ca-chinese")]
    public void IsStructurallyValid_WithExtensions_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("zh-cmn")]
    [InlineData("zh-cmn-Hans")]
    [InlineData("zh-cmn-Hans-CN")]
    [InlineData("zh-cmn-Hans-CN-variant")]
    public void IsStructurallyValid_WithExtlang_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("en-GB-oed")]
    [InlineData("i-ami")]
    [InlineData("i-bnn")]
    [InlineData("i-default")]
    [InlineData("i-enochian")]
    [InlineData("i-hak")]
    [InlineData("i-klingon")]
    [InlineData("i-lux")]
    [InlineData("i-mingo")]
    [InlineData("i-navajo")]
    [InlineData("i-pwn")]
    [InlineData("i-tao")]
    [InlineData("i-tay")]
    [InlineData("i-tsu")]
    [InlineData("sgn-BE-FR")]
    [InlineData("sgn-BE-NL")]
    [InlineData("sgn-CH-DE")]
    [InlineData("art-lojban")]
    [InlineData("cel-gaulish")]
    [InlineData("no-bok")]
    [InlineData("no-nyn")]
    [InlineData("zh-guoyu")]
    [InlineData("zh-hakka")]
    [InlineData("zh-min")]
    [InlineData("zh-min-nan")]
    [InlineData("zh-xiang")]
    public void IsStructurallyValid_WithGrandfatheredTag_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("x")]
    [InlineData("en-x")]
    [InlineData("en-u")]
    [InlineData("en-a")]
    public void IsStructurallyValid_WithIncompleteSequence_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("en-abcdefghi")]
    [InlineData("x-abcdefghi")]
    public void IsStructurallyValid_WithInvalidSubtagLength_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("de-DE-abcd")]
    [InlineData("de-Latn-abcd")]
    [InlineData("de-DE-abc")]
    [InlineData("de-DE-abcdefghi")]
    public void IsStructurallyValid_WithInvalidVariant_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData("de-")]
    [InlineData("-de")]
    [InlineData("de--DE")]
    [InlineData("1")]
    [InlineData("a")]
    [InlineData("abcdefghi")]
    [InlineData("de_DE")]
    [InlineData("de DE")]
    [InlineData("de.DE")]
    [InlineData("ä")]
    public void IsStructurallyValid_WithMalformedTag_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("aaa-bbb-ccc")]
    [InlineData("aaa-bbb-ccc-ddd")]
    public void IsStructurallyValid_WithMultipleExtlangs_ReturnsFalse(string value)
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(value));
    }

    [Fact]
    public void IsStructurallyValid_WithNull_ReturnsFalse()
    {
        Assert.False(Bcp47Parser.IsStructurallyValid(null));
    }

    [Theory]
    [InlineData("en-0-abc")]
    [InlineData("de-9-abcdefgh")]
    public void IsStructurallyValid_WithNumericExtensionSingleton_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("x-company")]
    [InlineData("x-company-internal")]
    [InlineData("x-foo-123")]
    [InlineData("en-x-company")]
    [InlineData("de-DE-x-company-internal")]
    public void IsStructurallyValid_WithPrivateUse_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("de")]
    [InlineData("en")]
    [InlineData("de-DE")]
    [InlineData("en-US")]
    [InlineData("es-419")]
    [InlineData("zh-Hans")]
    [InlineData("zh-Hans-CN")]
    [InlineData("sr-Latn-RS")]
    [InlineData("sl-rozaj")]
    [InlineData("sl-rozaj-biske")]
    [InlineData("de-CH-1901")]
    public void IsStructurallyValid_WithValidTag_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("de-1901")]
    [InlineData("sl-rozaj")]
    [InlineData("sl-1994")]
    [InlineData("sl-rozaj-biske")]
    public void IsStructurallyValid_WithValidVariant_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsStructurallyValid(value));
    }

    [Theory]
    [InlineData("en-a-aaa-a-bbb")]
    [InlineData("en-u-ca-gregory-u-nu-latn")]
    public void IsWellFormed_WithDuplicateExtensionSingleton_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsWellFormed(value));
    }

    [Theory]
    [InlineData("sl-rozaj-rozaj")]
    [InlineData("de-1901-1901")]
    public void IsWellFormed_WithDuplicateVariant_ReturnsTrue(string value)
    {
        // Duplicate variants still satisfy the RFC 5646 ABNF.
        Assert.True(Bcp47Parser.IsWellFormed(value));
    }

    [Theory]
    [InlineData("aaa-bbb-ccc")]
    [InlineData("aaa-bbb-ccc-ddd")]
    public void IsWellFormed_WithMultipleExtlangs_ReturnsTrue(string value)
    {
        Assert.True(Bcp47Parser.IsWellFormed(value));
    }

    [Fact]
    public void IsWellFormed_WithNull_ReturnsFalse()
    {
        Assert.False(Bcp47Parser.IsWellFormed(null));
    }

    [Theory]
    [InlineData("de", "DE")]
    [InlineData("de-DE", "DE-de")]
    [InlineData("zh-Hans-CN", "ZH-hans-cn")]
    [InlineData("x-company", "X-COMPANY")]
    public void Normalize_CaseVariantsProduceSameResult(string first, string second)
    {
        Assert.Equal(
            Bcp47Parser.Normalize(first),
            Bcp47Parser.Normalize(second));
    }

    [Theory]
    [InlineData("de")]
    [InlineData("de-DE")]
    [InlineData("zh-Hans-CN")]
    [InlineData("de-CH-1901")]
    [InlineData("en-u-ca-gregory")]
    [InlineData("x-company-internal")]
    public void Normalize_IsIdempotent(string value)
    {
        var first = Bcp47Parser.Normalize(value);
        var second = Bcp47Parser.Normalize(first);

        Assert.Equal(first, second);
    }

    [Theory]
    [InlineData("DE", "de")]
    [InlineData("EN", "en")]
    [InlineData("de-de", "de-DE")]
    [InlineData("DE-de", "de-DE")]
    [InlineData("en-us", "en-US")]
    [InlineData("ES-419", "es-419")]
    [InlineData("ZH-hans", "zh-Hans")]
    [InlineData("ZH-hans-cn", "zh-Hans-CN")]
    [InlineData("SR-latn-rs", "sr-Latn-RS")]
    [InlineData("de-ch-1901", "de-CH-1901")]
    public void Normalize_NormalizesCase(string value, string expected)
    {
        var result = Bcp47Parser.Normalize(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("EN-us-U-CA-gregory", "en-US-u-ca-gregory")]
    [InlineData("DE-de-U-CO-phonebk", "de-DE-u-co-phonebk")]
    [InlineData("en-B-bbb-A-aaa", "en-a-aaa-b-bbb")]
    public void Normalize_WithExtensions_NormalizesAndOrdersExtensions(string value, string expected)
    {
        var result = Bcp47Parser.Normalize(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("zh-CMN", "zh-cmn")]
    [InlineData("ZH-cmn-hans-cn", "zh-cmn-Hans-CN")]
    public void Normalize_WithExtlang_NormalizesCase(string value, string expected)
    {
        var result = Bcp47Parser.Normalize(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("I-KLINGON", "i-klingon")]
    [InlineData("EN-gb-OED", "en-GB-oed")]
    [InlineData("SGN-be-fr", "sgn-BE-FR")]
    [InlineData("ZH-min-NAN", "zh-min-nan")]
    public void Normalize_WithGrandfatheredTag_ReturnsRegisteredCase(string value, string expected)
    {
        var result = Bcp47Parser.Normalize(value);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Normalize_WithInvalidTag_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(
            () => Bcp47Parser.Normalize("de_DE"));
    }

    [Fact]
    public void Normalize_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => Bcp47Parser.Normalize(null!));
    }

    [Theory]
    [InlineData("X-COMPANY", "x-company")]
    [InlineData("X-COMPANY-Internal", "x-company-internal")]
    [InlineData("DE-de-X-COMPANY-Internal", "de-DE-x-company-internal")]
    public void Normalize_WithPrivateUse_NormalizesCase(string value, string expected)
    {
        var result = Bcp47Parser.Normalize(value);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_NormalizesAllComponents()
    {
        var tag = Bcp47Parser.Parse("ZH-CMN-hANS-cn-VARIANT-B-BBB-A-AAA-X-PRIVATE");

        Assert.Equal("zh-cmn-Hans-CN-variant-a-aaa-b-bbb-x-private", tag.ToString());

        Assert.Equal("zh", tag.Language);
        Assert.Equal(["cmn"], tag.Extlangs);
        Assert.Equal("Hans", tag.Script);
        Assert.Equal("CN", tag.Region);
        Assert.Equal(["variant"], tag.Variants);

        Assert.Equal(2, tag.Extensions.Count);
        Assert.Equal('a', tag.Extensions[0].Singleton);
        Assert.Equal(["aaa"], tag.Extensions[0].Subtags);
        Assert.Equal('b', tag.Extensions[1].Singleton);
        Assert.Equal(["bbb"], tag.Extensions[1].Subtags);
        Assert.Equal(["private"], tag.PrivateUse);
    }

    [Fact]
    public void Parse_WithCompleteTag_ReturnsExpectedComponents()
    {
        var tag = Bcp47Parser.Parse("zh-cmn-Hans-CN-variant-u-ca-chinese-x-private");

        Assert.Equal("zh-cmn-Hans-CN-variant-u-ca-chinese-x-private", tag.Value);
        Assert.Equal("zh-cmn-Hans-CN-variant-u-ca-chinese-x-private", tag.ToString());

        Assert.Equal("zh", tag.Language);

        _ = Assert.Single(tag.Extlangs);
        Assert.Equal("cmn", tag.Extlangs[0]);

        Assert.Equal("Hans", tag.Script);
        Assert.Equal("CN", tag.Region);

        _ = Assert.Single(tag.Variants);
        Assert.Equal("variant", tag.Variants[0]);

        _ = Assert.Single(tag.Extensions);
        Assert.Equal('u', tag.Extensions[0].Singleton);
        Assert.Equal(["ca", "chinese"], tag.Extensions[0].Subtags);
        Assert.Equal(["private"], tag.PrivateUse);

        Assert.False(tag.IsGrandfathered);
        Assert.False(tag.IsPrivateUseOnly);
    }

    [Fact]
    public void Parse_WithGrandfatheredTag_ReturnsExpectedComponents()
    {
        var tag = Bcp47Parser.Parse("I-KLINGON");

        Assert.Equal("I-KLINGON", tag.Value);
        Assert.Equal("i-klingon", tag.ToString());

        Assert.Null(tag.Language);
        Assert.Empty(tag.Extlangs);
        Assert.Null(tag.Script);
        Assert.Null(tag.Region);
        Assert.Empty(tag.Variants);
        Assert.Empty(tag.Extensions);
        Assert.Empty(tag.PrivateUse);

        Assert.True(tag.IsGrandfathered);
        Assert.False(tag.IsPrivateUseOnly);
    }

    [Fact]
    public void Parse_WithInvalidTag_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(
            () => Bcp47Parser.Parse("de_DE"));
    }

    [Fact]
    public void Parse_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => Bcp47Parser.Parse(null!));
    }

    [Fact]
    public void Parse_WithPrivateUseOnly_ReturnsExpectedComponents()
    {
        var tag = Bcp47Parser.Parse("X-COMPANY-Internal");

        Assert.Equal("X-COMPANY-Internal", tag.Value);
        Assert.Equal("x-company-internal", tag.ToString());

        Assert.Null(tag.Language);
        Assert.Empty(tag.Extlangs);
        Assert.Null(tag.Script);
        Assert.Null(tag.Region);
        Assert.Empty(tag.Variants);
        Assert.Empty(tag.Extensions);
        Assert.Equal(["company", "internal"], tag.PrivateUse);
        Assert.False(tag.IsGrandfathered);
        Assert.True(tag.IsPrivateUseOnly);
    }

    [Fact]
    public void ToString_ReturnsNormalizedTag()
    {
        var tag = Bcp47Parser.Parse("ZH-hans-cn");

        Assert.Equal("zh-Hans-CN", tag.ToString());
    }

    [Fact]
    public void TryNormalize_WithInvalidTag_ReturnsFalse()
    {
        var success = Bcp47Parser.TryNormalize("de_DE", out var normalized);

        Assert.False(success);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void TryNormalize_WithNull_ReturnsFalse()
    {
        var success = Bcp47Parser.TryNormalize(null, out var normalized);

        Assert.False(success);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void TryNormalize_WithValidTag_ReturnsTrue()
    {
        var success = Bcp47Parser.TryNormalize("ZH-hans-cn", out var normalized);

        Assert.True(success);
        Assert.Equal("zh-Hans-CN", normalized);
    }

    [Fact]
    public void TryParse_WithInvalidTag_ReturnsFalse()
    {
        var success = Bcp47Parser.TryParse("de_DE", out var tag);

        Assert.False(success);
        Assert.Null(tag);
    }

    [Fact]
    public void TryParse_WithNull_ReturnsFalse()
    {
        var success = Bcp47Parser.TryParse(null!, out var tag);

        Assert.False(success);
        Assert.Null(tag);
    }

    [Fact]
    public void TryParse_WithValidTag_ReturnsTrue()
    {
        var success = Bcp47Parser.TryParse("de-DE", out var tag);

        Assert.True(success);
        Assert.NotNull(tag);
        Assert.Equal("de", tag.Language);
        Assert.Equal("DE", tag.Region);
        Assert.Equal("de-DE", tag.ToString());
    }
}