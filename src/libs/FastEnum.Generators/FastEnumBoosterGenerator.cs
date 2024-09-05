using FastEnumUtility.Generators.Emitters;
using Microsoft.CodeAnalysis;

namespace FastEnumUtility.Generators;



[Generator(LanguageNames.CSharp)]
public sealed class FastEnumBoosterGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        ClassDeclarationEmitter.Register(context);
    }
}
