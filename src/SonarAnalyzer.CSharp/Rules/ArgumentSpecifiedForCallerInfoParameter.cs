﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2016 SonarSource SA
 * mailto:contact@sonarsource.com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02
 */

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Common;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public class ArgumentSpecifiedForCallerInfoParameter : SonarDiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S3236";
        internal const string MessageFormat = "Remove this argument from the method call; it hides the caller information.";

        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);

        protected sealed override DiagnosticDescriptor Rule => rule;

        protected override void Initialize(SonarAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var methodCall = (InvocationExpressionSyntax)c.Node;
                    var methodParameterLookup = new MethodParameterLookup(methodCall, c.SemanticModel);
                    var argumentMappings = methodParameterLookup.GetAllArgumentParameterMappings()
                        .ToList();

                    var methodSymbol = methodParameterLookup.MethodSymbol;
                    if (methodSymbol == null)
                    {
                        return;
                    }

                    foreach (var argumentMapping in argumentMappings)
                    {
                        var parameter = argumentMapping.Parameter;
                        var argument = argumentMapping.Argument;

                        var callerInfoAttributeDataOnCall = GetCallerInfoAttribute(parameter);
                        if (callerInfoAttributeDataOnCall == null)
                        {
                            continue;
                        }

                        var symbolForArgument = c.SemanticModel.GetSymbolInfo(argument.Expression).Symbol as IParameterSymbol;
                        if (symbolForArgument != null &&
                            object.Equals(callerInfoAttributeDataOnCall.AttributeClass, GetCallerInfoAttribute(symbolForArgument)?.AttributeClass))
                        {
                            continue;
                        }

                        c.ReportDiagnostic(Diagnostic.Create(Rule, argument.GetLocation()));
                    }
                },
                SyntaxKind.InvocationExpression);
        }

        private static AttributeData GetCallerInfoAttribute(IParameterSymbol parameter)
        {
            return parameter.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass.IsAny(KnownType.CallerInfoAttributes));
        }
    }
}
