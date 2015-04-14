﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace SonarQube.CodeAnalysis.Descriptor.RuleDescriptors
{
    public class SqaleRemediation
    {
        public SqaleRemediation()
        {
            Properties = new List<SqaleRemediationProperty>();
        }

        [XmlElement("rule-key")]
        public string RuleKey { get; set; }

        [XmlElement("prop")]
        public List<SqaleRemediationProperty> Properties { get; set; }
    }
}