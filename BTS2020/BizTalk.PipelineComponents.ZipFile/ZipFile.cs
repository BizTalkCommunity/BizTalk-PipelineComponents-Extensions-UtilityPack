using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Reflection;
using System.IO.Compression;

namespace BizTalk.PipelineComponents.ZipFile
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    [System.Runtime.InteropServices.Guid("e449a463-44ed-44c3-b85d-0e5382d36ddf")]
    public class ZipFile : IBaseComponent, IComponentUI, Microsoft.BizTalk.Component.Interop.IComponent, IPersistPropertyBag
    {
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.ZipFile.ZipFile", Assembly.GetExecutingAssembly());


        #region IBaseComponent
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

        #region IComponentUI

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
            return null;
        }

        #endregion

        #region IPersistPropertyBag

        public string FileExtension { get; set; }
        public bool Enabled { get; set; }

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("e449a463-44ed-44c3-b85d-0e5382d36ddf");
        }

        public void InitNew()
        {
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            object val;
            val = ReadPropertyBag(propertyBag, "FileExtension");
            if (val != null) this.FileExtension = (string)val;

            val = ReadPropertyBag(propertyBag, "Enabled");
            if (val != null) this.Enabled = (bool)val;


        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object val;
            val = (object)FileExtension;
            propertyBag.Write("FileExtension", ref val);

            val = (object)Enabled;
            propertyBag.Write("Enabled", ref val);
        }

        private static object ReadPropertyBag(IPropertyBag pb, string propName)
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
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format(
                    "Error while reading property '{0}' from PropertyBag",
                    propName), ex);
            }
            return val;
        }

        #endregion


        #region IComponent

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            IBaseMessagePart bodyPart = pInMsg.BodyPart;
            IBaseMessageContext context = pInMsg.Context;

            // Check if the zip Component is activated
            if (!this.Enabled)
                return pInMsg;

            string fileName = context.Read("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties").ToString();

            if (bodyPart != null)
            {
                string bodyPartName = pInMsg.BodyPartName;
                try
                {
                    Stream currentPartSource = pInMsg.BodyPart.GetOriginalDataStream();
                    byte[] currentPartBuffer = new byte[currentPartSource.Length];
                    currentPartSource.Read(currentPartBuffer, 0, currentPartBuffer.Length);
                    byte[] compressedPartBuffer;

                    using (var outStream = new MemoryStream())
                    {
                        outStream.Write(currentPartBuffer, 0, currentPartBuffer.Length);
                        using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                        {
                            var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);

                            using (Stream entryStream = fileInArchive.Open())
                            using (var fileToCompression = new MemoryStream(currentPartBuffer))
                            {
                                fileToCompression.CopyTo(entryStream);
                            }
                        }
                        compressedPartBuffer = outStream.ToArray();
                    }

                    MemoryStream tempMemStream = new MemoryStream(compressedPartBuffer); //currentPartBuffer)**;
                    pInMsg.BodyPart.Data = tempMemStream;

                    //string sourcePartName = fileName + this.FileExtension;
                    string sourcePartName = fileName.Substring(0, fileName.LastIndexOf('.')) + this.FileExtension; // fil.xml -> fil.zip (including '.')


                    pInMsg.Context.Promote("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", sourcePartName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return pInMsg;

            #endregion
        }
    }
}

