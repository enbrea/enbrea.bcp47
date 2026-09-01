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
using System.Text;

namespace Enbrea.Bcp47;

/// <summary>
/// Provides methods for parsing, validating, and normalizing BCP 47 language tags.
/// </summary>
/// <remarks>
/// Validates BCP 47 syntax and registry-independent constraints without validating individual subtags against
/// the IANA Language Subtag Registry.
/// </remarks>
public static class Bcp47Parser
{
    /// <summary>
    /// Contains the set of grandfathered language tags defined by RFC 5646.
    /// </summary>
    private static readonly Dictionary<string, string> GrandfatheredTags =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Irregular
            ["en-GB-oed"] = "en-GB-oed",
            ["i-ami"] = "i-ami",
            ["i-bnn"] = "i-bnn",
            ["i-default"] = "i-default",
            ["i-enochian"] = "i-enochian",
            ["i-hak"] = "i-hak",
            ["i-klingon"] = "i-klingon",
            ["i-lux"] = "i-lux",
            ["i-mingo"] = "i-mingo",
            ["i-navajo"] = "i-navajo",
            ["i-pwn"] = "i-pwn",
            ["i-tao"] = "i-tao",
            ["i-tay"] = "i-tay",
            ["i-tsu"] = "i-tsu",
            ["sgn-BE-FR"] = "sgn-BE-FR",
            ["sgn-BE-NL"] = "sgn-BE-NL",
            ["sgn-CH-DE"] = "sgn-CH-DE",

