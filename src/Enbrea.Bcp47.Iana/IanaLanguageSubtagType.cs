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
/// Specifies an entry type in the IANA Language Subtag Registry.
/// </summary>
public enum IanaLanguageSubtagType
{
    /// <summary>
    /// A primary language subtag entry.
    /// </summary>
    Language,

    /// <summary>
    /// An extended language subtag entry.
    /// </summary>
    Extlang,

    /// <summary>
    /// A script subtag entry.
    /// </summary>
    Script,

    /// <summary>
    /// A region subtag entry.
    /// </summary>
    Region,

    /// <summary>
    /// A variant subtag entry.
    /// </summary>
    Variant,

    /// <summary>
    /// A grandfathered full-tag entry.
    /// </summary>
    Grandfathered,

    /// <summary>
    /// A redundant full-tag entry.
    /// </summary>
    Redundant
}
