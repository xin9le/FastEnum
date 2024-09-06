﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FastEnumUtility.Generators;



[Generator(LanguageNames.CSharp)]
public sealed class FastEnumBoosterGenerator : IIncrementalGenerator
{
    #region IIncrementalGenerator
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeDeclarations
            = context.SyntaxProvider.ForAttributeWithMetadataName
            (
                fullyQualifiedMetadataName: "FastEnumUtility.FastEnumAttribute`1",
                predicate: static (node, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return node
                        is ClassDeclarationSyntax
                        or StructDeclarationSyntax
                        or RecordDeclarationSyntax;
                },
                transform: static (context, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return context;
                }
            );
        var parseOptions = context.ParseOptionsProvider;
        var source = typeDeclarations.Combine(parseOptions);

        context.RegisterSourceOutput(source, static (context, source) =>
        {
            var syntax = source.Left;
            var options = (CSharpParseOptions)source.Right;
            var param = new GenerateParameters(syntax, options);
            var code = Generate(param);
            context.AddSource(param.FileName, code);
        });
    }
    #endregion


    #region Helpers
    private static string Generate(GenerateParameters param)
    {
        var sb = new StringBuilder();

        //--- header
        sb.AppendLine("""
            // <auto-generated>
            // This .cs file is generated by FastEnum source generator.
            // </auto-generated>
            #nullable enable

            using System;
            using System.Globalization;
            using System.Runtime.CompilerServices;
            using FastEnumUtility;
            """);

        //--- start namespace
        if (!param.ContainerType.IsGlobalNamespace)
        {
            sb.AppendLine($"""

                namespace {param.ContainerType.Namespace};
                """);
        }

        //--- start class
        sb.AppendLine($$"""

            partial {{param.ContainerType.TypeKind}} {{param.ContainerType.TypeName}} : IFastEnumBooster<{{param.EnumType.TypeName}}>
            {
                #region IFastEnumBooster<T>
            """);

        //--- .GetName()
        {
            sb.AppendLine($$"""
                    /// <inheritdoc/>
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    static string? IFastEnumBooster<{{param.EnumType.TypeName}}>.GetName({{param.EnumType.TypeName}} value)
                    {
                        return value switch
                        {
                """);
            foreach (var filed in param.EnumType.Fields)
            {
                sb.AppendLine($"            {param.EnumType.TypeName}.{filed.Name} => nameof({param.EnumType.TypeName}.{filed.Name}),");
            }
            sb.AppendLine($$"""
                            _ => null,
                        };
                    }
                """);
        }

        //--- .IsDefined(TEnum)
        {
            sb.AppendLine($$"""

                    /// <inheritdoc/>
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    static bool IFastEnumBooster<{{param.EnumType.TypeName}}>.IsDefined({{param.EnumType.TypeName}} value)
                    {
                        return value switch
                        {
                """);
            foreach (var filed in param.EnumType.Fields)
            {
                sb.AppendLine($"            {param.EnumType.TypeName}.{filed.Name} => true,");
            }
            sb.AppendLine($$"""
                            _ => false,
                        };
                    }
                """);
        }

        //--- .IsDefined(ReadOnlySpan<char>)
        {
            sb.AppendLine($$"""

                    /// <inheritdoc/>
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    static bool IFastEnumBooster<{{param.EnumType.TypeName}}>.IsDefined(ReadOnlySpan<char> name)
                    {
                        return name switch
                        {
                """);
            foreach (var filed in param.EnumType.Fields)
            {
                sb.AppendLine($"            nameof({param.EnumType.TypeName}.{filed.Name}) => true,");
            }
            sb.AppendLine($$"""
                            _ => false,
                        };
                    }
                """);
        }

        //--- .TryParseName()
        {
            sb.AppendLine($$"""

                    /// <inheritdoc/>
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    static bool IFastEnumBooster<{{param.EnumType.TypeName}}>.TryParseName(ReadOnlySpan<char> text, bool ignoreCase, out {{param.EnumType.TypeName}} result)
                    {
                        return ignoreCase
                            ? caseInsensitive(text, out result)
                            : caseSensitive(text, out result);


                        #region Local Functions
                        [MethodImpl(MethodImplOptions.AggressiveInlining)]
                        static bool caseSensitive(ReadOnlySpan<char> text, out {{param.EnumType.TypeName}} result)
                        {
                            switch (text)
                            {
                """);
            foreach (var filed in param.EnumType.Fields)
            {
                sb.AppendLine($$"""
                                    case nameof({{param.EnumType.TypeName}}.{{filed.Name}}):
                                        result = {{param.EnumType.TypeName}}.{{filed.Name}};
                                        return true;

                    """);
            }
            sb.AppendLine($$"""
                                default:
                                    result = default;
                                    return false;
                            }
                        }


                        [MethodImpl(MethodImplOptions.AggressiveInlining)]
                        static bool caseInsensitive(ReadOnlySpan<char> text, out {{param.EnumType.TypeName}} result)
                        {
                            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                """);
            foreach (var filed in param.EnumType.Fields)
            {
                sb.AppendLine($$"""
                                if (text.Equals(nameof({{param.EnumType.TypeName}}.{{filed.Name}}), comparison))
                                {
                                    result = {{param.EnumType.TypeName}}.{{filed.Name}};
                                    return true;
                                }
                    """);
            }
            sb.AppendLine($$"""
                            result = default;
                            return false;
                        }
                        #endregion
                    }
                """);
        }

        //--- end class
        sb.AppendLine("""
                #endregion
            }
            """);

        //--- ok
        return sb.ToString();
    }
    #endregion


    #region Nested Types
    private sealed class GenerateParameters
    {
        #region Properties
        public LanguageVersion LanguageVersion { get; }
        public string FileName { get; }
        public ContainerTypeMetadata ContainerType { get; }
        public EnumTypeMetadata EnumType { get; }
        #endregion


        #region Constructors
        public GenerateParameters(GeneratorAttributeSyntaxContext context, CSharpParseOptions parseOptions)
        {
            var containerSymbol = (INamedTypeSymbol)context.TargetSymbol;
            var enumSymbol = getEnumSymbol(context);

            this.LanguageVersion = parseOptions.LanguageVersion;
            this.FileName = createFileName(containerSymbol);
            this.ContainerType = new(containerSymbol);
            this.EnumType = new(enumSymbol);


            #region Local Functions
            static string createFileName(ISymbol symbol)
            {
                var typeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var escaped = typeName.Replace("global::", "").Replace("<", "_").Replace(">", "_");
                return $"{escaped}.FastEnumBooster.g.cs";
            }


            static INamedTypeSymbol getEnumSymbol(GeneratorAttributeSyntaxContext context)
            {
                var attr = context.Attributes.First(static x => x.AttributeClass!.MetadataName is "FastEnumAttribute`1");
                return (INamedTypeSymbol)attr.AttributeClass!.TypeArguments[0];
            }
            #endregion
        }
        #endregion
    }



    private sealed class ContainerTypeMetadata
    {
        public bool IsGlobalNamespace { get; }
        public bool IsGenericType { get; }
        public string Namespace { get; }
        public string TypeKind { get; }
        public string TypeName { get; }


        public ContainerTypeMetadata(INamedTypeSymbol symbol)
        {
            this.IsGlobalNamespace = symbol.ContainingNamespace.IsGlobalNamespace;
            this.IsGenericType = symbol.IsGenericType;
            this.Namespace = symbol.ContainingNamespace.ToDisplayString();
            this.TypeKind = toTypeKind(symbol);
            this.TypeName = symbol.Name;


            #region Local Functions
            static string toTypeKind(INamedTypeSymbol symbol)
            {
                return (symbol.IsRecord, symbol.IsValueType) switch
                {
                    (true, true) => "record struct",
                    (true, false) => "record class",
                    (false, true) => "struct",
                    (false, false) => "class",
                };
            }
            #endregion
        }
    }



    private sealed class EnumTypeMetadata(INamedTypeSymbol symbol)
    {
        public string TypeName { get; } = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        public string UnderlyingType { get; } = symbol.EnumUnderlyingType?.ToDisplayString() ?? "int";
        public IReadOnlyList<IFieldSymbol> Fields { get; } = symbol.GetMembers().OfType<IFieldSymbol>().ToArray();
    }
    #endregion
}
