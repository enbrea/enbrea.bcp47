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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Enbrea.Bcp47.Iana.SourceGenerator.XUnit;

/// <summary>
/// Unit tests for <see cref="IanaLanguageSubtagRegistryGenerator"/>.
/// </summary>
public class IanaLanguageSubtagRegistryGeneratorTests
{
    private const string ValidRegistryJson = """
        {
          "fileDate": "2026-08-08",
          "records": [
            {
              "type": "language",
              "subtag": "de",
              "suppressScript": "Latn"
            },
            {
              "type": "language",
              "subtag": "bh",
              "deprecated": "2026-06-14",
              "preferredValue": "bih"
            },
            {
              "type": "language",
              "subtag": "bih"
            },
            {
              "type": "language",
              "subtag": "qaa..qtz"
            },
            {
              "type": "script",
              "subtag": "Qaaa..Qabx"
            },
            {
              "type": "region",
              "subtag": "QM..QZ"
            },
            {
              "type": "variant",
              "subtag": "1901",
              "prefixes": [
                "de"
              ]
            }
          ]
        }
        """;

    [Fact]
    public void Generate_ProducesDeterministicSortedOutput()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                "language-subtag-registry.json",
                """
                {
                  "fileDate": "2026-08-08",
                  "records": [
                    {
                      "type": "language",
                      "subtag": "fr"
                    },
                    {
                      "type": "language",
                      "subtag": "de"
                    },
                    {
                      "type": "language",
                      "subtag": "en"
                    }
                  ]
                }
                """));

        Assert.Empty(result.Diagnostics);

        var source = Assert
            .Single(result.Results)
            .GeneratedSources
            .Single()
            .SourceText
            .ToString();

        var de = source.IndexOf("\"de\"");
        var en = source.IndexOf("\"en\"");
        var fr = source.IndexOf("\"fr\"");

        Assert.True(de >= 0);
        Assert.True(en > de);
        Assert.True(fr > en);
    }

    [Fact]
    public void Generate_WithDifferentRegistryFileNameCasing_GeneratesSource()
    {
        var result = RunGenerator(new TestAdditionalText("LANGUAGE-SUBTAG-REGISTRY.JSON", ValidRegistryJson));

        Assert.Empty(result.Diagnostics);
        Assert.Single(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void Generate_WithInvalidFileDate_ReportsBCP47003()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                "language-subtag-registry.json",
                """
                {
                  "fileDate": "08/08/2026",
                  "records": []
                }
                """));

        var diagnostic = Assert.Single(result.Diagnostics);

        Assert.Equal("BCP47003", diagnostic.Id);
        Assert.Contains("fileDate", diagnostic.GetMessage());
        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void Generate_WithMalformedJson_ReportsBCP47003()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                "language-subtag-registry.json",
                """
                {
                  "fileDate": "2026-08-08",
                  "records":
                }
                """));

        var diagnostic = Assert.Single(result.Diagnostics);

        Assert.Equal("BCP47003", diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void Generate_WithMissingSubtag_ReportsBCP47003()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                "language-subtag-registry.json",
                """
                {
                  "fileDate": "2026-08-08",
                  "records": [
                    {
                      "type": "language"
                    }
                  ]
                }
                """));

        var diagnostic = Assert.Single(result.Diagnostics);

        Assert.Equal("BCP47003", diagnostic.Id);
        Assert.Contains("subtag or tag", diagnostic.GetMessage());
    }

    [Fact]
    public void Generate_WithMultipleRegistries_ReportsBCP47002()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                @"first\language-subtag-registry.json",
                ValidRegistryJson),
            new TestAdditionalText(
                @"second\language-subtag-registry.json",
                ValidRegistryJson));

        var diagnostic = Assert.Single(result.Diagnostics);

        Assert.Equal("BCP47002", diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void Generate_WithoutRecords_ReportsBCP47003()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                "language-subtag-registry.json",
                """
                {
                  "fileDate": "2026-08-08"
                }
                """));

        var diagnostic = Assert.Single(result.Diagnostics);

        Assert.Equal("BCP47003", diagnostic.Id);
        Assert.Contains("records", diagnostic.GetMessage());
        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void Generate_WithoutRegistry_ReportsBCP47001()
    {
        var result = RunGenerator();

        var diagnostic = Assert.Single(result.Diagnostics);

        Assert.Equal("BCP47001", diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void Generate_WithUnsupportedRecordType_ReportsBCP47003()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                "language-subtag-registry.json",
                """
                {
                  "fileDate": "2026-08-08",
                  "records": [
                    {
                      "type": "unknown",
                      "subtag": "abc"
                    }
                  ]
                }
                """));

        var diagnostic = Assert.Single(result.Diagnostics);

        Assert.Equal("BCP47003", diagnostic.Id);
        Assert.Contains("unsupported record type", diagnostic.GetMessage());
        Assert.Empty(result.Results.Single().GeneratedSources);
    }

    [Fact]
    public void Generate_WithValidRegistry_GeneratesExpectedSource()
    {
        var result = RunGenerator(
            new TestAdditionalText(
                "language-subtag-registry.json",
                ValidRegistryJson));

        Assert.Empty(result.Diagnostics);

        var generatorResult = Assert.Single(result.Results);
        var generatedSource = Assert.Single(generatorResult.GeneratedSources);

        Assert.Equal("IanaLanguageSubtagRegistry.Generated.cs", generatedSource.HintName);

        var source = generatedSource.SourceText.ToString();

        Assert.Contains("public static partial class IanaLanguageSubtagRegistry", source);
        Assert.Contains("public static readonly global::System.DateOnly FileDate = new(2026, 8, 8);", source);
        Assert.Contains("\"de\"", source);
        Assert.Contains("\"bh\"", source);
        Assert.Contains("\"bih\"", source);
        Assert.Contains("new(\"qaa\", \"qtz\")", source);
        Assert.Contains("new(\"Qaaa\", \"Qabx\")", source);
        Assert.Contains("new(\"QM\", \"QZ\")", source);
        Assert.Contains("new(\"bh\", \"bih\")", source);
        Assert.Contains("new(\"de\", \"Latn\")", source);
        Assert.Contains("new(\"1901\", \"de\")", source);
    }

    private static GeneratorDriverRunResult RunGenerator(params AdditionalText[] additionalTexts)
    {
        var generator = new IanaLanguageSubtagRegistryGenerator();

        var compilation = CSharpCompilation.Create(assemblyName: "GeneratorTests");

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: additionalTexts,
            parseOptions: new CSharpParseOptions(LanguageVersion.Latest));

        driver = driver.RunGenerators(compilation);

        return driver.GetRunResult();
    }

    private sealed class TestAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        public TestAdditionalText(string path, string content)
        {
            Path = path;
            _text = SourceText.From(content, Encoding.UTF8);
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            return _text;
        }
    }
}