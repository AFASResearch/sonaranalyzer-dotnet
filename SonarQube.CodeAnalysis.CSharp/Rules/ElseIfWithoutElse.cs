﻿using System.Collections.Immutable;
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
    [SqaleSubCharacteristic(SqaleSubCharacteristic.LogicReliability)]
    [SqaleConstantRemediation("5min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    [Tags("cert", "misra")]
    public class ElseIfWithoutElse : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S126";
        internal const string Description = "\"if ... else if\" constructs shall be terminated with an \"else\" clause";
        internal const string MessageFormat = "Add the missing \"else\" clause.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Major; 
        internal const bool IsActivatedByDefault = false;

        internal static DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: "http://nemo.sonarqube.org/coding_rules#rule_key=csharpsquid%3AS126");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var ifNode = (IfStatementSyntax)c.Node;
                    if (IsElseIfWithoutElse(ifNode))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, ifNode.GetLocation()));
                    }
                },
                SyntaxKind.IfStatement);
        }

        private static bool IsElseIfWithoutElse(IfStatementSyntax node)
        {
            return node.Parent.IsKind(SyntaxKind.ElseClause) &&
                node.Else == null;
        }
    }
}
