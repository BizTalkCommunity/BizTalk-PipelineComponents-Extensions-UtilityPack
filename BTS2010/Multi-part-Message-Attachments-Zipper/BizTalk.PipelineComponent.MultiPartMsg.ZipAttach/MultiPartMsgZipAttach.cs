namespace BizTalk.PipelineComponent.MultiPartMsg.ZipAttach
{
    using System;
    using System.IO;
    using System.Text;
    using System.Drawing;
    using System.Resources;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections;
    using System.ComponentModel;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Messaging;
    using System.IO.Compression;
    
    
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("81ea0c81-dc10-4f91-a0f1-4c9fdb7cd25c")]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    public class MultiPartMsgZipAttach : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponent.MultiPartMsg.ZipAttach.MultiPartMsgZipAttach", Assembly.GetExecutingAssembly());
        
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
            classid = new System.Guid("81ea0c81-dc10-4f91-a0f1-4c9fdb7cd25c");
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
        }
        
        /// <summary>
        /// Saves the current component configuration into the property bag
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="fClearDirty">not used</param>
        /// <param name="fSaveAllProperties">not used</param>
        public virtual void Save(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, bool fClearDirty, bool fSaveAllProperties)
        {
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
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {           
            int PartCount = inmsg.PartCount;
            string BodyPartName = inmsg.BodyPartName;

            try
            {
                for (int i = 0; i < inmsg.PartCount; i++)
                {
                    string PartName;
                    IBaseMessagePart CurrentPart = inmsg.GetPartByIndex(i, out PartName);

                    if (!PartName.Equals(BodyPartName))
                    {
                        Stream CurrentPartSource = CurrentPart.GetOriginalDataStream();
                        byte[] CurrentPartBuffer = new byte[CurrentPartSource.Length];
                        CurrentPartSource.Read(CurrentPartBuffer, 0, CurrentPartBuffer.Length);
                        
                        byte[] CompressedPartBuffer;
                        using (MemoryStream TempStream = new MemoryStream())
                        {
                            using (GZipStream CompressedStream = new GZipStream(TempStream, CompressionMode.Compress, true))
                            {
                                CompressedStream.Write(CurrentPartBuffer, 0, CurrentPartBuffer.Length);
                                CompressedStream.Close();
                            }
                            CompressedPartBuffer = TempStream.ToArray();
                        }

                        MemoryStream TempCompressedStream = new MemoryStream(CompressedPartBuffer);
                        inmsg.GetPartByIndex(i, out PartName).Data = TempCompressedStream;
                        string PropertyName = "FileName";
                        string PropertySchema = "http://schemas.microsoft.com/BizTalk/2003/mime-properties";
                        string SourcePartName = inmsg.GetPartByIndex(i, out PartName).PartProperties.Read(PropertyName,
                            PropertySchema).ToString();
                        SourcePartName += ".gz";

                        inmsg.GetPartByIndex(i, out PartName).PartProperties.Write("FileName", PropertySchema, SourcePartName);                        
                    }
                }
            }
            catch (Exception Exception)
            {
                //Exception.Message; 
            }

            return inmsg;
          
        }
        #endregion
    }
}
