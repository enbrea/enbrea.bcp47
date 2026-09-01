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

namespace Enbrea.Bcp47.Iana;

/// <summary>
/// Provides access to the embedded IANA Language Subtag Registry snapshot.
/// </summary>
/// <remarks>
/// The registry uses compact sorted arrays and binary search instead of runtime dictionaries. Registry data is 
/// generated via source generator from an authoritative IANA Language Subtag Registry snapshot.
/// </remarks>
public static partial class IanaLanguageSubtagRegistry
{
    /// <summary>
    /// Determines whether a value is registered for the specified registry type.
    /// </summary>
    /// <param name="type">The type of the subtag.</param>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered for the specified registry type; otherwise, <c>false</c>.</returns> 
    public static bool Contains(IanaLanguageSubtagType type, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return type switch
        {
            IanaLanguageSubtagType.Language => ContainsLanguage(value),
            IanaLanguageSubtagType.Extlang => ContainsExtlang(value),
            IanaLanguageSubtagType.Script => ContainsScript(value),
            IanaLanguageSubtagType.Region => ContainsRegion(value),
            IanaLanguageSubtagType.Variant => ContainsVariant(value),
            IanaLanguageSubtagType.Grandfathered => ContainsGrandfatheredTag(value),
            IanaLanguageSubtagType.Redundant => ContainsRedundantTag(value),
            _ => false
        };
    }

    /// <summary>
    /// Determines whether a value is registered as an extlang subtag.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered as an extlang subtag; otherwise, <c>false</c>.</returns>
    public static bool ContainsExtlang(string value)
    {
        return Contains(Extlangs, value) || Contains(ExtlangRanges, value);
    }

    /// <summary>
    /// Determines whether a value is registered as a grandfathered tag.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered as a grandfathered tag; otherwise, <c>false</c>.</returns>
    public static bool ContainsGrandfatheredTag(string value)
    {
        return Contains(GrandfatheredTags, value);
    }

    /// <summary>
    /// Determines whether a value is registered or reserved for use as a language subtag.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered or reserved for use as a language subtag; otherwise, <c>false</c>.</returns>
    public static bool ContainsLanguage(string value)
    {
        return Contains(Languages, value) || Contains(LanguageRanges, value);
    }

    /// <summary>
    /// Determines whether a value is registered as a redundant tag.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered as a redundant tag; otherwise, <c>false</c>.</returns>  
    public static bool ContainsRedundantTag(string value)
    { 
        return Contains(RedundantTags, value);
    }

    /// <summary>
    /// Determines whether a value is valid for use as a region subtag according to the IANA Language Subtag Registry, 
    /// including values from reserved region ranges.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered as a region subtag; otherwise, <c>false</c>.</returns>
    public static bool ContainsRegion(string value)
    {
        return Contains(Regions, value) || Contains(RegionRanges, value);
    }

    /// <summary>
    /// Determines whether a value is valid for use as a script subtag according to the IANA Language Subtag Registry, 
    /// including values from reserved script ranges.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered as a script subtag; otherwise, <c>false</c>.</returns>
    public static bool ContainsScript(string value)
    {
        return Contains(Scripts, value) || Contains(ScriptRanges, value);
    }

    /// <summary>
    /// Determines whether a value is registered as a variant subtag.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is registered as a variant subtag; otherwise, <c>false</c>.</returns>
    public static bool ContainsVariant(string value)
    {
        return Contains(Variants, value) || Contains(VariantRanges, value);
    }

