﻿using Microsoft.CodeAnalysis;

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
#pragma warning restore RS2008
}