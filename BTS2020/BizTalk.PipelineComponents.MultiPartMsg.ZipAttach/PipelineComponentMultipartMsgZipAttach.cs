namespace BiztalkPipelineComponentMultipartMsgZipAttach
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.ComponentModel;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Component.Interop;
    using System.IO.Compression;


    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("78832cda-6f50-405b-b442-2b3da058ac5a")]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    public class PipelineComponentMultipartMsgZipAttach : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {

        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BiztalkPipelineComponentMultipartMsgZipAttach.PipelineComponentMultipartMsgZipAttach", Assembly.GetExecutingAssembly());

        private string _FileExtension;

        public string FileExtension
        {
            get
            {
                return _FileExtension;
            }
            set
            {
                _FileExtension = value;
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

        #region IPersistPropertyBag members
        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classid">
        /// Class ID of the component
        /// </param>
        public void GetClassID(out Guid classid)
        {
            classid = new Guid("78832cda-6f50-405b-b442-2b3da058ac5a");
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
        public virtual void Load(IPropertyBag pb, int errlog)
        {
            string val = null;
            val = (string)ReadPropertyBag(pb, "FileExtension");
            if ((val != null))
            {
                _FileExtension = val;
            }
        }

        /// <summary>
        /// Saves the current component configuration into the property bag
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="fClearDirty">not used</param>
        /// <param name="fSaveAllProperties">not used</param>
        public virtual void Save(IPropertyBag pb, bool fClearDirty, bool fSaveAllProperties)
        {
            WritePropertyBag(pb, "FileExtension", FileExtension);
        }

        #region utility functionality
        /// <summary>
        /// Reads property value from property bag
        /// </summary>
        /// <param name="pb">Property bag</param>
        /// <param name="propName">Name of property</param>
        /// <returns>Value of the property</returns>
        private object ReadPropertyBag(IPropertyBag pb, string propName)
        {
            object val = null;
            try
            {
                pb.Read(propName, out val, 0);
            }
            catch (ArgumentException)
            {
                return val;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return val;
        }

        /// <summary>
        /// Writes property values into a property bag.
        /// </summary>
        /// <param name="pb">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <param name="val">Value of property.</param>
        private void WritePropertyBag(IPropertyBag pb, string propName, object val)
        {
            try
            {
                pb.Write(propName, ref val);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }
        #endregion
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
        public IBaseMessage Execute(IPipelineContext pc, IBaseMessage inmsg)
        {
            int PartCount = inmsg.PartCount;
            string BodyPartName = inmsg.BodyPartName;

            string systemPropertiesNamespace = @"http://schemas.microsoft.com/BizTalk/2003/system-properties";
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
                        SourcePartName += this.FileExtension;

                        inmsg.GetPartByIndex(i, out PartName).PartProperties.Write("FileName", PropertySchema, SourcePartName);
                    }
                }
            }
            catch
            {
                throw;
            }
            return inmsg;
        }
        #endregion
    }
}
