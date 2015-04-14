﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;
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
    [SqaleSubCharacteristic(SqaleSubCharacteristic.Readability)]
    [SqaleConstantRemediation("5min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    [LegacyKey("MethodName")]
    public class MethodName : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S100";
        internal const string Description = "Method name should comply with a naming convention";
        internal const string MessageFormat = "Rename this method to match the regular expression: {0}";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Minor; 
        internal const bool IsActivatedByDefault = true;

        internal static DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: "http://nemo.sonarqube.org/coding_rules#rule_key=csharpsquid%3AMethodName");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        [RuleParameter("format", PropertyType.String, "Regular expression used to check the method names against",
            "^[A-Z][a-zA-Z0-9]+$")]
        public string Convention { get; set; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var identifierNode = ((MethodDeclarationSyntax)c.Node).Identifier;

                    if (!Regex.IsMatch(identifierNode.Text, Convention))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, identifierNode.GetLocation(), Convention));
                    }
                },
                SyntaxKind.MethodDeclaration);
        }
    }
}