            // Regular
            ["art-lojban"] = "art-lojban",
            ["cel-gaulish"] = "cel-gaulish",
            ["no-bok"] = "no-bok",
            ["no-nyn"] = "no-nyn",
            ["zh-guoyu"] = "zh-guoyu",
            ["zh-hakka"] = "zh-hakka",
            ["zh-min"] = "zh-min",
            ["zh-min-nan"] = "zh-min-nan",
            ["zh-xiang"] = "zh-xiang"
        };

    /// <summary>
    /// Determines whether <paramref name="value"/> is structurally valid according to RFC 5646.
    /// </summary>
    /// <remarks>
    /// Structural validation verifies the BCP 47 grammar and registry-independent validity 
    /// constraints, such as duplicate variants, duplicate extension singletons, and multiple 
    /// extended language subtags. It does not verify subtags against the IANA Language Subtag 
    /// Registry.
    /// </remarks>
    /// <param name="value">The BCP 47 language tag to validate.</param>
    /// <returns><c>true</c> if the value is structurally valid; otherwise, <c>false</c>.</returns>
    public static bool IsStructurallyValid(string value)
    {
        return TryParse(value, validateStructure: true, out _);
    }

    /// <summary>
    /// Determines whether <paramref name="value"/> is syntactically well-formed according to the 
    /// BCP 47 grammar defined by RFC 5646.
    /// </summary>
    /// <param name="value">The BCP 47 language tag to validate.</param>
    /// <returns><c>true</c> if the value is well-formed; otherwise, <c>false</c>.</returns>
    public static bool IsWellFormed(string value)
    {
        return TryParse(value, validateStructure: false, out _);
    }

    /// <summary>
    /// Parses and normalizes a BCP 47 language tag.
    /// </summary>
    /// <remarks>
    /// Normalization applies registry-independent casing and extension-ordering rules. It does 
    /// not apply Preferred-Value mappings from the IANA Language Subtag Registry.
    /// </remarks>
    /// <param name="value">The BCP 47 language tag to normalize.</param>
    /// <returns>The normalized BCP 47 language tag.</returns>
    public static string Normalize(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!TryParse(value, validateStructure: true, out var result))
        {
            throw new FormatException($"'{value}' is not a valid BCP 47 language tag.");
        }

        var normalizedValue = Normalize(result);

        return BuildString(normalizedValue);
    }

    /// <summary>
    /// Parses a structurally valid BCP 47 language tag.
    /// </summary>
    /// <param name="value">The BCP 47 language tag to parse.</param>
    /// <returns>The parsed language tag.</returns>
    public static LanguageTag Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!TryParse(value, validateStructure: true, out var result))
        {
            throw new FormatException($"'{value}' is not a valid BCP 47 language tag.");
        }

        return CreateLanguageTag(value, Normalize(result));
    }

    /// <summary>
    /// Attempts to parse and normalize a BCP 47 language tag.
    /// </summary>
    /// <param name="value">The BCP 47 language tag to normalize.</param>
    /// <param name="normalizedValue">The normalized BCP 47 language tag if the value is valid;
    /// otherwise, an empty string.</param>
    /// <returns><c>true</c> if the value is a structurally valid BCP 47 language tag; 
    /// otherwise, <c>false</c>.</returns>
    public static bool TryNormalize(string value, out string normalizedValue)
    {
        if (!TryParse(value, validateStructure: true, out var result))
        {
            normalizedValue = string.Empty;
            return false;
        }

        normalizedValue = BuildString(Normalize(result));
        return true;
    }

    /// <summary>
    /// Attempts to parse a structurally valid BCP 47 language tag.
    /// </summary>
    /// <param name="value">The BCP 47 language tag to parse.</param>
    /// <param name="languageTag">The parsed language tag if parsing succeeds; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the value is a structurally valid BCP 47 language tag;
    /// otherwise, <c>false</c>.</returns>
    public static bool TryParse(string value, out LanguageTag languageTag)
    {
        if (!TryParse(value,validateStructure: true, out var result))
        {
            languageTag = null;
            return false;
        }

        languageTag = CreateLanguageTag(value!, Normalize(result));
        return true;
    }

    /// <summary>
    /// Determines whether a character is a valid extension singleton.
    /// </summary>
    internal static bool IsExtensionSingleton(char value)
    {
        if (!IsAsciiAlphaNumeric(value))
        {
            return false;
        }

        // "x" is reserved for private use
        return value is not ('x' or 'X');
    }

    /// <summary>
    /// Determines whether a string is a valid extension subtag.
    /// </summary>
    internal static bool IsExtensionSubtag(string value)
    {
        return IsAlphaNumeric(value, 2, 8);
    }

    /// <summary>
    /// Determines whether a string is a valid extended-language subtag.
    /// </summary>
    internal static bool IsExtlangSubtag(string value)
    {
        return IsLetters(value, 3, 3);
    }

    /// <summary>
    /// Determines whether a string is a valid language subtag.
    /// </summary>
    internal static bool IsLanguageSubtag(string value)
    {
        // language =
        //     2*3ALPHA
        //   / 4ALPHA
        //   / 5*8ALPHA
        return IsLetters(value, 2, 8);
    }

    /// <summary>
    /// Determines whether a string is a valid private-use subtag.
    /// </summary>
    internal static bool IsPrivateUseSubtag(string value)
    {
        return IsAlphaNumeric(value, 1, 8);
    }

    /// <summary>
    /// Determines whether a string is a valid region subtag.
    /// </summary>
    internal static bool IsRegionSubtag(string value)
    {
        // region =
        //     2ALPHA
        //   / 3DIGIT
        return
            IsLetters(value, 2, 2) ||
            IsDigits(value, 3, 3);
    }

    /// <summary>
    /// Determines whether a string is a valid script subtag.
    /// </summary>
    internal static bool IsScriptSubtag(string value)
    {
        return IsLetters(value, 4, 4);
    }

    /// <summary>
    /// Determines whether a string is a valid variant subtag.
    /// </summary>
    internal static bool IsVariantSubtag(string value)
    {
        // variant =
        //     5*8alphanum
        //   / (DIGIT 3alphanum)
        if (value.Length is >= 5 and <= 8)
        {
            foreach (var c in value)
            {
                if (!IsAsciiAlphaNumeric(c))
                {
                    return false;
                }
            }

            return true;
        }

        if (value.Length != 4 || !IsAsciiDigit(value[0]))
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
    /// Builds a BCP 47 language tag from normalized parsed components.
    /// </summary>
    private static string BuildString(Bcp47ParserResult result)
    {
        // Grandfathered tags already carry their normalized registered spelling
        if (result.GrandfatheredTag is not null)
        {
            return result.GrandfatheredTag;
        }

        var sb = new StringBuilder();

        // Primary language
        if (result.Language is not null)
        {
            sb.Append(result.Language);
        }

        // Extended language subtags
        foreach (var extlang in result.Extlangs)
        {
            sb.Append('-');
            sb.Append(extlang);
        }

        // Optional script
        if (result.Script is not null)
        {
            sb.Append('-');
            sb.Append(result.Script);
        }

        // Optional region
        if (result.Region is not null)
        {
            sb.Append('-');
            sb.Append(result.Region);
        }

        // Optional variants
        foreach (var variant in result.Variants)
        {
            sb.Append('-');
            sb.Append(variant);
        }

        // Optional extensions
        foreach (var extension in result.Extensions)
        {
            sb.Append('-');
            sb.Append(extension.Singleton);

            foreach (var subtag in extension.Subtags)
            {
                sb.Append('-');
                sb.Append(subtag);
            }
        }

        // Private-use subtags may either follow a normal tag or form the whole tag
        if (result.PrivateUse.Count > 0)
        {
            if (sb.Length > 0)
            {
                sb.Append('-');
            }

            sb.Append('x');

            foreach (var subtag in result.PrivateUse)
            {
                sb.Append('-');
                sb.Append(subtag);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Creates a <see cref="LanguageTag"/> instance from the original value and normalized parsed components.
    /// </summary>
    private static LanguageTag CreateLanguageTag(string value, Bcp47ParserResult result)
    {
        return new LanguageTag(
            value,
            BuildString(result),
            result.Language,
            result.Extlangs,
            result.Script,
            result.Region,
            result.Variants,
            result.Extensions,
            result.PrivateUse,
            result.IsGrandfathered);
    }

    /// <summary>
    /// Determines whether a string consists only of ASCII alphanumeric characters.
    /// </summary>
    private static bool IsAlphaNumeric(string value, int minLength, int maxLength)
    {
        if (value.Length < minLength || value.Length > maxLength)
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
    /// Determines whether a string consists only of ASCII digits.
    /// </summary>
    private static bool IsDigits(string value, int minLength, int maxLength)
    {
        if (value.Length < minLength || value.Length > maxLength)
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!IsAsciiDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether a string is a valid extension singleton.
    /// </summary>
    private static bool IsExtensionSingleton(string value)
    {
        return value.Length == 1 && IsExtensionSingleton(value[0]);
    }

    /// <summary>
    /// Determines whether a string consists only of ASCII letters.
    /// </summary>
    private static bool IsLetters(string value, int minLength, int maxLength)
    {
        if (value.Length < minLength || value.Length > maxLength)
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
    /// Determines whether a string is a private-use singleton ("x" or "X").
    /// </summary>
    private static bool IsPrivateUseSingleton(string value)
    {
        return
            value.Length == 1 &&
            value[0] is 'x' or 'X';
    }

    /// <summary>
    /// Creates a normalized representation of a parsed BCP 47 language tag.
    /// </summary>
    private static Bcp47ParserResult Normalize(Bcp47ParserResult result)
    {
        // Grandfathered tags already use their registered normalized spelling
        if (result.IsGrandfathered)
        {
            return result;
        }

        // Normalize extension singletons and subtags
        var normalizedExtensions = new List<LanguageTagExtension>(result.Extensions.Count);

        foreach (var extension in result.Extensions)
        {
            normalizedExtensions.Add(
                new LanguageTagExtension(
                    ToAsciiLower(extension.Singleton),
                    ToAsciiLower(extension.Subtags)));
        }

        // Keep extension sequences in a deterministic singleton order
        normalizedExtensions.Sort(static (x, y) => x.Singleton.CompareTo(y.Singleton));

        // Normalize every parsed component according to BCP 47 casing conventions
        return new Bcp47ParserResult(
            Language: result.Language is null
                ? null
                : ToAsciiLower(result.Language),

            Extlangs: ToAsciiLower(result.Extlangs),

            Script: result.Script is null
                ? null
                : ToAsciiTitleCase(result.Script),

            Region: result.Region is null
                ? null
                : IsLetters(result.Region, 2, 2)
                    ? ToAsciiUpper(result.Region)
                    : result.Region,

            Variants: ToAsciiLower(result.Variants),

            Extensions: normalizedExtensions,

            PrivateUse: ToAsciiLower(result.PrivateUse),

            GrandfatheredTag: null);
    }

    /// <summary>
    /// Returns a string converted to lowercase using ASCII rules.
    /// </summary>
    private static string ToAsciiLower(string value)
    {
        return string.Create(value.Length, value, static (span, source) =>
        {
            for (var i = 0; i < source.Length; i++)
            {
                span[i] = ToAsciiLower(source[i]);
            }
        });
    }

    /// <summary>
    /// Returns all strings converted to lowercase using ASCII rules.
    /// </summary>
    private static List<string> ToAsciiLower(IReadOnlyList<string> values)
    {
        var result = new List<string>(values.Count);

        foreach (var value in values)
        {
            result.Add(ToAsciiLower(value));
        }

        return result;
    }

    /// <summary>
    /// Converts an ASCII letter to lowercase. Other characters are returned unchanged.
    /// </summary>
    private static char ToAsciiLower(char c)
    {
        return c is >= 'A' and <= 'Z'
            ? (char)(c + ('a' - 'A'))
            : c;
    }

    /// <summary>
    /// Returns a string converted to title case using ASCII rules.
    /// </summary>
    private static string ToAsciiTitleCase(string value)
    {
        return string.Create(value.Length, value, static (span, source) =>
        {
            span[0] = source[0] is >= 'a' and <= 'z'
                ? (char)(source[0] - ('a' - 'A'))
                : source[0];

            for (var i = 1; i < source.Length; i++)
            {
                span[i] = ToAsciiLower(source[i]);
            }
        });
    }

    /// <summary>
    /// Returns a string converted to uppercase using ASCII rules.
    /// </summary>
    private static string ToAsciiUpper(string value)
    {
        return string.Create(value.Length, value, static (span, source) =>
        {
            for (var i = 0; i < source.Length; i++)
            {
                var c = source[i];
                span[i] = c is >= 'a' and <= 'z'
                    ? (char)(c - ('a' - 'A'))
                    : c;
            }
        });
    }

    /// <summary>
    /// Attempts to parse a BCP 47 language tag into its individual components.
    /// </summary>
    private static bool TryParse(string value, bool validateStructure, out Bcp47ParserResult result)
    {
        result = null;

        // A language tag must not be empty
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // Grandfathered tags form a fixed legacy set and must be handled before applying
        // the regular BCP 47 grammar
        if (GrandfatheredTags.TryGetValue(value, out var grandfathered))
        {
            result = new Bcp47ParserResult(
                Language: null,
                Extlangs: [],
                Script: null,
                Region: null,
                Variants: [],
                Extensions: [],
                PrivateUse: [],
                GrandfatheredTag: grandfathered);

            return true;
        }

        // Split into subtags and reject empty components such as in "de--DE"
        var parts = value.Split('-');

        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length == 0)
            {
                return false;
            }
        }

        // A tag may consist entirely of private-use subtags introduced by "x"
        if (IsPrivateUseSingleton(parts[0]))
        {
            if (parts.Length < 2)
            {
                return false;
            }

            for (var i = 1; i < parts.Length; i++)
            {
                if (!IsPrivateUseSubtag(parts[i]))
                {
                    return false;
                }
            }

            result = new Bcp47ParserResult(
                Language: null,
                Extlangs: [],
                Script: null,
                Region: null,
                Variants: [],
                Extensions: [],
                PrivateUse: parts[1..],
                GrandfatheredTag: null);

            return true;
        }

        var index = 0;

        // Every regular tag starts with a primary language subtag
        var language = parts[index];

        if (!IsLanguageSubtag(language))
        {
            return false;
        }

        index++;

        // A 2- or 3-letter language may be followed by up to three extlangs
        var extlangs = new List<string>();

        if (language.Length is >= 2 and <= 3)
        {
            while (index < parts.Length && extlangs.Count < 3 && IsExtlangSubtag(parts[index]))
            {
                extlangs.Add(parts[index]);
                index++;
            }
        }

        // RFC 5646 reserves extlang positions two and three, making them
        // well-formed by ABNF but structurally invalid
        if (validateStructure && extlangs.Count > 1)
        {
            return false;
        }

        // Parse the optional four-letter script subtag
        string script = null;

        if (index < parts.Length && IsScriptSubtag(parts[index]))
        {
            script = parts[index];
            index++;
        }

        // Parse the optional alphabetic or numeric region subtag
        string region = null;

        if (index < parts.Length && IsRegionSubtag(parts[index]))
        {
            region = parts[index];
            index++;
        }

        // Parse zero or more variant subtags
        var variants = new List<string>();

        // Structural validation additionally rejects duplicate variants
        HashSet<string> seenVariants = validateStructure
            ? new(StringComparer.OrdinalIgnoreCase)
            : null;

        while (index < parts.Length && IsVariantSubtag(parts[index]))
        {
            var variant = parts[index];

            if (seenVariants is not null && !seenVariants.Add(variant))
            {
                return false;
            }

            variants.Add(variant);
            index++;
        }

        // Parse extension sequences consisting of a singleton and its subtags
        var extensions = new List<LanguageTagExtension>();

        // Extension singletons are case-insensitive and may occur only once
        HashSet<char> seenSingletons = validateStructure
            ? []
            : null;

        while (index < parts.Length && IsExtensionSingleton(parts[index]))
        {
            var singleton = parts[index][0];

            if (seenSingletons is not null)
            {
                var normalizedSingleton = ToAsciiLower(singleton);

                if (!seenSingletons.Add(normalizedSingleton))
                {
                    return false;
                }
            }

            index++;

            var subtags = new List<string>();

            // Each extension requires one or more 2- to 8-character subtags
            while (index < parts.Length && IsExtensionSubtag(parts[index]))
            {
                subtags.Add(parts[index]);
                index++;
            }

            if (subtags.Count == 0)
            {
                return false;
            }

            extensions.Add(new LanguageTagExtension(singleton, subtags));
        }

        // A regular tag may end with a private-use section introduced by "x"
        var privateUse = new List<string>();

        if (index < parts.Length && IsPrivateUseSingleton(parts[index]))
        {
            index++;

            while (index < parts.Length)
            {
                if (!IsPrivateUseSubtag(parts[index]))
                {
                    return false;
                }

                privateUse.Add(parts[index]);
                index++;
            }

            // A private-use singleton must be followed by at least one subtag
            if (privateUse.Count == 0)
            {
                return false;
            }
        }

        // Any remaining subtag indicates an invalid sequence
        if (index != parts.Length)
        {
            return false;
        }

        // Return the parsed components without applying normalization
        result = new Bcp47ParserResult(
            Language: language,
            Extlangs: extlangs,
            Script: script,
            Region: region,
            Variants: variants,
            Extensions: extensions,
            PrivateUse: privateUse,
            GrandfatheredTag: null);

        return true;
    }

    /// <summary>
    /// Represents the result of parsing a BCP 47 language tag into its individual components.
    /// </summary>
    private sealed record Bcp47ParserResult(
        string Language,
        IReadOnlyList<string> Extlangs,
        string Script,
        string Region,
        IReadOnlyList<string> Variants,
        IReadOnlyList<LanguageTagExtension> Extensions,
        IReadOnlyList<string> PrivateUse,
        string GrandfatheredTag)
    {
        public bool IsGrandfathered => GrandfatheredTag is not null;
    }
}
