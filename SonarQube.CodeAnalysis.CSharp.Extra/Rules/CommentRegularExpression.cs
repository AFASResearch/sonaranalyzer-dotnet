﻿using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarQube.CodeAnalysis.CSharp.Helpers;
using SonarQube.CodeAnalysis.CSharp.SonarQube.Settings;
using SonarQube.CodeAnalysis.CSharp.SonarQube.Settings.Sqale;

namespace SonarQube.CodeAnalysis.CSharp.Rules
{
    public class CommentRegularExpressionRule
    {
        public DiagnosticDescriptor Descriptor;
        public string RegularExpression;
    }

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoSqaleRemediation]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault, true)]
    [LegacyKey("CommentRegularExpression")]
    public class CommentRegularExpression : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S124";
        internal const string Description = "Regular expression on comment";
        internal const string MessageFormat = "The regular expression matches this comment";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Major;
        internal const bool IsActivatedByDefault = false;

        public static DiagnosticDescriptor CreateDiagnosticDescriptor(string diagnosticId, string messageFormat)
        {
            return new DiagnosticDescriptor(diagnosticId, Description, messageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: "http://nemo.sonarqube.org/coding_rules#rule_key=csharpsquid%3ACommentRegularExpression");
        }

        public ImmutableArray<CommentRegularExpressionRule> Rules;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return Rules.Select(r => r.Descriptor).ToImmutableArray(); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(
                c =>
                {
                    var comments = from t in c.Tree.GetCompilationUnitRoot().DescendantTrivia()
                                   where IsComment(t)
                                   select t;

                    foreach (var comment in comments)
                    {
                        var text = comment.ToString();
                        foreach (var rule in Rules.Where(rule => Regex.IsMatch(text, rule.RegularExpression)))
                        {
                            c.ReportDiagnostic(Diagnostic.Create(rule.Descriptor, comment.GetLocation()));
                        }
                    }
                });
        }

        private static bool IsComment(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia);
        }
    }
}
