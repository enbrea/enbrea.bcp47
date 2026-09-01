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
using System.Collections.Generic;

namespace Enbrea.Bcp47;

/// <summary>
/// Represents a parsed BCP 47 language tag.
/// </summary>
/// <remarks>
/// Parsed components are exposed in normalized form. <see cref="Value"/> preserves
/// the original input, while <see cref="ToString()"/> returns the normalized tag.
/// </remarks>
public sealed class LanguageTag : IEquatable<LanguageTag>
{
    private readonly string _normalizedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageTag"/> class.
    /// </summary>
    internal LanguageTag(
        string value,
        string normalizedValue,
        string language,
        IReadOnlyList<string> extlangs,
        string script,
        string region,
        IReadOnlyList<string> variants,
        IReadOnlyList<LanguageTagExtension> extensions,
        IReadOnlyList<string> privateUse,
        bool isGrandfathered)
    {
        Value = value;
        Language = language;
        Extlangs = extlangs;
        Script = script;
        Region = region;
        Variants = variants;
        Extensions = extensions;
        PrivateUse = privateUse;
        IsGrandfathered = isGrandfathered;

        _normalizedValue = normalizedValue;
    }

    /// <summary>
    /// Gets the extension sequences.
    /// </summary>
    public IReadOnlyList<LanguageTagExtension> Extensions { get; }

    /// <summary>
    /// Gets the extended language subtags.
    /// </summary>
    public IReadOnlyList<string> Extlangs { get; }

    /// <summary>
    /// Gets a value indicating whether this is a grandfathered language tag.
    /// </summary>
    public bool IsGrandfathered { get; }

    /// <summary>
    /// Gets a value indicating whether the language tag consists entirely
    /// of private-use subtags.
    /// </summary>
    public bool IsPrivateUseOnly =>
        !IsGrandfathered &&
        Language is null &&
        PrivateUse.Count > 0;

    /// <summary>
    /// Gets the primary language subtag, or <c>null</c> if the tag is
    /// grandfathered or consists entirely of private-use subtags.
    /// </summary>
    public string Language { get; }

    /// <summary>
    /// Gets the private-use subtags.
    /// </summary>
    public IReadOnlyList<string> PrivateUse { get; }

    /// <summary>
    /// Gets the region subtag.
    /// </summary>
    public string Region { get; }

    /// <summary>
    /// Gets the script subtag.
    /// </summary>
    public string Script { get; }

    /// <summary>
    /// Gets the original language tag.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the variant subtags.
    /// </summary>
    public IReadOnlyList<string> Variants { get; }

    /// <summary>
    /// Determines whether the specified BCP 47 language tag string is structurally valid.
    /// </summary>
    /// <param name="value">The BCP 47 language tag string to check.</param>
    /// <returns><c>true</c> if the specified language tag string is structurally valid; otherwise, <c>false</c>.</returns>
    public static bool IsStructurallyValid(string value)
    {
        return Bcp47Parser.IsStructurallyValid(value);
    }

    /// <summary>
    /// Determines whether the specified BCP 47 language tag string is well-formed.
    /// </summary>
    /// <param name="value">The BCP 47 language tag string to check.</param>
    /// <returns><c>true</c> if the specified language tag string is well-formed; otherwise, <c>false</c>.</returns>
    public static bool IsWellFormed(string value)
    {
        return Bcp47Parser.IsWellFormed(value);
    }

    /// <summary>
    /// Normalizes the specified BCP 47 language tag string to its registry-independent normalized form.
    /// </summary>
    /// <param name="value">The BCP 47 language tag string to normalize.</param>
    /// <returns>The normalized BCP 47 language tag string.</returns>
    public static string Normalize(string value)
    {
        return Bcp47Parser.Normalize(value);
    }

    /// <summary>
    /// Indicates whether two <see cref="LanguageTag"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="LanguageTag"/> to compare.</param>
    /// <param name="right">The second <see cref="LanguageTag"/> to compare.</param>
    /// <returns><c>true</c> if the two <see cref="LanguageTag"/> instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(LanguageTag left, LanguageTag right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Indicates whether two <see cref="LanguageTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="LanguageTag"/> to compare.</param>
    /// <param name="right">The second <see cref="LanguageTag"/> to compare.</param>
    /// <returns><c>true</c> if the two <see cref="LanguageTag"/> instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(LanguageTag left, LanguageTag right)
    {
        return EqualityComparer<LanguageTag>.Default.Equals(left, right);
    }

    /// <summary>
    /// Parses a BCP 47 language tag string into a <see cref="LanguageTag"/> instance.
    /// </summary>
    /// <param name="value">The BCP 47 language tag string to parse.</param>
    /// <returns>A <see cref="LanguageTag"/> instance representing the parsed language tag.</returns>
    public static LanguageTag Parse(string value)
    {
        return Bcp47Parser.Parse(value);
    }

    /// <summary>
    /// Tries to normalize the specified BCP 47 language tag string to its registry-independent normalized form.
    /// </summary>
    /// <param name="value">The BCP 47 language tag string to normalize.</param>
    /// <param name="normalized">When this method returns, contains the normalized BCP 47 language tag string
    /// if normalization succeeded; otherwise, an empty string.</param>
    /// <returns><c>true</c> if the normalization succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryNormalize(string value, out string normalized)
    {
        return Bcp47Parser.TryNormalize(value, out normalized);
    }

    /// <summary>
    /// Tries to parse a BCP 47 language tag string into a <see cref="LanguageTag"/> instance.
    /// </summary>
    /// <param name="value">The BCP 47 language tag string to parse.</param>
    /// <param name="languageTag">When this method returns, contains the parsed <see cref="LanguageTag"/> if the parse 
    /// succeeded, or <c>null</c> if the parse failed.</param>
    /// <returns><c>true</c> if the parse succeeded; otherwise, <c>false</c>.</returns> 
    public static bool TryParse(string value, out LanguageTag languageTag)
    {
        return Bcp47Parser.TryParse(value, out languageTag);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(LanguageTag other)
    {
        return 
            other is not null && 
            (ReferenceEquals(this, other) || string.Equals(_normalizedValue, other._normalizedValue, StringComparison.Ordinal));
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current language tag.
    /// </summary>
    /// <param name="obj">The object to compare with the current language tag.</param>
    /// <returns><c>true</c> if the specified object is an equivalent language tag; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return obj is LanguageTag other && Equals(other);
    }

    /// <summary>
    /// Returns a hash code for the current language tag.
    /// </summary>
    /// <returns>A hash code for the current language tag.</returns>
    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(_normalizedValue);
    }

    /// <summary>
    /// Returns a string that represents the normalized BCP 47 language tag.
    /// </summary>
    /// <returns>The normalized BCP 47 language tag.</returns>
    public override string ToString()
    {
        return _normalizedValue;
    }
}
