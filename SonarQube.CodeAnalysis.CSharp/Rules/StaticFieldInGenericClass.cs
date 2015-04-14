using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarQube.CodeAnalysis.CSharp.Helpers;
using SonarQube.CodeAnalysis.CSharp.SonarQube.Settings;
using SonarQube.CodeAnalysis.CSharp.SonarQube.Settings.Sqale;

namespace SonarQube.CodeAnalysis.CSharp.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.InstructionReliability)]
    [SqaleConstantRemediation("10min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    public class StaticFieldInGenericClass : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S2743";
        internal const string Description = @"Static fields should not be used in generic types";
        internal const string MessageFormat = @"A static field in a generic type is not shared among instances of different close constructed types.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Critical; 
        internal const bool IsActivatedByDefault = true;

        internal static DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: "http://nemo.sonarqube.org/coding_rules#rule_key=csharpsquid%3AS2743");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var classDeclaration = (ClassDeclarationSyntax)c.Node;

                    if (classDeclaration.TypeParameterList == null ||
                        classDeclaration.TypeParameterList.Parameters.Count < 1)
                    {
                        return;
                    }
                    
                    var typeParameterNames =
                        classDeclaration.TypeParameterList.Parameters.Select(p => p.Identifier.ToString()).ToList();

                    var fields = classDeclaration.Members
                        .OfType<FieldDeclarationSyntax>()
                        .Where(f => f.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
                        .ToList();

                    fields.ForEach(field => CheckMember(field, field.Declaration.Type, typeParameterNames, c));

                    var properties = classDeclaration.Members
                        .OfType<PropertyDeclarationSyntax>()
                        .Where(p => p.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
                        .ToList();

                    properties.ForEach(property => CheckMember(property, property.Type, typeParameterNames, c));
                },
                SyntaxKind.ClassDeclaration);
        }

        private static void CheckMember(SyntaxNode node, TypeSyntax type, IEnumerable<string> typeParameterNames, 
            SyntaxNodeAnalysisContext c)
        {
            var genericTypeName = type as GenericNameSyntax;

            if (genericTypeName != null &&
                GetTypeArguments(genericTypeName)
                    .Intersect(typeParameterNames)
                    .Any())
            {
                return;
            }

            c.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
        }

        private static IEnumerable<string> GetTypeArguments(GenericNameSyntax genericTypeName)
        {
            var typeParameters = new List<string>();

            var arguments = genericTypeName.TypeArgumentList.Arguments;

            foreach (var typeSyntax in arguments)
            {
                var innerGenericTypeName = typeSyntax as GenericNameSyntax;
                if (innerGenericTypeName != null)
                {
                    typeParameters.AddRange(GetTypeArguments(innerGenericTypeName));
                }
                else
                {
                    typeParameters.Add(typeSyntax.ToString());
                }
            }

            return typeParameters;
        }
    }
}