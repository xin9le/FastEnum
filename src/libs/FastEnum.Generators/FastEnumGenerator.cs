using Microsoft.CodeAnalysis;

namespace FastEnumUtility.Generators;



[Generator(LanguageNames.CSharp)]
public sealed class FastEnumGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context.SyntaxProvider.ForAttributeWithMetadataName
        (
            fullyQualifiedMetadataName: "FastEnumUtility.FastEnumAttribute",
            predicate: static (node, cancellationToken) => true,
            transform: static (context, cancellationToken) => context
        );
        context.RegisterSourceOutput(source, SourceCodeEmitter.Emit);
    }
}



file static class SourceCodeEmitter
{
    public static void Emit(SourceProductionContext context, GeneratorAttributeSyntaxContext source)
    {
        const string code = """
using System;

namespace FastEnumUtility.Generated;



file static class SampleClass
{
}
""";
        context.AddSource($"FastEnum.GeneratorSample.g.cs", code);
    }
}
