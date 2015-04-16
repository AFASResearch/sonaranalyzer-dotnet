﻿using System.Xml.Serialization;

namespace SonarQube.CSharp.CodeAnalysis.Descriptor.RuleDescriptors
{
    public class SqaleRemediationProperty
    {
        [XmlElement("key")]
        public string Key { get; set; }
        [XmlElement("txt")]
        public string Text { get; set; }
        [XmlElement("val")]
        public string Value { get; set; }
    }
}