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
/// Provides IANA registry-aware extension methods for <see cref="LanguageTag"/>.
/// </summary>
public static class LanguageTagExtensions
{
    /// <summary>
    /// Determines whether the tag contains a registry entry marked as deprecated.
    /// </summary>
    /// <param name="languageTag">The language tag to check.</param>
    /// <returns><c>true</c> if the language tag is deprecated; otherwise, <c>false</c>.</returns>
    public static bool IsDeprecated(this LanguageTag languageTag)
    {
        ArgumentNullException.ThrowIfNull(languageTag);

        return IanaLanguageTagValidator.IsDeprecated(languageTag);
    }

    /// <summary>
    /// Determines whether all applicable components are registered in the embedded IANA Language Subtag Registry snapshot.
    /// </summary>
    /// <param name="languageTag">The language tag to validate.</param>
    /// <returns><c>true</c> if the language tag is valid against the IANA Language Subtag Registry; otherwise, <c>false</c>.</returns>
    public static bool IsValidAgainstIanaRegistry(this LanguageTag languageTag)
    {
        ArgumentNullException.ThrowIfNull(languageTag);

        return IanaLanguageTagValidator.IsValid(languageTag);
    }

    /// <summary>
    /// Validates the tag against the embedded IANA Language Subtag Registry snapshot.
    /// </summary>
    /// <param name="languageTag">The language tag to validate.</param>
    /// <returns>An <see cref="IanaLanguageTagValidationResult"/> representing the validation result.</returns>
    public static IanaLanguageTagValidationResult ValidateAgainstIanaRegistry(this LanguageTag languageTag)
    {
        ArgumentNullException.ThrowIfNull(languageTag);

        return IanaLanguageTagValidator.Validate(languageTag);
    }
}
