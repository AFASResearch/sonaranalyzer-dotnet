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
    [SqaleSubCharacteristic(SqaleSubCharacteristic.Understandability)]
    [SqaleConstantRemediation("5min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    [Tags("clumsy", "unused")]
    public class CatchRethrow : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S2737";
        internal const string Description = "\"catch\" clauses should do more than rethrow";
        internal const string MessageFormat = @"Add logic to this catch clause or eliminate it and rethrow the exception automatically.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Major;
        internal const bool IsActivatedByDefault = true;

        private readonly BlockSyntax throwBlock = SyntaxFactory.Block(SyntaxFactory.ThrowStatement());

        internal static DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: "http://nemo.sonarqube.org/coding_rules#rule_key=csharpsquid%3AS2737");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var tryStatement = (TryStatementSyntax)c.Node;
                    
                    var lastCatchClause = tryStatement.Catches.LastOrDefault();
                    
                    if (lastCatchClause!=null && 
                        SyntaxFactory.AreEquivalent(lastCatchClause.Block, throwBlock))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(
                                Rule,
                                lastCatchClause.GetLocation()));
                    }
                },
                SyntaxKind.TryStatement);
        }
    }
}
