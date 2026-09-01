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
using System.Text;

namespace Enbrea.Bcp47;

/// <summary>
/// Provides methods for parsing RFC 4647 basic and extended language ranges.
/// </summary>
public static class LanguageRangeParser
{
    /// <summary>
    /// Determines whether <paramref name="value"/> conforms to the basic language-range syntax defined by RFC 4647.
    /// </summary>
    /// <param name="value">The language range to test.</param>
    /// <returns><c>true</c> if the value is a valid basic language range; otherwise, <c>false</c>.</returns>
    public static bool IsBasic(string value)
    {
        return TryParseCore(value, basicOnly: true, out _);
    }

    /// <summary>
    /// Determines whether <paramref name="value"/> conforms to the extended language-range syntax defined by RFC 4647.
    /// </summary>
    /// <param name="value">The language range to test.</param>
    /// <returns><c>true</c> if the value is a valid extended language range; otherwise, <c>false</c>.</returns>
    public static bool IsExtended(string value)
    {
        return TryParseCore(value, basicOnly: false, out _);
    }

    /// <summary>
    /// Parses an RFC 4647 language range.
    /// </summary>
    /// <remarks>
    /// This method accepts the extended language-range syntax, which includes all
    /// valid basic language ranges. Use <see cref="ParseBasic"/> when a basic range
    /// is specifically required.
    /// </remarks>
    /// <param name="value">The language range to parse.</param>
    /// <returns>The parsed language range.</returns>
    public static LanguageRange Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!TryParseCore(value, basicOnly: false, out var languageRange))
        {
            throw new FormatException($"'{value}' is not a valid RFC 4647 language range.");
        }

        return languageRange;
    }

    /// <summary>
    /// Parses an RFC 4647 basic language range.
    /// </summary>
    /// <param name="value">The basic language range to parse.</param>
    /// <returns>The parsed language range.</returns>
    public static LanguageRange ParseBasic(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!TryParseCore(value, basicOnly: true, out var languageRange))
        {
            throw new FormatException($"'{value}' is not a valid RFC 4647 basic language range.");
        }

        return languageRange;
    }

    /// <summary>
    /// Attempts to parse an RFC 4647 language range.
    /// </summary>
    /// <remarks>
    /// This method accepts the extended language-range syntax, which includes all
    /// valid basic language ranges.
    /// </remarks>
    /// <param name="value">The language range to parse.</param>
    /// <param name="languageRange">The parsed range if parsing succeeds; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the value is a valid language range; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string value, out LanguageRange languageRange)
    {
        return TryParseCore(value, basicOnly: false, out languageRange);
    }

    /// <summary>
    /// Attempts to parse an RFC 4647 basic language range.
    /// </summary>
    /// <param name="value">The basic language range to parse.</param>
    /// <param name="languageRange">The parsed range if parsing succeeds; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the value is a valid basic language range; otherwise, <c>false</c>.</returns>
    public static bool TryParseBasic(string value, out LanguageRange languageRange)
    {
        return TryParseCore(value, basicOnly: true, out languageRange);
    }

    /// <summary>
    /// Determines whether a character is an ASCII alphanumeric character.
    /// </summary>
    private static bool IsAsciiAlphaNumeric(char c)
    {
        return IsAsciiLetter(c) || IsAsciiDigit(c);
    }

    /// <summary>
    /// Determines whether a character is an ASCII digit.
    /// </summary>
    private static bool IsAsciiDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    /// <summary>
    /// Determines whether a character is an ASCII letter.
    /// </summary>
    private static bool IsAsciiLetter(char c)
    {
        return
            c is >= 'A' and <= 'Z' ||
            c is >= 'a' and <= 'z';
    }

    /// <summary>
    /// Determines whether the first subtag consists of 1 to 8 ASCII letters.
    /// </summary>
    private static bool IsPrimarySubtag(string value)
    {
        if (value.Length is < 1 or > 8)
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!IsAsciiLetter(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether a subtag consists of 1 to 8 ASCII alphanumeric characters.
    /// </summary>
    private static bool IsSubtag(string value)
    {
        if (value.Length is < 1 or > 8)
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!IsAsciiAlphaNumeric(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Converts an ASCII string to lowercase.
    /// </summary>
    private static string ToAsciiLower(string value)
    {
        return string.Create(value.Length, value, static (span, source) =>
        {
            for (var i = 0; i < source.Length; i++)
            {
                var c = source[i];

                span[i] = c is >= 'A' and <= 'Z'
                    ? (char)(c + ('a' - 'A'))
                    : c;
            }
        });
    }

    /// <summary>
    /// Attempts to parse an RFC 4647 language range.
    /// </summary>
    private static bool TryParseCore(string value, bool basicOnly, out LanguageRange languageRange)
    {
        languageRange = null;

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // The standalone wildcard is valid in both basic and extended syntax.
        if (value == "*")
        {
            languageRange = new LanguageRange(value, "*", ["*"], isBasic: true);
            return true;
        }

        // Language ranges consist of hyphen-separated subtags. Empty subtags
        // such as in "de--DE" are not permitted.
        var parts = value.Split('-');

        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length == 0)
            {
                return false;
            }
        }

        // The first subtag must contain 1 to 8 ASCII letters. Extended ranges
        // may alternatively start with the wildcard "*".
        if (parts[0] != "*" && !IsPrimarySubtag(parts[0]))
        {
            return false;
        }

        // In basic syntax "*" is valid only as the complete range.
        if (basicOnly && parts[0] == "*")
        {
            return false;
        }

        var isBasic = true;

        // Following subtags contain 1 to 8 ASCII alphanumeric characters.
        // Extended syntax additionally permits a wildcard in any position.
        for (var i = 1; i < parts.Length; i++)
        {
            if (parts[i] == "*")
            {
                isBasic = false;

                if (basicOnly)
                {
                    return false;
                }

                continue;
            }

            if (!IsSubtag(parts[i]))
            {
                return false;
            }
        }

        // A leading wildcard followed by more subtags is an extended range.
        if (parts[0] == "*")
        {
            isBasic = false;
        }

        var normalizedParts = new string[parts.Length];
        var sb = new StringBuilder(value.Length);

        // Ranges are case-insensitive and their subtag positions do not carry
        // BCP 47 component semantics, so normalize all non-wildcard subtags to lowercase.
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i] == "*"
                ? "*"
                : ToAsciiLower(parts[i]);

            normalizedParts[i] = part;

            if (i > 0)
            {
                sb.Append('-');
            }

            sb.Append(part);
        }

        languageRange = new LanguageRange(value, sb.ToString(), normalizedParts, isBasic);

        return true;
    }
}
