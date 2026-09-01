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
/// Provides a fluent API for constructing BCP 47 language tags.
/// </summary>
public sealed class LanguageTagBuilder
{
	private readonly List<BuilderExtension> _extensions = [];
	private readonly List<string> _extlangs = [];
	private readonly List<string> _privateUse = [];
	private readonly List<string> _variants = [];
	private string _language;
	private string _region;
	private string _script;

	/// <summary>
	/// Creates a new <see cref="LanguageTagBuilder"/> from an existing <see cref="LanguageTag"/>.
	/// </summary>
	/// <param name="tag">The existing <see cref="LanguageTag"/> to initialize the builder with.</param>
	/// <returns>A new <see cref="LanguageTagBuilder"/> initialized with the components of the specified <see cref="LanguageTag"/>.</returns>
	public static LanguageTagBuilder From(LanguageTag tag)
	{
		ArgumentNullException.ThrowIfNull(tag);

		if (tag.IsGrandfathered)
		{
			throw new ArgumentException("Grandfathered language tags cannot be decomposed into builder components.", nameof(tag));
		}

		var builder = new LanguageTagBuilder();

		if (tag.Language is not null)
		{
			builder.Language(tag.Language);
		}

		foreach (var extlang in tag.Extlangs)
		{
			builder.Extlang(extlang);
		}

		if (tag.Script is not null)
		{
			builder.Script(tag.Script);
		}

		if (tag.Region is not null)
		{
			builder.Region(tag.Region);
		}

		foreach (var variant in tag.Variants)
		{
			builder.Variant(variant);
		}

		foreach (var extension in tag.Extensions)
		{
			builder.Extension(
				extension.Singleton,
				[.. extension.Subtags]);
		}

		if (tag.PrivateUse.Count > 0)
		{
			builder.PrivateUse([.. tag.PrivateUse]);
		}

		return builder;
	}

	/// <summary>
	/// Builds a <see cref="LanguageTag"/> from the specified components.
	/// </summary>
	/// <returns>A <see cref="LanguageTag"/> instance representing the constructed language tag.</returns>
	public LanguageTag Build()
	{
		return Bcp47Parser.Parse(BuildValue());
	}

	/// <summary>
	/// Adds an extension sequence to the language tag.
	/// </summary>
	/// <param name="singleton">The singleton character identifying the extension.</param>
	/// <param name="subtags">The subtags for the extension.</param>
	/// <returns>The current <see cref="LanguageTagBuilder"/> instance.</returns>
	public LanguageTagBuilder Extension(char singleton, params string[] subtags)
	{
		ArgumentNullException.ThrowIfNull(subtags);

		if (!Bcp47Parser.IsExtensionSingleton(singleton))
		{
			throw new ArgumentException($"'{singleton}' is not a valid extension singleton.", nameof(singleton));
		}

		if (subtags.Length == 0)
		{
			throw new ArgumentException("At least one extension subtag is required.", nameof(subtags));
		}

		foreach (var subtag in subtags)
		{
			if (string.IsNullOrEmpty(subtag) || !Bcp47Parser.IsExtensionSubtag(subtag))
			{
				throw new ArgumentException($"'{subtag}' is not a valid extension subtag.", nameof(subtags));
			}
		}

		_extensions.Add(new BuilderExtension(singleton, [.. subtags]));

		return this;
	}

	/// <summary>
	/// Adds an extended language subtag to the language tag.
	/// </summary>
	/// <param name="value">The extended language subtag to add.</param>
	/// <returns>The current <see cref="LanguageTagBuilder"/> instance.</returns>
	public LanguageTagBuilder Extlang(string value)
	{
		ArgumentException.ThrowIfNullOrEmpty(value);

		if (!Bcp47Parser.IsExtlangSubtag(value))
		{
			throw new ArgumentException($"'{value}' is not a valid extended-language subtag.", nameof(value));
		}

		_extlangs.Add(value);
		return this;
	}

