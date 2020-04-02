namespace PipelineComponents.JSONEncoder
{
    using Microsoft.BizTalk.Common.JsonExtension;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Streaming;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Linq;


    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("11548257-0956-4de0-80ac-2cdc9c937311")]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    public class JSONEncoder : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("PipelineComponents.JSONEncoder.JSONEncoder", Assembly.GetExecutingAssembly());
        
        private bool _RemoveOuterEnvelope;
        
        public bool RemoveOuterEnvelope
        {
            get
            {
                return _RemoveOuterEnvelope;
            }
            set
            {
                _RemoveOuterEnvelope = value;
            }
        }

        private bool _UseCustomEncoder;

        public bool UseCustomEncoder
        {
            get
            {
                return _UseCustomEncoder;
            }
            set
            {
                _UseCustomEncoder = value;
            }
        }

        #region IBaseComponent members
        /// <summary>
        /// Name of the component
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get
            {
                return resourceManager.GetString("COMPONENTNAME", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        
        /// <summary>
        /// Version of the component
        /// </summary>
        [Browsable(false)]
        public string Version
        {
            get
            {
                return resourceManager.GetString("COMPONENTVERSION", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        
        /// <summary>
        /// Description of the component
        /// </summary>
        [Browsable(false)]
        public string Description
        {
            get
            {
                return resourceManager.GetString("COMPONENTDESCRIPTION", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        #endregion
        
        #region IPersistPropertyBag members
        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classid">
        /// Class ID of the component
        /// </param>
        public void GetClassID(out System.Guid classid)
        {
            classid = new System.Guid("b8eef8b9-e9cf-4d07-a64d-7f190b124c40");
        }
        
        /// <summary>
        /// not implemented
        /// </summary>
        public void InitNew()
        {
        }
        
        /// <summary>
        /// Loads configuration properties for the component
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="errlog">Error status</param>
        public virtual void Load(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, int errlog)
        {
            object val = null;
            val = this.ReadPropertyBag(pb, "RemoveOuterEnvelope");
            if ((val != null))
            {
                this._RemoveOuterEnvelope = ((bool)(val));
            }
            val = this.ReadPropertyBag(pb, "UseCustomEncoder");
            if ((val != null))
            {
                this._UseCustomEncoder = ((bool)(val));
            }
        }
        
        /// <summary>
        /// Saves the current component configuration into the property bag
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="fClearDirty">not used</param>
        /// <param name="fSaveAllProperties">not used</param>
        public virtual void Save(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, bool fClearDirty, bool fSaveAllProperties)
        {
            this.WritePropertyBag(pb, "RemoveOuterEnvelope", this.RemoveOuterEnvelope);
            this.WritePropertyBag(pb, "UseCustomEncoder", this.UseCustomEncoder);
        }
        
        #region utility functionality
        /// <summary>
        /// Reads property value from property bag
        /// </summary>
        /// <param name="pb">Property bag</param>
        /// <param name="propName">Name of property</param>
        /// <returns>Value of the property</returns>
        private object ReadPropertyBag(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, string propName)
        {
            object val = null;
            try
            {
                pb.Read(propName, out val, 0);
            }
            catch (System.ArgumentException )
            {
                return val;
            }
            catch (System.Exception e)
            {
                throw new System.ApplicationException(e.Message);
            }
            return val;
        }
        
        /// <summary>
        /// Writes property values into a property bag.
        /// </summary>
        /// <param name="pb">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <param name="val">Value of property.</param>
        private void WritePropertyBag(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, string propName, object val)
        {
            try
            {
                pb.Write(propName, ref val);
            }
            catch (System.Exception e)
            {
                throw new System.ApplicationException(e.Message);
            }
        }
        #endregion
        #endregion
        
        #region IComponentUI members
        /// <summary>
        /// Component icon to use in BizTalk Editor
        /// </summary>
        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return ((System.Drawing.Bitmap)(this.resourceManager.GetObject("COMPONENTICON", System.Globalization.CultureInfo.InvariantCulture))).GetHicon();
            }
        }
        
        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build 
        /// of a BizTalk project.
        /// </summary>
        /// <param name="obj">An Object containing the configuration properties.</param>
        /// <returns>The IEnumerator enables the caller to enumerate through a collection of strings containing error messages. These error messages appear as compiler error messages. To report successful property validation, the method should return an empty enumerator.</returns>
        public System.Collections.IEnumerator Validate(object obj)
        {
            // example implementation:
            // ArrayList errorList = new ArrayList();
            // errorList.Add("This is a compiler error");
            // return errorList.GetEnumerator();
            return null;
        }
        #endregion
        
        #region IComponent members
        /// <summary>
        /// Implements IComponent.Execute method.
        /// </summary>
        /// <param name="pc">Pipeline context</param>
        /// <param name="inmsg">Input message</param>
        /// <returns>Original input message</returns>
        /// <remarks>
        /// IComponent.Execute method is used to initiate
        /// the processing of the message in this pipeline component.
        /// </remarks>
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pipelineContext, Microsoft.BizTalk.Message.Interop.IBaseMessage inputMsg)
        {
            // 
            // TODO: implement component logic
            // 
            // this way, it's a passthrough pipeline component
            IBaseMessagePart bodyPart = inputMsg.BodyPart;
            if (bodyPart != null)
            {
                using (Stream stream = bodyPart.GetOriginalDataStream())
                {
                    if (stream == null)
                    {
                        return inputMsg;
                    }
                    using (Stream stream2 = new ReadOnlySeekableStream(stream))
                    {
                        

                        if (this.UseCustomEncoder)
                        {
                            string jsonStg = "";

                            XDocument xdoc = new XDocument();
                            xdoc = XDocument.Load(stream2);
                            stream2.Position = 0L;

                            xdoc.Declaration = null;
                            xdoc.Descendants()
                               .Attributes()
                               .Where(x => x.IsNamespaceDeclaration)
                               .Remove();

                            XmlDocument node = new XmlDocument();
                            node = ToXmlDocument(xdoc);

                            if (this.RemoveOuterEnvelope)
                                jsonStg = JsonConvert.SerializeXmlNode(node, Newtonsoft.Json.Formatting.None, true);
                            else jsonStg = JsonConvert.SerializeXmlNode(node);

                            bodyPart.Data = GenerateStreamFromString(jsonStg);
                        }
                        else
                        {
                            XmlSchema matchingSchema = null;
                            XmlDocument node = new XmlDocument();
                            node.Load(stream2);
                            stream2.Position = 0L;

                            matchingSchema = this.GetMatchingSchema(pipelineContext, stream2);
                            bodyPart.Data = JsonConvertExtension.SerializeXmlNodeToStream(node, this.RemoveOuterEnvelope, true, true, matchingSchema);
                        }
                    }
                }
            }

            return inputMsg;
        }


        private static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }


        private XmlSchema GetMatchingSchema(IPipelineContext pipelineContext, Stream xmlStream)
        {
            XmlSchema schema = null;
            IDocumentSpec documentSpecByType = null;
            string docType = Utils.GetDocType(MarkableForwardOnlyEventingReadStream.EnsureMarkable(xmlStream));
            try
            {
                documentSpecByType = pipelineContext.GetDocumentSpecByType(docType);
            }
            catch (Exception)
            {
                return null;
            }
            XmlSchemaSet schemaSet = (documentSpecByType as IDocumentSpec2).GetSchemaSet();
            if (schemaSet.Count == 0)
            {
                return null;
            }
            foreach (XmlSchema schema2 in schemaSet.Schemas())
            {
                if (docType.Contains(schema2.TargetNamespace + "#"))
                {
                    schema = schema2;
                }
            }
            return schema;
        }

        private Stream GenerateStreamFromString(string msg)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(msg));
        }
        #endregion
    }
}
