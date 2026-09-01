[![NuGet Enbrea.Bcp47](https://img.shields.io/nuget/v/Enbrea.Bcp47?label=Enbrea.Bcp47)](https://www.nuget.org/packages/Enbrea.Bcp47/)
[![NuGet Enbrea.Bcp47.Iana](https://img.shields.io/nuget/v/Enbrea.Bcp47.Iana?label=Enbrea.Bcp47.Iana)](https://www.nuget.org/packages/Enbrea.Bcp47.Iana/)
![GitHub](https://img.shields.io/github/license/enbrea/enbrea.bcp47)

# Enbrea.Bcp47

A .NET implementation of [IETF BCP 47](https://www.rfc-editor.org/info/bcp47) for parsing, validating, inspecting, normalizing, and matching language tags and language ranges.

This repository contains two packages:

+ [`Enbrea.Bcp47`](https://www.nuget.org/packages/Enbrea.Bcp47) provides registry-independent parsing, structural validation, normalization, language-tag construction, and language-range matching based on RFC 5646 and RFC 4647.
+ [`Enbrea.Bcp47.Iana`](https://www.nuget.org/packages/Enbrea.Bcp47.Iana) extends the core library with validation and metadata based on a bundled snapshot of the IANA Language Subtag Registry.

## Enbrea.Bcp47

The core package provides BCP 47 functionality without requiring registry data.

+ Supports `.NET 10`, `.NET 9`, and `.NET 8`.
+ Implements the language tag grammar defined by [RFC 5646](https://datatracker.ietf.org/doc/html/rfc5646).
+ Implements language ranges and matching as defined by [RFC 4647](https://datatracker.ietf.org/doc/html/rfc4647).
+ Parses language tags into strongly typed components.
+ Supports primary language, extended-language, script, region, and variant subtags.
+ Supports extensions and private-use subtags.
+ Supports grandfathered language tags.
+ Provides `Parse` and `TryParse` APIs for language tags and language ranges.
+ Provides a fluent `LanguageTagBuilder` for constructing language tags.
+ Performs registry-independent structural validation.
+ Provides normalization of language tags and language ranges.
+ Supports Basic Filtering, Extended Filtering, and Lookup.
+ Has zero external dependencies.

## Enbrea.Bcp47.Iana

The optional IANA package adds registry-aware functionality based on the [IANA Language Subtag Registry](https://www.iana.org/assignments/language-subtag-registry/language-subtag-registry).

+ Depends on `Enbrea.Bcp47`.
+ Contains a bundled snapshot of the IANA Language Subtag Registry.
+ Validates language, extended-language, script, region, and variant subtags against the registry.
+ Supports grandfathered and redundant language tags registered by IANA.
+ Provides access to registry metadata such as deprecated subtags, preferred values, suppress-script values, macrolanguages, and prefixes.
+ Provides registry-aware validation through `IanaLanguageTagValidator`.
+ Provides convenient extension methods for parsed `LanguageTag` instances.
+ Requires no network access at runtime.

## Installation

Install the core package:

``` shell
dotnet add package Enbrea.Bcp47
```

For registry-aware validation and access to IANA registry metadata, install the IANA package:

``` shell
dotnet add package Enbrea.Bcp47.Iana
```

`Enbrea.Bcp47.Iana` depends on `Enbrea.Bcp47`, so installing the IANA package automatically includes the core package.

## Getting started

Documentation is available in the [GitHub wiki](https://github.com/enbrea/enbrea.bcp47/wiki).

## Can I help?

Yes, that would be much appreciated. The best way to help is to post a response via the Issue Tracker and/or submit a Pull Request.
