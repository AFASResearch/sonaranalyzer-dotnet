﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarQube.CodeAnalysis.CSharp.Rules;

namespace SonarQube.CodeAnalysis.Test.Rules
{
    [TestClass]
    public class MethodNameTest
    {
        [TestMethod]
        public void MethodName()
        {
            var diagnostic = new MethodName {Convention = "^[A-Z][a-zA-Z0-9]+$"};
            Verifier.Verify(@"TestCases\MethodName.cs", diagnostic);
        }
    }
}
