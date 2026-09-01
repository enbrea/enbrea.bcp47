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

namespace Enbrea.Bcp47.Iana.XUnit;

/// <summary>
/// Unit tests for <see cref="IanaLanguageSubtagRegistry"/>.
/// </summary>
public class IanaLanguageSubtagRegistryTests
{
    [Theory]
    [InlineData(IanaLanguageSubtagType.Language, "DE")]
    [InlineData(IanaLanguageSubtagType.Extlang, "CMN")]
    [InlineData(IanaLanguageSubtagType.Script, "hans")]
    [InlineData(IanaLanguageSubtagType.Region, "de")]
    [InlineData(IanaLanguageSubtagType.Grandfathered, "I-KLINGON")]
    [InlineData(IanaLanguageSubtagType.Redundant, "ZH-CMN")]
    public void Contains_IsCaseInsensitive(IanaLanguageSubtagType type, string value)
    {
        Assert.True(IanaLanguageSubtagRegistry.Contains(type, value));
    }

    [Theory]
    [InlineData(IanaLanguageSubtagType.Language, "de")]
    [InlineData(IanaLanguageSubtagType.Extlang, "cmn")]
    [InlineData(IanaLanguageSubtagType.Script, "Hans")]
    [InlineData(IanaLanguageSubtagType.Region, "DE")]
    [InlineData(IanaLanguageSubtagType.Variant, "1901")]
    [InlineData(IanaLanguageSubtagType.Grandfathered, "i-klingon")]
    [InlineData(IanaLanguageSubtagType.Redundant, "zh-cmn")]
    public void Contains_WithRegisteredValue_ReturnsTrue(IanaLanguageSubtagType type, string value)
    {
        Assert.True(IanaLanguageSubtagRegistry.Contains(type, value));
    }

    [Theory]
    [InlineData("qaa..qtz")]
    [InlineData("Qaaa..Qabx")]
    [InlineData("QM..QZ")]
    [InlineData("XA..XZ")]
    public void Contains_WithRegistryRangeExpression_ReturnsFalse(string value)
    {
        Assert.False(IanaLanguageSubtagRegistry.ContainsLanguage(value));
        Assert.False(IanaLanguageSubtagRegistry.ContainsScript(value));
        Assert.False(IanaLanguageSubtagRegistry.ContainsRegion(value));
    }

    [Fact]
    public void Contains_WithUnknownType_ReturnsFalse()
    {
        Assert.False(IanaLanguageSubtagRegistry.Contains((IanaLanguageSubtagType)999, "de"));
    }

    [Fact]
    public void Contains_WithUnknownValue_ReturnsFalse()
    {
        Assert.False(IanaLanguageSubtagRegistry.ContainsLanguage("zzzzzzzz"));
        Assert.False(IanaLanguageSubtagRegistry.ContainsLanguage("qa["));
        Assert.False(IanaLanguageSubtagRegistry.ContainsExtlang("zzz"));
        Assert.False(IanaLanguageSubtagRegistry.ContainsScript("Abcd"));
        Assert.False(IanaLanguageSubtagRegistry.ContainsScript("Qaa1"));
        Assert.False(IanaLanguageSubtagRegistry.ContainsRegion("999"));
        Assert.False(IanaLanguageSubtagRegistry.ContainsVariant("abcde"));
    }

    [Theory]
    [InlineData("qaa")]
    [InlineData("qaz")]
    [InlineData("qtz")]
    [InlineData("QAA")]
    public void ContainsLanguage_WithPrivateUseRange_ReturnsTrue(string value)
    {
        Assert.True(IanaLanguageSubtagRegistry.ContainsLanguage(value));
    }

    [Theory]
    [InlineData("QM")]
    [InlineData("QZ")]
    [InlineData("XA")]
    [InlineData("XZ")]
    [InlineData("qm")]
    public void ContainsRegion_WithPrivateUseRange_ReturnsTrue(string value)
    {
        Assert.True(IanaLanguageSubtagRegistry.ContainsRegion(value));
    }

    [Theory]
    [InlineData("Qaaa")]
    [InlineData("Qaaz")]
    [InlineData("Qabx")]
    [InlineData("qaaa")]
    public void ContainsScript_WithPrivateUseRange_ReturnsTrue(string value)
    {
        Assert.True(IanaLanguageSubtagRegistry.ContainsScript(value));
    }

