using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.Component.Interop;
using System.Collections;
using Microsoft.BizTalk.Streaming;
using System.Xml;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace RemoveXmlNamespace.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("88A3F169-A718-419e-B1DE-7522372E3D03")]
    public class XmlNamespaceRemover : IBaseComponent, IPersistPropertyBag, 
        IComponentUI, Microsoft.BizTalk.Component.Interop.IComponent
    {
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("RemoveXmlNamespace.PipelineComponents.RemoveResources", Assembly.GetExecutingAssembly());

        #region IBaseComponent Members

        public string Description
        {
            get
            {
                return resourceManager.GetString("COMPONENTDESCRIPTION", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string Name
        {
            get
            {
                return resourceManager.GetString("COMPONENTNAME", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string Version
        {
            get
            {
                return resourceManager.GetString("COMPONENTVERSION", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region IPersistPropertyBag Members

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("88A3F169-A718-419e-B1DE-7522372E3D03");
        }

        public void InitNew()
        {
            
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
           
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            
        }

        #endregion

        #region IComponentUI Members

        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return ((System.Drawing.Bitmap)(this.resourceManager.GetObject("COMPONENTICON", System.Globalization.CultureInfo.InvariantCulture))).GetHicon();
            }
        }

        public System.Collections.IEnumerator Validate(object projectSystem)
        {
            return new ArrayList().GetEnumerator();
        }

        #endregion

        #region IComponent Members

        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(
            IPipelineContext pContext, 
            Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            pInMsg.BodyPart.Data = new XmlNamespaceRemoverStream(
                pInMsg.BodyPart.GetOriginalDataStream());
            return pInMsg;
        }

        #endregion
    }

    public class XmlNamespaceRemoverStream : XmlTranslatorStream
    {
        protected override void TranslateStartElement(
            string prefix, string localName, string nsURI)
        {
            base.TranslateStartElement(null, localName, null);
        }

        protected override void TranslateAttribute()
        {
            if (this.m_reader.Prefix != "xmlns")
                base.TranslateAttribute();
        }

        public XmlNamespaceRemoverStream(Stream input)
            : base(new XmlTextReader(input), Encoding.Default)
        { }
    }
}
