﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace SonarQube.CSharp.CodeAnalysis.Descriptor.RuleDescriptors
{
    [XmlRoot("rules", Namespace = "")]
    public class RuleDescriptorRoot
    {
        public RuleDescriptorRoot()
        {
            Rules= new List<RuleDescriptor>();
        }
        [XmlElement("rule")]
        public List<RuleDescriptor> Rules { get; set; }
    }
}