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

namespace Enbrea.Bcp47.Iana;

/// <summary>
/// Provides IANA Language Subtag Registry-aware validation for BCP 47 language tags.
/// </summary>
/// <remarks>
/// Validation verifies that the language, extlang, script, region, and variant subtags occur in the embedded registry 
/// snapshot. Private-use subtags are not registered and extension-specific rules are outside the scope of the Language 
/// Subtag Registry. Registry Prefix and Suppress-Script fields are usage guidance and are therefore not treated as validity 
/// requirements.
/// </remarks>
public static class IanaLanguageTagValidator
{
	/// <summary>
	/// Determines whether a language tag is deprecated according to the embedded IANA Language Subtag Registry snapshot.
	/// </summary>
	/// <param name="languageTag">The language tag to check for deprecation.</param>
    /// <returns><c>true</c> if the language tag is deprecated; otherwise, <c>false</c>.</returns>
	public static bool IsDeprecated(LanguageTag languageTag)
    {
        ArgumentNullException.ThrowIfNull(languageTag);

        // Private-use-only tags do not contain registry-controlled subtags.
        if (languageTag.IsPrivateUseOnly)
        {
            return false;
        }

        // Grandfathered tags are registered as complete tags rather than as components.
        if (languageTag.IsGrandfathered)
        {
            return IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Grandfathered, languageTag.ToString());
        }

        // Redundant records are complete tags and can be deprecated independently from the individual subtags that compose them.
        if (IanaLanguageSubtagRegistry.ContainsRedundantTag(languageTag.ToString()) &&
            IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Redundant, languageTag.ToString()))
        {
            return true;
        }

        // Validate individual subtags against the registry for deprecation.
        if (languageTag.Language is not null && 
            IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Language, languageTag.Language))
        {
            return true;
        }

        // Validate extlangs against the registry for deprecation.
        foreach (var extlang in languageTag.Extlangs)
        {
            if (IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Extlang, extlang))
            {
                return true;
            }
        }

        // Validate script against the registry for deprecation.
        if (languageTag.Script is not null && 
            IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Script, languageTag.Script))
        {
            return true;
        }

        // Validate region against the registry for deprecation.
        if (languageTag.Region is not null && 
            IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Region, languageTag.Region))
        {
            return true;
        }

        // Validate variants against the registry for deprecation.
        foreach (var variant in languageTag.Variants)
        {
            if (IanaLanguageSubtagRegistry.IsDeprecated(IanaLanguageSubtagType.Variant, variant))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether a language tag is structurally valid and valid against the embedded IANA Language 
    /// Subtag Registry snapshot.
    /// </summary>
    /// <param name="value">The language tag to validate.</param>
    /// <returns><c>true</c> if the language tag is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Determines whether a parsed language tag is valid against the embedded IANA Language 
    /// Subtag Registry snapshot.
    /// </summary>
    /// <param name="languageTag">The parsed language tag to validate.</param>
    /// <returns><c>true</c> if the language tag is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValid(LanguageTag languageTag)
    {
        ArgumentNullException.ThrowIfNull(languageTag);

        return Validate(languageTag).IsValid;
    }

    /// <summary>
    /// Validates a language tag against BCP 47 structural rules and the embedded IANA Language 
    /// Subtag Registry snapshot.
    /// </summary>
    /// <param name="value">The language tag to validate.</param>
    /// <returns>The result of the validation.</returns>
    public static IanaLanguageTagValidationResult Validate(string value)
    {
        if (!Bcp47Parser.TryParse(value, out var languageTag))
        {
            return new IanaLanguageTagValidationResult(false, IanaLanguageTagValidationError.StructurallyInvalid);
        }

        return Validate(languageTag);
    }

	/// <summary>
	/// Validates a parsed language tag against the embedded IANA Language Subtag Registry snapshot.
	/// </summary>
	/// <param name="languageTag">The parsed language tag to validate.</param>
	/// <returns>The result of the validation.</returns>
	public static IanaLanguageTagValidationResult Validate(LanguageTag languageTag)
    {
        ArgumentNullException.ThrowIfNull(languageTag);

        // Private-use-only tags do not contain registry-controlled subtags.
        if (languageTag.IsPrivateUseOnly)
        {
            return IanaLanguageTagValidationResult.Success;
        }

        // Grandfathered tags are registered as complete tags rather than as components.
        if (languageTag.IsGrandfathered)
        {
            return IanaLanguageSubtagRegistry.ContainsGrandfatheredTag(languageTag.ToString())
                ? IanaLanguageTagValidationResult.Success
                : Failure(
                    IanaLanguageTagValidationError.UnregisteredGrandfatheredTag,
                    languageTag.ToString(),
                    IanaLanguageSubtagType.Grandfathered);
        }

        // Validate language against the registry.
        if (languageTag.Language is not null &&
            !IanaLanguageSubtagRegistry.ContainsLanguage(languageTag.Language))
        {
            return Failure(
                IanaLanguageTagValidationError.UnregisteredLanguage,
                languageTag.Language,
                IanaLanguageSubtagType.Language);
        }

        // Validate extlangs against the registry.
        foreach (var extlang in languageTag.Extlangs)
        {
            if (!IanaLanguageSubtagRegistry.ContainsExtlang(extlang))
            {
                return Failure(
                    IanaLanguageTagValidationError.UnregisteredExtlang,
                    extlang,
                    IanaLanguageSubtagType.Extlang);
            }
        }

        // Validate script against the registry.
        if (languageTag.Script is not null &&
            !IanaLanguageSubtagRegistry.ContainsScript(languageTag.Script))
        {
            return Failure(
                IanaLanguageTagValidationError.UnregisteredScript,
                languageTag.Script,
                IanaLanguageSubtagType.Script);
        }

        // Validate region against the registry.
        if (languageTag.Region is not null &&
            !IanaLanguageSubtagRegistry.ContainsRegion(languageTag.Region))
        {
            return Failure(
                IanaLanguageTagValidationError.UnregisteredRegion,
                languageTag.Region,
                IanaLanguageSubtagType.Region);
        }

        // Validate variants against the registry.
        foreach (var variant in languageTag.Variants)
        {
            if (!IanaLanguageSubtagRegistry.ContainsVariant(variant))
            {
                return Failure(
                    IanaLanguageTagValidationError.UnregisteredVariant,
                    variant,
                    IanaLanguageSubtagType.Variant);
            }
        }

        return IanaLanguageTagValidationResult.Success;
    }

    /// <summary>
    /// Creates a validation result indicating failure with the specified error, subtag, and subtag type.
    /// </summary>
    private static IanaLanguageTagValidationResult Failure(IanaLanguageTagValidationError error, string subtag, IanaLanguageSubtagType subtagType)
    {
        return new IanaLanguageTagValidationResult(false, error, subtag, subtagType);
    }
}
