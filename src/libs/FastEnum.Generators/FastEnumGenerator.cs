using Microsoft.CodeAnalysis;

namespace FastEnumUtility.Generators;



[Generator(LanguageNames.CSharp)]
public sealed class FastEnumGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    { }
}
