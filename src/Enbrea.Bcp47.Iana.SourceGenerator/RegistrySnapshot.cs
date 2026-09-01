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
/// Represents the JSON snapshot consumed by the source generator.
/// </summary>
internal sealed class RegistrySnapshot
{
    /// <summary>
    /// Gets or sets the date of the registry snapshot.
    /// </summary>
    public string FileDate { get; set; }

    /// <summary>
    /// Gets or sets the list of records in the registry snapshot.
    /// </summary>
    public List<RegistryRecord> Records { get; set; }
}
