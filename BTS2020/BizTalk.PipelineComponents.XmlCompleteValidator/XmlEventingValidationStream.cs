using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.BizTalk.Streaming;

namespace BizTalk.PipelineComponents.XmlCompleteValidator
{
	/// <summary>
	/// subclass of <see cref="XmlTranslatorStream"/>. essentially a
	/// copy of <see cref="XmlValidatingStream" />, but with access to it's reader
	/// to allow attachment of the <see cref="ValidationEventHandler" />
	/// </summary>
	public class XmlEventingValidationStream : XmlTranslatorStream
	{
		/// <summary>
		/// initializes the class, setting the private XmlValidatingReader
		/// to use the given <see cref="Stream"/>
		/// </summary>
		/// <param name="data">the <see cref="Stream"/> to use for this instance</param>
		public XmlEventingValidationStream(Stream data) : base(null)
		{
			XmlTextReader reader = new XmlTextReader(data);
			this.m_reader = new XmlValidatingReader(reader);
		}

		/// <summary>
		/// allows access to the <see cref="Schemas"/> property of the private
		/// <see cref="XmlValidatingReader"/>
		/// </summary>
		public XmlSchemaCollection Schemas
		{
			get
			{
				return (this.m_reader as XmlValidatingReader).Schemas;
			}
		}
 
		/// <summary>
		/// allows access to the private <see cref="XmlValidatingReader"/> in order
		/// to bind to it's event
		/// </summary>
		public XmlValidatingReader ValidatingReader
		{
			get
			{
				return this.m_reader as XmlValidatingReader;
			}
		}
	}
}