	/// <summary>
	/// Sets the primary language subtag.
	/// </summary>
	/// <param name="value">The primary language subtag to set.</param>
	/// <returns>The current <see cref="LanguageTagBuilder"/> instance.</returns>   
	public LanguageTagBuilder Language(string value)
	{
		ArgumentException.ThrowIfNullOrEmpty(value);

		if (!Bcp47Parser.IsLanguageSubtag(value))
		{
			throw new ArgumentException($"'{value}' is not a valid primary language subtag.", nameof(value));
		}

		_language = value;
		return this;
	}
	/// <summary>
	/// Adds private-use subtags.
	/// </summary>
	/// <param name="subtags">The private-use subtags to add.</param>
	/// <returns>The current <see cref="LanguageTagBuilder"/> instance.</returns>
	public LanguageTagBuilder PrivateUse(params string[] subtags)
	{
		ArgumentNullException.ThrowIfNull(subtags);

		if (subtags.Length == 0)
		{
			throw new ArgumentException("At least one private-use subtag is required.", nameof(subtags));
		}

		foreach (var subtag in subtags)
		{
			if (string.IsNullOrEmpty(subtag) || !Bcp47Parser.IsPrivateUseSubtag(subtag))
			{
				throw new ArgumentException($"'{subtag}' is not a valid private-use subtag.", nameof(subtags));
			}
		}

		_privateUse.AddRange(subtags);

		return this;
	}

	/// <summary>
	/// Sets the region subtag.
	/// </summary>
	/// <param name="value">The region subtag to set.</param>
	/// <returns>The current <see cref="LanguageTagBuilder"/> instance.</returns>
	public LanguageTagBuilder Region(string value)
	{
		ArgumentException.ThrowIfNullOrEmpty(value);

		if (!Bcp47Parser.IsRegionSubtag(value))
		{
			throw new ArgumentException($"'{value}' is not a valid region subtag.", nameof(value));
		}

		_region = value;
		return this;
	}

	/// <summary>
	/// Sets the script subtag.
	/// </summary>
	/// <param name="value">The script subtag to set.</param>
	/// <returns>The current <see cref="LanguageTagBuilder"/> instance.</returns>   
	public LanguageTagBuilder Script(string value)
	{
		ArgumentException.ThrowIfNullOrEmpty(value);

		if (!Bcp47Parser.IsScriptSubtag(value))
		{
			throw new ArgumentException($"'{value}' is not a valid script subtag.", nameof(value));
		}

		_script = value;
		return this;
	}

	/// <summary>
	/// Adds a variant subtag.
	/// </summary>
	/// <param name="value">The variant subtag to add.</param>
	/// <returns>The current <see cref="LanguageTagBuilder"/> instance.</returns>
	public LanguageTagBuilder Variant(string value)
	{
		ArgumentException.ThrowIfNullOrEmpty(value);

		if (!Bcp47Parser.IsVariantSubtag(value))
		{
			throw new ArgumentException($"'{value}' is not a valid variant subtag.", nameof(value));
		}

		_variants.Add(value);
		return this;
	}

	/// <summary>
	/// Builds the language tag value from the specified components.
	/// </summary>
	private string BuildValue()
	{
		// Without a primary language only private-use-only tags are possible.
		if (_language is null)
		{
			if (_privateUse.Count == 0)
			{
				throw new InvalidOperationException("A primary language or private-use subtag is required.");
			}

			if (_extlangs.Count > 0 ||
				_script is not null ||
				_region is not null ||
				_variants.Count > 0 ||
				_extensions.Count > 0)
			{
				throw new InvalidOperationException("A primary language is required when using extlang, script, region, variant, or extension subtags.");
			}

			return "x-" + string.Join('-', _privateUse);
		}

		var sb = new StringBuilder(_language);

		foreach (var extlang in _extlangs)
		{
			sb.Append('-').Append(extlang);
		}

		if (_script is not null)
		{
			sb.Append('-').Append(_script);
		}

		if (_region is not null)
		{
			sb.Append('-').Append(_region);
		}

		foreach (var variant in _variants)
		{
			sb.Append('-').Append(variant);
		}

		foreach (var extension in _extensions)
		{
			sb.Append('-').Append(extension.Singleton);

			foreach (var subtag in extension.Subtags)
			{
				sb.Append('-').Append(subtag);
			}
		}

		if (_privateUse.Count > 0)
		{
			sb.Append("-x");

			foreach (var subtag in _privateUse)
			{
				sb.Append('-').Append(subtag);
			}
		}

		return sb.ToString();
	}

	/// <summary>
	/// Represents an extension sequence in the language tag builder.
	/// </summary>
	private sealed record BuilderExtension(
		char Singleton,
		IReadOnlyList<string> Subtags);
}