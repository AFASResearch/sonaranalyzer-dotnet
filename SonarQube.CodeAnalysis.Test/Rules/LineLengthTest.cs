﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarQube.CodeAnalysis.CSharp.Rules;

namespace SonarQube.CodeAnalysis.Test.Rules
{
    [TestClass]
    public class LineLengthTest
    {
        [TestMethod]
        public void LineLength()
        {
            var diagnostic = new LineLength {Maximum = 47};
            Verifier.Verify(@"TestCases\LineLength.cs", diagnostic);
        }
    }
}
