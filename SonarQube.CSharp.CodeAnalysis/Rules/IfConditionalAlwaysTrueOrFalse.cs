﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarQube.CSharp.CodeAnalysis.Helpers;
using SonarQube.CSharp.CodeAnalysis.SonarQube.Settings;
using SonarQube.CSharp.CodeAnalysis.SonarQube.Settings.Sqale;

namespace SonarQube.CSharp.CodeAnalysis.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.Readability)]
    [SqaleConstantRemediation("2min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    [Tags("bug", "cwe", "misra", "security")]
    public class IfConditionalAlwaysTrueOrFalse : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1145";
        internal const string Description = "Useless \"if(true) {...}\" and \"if(false){...}\" blocks should be removed";
        internal const string MessageFormat = "Remove this \"if\" statement.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Major; 
        internal const bool IsActivatedByDefault = true;

        internal static DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: "http://nemo.sonarqube.org/coding_rules#rule_key=csharpsquid%3AS1145");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var ifNode = (IfStatementSyntax)c.Node;

                    if (HasBooleanLiteralExpressionAsCondition(ifNode))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, ifNode.GetLocation()));
                    }
                },
                SyntaxKind.IfStatement);
        }

        private static bool HasBooleanLiteralExpressionAsCondition(IfStatementSyntax node)
        {
            return node.Condition.IsKind(SyntaxKind.TrueLiteralExpression) ||
                node.Condition.IsKind(SyntaxKind.FalseLiteralExpression);
        }
    }
}
