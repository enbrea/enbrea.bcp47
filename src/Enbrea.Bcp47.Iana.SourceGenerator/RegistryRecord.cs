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

namespace Enbrea.Bcp47.Iana.SourceGenerator;

/// <summary>
/// Represents a single record in the JSON snapshot. Only fields required by the generated 
/// runtime registry are modeled.
/// </summary>
internal sealed class RegistryRecord
{
    /// <summary>
    /// Gets or sets the deprecated value of the record, if any.
    /// </summary>
    public string Deprecated { get; set; }

    /// <summary>
    /// Gets or sets the description of the record.
    /// </summary>
    public string Key => Subtag ?? Tag;

    /// <summary>
    /// Gets or sets the description of the record.
    /// </summary>
    public string Macrolanguage { get; set; }

    /// <summary>
    /// Gets or sets the description of the record.
    /// </summary>
    public string PreferredValue { get; set; }

    /// <summary>
    /// Gets or sets the list of prefixes for the record, if any.
    /// </summary>
    public List<string> Prefixes { get; set; }

    /// <summary>
    /// Gets or sets the subtag of the record, if any.
    /// </summary>
    public string Subtag { get; set; }

    /// <summary>
    /// Gets or sets the suppress-script value of the record, if any.
    /// </summary>
    public string SuppressScript { get; set; }

    /// <summary>
    /// Gets or sets the tag of the record, if any.
    /// </summary>
    public string Tag { get; set; }

    /// <summary>
    /// Gets or sets the type of the record.
    /// </summary>
    public string Type { get; set; }
}
