﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarQube.CSharp.CodeAnalysis.Rules;

namespace SonarQube.CSharp.Test.Rules
{
    [TestClass]
    public class UnusedLocalVariableTest
    {
        [TestMethod]
        public void UnusedLocalVariable()
        {
            Verifier.Verify(@"TestCases\UnusedLocalVariable.cs", new UnusedLocalVariable());
        }

        
    }
}
