using Microsoft.CodeAnalysis;

namespace FastEnumUtility.Generators.Internals;



internal static class DiagnosticDescriptorProvider
{
    #region Constants
    const string Category = "FastEnum";
    #endregion


    public static readonly DiagnosticDescriptor MustBePartial
        = new(
            id: "FE0001",
            title: "FastEnum booster type must be partial",
            messageFormat: "FastEnum booster type '{0}' must be partial",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
}
