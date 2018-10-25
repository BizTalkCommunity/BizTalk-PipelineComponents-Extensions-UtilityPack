using System;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.ComponentModel;

namespace BizTalk.PipelineComponents.UnZipBatch
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [System.Runtime.InteropServices.Guid("653439e8-e982-4876-bda0-823afcb8343c")]
    public class UnZipBatch : IBaseComponent, IComponentUI, IDisassemblerComponent, IPersistPropertyBag
    {

        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.UnZipBatch.UnZipBatch", Assembly.GetExecutingAssembly());

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

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("653439e8-e982-4876-bda0-823afcb8343c");
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
        private System.Collections.Queue inboundMsgQueue = new System.Collections.Queue();

        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            if (pInMsg.BodyPart != null)
            {
                Stream currentPartSource = pInMsg.BodyPart.GetOriginalDataStream();
                byte[] currentPartBuffer = new byte[currentPartSource.Length];
                currentPartSource.Read(currentPartBuffer, 0, currentPartBuffer.Length);
                byte[] decompressedPartBuffer;

                string filename = "";
                //Unzip arquive
                using (var unzipArchive = new ZipArchive(currentPartSource, ZipArchiveMode.Read, true))
                {
                    //Loop arquive file for message
                    foreach (var entryStream in unzipArchive.Entries)
                    {
                        IBaseMessage inboundInstanceMessage;
                        //Here we are gets the filename (with extension) of an entery present in the arquive file
                        filename = entryStream.FullName; 
                        using (var outStream = new MemoryStream())
                        {
                            entryStream.Open().CopyTo(outStream);
                            decompressedPartBuffer = outStream.ToArray();
                        }
                        
                        MemoryStream messageInstanceStream = new MemoryStream(decompressedPartBuffer);
                        messageInstanceStream.Position = 0;

                        #region Generating a new inbound message 

                        //Creating a new message from the pipeline context
                        inboundInstanceMessage = pContext.GetMessageFactory().CreateMessage();
                        //Cloning the context to the new message, instead of assigning it
                        inboundInstanceMessage.Context = PipelineUtil.CloneMessageContext(pInMsg.Context);

                        inboundInstanceMessage.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);
                        inboundInstanceMessage.BodyPart.Data = messageInstanceStream;

                        inboundInstanceMessage.BodyPart.Charset = "UTF-8";
                        inboundInstanceMessage.BodyPart.ContentType = "text/xml";
                        //we promote the filename so that we can access it in the Send Port.
                        inboundInstanceMessage.Context.Promote("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", filename);
                        
                        #endregion

                        inboundMsgQueue.Enqueue(inboundInstanceMessage);
                    }
                }
            }
        }

        public IBaseMessage GetNext(IPipelineContext pContext)
        {
            IBaseMessage iBaseMessage = null;
            if ((inboundMsgQueue.Count > 0))
            {
                iBaseMessage = ((IBaseMessage)(inboundMsgQueue.Dequeue()));
            }
            return iBaseMessage;
        }
        #endregion
    }
}
