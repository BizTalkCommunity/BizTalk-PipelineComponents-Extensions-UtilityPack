/*#############################################################################
  # Description  : ArchiveComponent                                           #
  # Author       : Sandro Pereira e Diogo Formosinho (based on Randy Paulo)   #
  #############################################################################*/

namespace BizTalk.Archiving.PipelineComponents
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
    using BizTalk.Archiving.Common.Configuration;
    using BizTalk.Archiving.Common;

    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("d0ecc216-f60c-4679-8eac-8a686c56bf44")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public class ArchiveComponent : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        
        private System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("BizTalk.Archiving.PipelineComponents.ArchiveComponent", Assembly.GetExecutingAssembly());
        
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

        #region Properties

        /// <summary>
        /// Enabled flag for Db Archive
        /// </summary>
        public bool IsArchiveToDb { get; set; }

        /// <summary>
        /// File Extension
        /// </summary>
        public string DbFileExtensions { get; set; }

        /// <summary>
        /// ConnectionString
        /// </summary>
        public string DbConnStr { get; set; }

        /// <summary>
        /// Provider Name: System.Data.SqlClient
        /// </summary>
        public string DbConnProvider { get; set; }

        /// <summary>
        /// List of message properties that will be included in the xml that will be password to stored procedure
        /// Format is property name;namespace
        /// Ex: FileCreationTime;http://schemas.microsoft.com/BizTalk/2003/file-properties
        /// For multiple properties delimit it by | pipe symbol
        /// </summary>
        public string DbPropList { get; set; }

        /// <summary>
        /// Name of stored procedure for archiving, it should accept Xml, and VarBinaryData
        /// </summary>
        public string DbSPName { get; set; }

        /// <summary>
        /// File Archive Enabled Flag
        /// </summary>
        public bool IsArchiveToFile { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        public string FileArchiveFileName { get; set; }

        /// <summary>
        /// Backup Folder
        /// </summary>
        public string FileArchiveBackupFolder { get; set; }

        /// <summary>
        /// Zip Password
        /// </summary>
        public string CompressionPassword { get; set; }

        /// <summary>
        /// Is Overwrite file
        /// </summary>
        public bool FileArchiveIsOverwriteFiles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCompressFile { get; set; }

        /// <summary>
        /// Connection User Name
        /// </summary>
        public string FileArchiveUserName { get; set; }

        /// <summary>
        /// Connection Domain
        /// </summary>
        public string FileArchiveUserDomain { get; set; }

        /// <summary>
        /// Connection Password
        /// </summary>
        public string FileArchiveUserPwd { get; set; }

        #endregion

        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classID">
        /// Class ID of the component
        /// </param>
        public void GetClassID(out System.Guid classID)
        {
            classID = new System.Guid("d0ecc216-f60c-4679-8eac-8a686c56bf44");
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
            object sval = null;

            val = this.ReadPropertyBag(propertyBag, "IsArchiveToDb");
            if ((val != null))
            {
                this.IsArchiveToDb = ((bool)(val));
            }

            sval = this.ReadPropertyBag(propertyBag, "DbConnStr");
            if ((sval != null))
            {
                this.DbConnStr = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "DbConnProvider");
            if ((sval != null))
            {
                this.DbConnProvider = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "DbPropList");
            if ((sval != null))
            {
                this.DbPropList = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "DbSPName");
            if ((sval != null))
            {
                this.DbSPName = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "CompressionPassword");
            if ((sval != null))
            {
                this.CompressionPassword = ((string)(sval));
            }

            val = this.ReadPropertyBag(propertyBag, "IsArchiveToFile");
            if ((val != null))
            {
                this.IsArchiveToFile = ((bool)(val));
            }

            val = this.ReadPropertyBag(propertyBag, "IsCompressFile");
            if ((val != null))
            {
                this.IsCompressFile = ((bool)(val));
            }

            sval = this.ReadPropertyBag(propertyBag, "FileArchiveFileName");
            if ((sval != null))
            {
                this.FileArchiveFileName = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "FileArchiveBackupFolder");
            if ((sval != null))
            {
                this.FileArchiveBackupFolder = ((string)(sval));
            }


            val = this.ReadPropertyBag(propertyBag, "FileArchiveIsOverwriteFiles");
            if ((val != null))
            {
                this.FileArchiveIsOverwriteFiles = ((bool)(val));
            }

            sval = this.ReadPropertyBag(propertyBag, "FileArchiveUserName");
            if ((sval != null))
            {
                this.FileArchiveUserName = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "FileArchiveUserDomain");
            if ((sval != null))
            {
                this.FileArchiveUserDomain = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "FileArchiveUserPwd");
            if ((sval != null))
            {
                this.FileArchiveUserPwd = ((string)(sval));
            }

            sval = this.ReadPropertyBag(propertyBag, "DbFileExtensions");
            if ((sval != null))
            {
                this.DbFileExtensions = ((string)(sval));
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
            WritePropertyBag(propertyBag, "IsArchiveToDb", (object)IsArchiveToDb);
            WritePropertyBag(propertyBag, "DbConnProvider", (object)DbConnProvider);
            WritePropertyBag(propertyBag, "DbConnStr", (object)DbConnStr);
            WritePropertyBag(propertyBag, "DbPropList", (object)DbPropList);
            WritePropertyBag(propertyBag, "DbSPName", (object)DbSPName);
            WritePropertyBag(propertyBag, "CompressionPassword", (object)CompressionPassword);
            WritePropertyBag(propertyBag, "FileArchiveBackupFolder", (object)FileArchiveBackupFolder);
            WritePropertyBag(propertyBag, "FileArchiveUserDomain", (object)FileArchiveUserDomain);
            WritePropertyBag(propertyBag, "FileArchiveUserName", (object)FileArchiveUserName);
            WritePropertyBag(propertyBag, "FileArchiveUserPwd", (object)FileArchiveUserPwd);
            WritePropertyBag(propertyBag, "FileArchiveFileName", (object)FileArchiveFileName);
            WritePropertyBag(propertyBag, "IsArchiveToFile", (object)IsArchiveToFile);
            WritePropertyBag(propertyBag, "FileArchiveIsOverwriteFiles", (object)FileArchiveIsOverwriteFiles);
            WritePropertyBag(propertyBag, "DbFileExtensions", (object)DbFileExtensions);
            WritePropertyBag(propertyBag, "IsCompressFile", (object)IsCompressFile);
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
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pContext, 
            Microsoft.BizTalk.Message.Interop.IBaseMessage inMsg)
        {
            //Check if Archiving is Enabled
            if (IsArchiveToDb || IsArchiveToFile)
            {
                try
                {
                    string fileName = string.Empty;
                    //Get Message Id
                    Guid msgId = inMsg.MessageID;

                    //Get Provider
                    //In the future custom Message Archiver will be supported.
                    var provider = GetProvider(true);

                    //Check if Db Archive is enabled
                    if (IsArchiveToDb)
                    {
                        string xmlStringProperties = GetMessageProperties(inMsg.Context);
                        ArchiveToDb(provider, inMsg.BodyPart.Data, xmlStringProperties, msgId);
                    }

                    //Archive to File
                    if (IsArchiveToFile)
                    {
                        ArchiveToFile(provider, msgId, inMsg.Context, inMsg.BodyPart.Data);
                    }
                }
                catch (Exception exc)
                {
                    System.Diagnostics.EventLog.WriteEntry("BizTalk Message Archiving Component", string.Format("Encountered an error: '{0}' : '{1}'", exc.Message, exc.ToString()), System.Diagnostics.EventLogEntryType.Error);
                }
            }

            inMsg.BodyPart.Data.Position = 0;
            return inMsg;
        }

        #region Private Methods

        /// <summary>
        /// Archive To File
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="msgId"></param>
        /// <param name="context"></param>
        /// <param name="msgStream"></param>
        protected virtual void ArchiveToFile(IMessageArchiveProvider provider, Guid msgId, IBaseMessageContext context, Stream msgStream)
        {
            //Get Original FileName using FTP 
            string ftpNs = @"http://schemas.microsoft.com/BizTalk/2003/ftp-properties";
            string fileNs = @"http://schemas.microsoft.com/BizTalk/2003/file-properties";
            string name = "ReceivedFileName";

            //Get Filename by FileAdapter NS
            string fileName = GetPropertyContext(context, name, fileNs);
            if (string.IsNullOrEmpty(fileName))
            {
                //Get Filename by FTP NS
                fileName = GetPropertyContext(context, name, ftpNs);
            }

            //Get Filename without paths
            FileInfo fInfo = new FileInfo(fileName);
            string origFileName = fInfo.Name;
            string origFileNameExt = fInfo.Extension;
            //Remove Extension
            origFileName = origFileName.Replace(origFileNameExt, "");

            //Update Filename based on macro's
            string archiveFileName = FileArchiveFileName
                .Replace("%SourceFileName%", origFileName)
                .Replace("%MessageID%", msgId.ToString())
                .Replace("%datetime%", DateTime.Now.ToString("yyyy-MM-ddhhmmsstt"))
                .Replace("%time%", DateTime.Now.ToString("hhmmsstt"));

            //Set the fileName to message Id if blank
            if (string.IsNullOrEmpty(archiveFileName))
                archiveFileName = msgId.ToString() + ".part";
            else
            {
                //Apply Extension
                archiveFileName += origFileNameExt;
            }
            provider.ArchiveToFile(archiveFileName, FileArchiveBackupFolder, FileArchiveIsOverwriteFiles, msgStream, FileArchiveUserName, FileArchiveUserPwd, 
                        FileArchiveUserDomain, IsCompressFile, CompressionPassword);
        }

        /// <summary>
        /// Archives the Msg to database
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="msgStream"></param>
        /// <param name="props"></param>
        /// <param name="msgId"></param>
        protected virtual void ArchiveToDb(IMessageArchiveProvider provider, Stream msgStream, string props, Guid msgId)
        {
            //Set Variables
            Stream compressedStream = null;
            long compressedLength = 0;
            long length = 0;

            msgStream.Position = 0;
            length = msgStream.Length;

            //Zip the stream if CompressFile = true
            if (IsCompressFile)
            {
                //Compressed the Stream
                compressedStream = Utility.GetZipStream(
                    Path.Combine(msgId.ToString(), DbFileExtensions),
                    msgStream,
                    CompressionPassword);

                compressedLength = compressedStream.Length;
            }
            //Use the uncompressed one
            else
            {
                msgStream.CopyTo(compressedStream);
            }

            //Archive to Database
            provider.ArchiveToDb(DbConnStr, DbConnProvider, DbSPName,
                msgId, compressedStream, props, length, IsCompressFile, compressedLength);
        }

        /// <summary>
        /// Returns the Message Properties based on DbPropList
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected string GetMessageProperties(IBaseMessageContext context)
        {
            //Get Msg Properties
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<MessageProperties>");
            //Loop for every properties and retrieve from msg context
            foreach (string p in DbPropList.Split('|'))
            {
                string[] values = p.Split(';');
                if (values.Length == 2)
                {
                    string name = values[0];
                    string ns = values[1];
                    sb.AppendLine(string.Format("<{0}>", name));
                    sb.AppendLine(GetPropertyContext(context, name, ns));
                    sb.AppendLine(string.Format("</{0}>", name));
                }
            }
            sb.AppendLine("</MessageProperties>");
            return sb.ToString();
        }

        /// <summary>
        /// Retrieves the value of Property from Message Context by Name and namespace
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected string GetPropertyContext(IBaseMessageContext context, string name, string ns)
        {
            string result = string.Empty;
            if (context != null)
            {
                try
                {
                    var res = context.Read(name, ns);
                    if (res != null)
                    {
                        result = res.ToString();
                    }
                }
                catch
                {
                    //Ignore and return empty string
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the Provider to be used Default or Built-in
        /// </summary>
        /// <param name="isUseCustom"></param>
        /// <returns></returns>
        protected virtual IMessageArchiveProvider GetProvider(bool isUseCustom = false)
        {
            IMessageArchiveProvider provider = null;
            try
            {
                if (isUseCustom)
                {
                    provider = new MessageArchiver();
                }
                else
                {
                    provider = new MessageArchiver();
                }
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format("Unable to create Message Archive Provider, IsUseCustom: '{0}', Message: '{1}', Details: '{2}'", isUseCustom, exc.Message, exc.ToString()));
            }
            return provider;
        }

        #endregion

        #endregion
    }
}