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
/// Specifies why an IANA-aware BCP 47 validation failed.
/// </summary>
public enum IanaLanguageTagValidationError
{
    /// <summary>
    /// No validation error occurred.
    /// </summary>
    None,

    /// <summary>
    /// The tag is not structurally valid according to BCP 47.
    /// </summary>
    StructurallyInvalid,

    /// <summary>
    /// The grandfathered tag is not registered.
    /// </summary>
    UnregisteredGrandfatheredTag,

    /// <summary>
    /// The language subtag is not registered.
    /// </summary>
    UnregisteredLanguage,

    /// <summary>
    /// The extlang subtag is not registered.
    /// </summary>
    UnregisteredExtlang,

    /// <summary>
    /// The script subtag is not registered.
    /// </summary>
    UnregisteredScript,

    /// <summary>
    /// The region subtag is not registered.
    /// </summary>
    UnregisteredRegion,

    /// <summary>
    /// The variant subtag is not registered.
    /// </summary>
    UnregisteredVariant
}
