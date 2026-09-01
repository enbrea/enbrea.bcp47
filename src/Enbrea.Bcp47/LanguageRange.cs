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

using System.Collections.Generic;

namespace Enbrea.Bcp47;

/// <summary>
/// Represents a parsed RFC 4647 language range.
/// </summary>
/// <remarks>
/// Language ranges are case-insensitive. Parsed subtags are exposed in lowercase
/// normalized form, while <see cref="Value"/> preserves the original input.
/// </remarks>
public sealed class LanguageRange
{
    private readonly string _normalizedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageRange"/> class.
    /// </summary>
    internal LanguageRange(
        string value,
        string normalizedValue,
        IReadOnlyList<string> subtags,
        bool isBasic)
    {
        Value = value;
        Subtags = subtags;
        IsBasic = isBasic;

        _normalizedValue = normalizedValue;
    }

    /// <summary>
    /// Gets a value indicating whether the range also conforms to the basic
    /// language-range syntax defined by RFC 4647.
    /// </summary>
    public bool IsBasic { get; }

    /// <summary>
    /// Gets a value indicating whether the range consists only of the wildcard "*".
    /// </summary>
    public bool IsWildcard =>
        Subtags.Count == 1 &&
        Subtags[0] == "*";

    /// <summary>
    /// Gets the normalized subtags of the language range.
    /// </summary>
    public IReadOnlyList<string> Subtags { get; }

    /// <summary>
    /// Gets the original language range.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Returns the normalized language range.
    /// </summary>
    public override string ToString()
    {
        return _normalizedValue;
    }
}
