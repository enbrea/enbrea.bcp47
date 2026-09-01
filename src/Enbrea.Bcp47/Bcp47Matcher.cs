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
/// Provides RFC 4647 language-tag matching using Basic Filtering, Extended Filtering, and Lookup.
/// </summary>
/// <remarks>
/// Matching is case-insensitive and registry-independent. Basic Filtering and Lookup require basic 
/// language ranges. Extended Filtering accepts both basic and extended language ranges.
/// </remarks>
public static class Bcp47Matcher
{
    /// <summary>
    /// Filters language tags using RFC 4647 Basic Filtering.
    /// </summary>
    /// <param name="range">The basic language range.</param>
    /// <param name="tags">The language tags to filter.</param>
    /// <returns>The matching language tags in their original input order.</returns>
    public static IReadOnlyList<LanguageTag> BasicFilter(LanguageRange range, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tags);

        EnsureBasicRange(range);

        var result = new List<LanguageTag>();

        foreach (var tag in tags)
        {
            ArgumentNullException.ThrowIfNull(tag);

            if (IsBasicMatchCore(range, tag))
            {
                result.Add(tag);
            }
        }

        return result;
    }

    /// <summary>
    /// Filters language tags using an RFC 4647 basic language priority list.
    /// </summary>
    /// <remarks>
    /// A language tag is returned once if it matches at least one range. Results preserve the order 
    /// of <paramref name="tags"/>.
    /// </remarks>
    /// <param name="ranges">The basic language ranges in priority order.</param>
    /// <param name="tags">The language tags to filter.</param>
    /// <returns>The matching language tags in their original input order.</returns>
    public static IReadOnlyList<LanguageTag> BasicFilter(IEnumerable<LanguageRange> ranges, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(ranges);
        ArgumentNullException.ThrowIfNull(tags);

        var rangeList = MaterializeRanges(ranges, requireBasic: true);
        var result = new List<LanguageTag>();

        foreach (var tag in tags)
        {
            ArgumentNullException.ThrowIfNull(tag);

            foreach (var range in rangeList)
            {
                if (IsBasicMatchCore(range, tag))
                {
                    result.Add(tag);
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Filters language tags using RFC 4647 Extended Filtering.
    /// </summary>
    /// <param name="range">The language range.</param>
    /// <param name="tags">The language tags to filter.</param>
    /// <returns>The matching language tags in their original input order.</returns>
    public static IReadOnlyList<LanguageTag> ExtendedFilter(LanguageRange range, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tags);

        var result = new List<LanguageTag>();

        foreach (var tag in tags)
        {
            ArgumentNullException.ThrowIfNull(tag);

            var tagParts = tag.Value.Split('-');

            if (IsExtendedMatchCore(range, tagParts))
            {
                result.Add(tag);
            }
        }

        return result;
    }

    /// <summary>
    /// Filters language tags using an RFC 4647 extended language priority list.
    /// </summary>
    /// <remarks>
    /// A language tag is returned once if it matches at least one range. Results preserve the order 
    /// of <paramref name="tags"/>.
    /// </remarks>
    /// <param name="ranges">The language ranges in priority order.</param>
    /// <param name="tags">The language tags to filter.</param>
    /// <returns>The matching language tags in their original input order.</returns>
    public static IReadOnlyList<LanguageTag> ExtendedFilter(IEnumerable<LanguageRange> ranges, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(ranges);
        ArgumentNullException.ThrowIfNull(tags);

        var rangeList = MaterializeRanges(ranges, requireBasic: false);
        var result = new List<LanguageTag>();

        foreach (var tag in tags)
        {
            ArgumentNullException.ThrowIfNull(tag);

            var tagParts = tag.Value.Split('-');

            foreach (var range in rangeList)
            {
                if (IsExtendedMatchCore(range, tagParts))
                {
                    result.Add(tag);
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Determines whether a language tag matches a basic language range using RFC 4647 Basic Filtering.
    /// </summary>
    /// <param name="range">The basic language range.</param>
    /// <param name="tag">The language tag to test.</param>
    /// <returns><c>true</c> if the tag matches the range; otherwise, <c>false</c>.</returns>
    public static bool IsBasicMatch(LanguageRange range, LanguageTag tag)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tag);

        EnsureBasicRange(range);

        return IsBasicMatchCore(range, tag);
    }

    /// <summary>
    /// Determines whether a language tag matches an extended language range using RFC 4647 Extended Filtering.
    /// </summary>
    /// <param name="range">The extended language range.</param>
    /// <param name="tag">The language tag to test.</param>
    /// <returns><c>true</c> if the tag matches the range; otherwise, <c>false</c>.</returns>
    public static bool IsExtendedMatch(LanguageRange range, LanguageTag tag)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tag);

        var tagParts = tag.Value.Split('-');

        return IsExtendedMatchCore(range, tagParts);
    }

    /// <summary>
    /// Selects the best available language tag for a basic language range using RFC 4647 Lookup.
    /// </summary>
    /// <param name="range">The basic language range.</param>
    /// <param name="tags">The available language tags.</param>
    /// <returns>The best matching language tag, or <c>null</c> if no match exists.</returns>
    public static LanguageTag Lookup(LanguageRange range, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tags);

        EnsureBasicRange(range);

        var tagList = MaterializeTags(tags);

        return LookupCore(range, tagList);
    }

    /// <summary>
    /// Selects the best available language tag for an RFC 4647 basic language priority list using Lookup.
    /// </summary>
    /// <remarks>
    /// Ranges are processed in priority order. The wildcard range "*" is skipped, because it does not identify 
    /// a single best language tag. If no concrete range matches, this method returns <c>null</c> and leaves 
    /// default selection
    /// to the caller.
    /// </remarks>
    /// <param name="ranges">The basic language ranges in priority order.</param>
    /// <param name="tags">The available language tags.</param>
    /// <returns>The best matching language tag, or <c>null</c> if no match exists.</returns>
    public static LanguageTag Lookup(IEnumerable<LanguageRange> ranges, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(ranges);
        ArgumentNullException.ThrowIfNull(tags);

        var rangeList = MaterializeRanges(ranges, requireBasic: true);
        var tagList = MaterializeTags(tags);

        foreach (var range in rangeList)
        {
            var result = LookupCore(range, tagList);

            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Ensures that a range can be used by an RFC 4647 operation that requires basic language ranges.
    /// </summary>
    private static void EnsureBasicRange(LanguageRange range)
    {
        if (!range.IsBasic)
        {
            throw new ArgumentException(
                $"The language range '{range}' is not a basic RFC 4647 language range.", nameof(range));
        }
    }

    /// <summary>
    /// Determines whether a basic range matches a tag.
    /// </summary>
    private static bool IsBasicMatchCore(LanguageRange range, LanguageTag tag)
    {
        if (range.IsWildcard)
        {
            return true;
        }

        var rangeValue = range.ToString();
        var tagValue = tag.Value;

        if (!tagValue.StartsWith(rangeValue, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // The range must either equal the complete tag or end at a subtag boundary.
        return tagValue.Length == rangeValue.Length || tagValue[rangeValue.Length] == '-';
    }

    /// <summary>
    /// Determines whether an extended range matches a tag using the algorithm from RFC 4647 section 3.3.2.
    /// </summary>
    private static bool IsExtendedMatchCore(LanguageRange range, string[] tagParts)
    {
        var rangeParts = range.Subtags;

        var rangeIndex = 0;
        var tagIndex = 0;

        // The first subtags must match directly; a leading wildcard matches any first subtag.
        if (rangeParts[0] != "*" &&
            !rangeParts[0].Equals(tagParts[0], StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        rangeIndex++;
        tagIndex++;

        while (rangeIndex < rangeParts.Count)
        {
            var rangePart = rangeParts[rangeIndex];

            // Wildcards after the first position are ignored by Extended Filtering.
            if (rangePart == "*")
            {
                rangeIndex++;
                continue;
            }

            if (tagIndex >= tagParts.Length)
            {
                return false;
            }

            if (rangePart.Equals(tagParts[tagIndex], StringComparison.OrdinalIgnoreCase))
            {
                rangeIndex++;
                tagIndex++;
                continue;
            }

            // A mismatch may skip ordinary subtags, but never an extension or private-use singleton.
            if (tagParts[tagIndex].Length == 1)
            {
                return false;
            }

            tagIndex++;
        }

        return true;
    }

    /// <summary>
    /// Performs lookup for a single basic language range.
    /// </summary>
    private static LanguageTag LookupCore(LanguageRange range, IReadOnlyList<LanguageTag> tags)
    {
        // RFC 4647 leaves wildcard lookup to the caller's default selection.
        if (range.IsWildcard)
        {
            return null;
        }

        var candidate = range.ToString();

        while (candidate.Length > 0)
        {
            foreach (var tag in tags)
            {
                if (candidate.Equals(tag.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return tag;
                }
            }

            candidate = TruncateLookupRange(candidate);
        }

        return null;
    }

    /// <summary>
    /// Materializes and validates a sequence of language ranges.
    /// </summary>
    private static List<LanguageRange> MaterializeRanges(IEnumerable<LanguageRange> ranges, bool requireBasic)
    {
        var result = new List<LanguageRange>();

        foreach (var range in ranges)
        {
            ArgumentNullException.ThrowIfNull(range);

            if (requireBasic)
            {
                EnsureBasicRange(range);
            }

            result.Add(range);
        }

        return result;
    }

    /// <summary>
    /// Materializes a sequence of language tags because Lookup may enumerate it repeatedly while progressively 
    /// truncating ranges.
    /// </summary>
    private static List<LanguageTag> MaterializeTags(IEnumerable<LanguageTag> tags)
    {
        var result = new List<LanguageTag>();

        foreach (var tag in tags)
        {
            ArgumentNullException.ThrowIfNull(tag);

            result.Add(tag);
        }

        return result;
    }

    /// <summary>
    /// Removes the least-specific subtag for RFC 4647 Lookup. If truncation exposes a singleton, the singleton 
    /// is removed at the same time.
    /// </summary>
    private static string TruncateLookupRange(string value)
    {
        var separator = value.LastIndexOf('-');

        if (separator < 0)
        {
            return string.Empty;
        }

        value = value[..separator];

        separator = value.LastIndexOf('-');

        if (separator >= 0 && value.Length - separator - 1 == 1)
        {
            value = value[..separator];
        }

        return value;
    }
}
