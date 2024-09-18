using Microsoft.CodeAnalysis;

namespace FastEnumUtility.Generators.Internals;



internal static class DiagnosticDescriptorProvider
{
    #region Constants
    const string Category = "FastEnum";
    #endregion


#pragma warning disable RS2008
    public static readonly DiagnosticDescriptor MustBePartial
        = new(
            id: "FE0001",
            title: "FastEnum booster type must be partial",
            messageFormat: "FastEnum booster type '{0}' must be partial",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );


    public static readonly DiagnosticDescriptor MustNotBeNested
        = new(
            id: "FE0002",
            title: "FastEnum booster type must not be nested",
            messageFormat: "FastEnum booster type '{0}' must not be nested",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );


    public static readonly DiagnosticDescriptor MustBePublicOrInternal
        = new(
            id: "FE0003",
            title: "The accessibility of the enum type that is being boosted must be public or internal",
            messageFormat: "The accessibility of the enum type '{0}' that is being boosted must be public or internal",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );


    public static readonly DiagnosticDescriptor MustBeEnumType
        = new(
            id: "FE0004",
            title: "The generic type argument of the FastEnumAttribute<T> must be an enum type",
            messageFormat: "The generic type argument '{0}' of the FastEnumAttribute<T> must be an enum type",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
#pragma warning restore RS2008
}
