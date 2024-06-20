namespace BizTalk.PipelineComponents.LocalFolderArchiveDateStructure
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
    using System.Globalization;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Messaging;
    using Microsoft.BizTalk.Streaming;
    
    
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("741b04fc-309d-4cb8-a2f0-306135c8f720")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public class LocalFolderArchiveDateStructure : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        
        private System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.LocalFolderArchiveDateStructure.LocalFolderArchiveDateStructure", Assembly.GetExecutingAssembly());
        
        private bool _enabled;
        
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }
        
        private string _baseArchivePath;
        
        public string BaseArchivePath
        {
            get
            {
                return _baseArchivePath;
            }
            set
            {
                _baseArchivePath = value;
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
                return _resourceManager.GetString("COMPONENTNAME", CultureInfo.InvariantCulture);
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
                return _resourceManager.GetString("COMPONENTVERSION", CultureInfo.InvariantCulture);
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
                return _resourceManager.GetString("COMPONENTDESCRIPTION", CultureInfo.InvariantCulture);
            }
        }
        #endregion
        
        #region IPersistPropertyBag members
        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classID">
        /// Class ID of the component
        /// </param>
        public void GetClassID(out System.Guid classID)
        {
            classID = new System.Guid("741b04fc-309d-4cb8-a2f0-306135c8f720");
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
        /// <param name="propertyBag">Configuration property bag</param>
        /// <param name="errorLog">Error status</param>
        public virtual void Load(Microsoft.BizTalk.Component.Interop.IPropertyBag propertyBag, int errorLog)
        {
            object val = null;
            val = this.ReadPropertyBag(propertyBag, "Enabled");
            if ((val != null))
            {
                this._enabled = ((bool)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "BaseArchivePath");
            if ((val != null))
            {
                this._baseArchivePath = ((string)(val));
            }
        }
        
        /// <summary>
        /// Saves the current component configuration into the property bag
        /// </summary>
        /// <param name="propertyBag">Configuration property bag</param>
        /// <param name="clearDirty">not used</param>
        /// <param name="saveAllProperties">not used</param>
        public virtual void Save(Microsoft.BizTalk.Component.Interop.IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyBag(propertyBag, "Enabled", this.Enabled);
            this.WritePropertyBag(propertyBag, "BaseArchivePath", this.BaseArchivePath);
        }
        
        #region utility functionality
        /// <summary>
        /// Reads property value from property bag
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="propertyName">Name of property</param>
        /// <returns>Value of the property</returns>
        private object ReadPropertyBag(Microsoft.BizTalk.Component.Interop.IPropertyBag propertyBag, string propertyName)
        {
            object val = null;
            try
            {
                propertyBag.Read(propertyName, out val, 0);
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
        /// <param name="propertyBag">Property bag.</param>
        /// <param name="propertyName">Name of property.</param>
        /// <param name="val">Value of property.</param>
        private void WritePropertyBag(Microsoft.BizTalk.Component.Interop.IPropertyBag propertyBag, string propertyName, object val)
        {
            try
            {
                propertyBag.Write(propertyName, ref val);
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
                return ((System.Drawing.Bitmap)(this._resourceManager.GetObject("COMPONENTICON", CultureInfo.InvariantCulture))).GetHicon();
            }
        }
        
        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build 
        /// of a BizTalk project.
        /// </summary>
        /// <param name="projectSystem">An Object containing the configuration properties.</param>
        /// <returns>The IEnumerator enables the caller to enumerate through a collection of strings containing error messages. These error messages appear as compiler error messages. To report successful property validation, the method should return an empty enumerator.</returns>
        public System.Collections.IEnumerator Validate(object projectSystem)
        {
            // example implementation:
            // yield return "This is a compiler error";
            // yield break;
            return null;
        }
        #endregion
        
        #region IComponent members
        /// <summary>
        /// Implements IComponent.Execute method.
        /// </summary>
        /// <param name="pContext">Pipeline context</param>
        /// <param name="pInMsg">Input message</param>
        /// <returns>Original input message</returns>
        /// <remarks>
        /// IComponent.Execute method is used to initiate
        /// the processing of the message in this pipeline component.
        /// </remarks>
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pContext, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            if (this.Enabled)
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

                    string fullPath = Path.Combine(Path.GetDirectoryName(this.BaseArchivePath), DateTime.Now.ToString(@"yyyy\\MM\\dd"));

                    if (!new DirectoryInfo(fullPath).Exists)
                    {
                        try
                        {
                            Directory.CreateDirectory(fullPath);
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    Stream msgStream = inmsg.BodyPart.Data;

                    MemoryStream outStream = new MemoryStream();
                    msgStream.CopyTo(outStream);

                    SaveStreamToFile(outStream, Path.Combine(fullPath, Path.GetFileName(fileName)), true);

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
