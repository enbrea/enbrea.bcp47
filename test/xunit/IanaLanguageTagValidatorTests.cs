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
/// Unit tests for <see cref="IanaLanguageTagValidator"/>.
/// </summary>
public class IanaLanguageTagValidatorTests
{
    [Theory]
    [InlineData("de")]
    [InlineData("de-DE")]
    [InlineData("x-company")]
    [InlineData("qaa-Qaaa-QM")]
    public void IsDeprecated_WithCurrentOrPrivateUseTag_ReturnsFalse(string value)
    {
        var languageTag = Bcp47Parser.Parse(value);

        Assert.False(IanaLanguageTagValidator.IsDeprecated(languageTag));
    }

    [Theory]
    [InlineData("iw")]
    [InlineData("en-BU")]
    [InlineData("i-klingon")]
    [InlineData("zh-cmn")]
    public void IsDeprecated_WithDeprecatedTagOrComponent_ReturnsTrue(string value)
    {
        var languageTag = Bcp47Parser.Parse(value);

        Assert.True(IanaLanguageTagValidator.IsDeprecated(languageTag));
    }

    [Theory]
    [InlineData("i-klingon")]
    [InlineData("en-GB-oed")]
    [InlineData("zh-min-nan")]
    public void IsValid_WithGrandfatheredTag_ReturnsTrue(string value)
    {
        Assert.True(IanaLanguageTagValidator.IsValid(value));
    }

    [Theory]
    [InlineData("x-company")]
    [InlineData("x-company-internal")]
    public void IsValid_WithPrivateUseOnlyTag_ReturnsTrue(string value)
    {
        Assert.True(IanaLanguageTagValidator.IsValid(value));
    }

    [Theory]
    [InlineData("de")]
    [InlineData("de-DE")]
    [InlineData("zh-Hans-CN")]
    [InlineData("zh-cmn-Hans-CN")]
    [InlineData("sl-rozaj-biske")]
    [InlineData("de-CH-1901")]
    [InlineData("en-u-ca-gregory")]
    [InlineData("de-DE-x-company")]
    public void IsValid_WithRegisteredTag_ReturnsTrue(string value)
    {
        Assert.True(IanaLanguageTagValidator.IsValid(value));
    }

    [Theory]
    [InlineData("qaa-Qaaa-QM-x-southern")]
    [InlineData("de-Qaaa")]
    [InlineData("sr-Latn-QM")]
    [InlineData("sr-Qaaa-RS")]
    public void IsValid_WithRfcPrivateUseRegistryValues_ReturnsTrue(string value)
    {
        Assert.True(IanaLanguageTagValidator.IsValid(value));
    }

    [Fact]
    public void LanguageTagExtensions_DelegateToIanaValidator()
    {
        var valid = Bcp47Parser.Parse("de-DE");
        var invalid = Bcp47Parser.Parse("de-Abcd");
        var deprecated = Bcp47Parser.Parse("iw");

        Assert.True(valid.IsValidAgainstIanaRegistry());
        Assert.False(invalid.IsValidAgainstIanaRegistry());
        Assert.Equal(
            IanaLanguageTagValidationError.UnregisteredScript,
            invalid.ValidateAgainstIanaRegistry().Error);
        Assert.True(deprecated.IsDeprecated());
    }

    [Fact]
    public void Validate_WithNullParsedTag_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            IanaLanguageTagValidator.Validate((LanguageTag)null));
    }

    [Fact]
    public void Validate_WithParsedTag_ReturnsSuccess()
    {
        var languageTag = Bcp47Parser.Parse("DE-de");

        var result = IanaLanguageTagValidator.Validate(languageTag);

        Assert.True(result.IsValid);
        Assert.Equal(IanaLanguageTagValidationError.None, result.Error);
        Assert.Null(result.Subtag);
        Assert.Null(result.SubtagType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("de--DE")]
    [InlineData("en-a")]
    [InlineData("sl-rozaj-rozaj")]
    public void Validate_WithStructurallyInvalidTag_ReturnsStructuralError(string value)
    {
        var result = IanaLanguageTagValidator.Validate(value);

        Assert.False(result.IsValid);
        Assert.Equal(IanaLanguageTagValidationError.StructurallyInvalid, result.Error);
        Assert.Null(result.Subtag);
        Assert.Null(result.SubtagType);
    }

    [Theory]
    [InlineData("zzzzzzzz", IanaLanguageTagValidationError.UnregisteredLanguage, "zzzzzzzz", IanaLanguageSubtagType.Language)]
    [InlineData("zh-zzz", IanaLanguageTagValidationError.UnregisteredExtlang, "zzz", IanaLanguageSubtagType.Extlang)]
    [InlineData("de-Abcd", IanaLanguageTagValidationError.UnregisteredScript, "Abcd", IanaLanguageSubtagType.Script)]
    [InlineData("de-999", IanaLanguageTagValidationError.UnregisteredRegion, "999", IanaLanguageSubtagType.Region)]
    [InlineData("de-abcde", IanaLanguageTagValidationError.UnregisteredVariant, "abcde", IanaLanguageSubtagType.Variant)]
    public void Validate_WithUnregisteredComponent_ReturnsExpectedError(string value, IanaLanguageTagValidationError expectedError, string expectedSubtag, IanaLanguageSubtagType expectedType)
    {
        var result = IanaLanguageTagValidator.Validate(value);

        Assert.False(result.IsValid);
        Assert.Equal(expectedError, result.Error);
        Assert.Equal(expectedSubtag, result.Subtag);
        Assert.Equal(expectedType, result.SubtagType);
    }
}
