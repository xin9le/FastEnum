using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            predicate: static (node, cancellationToken) =>
            {
                return node is EnumDeclarationSyntax;
            },
            transform: static (context, cancellationToken) =>
            {
                return (EnumDeclarationSyntax)context.TargetNode;
            }
        );
        context.RegisterSourceOutput(source, SourceCodeEmitter.Emit);
    }
}



file static class SourceCodeEmitter
{
    public static void Emit(SourceProductionContext context, EnumDeclarationSyntax source)
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