    [Fact]
    public void FileDate_ReturnsGeneratedRegistryDate()
    {
        Assert.Equal(new DateOnly(2026, 8, 8), IanaLanguageSubtagRegistry.FileDate);
    }
    
    [Fact]
    public void GetVariantPrefixes_WithMultiplePrefixes_ReturnsAllPrefixes()
    {
        var prefixes = IanaLanguageSubtagRegistry.GetVariantPrefixes("1994");

        Assert.Equal(
            [
                "sl-rozaj",
                "sl-rozaj-biske",
                "sl-rozaj-njiva",
                "sl-rozaj-osojs",
                "sl-rozaj-solba"
            ],
            prefixes);
    }

    [Fact]
    public void GetVariantPrefixes_WithUnknownVariant_ReturnsEmptyList()
    {
        var prefixes = IanaLanguageSubtagRegistry.GetVariantPrefixes("abcde");

        Assert.Empty(prefixes);
    }

    [Fact]
    public void IsDeprecated_WithCurrentValue_ReturnsFalse()
    {
        Assert.False(IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Language, "de"));
        Assert.False(IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Region, "DE"));
    }

    [Theory]
    [InlineData(IanaLanguageSubtagType.Language, "iw")]
    [InlineData(IanaLanguageSubtagType.Region, "BU")]
    [InlineData(IanaLanguageSubtagType.Grandfathered, "i-klingon")]
    [InlineData(IanaLanguageSubtagType.Redundant, "zh-cmn")]
    public void IsDeprecated_WithDeprecatedValue_ReturnsTrue(IanaLanguageSubtagType type, string value)
    {
        Assert.True(IanaLanguageSubtagRegistry.IsDeprecated(type, value));
    }

    [Fact]
    public void RegistryMethods_WithNull_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => IanaLanguageSubtagRegistry.ContainsLanguage(null));
        Assert.Throws<ArgumentNullException>(() => IanaLanguageSubtagRegistry.GetVariantPrefixes(null));
        Assert.Throws<ArgumentNullException>(() => IanaLanguageSubtagRegistry.TryGetPreferredValue(IanaLanguageSubtagType.Language, null, out _));
    }

    [Fact]
    public void TryGetExtlangMetadata_WithKnownExtlang_ReturnsValues()
    {
        Assert.True(IanaLanguageSubtagRegistry.TryGetExtlangPrefix("cmn", out var prefix));
        Assert.Equal("zh", prefix);

        Assert.True(IanaLanguageSubtagRegistry.TryGetExtlangMacrolanguage("cmn", out var macrolanguage));
        Assert.Equal("zh", macrolanguage);

        Assert.False(IanaLanguageSubtagRegistry.TryGetExtlangSuppressScript("cmn", out var script));
        Assert.Null(script);
    }

    [Fact]
    public void TryGetLanguageMacrolanguage_WithEncompassedLanguage_ReturnsMacrolanguage()
    {
        var result = IanaLanguageSubtagRegistry.TryGetLanguageMacrolanguage("cmn", out var macrolanguage);

        Assert.True(result);
        Assert.Equal("zh", macrolanguage);
    }

    [Fact]
    public void TryGetLanguageSuppressScript_WithKnownLanguage_ReturnsScript()
    {
        var result = IanaLanguageSubtagRegistry.TryGetLanguageSuppressScript("de", out var script);

        Assert.True(result);
        Assert.Equal("Latn", script);
    }

    [Theory]
    [InlineData(IanaLanguageSubtagType.Language, "iw", "he")]
    [InlineData(IanaLanguageSubtagType.Extlang, "cmn", "cmn")]
    [InlineData(IanaLanguageSubtagType.Region, "BU", "MM")]
    [InlineData(IanaLanguageSubtagType.Grandfathered, "i-klingon", "tlh")]
    [InlineData(IanaLanguageSubtagType.Redundant, "zh-cmn", "cmn")]
    public void TryGetPreferredValue_WithMapping_ReturnsValue(IanaLanguageSubtagType type, string value, string expected)
    {
        var result = IanaLanguageSubtagRegistry.TryGetPreferredValue(type, value, out var preferredValue);

        Assert.True(result);
        Assert.Equal(expected, preferredValue);
    }

    [Fact]
    public void TryGetPreferredValue_WithoutMapping_ReturnsFalse()
    {
        var result = IanaLanguageSubtagRegistry.TryGetPreferredValue(IanaLanguageSubtagType.Language, "de", out var preferredValue);

        Assert.False(result);
        Assert.Null(preferredValue);
    }
}
