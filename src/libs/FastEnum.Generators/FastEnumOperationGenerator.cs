using FastEnumUtility.Generators.Emitters;
using Microsoft.CodeAnalysis;

namespace FastEnumUtility.Generators;



[Generator(LanguageNames.CSharp)]
public sealed class FastEnumOperationGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        EnumDeclarationEmitter.Register(context);
        ClassDeclarationEmitter.Register(context);
    }
}
