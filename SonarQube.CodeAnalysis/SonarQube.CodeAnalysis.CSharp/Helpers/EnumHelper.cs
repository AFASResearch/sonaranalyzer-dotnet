﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using SonarQube.CodeAnalysis.CSharp.SonarQube.Settings;
using SonarQube.CodeAnalysis.CSharp.SonarQube.Settings.Sqale;

namespace SonarQube.CodeAnalysis.CSharp.Helpers
{
    public static class EnumHelper
    {
        private static string[] SplitCamelCase(this string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }
        public static string ToSonarQubeString(this SqaleSubCharacteristic subCharacteristic)
        {
            var parts = subCharacteristic.ToString().SplitCamelCase();
            return string.Join("_", parts).ToUpper(CultureInfo.InvariantCulture);
        }
        public static string ToSonarQubeString(this PropertyType propertyType)
        {
            var parts = propertyType.ToString().SplitCamelCase();
            return string.Join("_", parts).ToUpper(CultureInfo.InvariantCulture);
        }

        public static DiagnosticSeverity ToDiagnosticSeverity(this Severity severity)
        {
            switch (severity)
            {
                case Severity.Info:
                    return DiagnosticSeverity.Info;
                case Severity.Minor:
                case Severity.Major:
                case Severity.Critical:
                case Severity.Blocker:
                    return DiagnosticSeverity.Warning;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
