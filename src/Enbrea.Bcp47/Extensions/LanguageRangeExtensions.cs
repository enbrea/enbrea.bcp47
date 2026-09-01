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
/// Provides matching operations for BCP 47 language ranges.
/// </summary>
public static class LanguageRangeExtensions
{
    /// <summary>
    /// Filters language tags using RFC 4647 Basic Filtering.
    /// </summary>
    /// <param name="range">The language range to filter by.</param>
    /// <param name="tags">The language tags to filter.</param>
    /// <returns>A read-only list of language tags that match the range.</returns>
    public static IReadOnlyList<LanguageTag> BasicFilter(this LanguageRange range, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tags);

        return Bcp47Matcher.BasicFilter(range, tags);
    }

    /// <summary>
    /// Filters language tags using RFC 4647 Extended Filtering.
    /// </summary>
    /// <param name="range">The language range to filter by.</param>
    /// <param name="tags">The language tags to filter.</param>
    /// <returns>A read-only list of language tags that match the range.</returns>
    public static IReadOnlyList<LanguageTag> ExtendedFilter(this LanguageRange range, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tags);

        return Bcp47Matcher.ExtendedFilter(range, tags);
    }

    /// <summary>
    /// Determines whether the language tag matches this range using RFC 4647 Basic Filtering.
    /// </summary>
    /// <param name="range">The language range to match against.</param>
    /// <param name="tag">The language tag to test.</param>
    /// <returns><c>true</c> if the tag matches the range; otherwise, <c>false</c>.</returns>
    public static bool IsBasicMatch(this LanguageRange range, LanguageTag tag)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tag);

        return Bcp47Matcher.IsBasicMatch(range, tag);
    }

    /// <summary>
    /// Determines whether the language tag matches this range using RFC 4647 Extended Filtering.
    /// </summary>
    /// <param name="range">The language range to match against.</param>
    /// <param name="tag">The language tag to test.</param>
    /// <returns><c>true</c> if the tag matches the range; otherwise, <c>false</c>.</returns>
    public static bool IsExtendedMatch(this LanguageRange range, LanguageTag tag)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tag);

        return Bcp47Matcher.IsExtendedMatch(range, tag);
    }

    /// <summary>
    /// Looks up the best matching language tag for this range from a collection of tags.
    /// </summary>
    /// <param name="range">The language range to match against.</param>
    /// <param name="tags">The collection of language tags to search.</param>
    /// <returns>The best matching language tag.</returns>
    public static LanguageTag Lookup(this LanguageRange range, IEnumerable<LanguageTag> tags)
    {
        ArgumentNullException.ThrowIfNull(range);
        ArgumentNullException.ThrowIfNull(tags);

        return Bcp47Matcher.Lookup(range, tags);
    }
}