    /// <summary>
    /// Gets the list of registered prefixes for a variant subtag.
    /// </summary>
    /// <param name="variant">The variant subtag to get prefixes for.</param>
    /// <returns>The list of registered prefixes for the specified variant subtag.</returns>
    public static IReadOnlyList<string> GetVariantPrefixes(string variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        var entries = VariantPrefixes;
        var first = FindFirstPrefix(entries, variant);

        if (first < 0)
        {
            return [];
        }

        var result = new List<string>();

        for (var i = first; i < entries.Length; i++)
        {
            if (!entries[i].Subtag.Equals(variant, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            result.Add(entries[i].Prefix);
        }

        return result;
    }

    /// <summary>
    /// Determines whether a registered value is marked as deprecated.
    /// </summary>
    /// <param name="type">The type of the subtag.</param>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is marked as deprecated; otherwise, <c>false</c>.</returns>   
    public static bool IsDeprecated(IanaLanguageSubtagType type, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return type switch
        {
            IanaLanguageSubtagType.Language =>
                Contains(DeprecatedLanguages, value) ||
                Contains(DeprecatedLanguageRanges, value),

            IanaLanguageSubtagType.Extlang =>
                Contains(DeprecatedExtlangs, value) ||
                Contains(DeprecatedExtlangRanges, value),

            IanaLanguageSubtagType.Script =>
                Contains(DeprecatedScripts, value) ||
                Contains(DeprecatedScriptRanges, value),

            IanaLanguageSubtagType.Region =>
                Contains(DeprecatedRegions, value) ||
                Contains(DeprecatedRegionRanges, value),

            IanaLanguageSubtagType.Variant =>
                Contains(DeprecatedVariants, value) ||
                Contains(DeprecatedVariantRanges, value),

            IanaLanguageSubtagType.Grandfathered =>
                Contains(DeprecatedGrandfatheredTags, value),

            IanaLanguageSubtagType.Redundant =>
                Contains(DeprecatedRedundantTags, value),

            _ => false
        };
    }

    /// <summary>
    /// Attempts to get the macrolanguage associated with an extlang subtag.
    /// </summary>
    /// <param name="extlang">The extlang subtag to get the macrolanguage for.</param>
    /// <param name="macrolanguage">The macrolanguage associated with the specified extlang subtag.</param>
    /// <returns><c>true</c> if the macrolanguage was found; otherwise, <c>false</c>.</returns> 
    public static bool TryGetExtlangMacrolanguage(string extlang, out string macrolanguage)
    {
        ArgumentNullException.ThrowIfNull(extlang);

        return TryGetValue(ExtlangMacrolanguages, extlang, out macrolanguage);   
    }

    /// <summary>
    /// Attempts to get the required registry prefix of an extlang subtag.
    /// </summary>
    /// <param name="extlang">The extlang subtag to get the prefix for.</param>
    /// <param name="prefix">The required registry prefix of the specified extlang subtag.</param>
    /// <returns><c>true</c> if the prefix was found; otherwise, <c>false</c>.</returns>    
    public static bool TryGetExtlangPrefix(string extlang, out string prefix)
    {
        ArgumentNullException.ThrowIfNull(extlang);

        return TryGetValue(ExtlangPrefixes, extlang, out prefix);
    }

    /// <summary>
    /// Attempts to get the Suppress-Script value for an extlang subtag.
    /// </summary>
    /// <param name="extlang">The extlang subtag to get the Suppress-Script value for.</param>
    /// <param name="script">The Suppress-Script value associated with the specified extlang subtag.</param>
    /// <returns><c>true</c> if the Suppress-Script value was found; otherwise, <c>false</c>.</returns>
    public static bool TryGetExtlangSuppressScript(string extlang, out string script)
    {
        ArgumentNullException.ThrowIfNull(extlang);

        return TryGetValue(ExtlangSuppressScripts, extlang, out script);
    }

    /// <summary>
    /// Attempts to get the macrolanguage associated with a language subtag.
    /// </summary>
    /// <param name="language">The language subtag to get the macrolanguage for.</param>
    /// <param name="macrolanguage">The macrolanguage associated with the specified language subtag.</param>
    /// <returns><c>true</c> if the macrolanguage was found; otherwise, <c>false</c>.</returns>
    public static bool TryGetLanguageMacrolanguage(string language, out string macrolanguage)
    {
        ArgumentNullException.ThrowIfNull(language);

        return TryGetValue(LanguageMacrolanguages, language, out macrolanguage);
    }

    /// <summary>
    /// Attempts to get the Suppress-Script value for a language subtag.
    /// </summary>
    /// <param name="language">The language subtag to get the Suppress-Script value for.</param>
    /// <param name="script">The Suppress-Script value associated with the specified language subtag.</param>
    /// <returns><c>true</c> if the Suppress-Script value was found; otherwise, <c>false</c>.</returns>
    public static bool TryGetLanguageSuppressScript(string language, out string script)
    {
        ArgumentNullException.ThrowIfNull(language);

        return TryGetValue(LanguageSuppressScripts, language, out script);
    }

    /// <summary>
    /// Attempts to get the registry Preferred-Value for a value.
    /// </summary>
    /// <param name="type">The type of the language subtag.</param>
    /// <param name="value">The value to get the Preferred-Value for.</param>
    /// <param name="preferredValue">The Preferred-Value associated with the specified value.</param>
    /// <returns><c>true</c> if the Preferred-Value was found; otherwise, <c>false</c>.</returns>   
    public static bool TryGetPreferredValue(IanaLanguageSubtagType type, string value, out string preferredValue)
    {
        ArgumentNullException.ThrowIfNull(value);

        var values = type switch
        {
            IanaLanguageSubtagType.Language => LanguagePreferredValues,
            IanaLanguageSubtagType.Extlang => ExtlangPreferredValues,
            IanaLanguageSubtagType.Script => ScriptPreferredValues,
            IanaLanguageSubtagType.Region => RegionPreferredValues,
            IanaLanguageSubtagType.Variant => VariantPreferredValues,
            IanaLanguageSubtagType.Grandfathered => GrandfatheredPreferredValues,
            IanaLanguageSubtagType.Redundant => RedundantPreferredValues,
            _ => []
        };

        return TryGetValue(values, value, out preferredValue);
    }

    /// <summary>
    /// Determines whether a value is contained in a sorted array of values.
    /// </summary>
    private static bool Contains(string[] values, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Registry range expressions such as "qaa..qtz" are storage syntax, not language subtags themselves.
        if (value.Contains('.'))
        {
            return false;
        }

        return Array.BinarySearch(values, value, StringComparer.OrdinalIgnoreCase) >= 0;
    }


    /// <summary>
    /// Determines whether a value is contained in one of the generated IANA registry ranges.
    /// </summary>
    private static bool Contains(RegistryRangeEntry[] ranges, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Registry range expressions such as "qaa..qtz" are storage syntax, not language subtags themselves.
        if (value.Contains('.'))
        {
            return false;
        }

        foreach (var range in ranges)
        {
            if (IsInRange(value, range.Start, range.End))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds the first index of a prefix entry in a sorted array of prefix entries.
    /// </summary>
    private static int FindFirstPrefix(RegistryPrefixEntry[] values, string key)
    {
        var lower = 0;
        var upper = values.Length - 1;
        var result = -1;

        while (lower <= upper)
        {
            var middle = lower + ((upper - lower) >> 1);
            var comparison = StringComparer.OrdinalIgnoreCase.Compare(values[middle].Subtag, key);

            if (comparison < 0)
            {
                lower = middle + 1;
            }
            else
            {
                if (comparison == 0)
                {
                    result = middle;
                }

                upper = middle - 1;
            }
        }

        return result;
    }

    /// <summary>
    /// Determines whether a value is contained in an IANA registry range expression.
    /// </summary>
    private static bool IsInRange(string value, string first, string last)
    {
        if (value.Length != first.Length)
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
            {
                return false;
            }
        }

        return
            StringComparer.OrdinalIgnoreCase.Compare(value, first) >= 0 &&
            StringComparer.OrdinalIgnoreCase.Compare(value, last) <= 0;
    }

    /// <summary>
    /// Attempts to get a value associated with a key in a sorted array of registry value entries.  
    /// </summary>
    /// <param name="values"></param>
    private static bool TryGetValue(RegistryValueEntry[] values, string key, out string value)
    {
        var lower = 0;
        var upper = values.Length - 1;

        while (lower <= upper)
        {
            var middle = lower + ((upper - lower) >> 1);
            var comparison = StringComparer.OrdinalIgnoreCase.Compare(values[middle].Key, key);

            if (comparison == 0)
            {
                value = values[middle].Value;
                return true;
            }

            if (comparison < 0)
            {
                lower = middle + 1;
            }
            else
            {
                upper = middle - 1;
            }
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Represents a key-value pair in the IANA registry.
    /// </summary>
    internal readonly record struct RegistryValueEntry(
        string Key,
        string Value);

    /// <summary>
    /// Represents a prefix entry in the IANA registry.
    /// </summary>
    internal readonly record struct RegistryPrefixEntry(
        string Subtag,
        string Prefix);

    /// <summary>
    /// Represents a range entry in the IANA registry.
    /// </summary>
    internal readonly record struct RegistryRangeEntry(
        string Start,
        string End);
}
