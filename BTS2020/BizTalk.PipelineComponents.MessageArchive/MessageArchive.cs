namespace BizTalk.PipelineComponents.MessageArchive
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
    using System.Text.RegularExpressions;

    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("e6987868-5d41-4468-b59b-890047e5f815")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public class MessageArchive : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        private string archiveFullPath = String.Empty;
        private const int MAX_FILENAME_LENGTH = 260;
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.MessageArchive.MessageArchive", Assembly.GetExecutingAssembly());

        #region Properties

        private bool overwriteExistingFile;
        
        public bool OverwriteExistingFile
        {
            get
            {
                return overwriteExistingFile;
            }
            set
            {
                overwriteExistingFile = value;
            }
        }
        
        private bool archivingEnabled;
        
        public bool ArchivingEnabled
        {
            get
            {
                return archivingEnabled;
            }
            set
            {
                archivingEnabled = value;
            }
        }
        
        private string archiveFilePath;
        
        public string ArchiveFilePath
        {
            get
            {
                return archiveFilePath;
            }
            set
            {
                archiveFilePath = value;
            }
        }
        
        private string archiveFilenameMacro;
        
        public string ArchiveFilenameMacro
        {
            get
            {
                return archiveFilenameMacro;
            }
            set
            {
                archiveFilenameMacro = value;
            }
        }
        
        private string additionalMacroIfExists;
        
        public string AdditionalMacroIfExists
        {
            get
            {
                return additionalMacroIfExists;
            }
            set
            {
                additionalMacroIfExists = value;
            }
        }

        private bool optimizeForPerformance;

        public bool OptimizeForPerformance
        {
            get
            {
                return optimizeForPerformance;
            }
            set
            {
                optimizeForPerformance = value;
            }
        }


        #endregion

        #region IBaseComponent members

        /// <summary>
        /// Name of the component
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get
            {
                return resourceManager.GetString("COMPONENTNAME", CultureInfo.InvariantCulture);
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
                return resourceManager.GetString("COMPONENTVERSION", CultureInfo.InvariantCulture);
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
                return resourceManager.GetString("COMPONENTDESCRIPTION", CultureInfo.InvariantCulture);
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
            classID = new System.Guid("e6987868-5d41-4468-b59b-890047e5f815");
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
            val = this.ReadPropertyBag(propertyBag, "OverwriteExistingFile");
            if ((val != null))
            {
                this.overwriteExistingFile = ((bool)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "ArchivingEnabled");
            if ((val != null))
            {
                this.archivingEnabled = ((bool)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "ArchiveFilePath");
            if ((val != null))
            {
                this.archiveFilePath = ((string)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "ArchiveFilenameMacro");
            if ((val != null))
            {
                this.archiveFilenameMacro = ((string)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "AdditionalMacroIfExists");
            if ((val != null))
            {
                this.additionalMacroIfExists = ((string)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "OptimizeForPerformance");
            if ((val != null))
            {
                this.optimizeForPerformance = ((bool)(val));
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
            this.WritePropertyBag(propertyBag, "OverwriteExistingFile", this.OverwriteExistingFile);
            this.WritePropertyBag(propertyBag, "ArchivingEnabled", this.ArchivingEnabled);
            this.WritePropertyBag(propertyBag, "ArchiveFilePath", this.ArchiveFilePath);
            this.WritePropertyBag(propertyBag, "ArchiveFilenameMacro", this.ArchiveFilenameMacro);
            this.WritePropertyBag(propertyBag, "AdditionalMacroIfExists", this.AdditionalMacroIfExists);
            this.WritePropertyBag(propertyBag, "OptimizeForPerformance", this.OptimizeForPerformance);
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
                return ((System.Drawing.Bitmap)(this.resourceManager.GetObject("COMPONENTICON", CultureInfo.InvariantCulture))).GetHicon();
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
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pContext, 
            Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            //Check if is to archive the message
            if (this.ArchivingEnabled)
            {
                if (pInMsg.BodyPart != null)
                {
                    try
                    {
                        string filename = DefineTheArchiveFilename(pInMsg);
                        string filepath = DefineTheArchivePath(pInMsg);
                        archiveFullPath = BuildArchiveFilePath(filename, filepath, pInMsg);

                        if (this.OptimizeForPerformance)
                        {
                            CForwardOnlyEventingReadStream eventingReadStream = new CForwardOnlyEventingReadStream(pInMsg.BodyPart.GetOriginalDataStream());
                            eventingReadStream.ReadEvent += new ReadEventHandler(EventingReadStream_ReadEvent);

                            pInMsg.BodyPart.Data = eventingReadStream;
                        }
                        else
                        {
                            SaveStreamToFile(pInMsg.BodyPart.Data, true);
                            pInMsg.BodyPart.Data.Position = 0;
                        }
                    }
                    catch(Exception ex)
                    {
                        throw;
                    }
                }
                else throw new Exception("The message could not be archived because the body part is null.");
            }

            return pInMsg;
        }

        #endregion


        #region Extended methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inmsg"></param>
        /// <returns></returns>
        protected string DefineTheArchiveFilename(Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            string filename = "";

            if(String.IsNullOrEmpty(this.ArchiveFilenameMacro))
            {
                filename = GetFileName(inmsg);
            }
            else
            {
                if (this.ArchiveFilenameMacro.Contains("%"))
                {
                    // Define Macro Regular Expression.
                    const string macroRegex = "(%)((?:[a-z][a-z]+))(%)";

                    // Find regex macro matches within the supplied string.
                    Regex regex = new Regex(macroRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MatchCollection listMacros = regex.Matches(this.ArchiveFilenameMacro);

                    filename = this.ArchiveFilenameMacro;

                    // Iterate through each macro match, retrieve the context property value and 
                    // update the supplied string. If the context property value cannot be found,
                    // the macro will not be updated.
                    foreach (Match macro in listMacros)
                    {
                        string macroName = macro.Value; //Groups[2].Value;
                        filename = filename.Replace(macroName, TranslateMacro(macroName, inmsg));
                    }
                }
                else throw new Exception("The ArchiveFilenameMacro property does not contain any macro. Please clean that field or add a macro to the file name template like: %SourceFileName%_%datetime%.xml. Available macros are %datetime%, %MessageID%, %FileName%, %FileNameWithoutExtension%, %FileNameExtension%, %Day%, %Month%, %Year%, %time%, %ReceivePort%, %ReceiveLocation%. %SendPort%, %InboundTransportType%, %InterchangeID%");

            }

            if(String.IsNullOrEmpty(filename))
                throw new Exception("Message Archive component was unable to define an archive filename.");

            return Path.GetFileName(filename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inmsg"></param>
        /// <returns></returns>
        protected string DefineTheArchivePath(Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            string filepath = "";

            if (String.IsNullOrEmpty(this.ArchiveFilePath))
            {
                throw new Exception("You need to define a valid archive path. You can make use of the available macros are %datetime%, %MessageID%, %FileName%, %FileNameWithoutExtension%, %FileNameExtension%, %Day%, %Month%, %Year%, %time%, %ReceivePort%, %ReceiveLocation%. %SendPort%, %InboundTransportType%, %InterchangeID%");
            }
            else
            {
                if (this.ArchiveFilePath.Contains("%"))
                {
                    // Define Macro Regular Expression.
                    const string macroRegex = "(%)((?:[a-z][a-z]+))(%)";

                    // Find regex macro matches within the supplied string.
                    Regex regex = new Regex(macroRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MatchCollection listMacros = regex.Matches(this.ArchiveFilePath);

                    filepath = this.ArchiveFilePath;

                    // Iterate through each macro match, retrieve the context property value and 
                    // update the supplied string. If the context property value cannot be found,
                    // the macro will not be updated.
                    foreach (Match macro in listMacros)
                    {
                        string macroName = macro.Value; //Groups[2].Value;
                        filepath = filepath.Replace(macroName, TranslateMacro(macroName, inmsg));
                    }
                }
                else filepath = this.ArchiveFilePath;
            }

            if (String.IsNullOrEmpty(filepath))
                throw new Exception("Message Archive component was unable to define an archive filepath.");

            return filepath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inmsg"></param>
        /// <returns></returns>
        protected string DefineTNonDuplicationMacro(Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            string sufixFilename = "";

            if (String.IsNullOrEmpty(this.AdditionalMacroIfExists))
                return sufixFilename;

            if (this.AdditionalMacroIfExists.Contains("%"))
            {
                // Define Macro Regular Expression.
                const string macroRegex = "(%)((?:[a-z][a-z]+))(%)";

                // Find regex macro matches within the supplied string.
                Regex regex = new Regex(macroRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection listMacros = regex.Matches(this.AdditionalMacroIfExists);

                sufixFilename = this.AdditionalMacroIfExists;

                // Iterate through each macro match, retrieve the context property value and 
                // update the supplied string. If the context property value cannot be found,
                // the macro will not be updated.
                foreach (Match macro in listMacros)
                {
                    string macroName = macro.Value; //Groups[2].Value;
                    sufixFilename = sufixFilename.Replace(macroName, TranslateMacro(macroName, inmsg));
                }
            }
            else throw new Exception("The ArchiveFilenameMacro property does not contain any macro. Please clean that field or add a macro to the file name template like: %SourceFileName%_%datetime%.xml. Available macros are %datetime%, %MessageID%, %FileName%, %FileNameWithoutExtension%, %FileNameExtension%, %Day%, %Month%, %Year%, %time%, %ReceivePort%, %ReceiveLocation%. %SendPort%, %InboundTransportType%, %InterchangeID%");

            if (String.IsNullOrEmpty(sufixFilename))
                throw new Exception("Message Archive component was unable to define an sufix archive filename.");

            return sufixFilename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="macroName"></param>
        /// <param name="inmsg"></param>
        /// <returns></returns>
        protected string TranslateMacro(string macroName, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            string macroValue = string.Empty;

            switch(macroName)
            {
                case "%datetime%":
                    macroValue = DateTime.Now.ToString("YYYY-MM-DDThhmmss");
                    break;
                case "%MessageID%":
                    macroValue = inmsg.MessageID.ToString();
                    break;
                case "%FileName%":
                    macroValue = GetFileName(inmsg);
                    break;
                case "%FileNameWithoutExtension%":
                    macroValue = GetFileName(inmsg);
                    macroValue = System.IO.Path.GetFileNameWithoutExtension(macroValue);
                    break;
                case "%FileNameExtension%":
                    macroValue = GetFileName(inmsg);
                    macroValue = System.IO.Path.GetExtension(macroValue);
                    break;
                case "%Day%":
                    macroValue = DateTime.Now.Day.ToString();
                    break;
                case "%Month%":
                    macroValue = DateTime.Now.Month.ToString();
                    break;
                case "%Year%":
                    macroValue = DateTime.Now.Year.ToString();
                    break;
                case "%time%":
                    macroValue = DateTime.Now.ToString("hhmmss");
                    break;
                case "%ReceivePort%":
                    macroValue = inmsg.Context.Read("ReceivePortName", "http://schemas.microsoft.com/BizTalk/2003/system-properties").ToString();
                    break;
                case "%ReceiveLocation%":
                    macroValue = inmsg.Context.Read("ReceiveLocationName", "http://schemas.microsoft.com/BizTalk/2003/system-properties").ToString();
                    break;
                case "%SendPort%":
                    macroValue = inmsg.Context.Read("SPName", "http://schemas.microsoft.com/BizTalk/2003/system-properties").ToString();
                    break;
                case "%InboundTransportType%":
                    macroValue = inmsg.Context.Read("InboundTransportType", "http://schemas.microsoft.com/BizTalk/2003/system-properties").ToString();
                    break;
                case "%InterchangeID%":
                    macroValue = inmsg.Context.Read("InterchangeID", "http://schemas.microsoft.com/BizTalk/2003/system-properties").ToString();
                    break;
                default:
                    throw new Exception("The ArchiveFilenameMacro contains an invalid macro. Available macros are %datetime%, %MessageID%, %SourceFileName%, %SourceFileNameWithoutExtension%, %SourceFileNameExtension%, %Day%, %Month%, %Year%, %time%, %ReceicePort%, %ReceiveLocation%. %SendPort%, %InboundTransportType%, %InterchangeID%");
            }

            return macroValue;
        }

        /// <summary>
        /// Retrive the filename from the context of the message.
        /// If it doesn't exist then the MessageId will be used.
        /// </summary>
        /// <param name="inmsg"></param>
        /// <returns></returns>
        protected string GetFileName(Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
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

            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <param name="inmsg"></param>
        /// <returns></returns>
        protected string BuildArchiveFilePath(string fileName, string filePath, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            string archiveFilename = String.Empty;
            filePath = filePath.TrimEnd('\\') + "\\";

            // Create the directory if it does not already exist.
            if (!new DirectoryInfo(filePath).Exists)
            {
                try
                {
                    Directory.CreateDirectory(filePath);
                }
                catch
                {
                    throw;
                }
            }

            // If the file exists **and** the file should not be overwritten, add a GUID onto the end 
            // of the filename before the file extension.
            if (File.Exists(filePath + fileName) && this.OverwriteExistingFile != true)
            {
                
                string additionalMacro = DefineTNonDuplicationMacro(inmsg);
                if(String.IsNullOrEmpty(additionalMacro))
                    archiveFilename = filePath + Path.GetFileNameWithoutExtension(fileName) + "-" + inmsg.MessageID.ToString() + Path.GetExtension(fileName);
                else archiveFilename = filePath + Path.GetFileNameWithoutExtension(fileName) + additionalMacro + Path.GetExtension(fileName);
            }
            else archiveFilename = filePath + fileName;

            // Modify the full file path if greater than the max filename length.
            if (archiveFilename.Length > MAX_FILENAME_LENGTH)
                throw new Exception("The full file path exceeded the operating system maximum length of " + MAX_FILENAME_LENGTH + " characters.");

            return archiveFilename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgStream"></param>
        /// <param name="fileName"></param>
        /// <param name="overWrite"></param>
        protected virtual void SaveStreamToFile(Stream msgStream, bool overWrite)
        {
            int bufferSize = 4096;
            byte[] buffer = new byte[4096];
            int numBytesRead = 0;

            using (FileStream fs = new FileStream(this.archiveFullPath, overWrite ? FileMode.Create : FileMode.CreateNew))
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


        /// <summary>
        /// Stream 'On Read Event'. Called as downstream components or the BizTalk Messaging Agent itself 
        /// reads the message. This method spools the portion of the stream read to disk. An 'optimised' copy.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        private void EventingReadStream_ReadEvent(object src, EventArgs args)
        {
            ReadEventArgs rargs = args as ReadEventArgs;

            if (rargs != null)
            {
                try
                {
                    using (
                        FileStream FileArchiveStream =
                            new FileStream(this.archiveFullPath, FileMode.Create, FileAccess.Write))
                    {
                        using (BinaryWriter FileBinaryWriter = new BinaryWriter(FileArchiveStream))
                        {
                            FileBinaryWriter.Write(rargs.buffer, 0, rargs.bytesRead);

                            // Close the file writer.
                            FileBinaryWriter.Flush();
                            FileBinaryWriter.Close();
                        }
                    }
                }
                catch (IOException ioEx)
                {
                    throw new Exception(String.Format("An error occured creating the archive file at {0}.\n\nThe following error was raised: {1}", "", ioEx.Message));
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("An error occured creating the archive file at {0}.\n\nThe following error was raised: {1}", "", ex.Message));
                }
            }
            else
            {
                throw new Exception(String.Format("An error occured creating the archive file at {0}.\n\nThe stream OnRead event did not contain Event Args, the archive file cannot be written.", ""));
            }
        }

        #endregion
    }
}