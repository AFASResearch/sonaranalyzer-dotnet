﻿using System.Collections.Generic;
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
    [SqaleSubCharacteristic(SqaleSubCharacteristic.Understandability)]
    [SqaleConstantRemediation("5min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    [Tags("unused")]
    [LegacyKey("UnusedVariable")]
    public class UnusedLocalVariable : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1481";
        internal const string Description = "Unused local variables should be removed";
        internal const string MessageFormat = "Remove this unused \"{0}\" local variable.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Major; 
        internal const bool IsActivatedByDefault = true;
        
        internal static DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: "http://nemo.sonarqube.org/coding_rules#rule_key=csharpsquid%3AS1481");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }
        
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCodeBlockStartAction<SyntaxKind>(cbc =>
            {
                var unusedLocals = new List<ISymbol>();
 
                cbc.RegisterSyntaxNodeAction(c =>
                {
                    unusedLocals.AddRange(
                        ((LocalDeclarationStatementSyntax) c.Node).Declaration
                            .Variables.Select(variable => c.SemanticModel.GetDeclaredSymbol(variable)));
                },
                SyntaxKind.LocalDeclarationStatement);

                cbc.RegisterSyntaxNodeAction(c =>
                {
                    var variableDeclarationSyntax = ((UsingStatementSyntax)c.Node).Declaration;
                    if (variableDeclarationSyntax != null)
                    {
                        unusedLocals.AddRange(
                            variableDeclarationSyntax
                                .Variables.Select(variable => c.SemanticModel.GetDeclaredSymbol(variable)));
                    }
                },
                SyntaxKind.UsingStatement);
 
                cbc.RegisterSyntaxNodeAction(c =>
                {
                    var symbolInfo = c.SemanticModel.GetSymbolInfo(c.Node);
                    unusedLocals.Remove(symbolInfo.Symbol);

                    foreach (var candidateSymbol in symbolInfo.CandidateSymbols)
                    {
                        unusedLocals.Remove(candidateSymbol);
                    }
                },
                SyntaxKind.IdentifierName);
 
                cbc.RegisterCodeBlockEndAction(c =>
                {
                    foreach (var unused in unusedLocals)
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, unused.Locations.First(), unused.Name));
                    }
                });
            });
        }
    }
}
