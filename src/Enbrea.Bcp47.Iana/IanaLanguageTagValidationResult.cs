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

namespace Enbrea.Bcp47.Iana;

/// <summary>
/// Represents the result of an IANA-aware BCP 47 validation.
/// </summary>
/// <param name="IsValid">Indicates whether the validation was successful.</param>
/// <param name="Error">The validation error, if any.</param>
/// <param name="Subtag">The subtag that was validated.</param>
/// <param name="SubtagType">The type of the subtag.</param>
public readonly record struct IanaLanguageTagValidationResult(
    bool IsValid,
    IanaLanguageTagValidationError Error,
    string Subtag = null,
    IanaLanguageSubtagType? SubtagType = null)
{
    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static IanaLanguageTagValidationResult Success { get; } =
        new(true, IanaLanguageTagValidationError.None);
}
