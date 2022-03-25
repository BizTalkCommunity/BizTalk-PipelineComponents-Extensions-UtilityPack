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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Blogical.Shared.PipelineComponents.Exceptions;

namespace Blogical.Shared.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("86b7e286-f1b1-4884-a897-fe89f6a7270c")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public partial class SqlArchiveComponents : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI, IDisposable
    {
        #region Constants
        const string SYSTEM_PROPERTY_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2003/system-properties";
        #endregion

        #region Private Fields
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("Blogical.Shared.PipelineComponents.SqlArchiveComponents", Assembly.GetExecutingAssembly());
        private bool _enabled = true;
        private string _connectionString = string.Empty;
        private string _source;
        private SqlConnection _connection;
        private Guid _interchangeID;
        private byte[] _pointer;
        private long _totalLenght;
        private int _totalNumberOfReads;
        private SqlCommand _appendMessage;
        private SqlParameter _offsetParm;
        private SqlParameter _pointerParam;
        private SqlParameter _bytesParam;
        #endregion

        #region Public properties
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
        public string ConnectionString
        {
            get
            {
                if (_connectionString.ToLower().StartsWith("config://"))
                {
                    string key = _connectionString.Replace("config://", "");
                    return ConfigurationManager.ConnectionStrings[key].ConnectionString;
                }
                return _connectionString;
            }
            set
            {
                _connectionString = value;
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
                return SqlArchiveComponentsResources.COMPONENTNAME;
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
                return SqlArchiveComponentsResources.COMPONENTVERSION;
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
                return SqlArchiveComponentsResources.COMPONENTDESCRIPTION;
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
        public void GetClassID(out System.Guid classID)
        {
            classID = new System.Guid("86b7e286-f1b1-4884-a897-fe89f6a7270c");
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
        public virtual void Load(IPropertyBag propertyBag, int errorLog)
        {
            object val = null;
            val = this.ReadPropertyBag(propertyBag, "Enabled");
            if ((val != null))
            {
                this._enabled = ((bool)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "ConnectionString");
            if ((val != null))
            {
                this._connectionString = ((string)(val));
            }
        }

        /// <summary>
        /// Saves the current component configuration into the property bag
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="fClearDirty">not used</param>
        /// <param name="fSaveAllProperties">not used</param>
        public virtual void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyBag(propertyBag, "Enabled", this.Enabled);
            this.WritePropertyBag(propertyBag, "ConnectionString", this.ConnectionString);
        }

        #region utility functionality
        /// <summary>
        /// Reads property value from property bag
        /// </summary>
        /// <param name="pb">Property bag</param>
        /// <param name="propName">Name of property</param>
        /// <returns>Value of the property</returns>
        private object ReadPropertyBag(IPropertyBag propertyBag, string propName)
        {
            object val = null;
            try
            {
                propertyBag.Read(propName, out val, 0);
            }
            catch (System.ArgumentException)
            {
                return val;
            }
            catch (System.Exception e)
            {
                throw new ArchiveException(e.Message);
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
            catch (System.Exception e)
            {
                throw new ArchiveException(e.Message);
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
                return ((System.Drawing.Bitmap)(SqlArchiveComponentsResources.COMPONENTICON)).GetHicon();
            }
        }

        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build 
        /// of a BizTalk project.
        /// </summary>
        /// <param name="obj">An Object containing the configuration properties.</param>
        /// <returns>The IEnumerator enables the caller to enumerate through a collection of strings containing error messages. These error messages appear as compiler error messages. To report successful property validation, the method should return an empty enumerator.</returns>
        public System.Collections.IEnumerator Validate(object projectSystem)
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
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            if (!this._enabled)
                return pInMsg;

            CForwardOnlyEventingReadStream eventingReadStream = new CForwardOnlyEventingReadStream(pInMsg.BodyPart.GetOriginalDataStream());

            eventingReadStream.BeforeFirstReadEvent += new BeforeFirstReadEventHandler(eventingReadStream_BeforeFirstReadEvent);
            eventingReadStream.ReadEvent += new ReadEventHandler(eventingReadStream_ReadEvent);
            eventingReadStream.AfterLastReadEvent += new AfterLastReadEventHandler(eventingReadStream_AfterLastReadEvent);


            _interchangeID = new Guid(pInMsg.Context.Read("InterchangeID", SYSTEM_PROPERTY_NAMESPACE).ToString());

            try
            {
                _source = pInMsg.Context.Read("SPName", SYSTEM_PROPERTY_NAMESPACE).ToString();
            }
            catch { }

            if (String.IsNullOrEmpty(_source))
                _source = pInMsg.Context.Read("ReceivePortName", SYSTEM_PROPERTY_NAMESPACE).ToString();

            pInMsg.BodyPart.Data = eventingReadStream;
            return pInMsg;
        }
        #endregion

        #region Events
        
        void eventingReadStream_BeforeFirstReadEvent(object src, EventArgs args)
        {
            //try
            //{
            //    Trace.WriteLine("[Blogical] BeforeFirstReadEvent");
            //    // Create the Sql Connection
            //    _connection = new SqlConnection(this.ConnectionString);

            //     //Insert the row and retrieve the pointer.
            //    SqlCommand command = new SqlCommand(
            //                    "INSERT INTO [dbo].[ArchiveStore] " +
            //                    "([InterchangeID] " +
            //                    ",[Source] " +
            //                    ",[Message]) " +
            //                    "VALUES(@InterchangeID,@Source,0x0)" +
            //                    "SELECT @Pointer = TEXTPTR(Message) FROM [dbo].[ArchiveStore] " +
            //                    "WHERE InterchangeID = @InterchangeID AND " +
            //                    "[Source] = @Source", _connection);


            //    command.Parameters.Add("@InterchangeID", SqlDbType.UniqueIdentifier).Value = _interchangeID;
            //    command.Parameters.Add("@Source", SqlDbType.VarChar, 255).Value = _source;

            //    SqlParameter ptrParm = command.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
            //    ptrParm.Direction = ParameterDirection.Output;

            //    _connection.Open();

            //    command.ExecuteNonQuery();
            //    command.Dispose();
            //    GC.SuppressFinalize(command);
            //    _pointer = (byte[])ptrParm.Value;


            //    // Prepare the append command
            //    _appendMessage = new SqlCommand(
            //        "UPDATETEXT ArchiveStore.Message @Pointer @Offset 0 @Bytes",
            //        _connection);

            //    _pointerParam = _appendMessage.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
            //    _pointerParam.Value = _pointer;

            //    _offsetParm = _appendMessage.Parameters.Add("@Offset", SqlDbType.Int);
            //    _offsetParm.Value = 0;
            //    _bytesParam = _appendMessage.Parameters.Add("@Bytes", SqlDbType.Image, 1024);
            //}
            //catch (Exception ex)
            //{

            //    throw new ArchiveException(SqlArchiveComponentsResources.UNABLETOARCHIVE, ex);
            //}

        }
        void eventingReadStream_ReadEvent(object src, EventArgs args)
        {
            
            try
            {
                byte[] buffer = ((ReadEventArgs)args).buffer;
                int length = ((ReadEventArgs)args).bytesRead;

                if (_appendMessage == null)
                {
                    FirstReadEvent((ReadEventArgs)args);
                    return;
                }

                Trace.WriteLine("[Blogical] ReadEvent ("+length.ToString()+")");

                _bytesParam.Size = length;
                _bytesParam.Value = buffer;
                _appendMessage.ExecuteNonQuery();

                int offSet = (int)_offsetParm.Value + length;
                _offsetParm.Value = offSet;

                _totalLenght += length;
                _totalNumberOfReads++;
            }
            catch (Exception ex)
            {
                CleanUp();
                throw new ArchiveException(SqlArchiveComponentsResources.UNABLETOARCHIVE, ex);
            }

        }
        void eventingReadStream_AfterLastReadEvent(object src, EventArgs args)
        {
            Trace.WriteLine("[Blogical] AfterLastReadEvent");
            Trace.WriteLine("[Blogical] **********************************");
            Trace.WriteLine("[Blogical] Total Bytes:"+_totalLenght.ToString());
            Trace.WriteLine("[Blogical] Total number of reads:"+_totalNumberOfReads.ToString());
            Trace.WriteLine("[Blogical] **********************************");
            _appendMessage = null;
            _offsetParm = null;
            _pointerParam = null;
            _bytesParam = null;
            _connection.Close();
            _connection = null;
           
        }
        void CleanUp()
        {
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("DELETE [dbo].[ArchiveStore] WHERE [InterchangeID] = @InterchangeID", _connection);
                    command.Parameters.Add("@InterchangeID", SqlDbType.UniqueIdentifier).Value = _interchangeID;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ArchiveException(SqlArchiveComponentsResources.UNABLETOCLEANUP, ex);
            }
        }

        void FirstReadEvent(ReadEventArgs args)
        {
            try
            {
                Trace.WriteLine("[Blogical] FirstReadEvent");
                // Create the Sql Connection
                _connection = new SqlConnection(this.ConnectionString);

                //Insert the row and retrieve the pointer.
                SqlCommand command = new SqlCommand(
                                "INSERT INTO [dbo].[ArchiveStore] " +
                                "([InterchangeID] " +
                                ",[Source] " +
                                ",[Message]) " +
                                "VALUES(@InterchangeID,@Source,@Message)" +
                                "SELECT @Pointer = TEXTPTR(Message) FROM [dbo].[ArchiveStore] " +
                                "WHERE InterchangeID = @InterchangeID AND " +
                                "[Source] = @Source", _connection);


                command.Parameters.Add("@InterchangeID", SqlDbType.UniqueIdentifier).Value = _interchangeID;
                command.Parameters.Add("@Source", SqlDbType.VarChar, 255).Value = _source;
                command.Parameters.Add("@Message", SqlDbType.Image, args.bytesRead).Value = args.buffer;
                
                SqlParameter ptrParm = command.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
                ptrParm.Direction = ParameterDirection.Output;

                _connection.Open();

                command.ExecuteNonQuery();
                command.Dispose();
                GC.SuppressFinalize(command);
                _pointer = (byte[])ptrParm.Value;

                _totalLenght += args.bytesRead;
                _totalNumberOfReads++;

                // Prepare the append command
                _appendMessage = new SqlCommand(
                    "UPDATETEXT ArchiveStore.Message @Pointer @Offset 0 @Bytes",
                    _connection);

                _pointerParam = _appendMessage.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
                _pointerParam.Value = _pointer;

                _offsetParm = _appendMessage.Parameters.Add("@Offset", SqlDbType.Int);
                _offsetParm.Value = args.bytesRead;
                _bytesParam = _appendMessage.Parameters.Add("@Bytes", SqlDbType.Image, 1024);

            }
            catch (Exception ex)
            {

                throw new ArchiveException(SqlArchiveComponentsResources.UNABLETOARCHIVE, ex);
            }

        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool force)
        {
            _appendMessage.Dispose();
            _connection.Dispose();
            GC.SuppressFinalize(_appendMessage);
            GC.SuppressFinalize(_connection);
        }


        #endregion
    }
}
