using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BizTalk.PipelineComponents.NamespaceMgmt
{
    public class XMLNamespaceMgnt : XmlTranslatorStream
    {
        public string NamespaceURI { get; set; }

        public string NamespacePrefix { get; set; }

        protected override void TranslateStartElement(string prefix, string localName, string nsURI)
        {
            base.TranslateStartElement(NamespacePrefix, localName, NamespaceURI);
        }

        protected override void TranslateAttribute()
        {
            if (this.m_reader.Prefix != "xmlns")
                base.TranslateAttribute();
        }

        public XMLNamespaceMgnt(Stream input)
                : base(new XmlTextReader(input), Encoding.Default)
            {
        }
        protected override void TranslateXmlDeclaration(string target, string value)
        {
            this.m_writer.WriteProcessingInstruction(this.m_reader.Name, this.m_reader.Value);
        }
    }
}