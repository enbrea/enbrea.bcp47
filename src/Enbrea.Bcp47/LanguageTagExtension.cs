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
/// Represents an extension sequence of a BCP 47 language tag.
/// </summary>
public sealed class LanguageTagExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageTagExtension"/> class.
    /// </summary>
    internal LanguageTagExtension(char singleton, IReadOnlyList<string> subtags)
    {
        Singleton = singleton;
        Subtags = subtags;
    }

    /// <summary>
    /// Gets the singleton character of the extension.
    /// </summary>
    public char Singleton { get; }

    /// <summary>
    /// Gets the list of subtags associated with the extension.
    /// </summary>
    public IReadOnlyList<string> Subtags { get; }
}