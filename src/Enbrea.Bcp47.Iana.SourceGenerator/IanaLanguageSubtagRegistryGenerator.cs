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
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Enbrea.Bcp47.Iana.SourceGenerator;

/// <summary>
/// A source generator that reads a JSON snapshot of the IANA Language Subtag Registry and generates
/// a C# source file containing the registry data.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class IanaLanguageSubtagRegistryGenerator : IIncrementalGenerator
{
    private const string GeneratedFileName = "IanaLanguageSubtagRegistry.Generated.cs";
    private const string RegistrySnapshotFileName = "language-subtag-registry.json";

    private static readonly DiagnosticDescriptor InvalidRegistry = new(
        id: "BCP47003",
        title: "Invalid IANA registry snapshot",
        messageFormat: "The IANA registry snapshot '{0}' is invalid: {1}",
        category: "Enbrea.Bcp47.Iana.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingRegistry = new(
        id: "BCP47001",
        title: "IANA registry snapshot not found",
        messageFormat: "The source generator requires exactly one AdditionalFiles item named '{0}'",
        category: "Enbrea.Bcp47.Iana.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MultipleRegistries = new(
        id: "BCP47002",
        title: "Multiple IANA registry snapshots found",
        messageFormat: "The source generator found multiple AdditionalFiles items named '{0}'. Keep exactly one registry snapshot.",
        category: "Enbrea.Bcp47.Iana.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Initializes the source generator by registering the input files and the execution logic.
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var registryFiles = context.AdditionalTextsProvider
            .Where(static file => string.Equals(Path.GetFileName(file.Path), RegistrySnapshotFileName, StringComparison.OrdinalIgnoreCase))
            .Collect();

        context.RegisterSourceOutput(registryFiles, (productionContext, files) => Execute(productionContext, files));
    }

    /// <summary>
    /// Executes the source generator logic to generate the registry data source file.
    /// </summary>
    private static void Execute(SourceProductionContext context, ImmutableArray<AdditionalText> files)
    {
        if (files.IsDefaultOrEmpty)
        {
            context.ReportDiagnostic(Diagnostic.Create(MissingRegistry, Location.None, RegistrySnapshotFileName));
            return;
        }

        if (files.Length != 1)
        {
            context.ReportDiagnostic(Diagnostic.Create(MultipleRegistries, Location.None, RegistrySnapshotFileName));
            return;
        }

        var file = files[0];

        try
        {
            var sourceText = file.GetText(CancellationToken.None);

            var registrySnapshot = JsonSerializer.Deserialize<RegistrySnapshot>(sourceText.ToString(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (registrySnapshot is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidRegistry, Location.None, file.Path, "the JSON document is empty"));
                return;
            }

            var source = RegistrySourceBuilder.Build(registrySnapshot);

            context.AddSource(GeneratedFileName, SourceText.From(source, Encoding.UTF8));
        }
        catch (Exception ex) when (ex is JsonException || ex is InvalidOperationException || ex is NotSupportedException)
        {
            context.ReportDiagnostic(Diagnostic.Create(InvalidRegistry, Location.None, file.Path, ex.Message));
        }
    }
}
