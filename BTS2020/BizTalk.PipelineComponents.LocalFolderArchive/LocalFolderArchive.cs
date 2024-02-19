namespace BizTalk.PipelineComponents.LocalFolderArchive
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
    using Microsoft.BizTalk.Streaming;
    
    
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("e28f297c-39ac-4e88-ae9f-011e9a3aa27a")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public class LocalFolderArchive : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.LocalFolderArchive.LocalFolderArchive", Assembly.GetExecutingAssembly());
        
        private bool _PerformBackup;
        
        public bool PerformBackup
        {
            get
            {
                return _PerformBackup;
            }
            set
            {
                _PerformBackup = value;
            }
        }
        
        private string _Folder;
        
        public string Folder
        {
            get
            {
                return _Folder;
            }
            set
            {
                _Folder = value;
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
            classid = new System.Guid("e28f297c-39ac-4e88-ae9f-011e9a3aa27a");
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
            val = this.ReadPropertyBag(pb, "PerformBackup");
            if ((val != null))
            {
                this._PerformBackup = ((bool)(val));
            }
            val = this.ReadPropertyBag(pb, "Folder");
            if ((val != null))
            {
                this._Folder = ((string)(val));
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
            this.WritePropertyBag(pb, "PerformBackup", this.PerformBackup);
            this.WritePropertyBag(pb, "Folder", this.Folder);
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
            if (this.PerformBackup)
            {
                try
                {
                    //Get Message Id
                    Guid msgId = inmsg.MessageID;

                    //Get Filename by FileAdapter NS
                    string fileName = inmsg.Context.Read("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties").ToString();
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = inmsg.Context.Read("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/ftp-properties").ToString();
                        if (string.IsNullOrEmpty(fileName))
                        {
                            fileName = msgId.ToString();
                        }
                    }

                    if (!new DirectoryInfo(this.Folder).Exists)
                    {
                        try
                        {
                            Directory.CreateDirectory(this.Folder);
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    Stream msgStream = inmsg.BodyPart.Data;

                    MemoryStream outStream = new MemoryStream();
                    msgStream.CopyTo(outStream);

                    SaveStreamToFile(outStream, Path.Combine(Folder, Path.GetFileName(fileName)), true);

                    inmsg.BodyPart.Data.Position = 0;
                }
                catch
                {
                    throw;
                }
            }

            return inmsg;
        }
        #endregion


        #region Extended methods
        protected virtual void SaveStreamToFile(Stream msgStream, string fileName, bool overWrite)
        {
            int bufferSize = 4096;
            byte[] buffer = new byte[4096];
            int numBytesRead = 0;

            using (FileStream fs = new FileStream(fileName, overWrite ? FileMode.Create : FileMode.CreateNew))
            {
                // Setup the stream writter and reader 
                BinaryWriter w = new BinaryWriter(fs);
                w.BaseStream.Seek(0,
                SeekOrigin.End);
                if (msgStream != null)
                {
                    msgStream.Seek(0,
                    SeekOrigin.Begin);
                    // Copy the data from the msg to the file 
                    int n = 0;
                    do
                    {
                        n = msgStream.Read(buffer, 0, bufferSize);
                        if (n == 0) // We're at EOF 
                            break;
                        w.Write(buffer, 0, n);
                        numBytesRead += n;
                    }
                    while (n > 0);
                }
                w.Flush();
            }
        }
        #endregion
    }
}