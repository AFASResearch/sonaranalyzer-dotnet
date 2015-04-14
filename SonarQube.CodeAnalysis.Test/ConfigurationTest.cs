﻿using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarQube.CodeAnalysis.CSharp.Rules;
using SonarQube.CodeAnalysis.Runner;

namespace SonarQube.CodeAnalysis.Test
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void Configuration()
        {
            Configuration conf = new Configuration(XDocument.Load("ConfigurationTest.xml"));
            conf.IgnoreHeaderComments.Should().BeTrue();
            conf.Files.Should().BeEquivalentTo(@"C:\MyClass1.cs", @"C:\MyClass2.cs");

            conf.AnalyzerIds.Should().BeEquivalentTo(
                "S1121",
                "S2306",
                "S1227",

                "S104",
                "S1541",
                "S103",
                "S1479",
                "S1067",
                "S107",
                "S109",
                "S101",
                "S100",
                "S124");

            var analyzers = conf.Analyzers(null);
            analyzers.OfType<FileLines>().Single().Maximum.ShouldBeEquivalentTo(1000);
            analyzers.OfType<LineLength>().Single().Maximum.ShouldBeEquivalentTo(200);
            analyzers.OfType<TooManyLabelsInSwitch>().Single().Maximum.ShouldBeEquivalentTo(30);
            analyzers.OfType<TooManyParameters>().Single().Maximum.ShouldBeEquivalentTo(7);
            analyzers.OfType<ExpressionComplexity>().Single().Maximum.ShouldBeEquivalentTo(3);
            analyzers.OfType<FunctionComplexity>().Single().Maximum.ShouldBeEquivalentTo(10);
            analyzers.OfType<ClassName>().Single().Convention.ShouldBeEquivalentTo("^(?:[A-HJ-Z][a-zA-Z0-9]+|I[a-z0-9][a-zA-Z0-9]*)$");
            analyzers.OfType<MethodName>().Single().Convention.ShouldBeEquivalentTo("^[A-Z][a-zA-Z0-9]+$");
            analyzers.OfType<MagicNumber>().Single().Exceptions.ShouldBeEquivalentTo(ImmutableHashSet.Create("0", "1", "0x0", "0x00", ".0", ".1", "0.0", "1.0"));

            var commentAnalyzer = analyzers.OfType<CommentRegularExpression>().Single();
            commentAnalyzer.Rules.Should().HaveCount(2);
            commentAnalyzer.Rules[0].Descriptor.Id.ShouldBeEquivalentTo("TODO");
            commentAnalyzer.Rules[0].Descriptor.MessageFormat.ToString().ShouldBeEquivalentTo("Fix this TODO");
            commentAnalyzer.Rules[0].RegularExpression.ShouldBeEquivalentTo(".*TODO.*");
            commentAnalyzer.Rules[1].Descriptor.Id.ShouldBeEquivalentTo("FIXME");
            commentAnalyzer.Rules[1].Descriptor.MessageFormat.ToString().ShouldBeEquivalentTo("Fix this FIXME");
            commentAnalyzer.Rules[1].RegularExpression.ShouldBeEquivalentTo(".*FIXME.*");
        }
    }
}
