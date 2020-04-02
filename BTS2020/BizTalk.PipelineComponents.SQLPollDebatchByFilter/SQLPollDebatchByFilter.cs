namespace BizTalk.PipelineComponents.SQLPollDebatchByFilter
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
    using System.Xml;

    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("f455311a-1d99-4238-935b-7ad5890e97e5")]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    public class SQLPollDebatchByFilter : Microsoft.BizTalk.Component.Interop.IDisassemblerComponent, Microsoft.BizTalk.Component.Interop.IProbeMessage, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        private System.Collections.Queue qOutputMsgs = new System.Collections.Queue();
        private string systemPropertiesNamespace = @"http://schemas.microsoft.com/BizTalk/2003/system-properties";
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.SQLPollDebatchByFilter.SQLPollDebatchByFilter", Assembly.GetExecutingAssembly());
        
        private int _GroupingElement;
        
        public int GroupingElement
        {
            get
            {
                return _GroupingElement;
            }
            set
            {
                _GroupingElement = value;
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
            classid = new System.Guid("f455311a-1d99-4238-935b-7ad5890e97e5");
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
            val = this.ReadPropertyBag(pb, "GroupingElement");
            if ((val != null))
            {
                this._GroupingElement = ((int)(val));
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
            this.WritePropertyBag(pb, "GroupingElement", this.GroupingElement);
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
        
        /// <summary>
        /// this variable will contain any message generated by the Disassemble method
        /// </summary>
        private System.Collections.Queue _msgs = new System.Collections.Queue();
        
        #region IDisassemblerComponent members
        /// <summary>
        /// called by the messaging engine until returned null, after disassemble has been called
        /// </summary>
        /// <param name="pc">the pipeline context</param>
        /// <returns>an IBaseMessage instance representing the message created</returns>
        public Microsoft.BizTalk.Message.Interop.IBaseMessage GetNext(Microsoft.BizTalk.Component.Interop.IPipelineContext pc)
        {
            // get the next message from the Queue and return it
            Microsoft.BizTalk.Message.Interop.IBaseMessage msg = null;
            if ((_msgs.Count > 0))
            {
                msg = ((Microsoft.BizTalk.Message.Interop.IBaseMessage)(_msgs.Dequeue()));
            }
            return msg;
        }
        
        /// <summary>
        /// called by the messaging engine when a new message arrives
        /// </summary>
        /// <param name="pc">the pipeline context</param>
        /// <param name="inmsg">the actual message</param>
public void Disassemble(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
{
    string originalDataString;

    try
    {
        //fetch original message
        Stream originalMessageStream = inmsg.BodyPart.GetOriginalDataStream();
        byte[] bufferOriginalMessage = new byte[originalMessageStream.Length];
        originalMessageStream.Read(bufferOriginalMessage, 0, Convert.ToInt32(originalMessageStream.Length));
        originalDataString = System.Text.ASCIIEncoding.ASCII.GetString(bufferOriginalMessage);
    }
    catch (Exception ex)
    {
        throw new ApplicationException("Error in reading original message: " + ex.Message);
    }

    XmlDocument originalMessageDoc = new XmlDocument();
    StringBuilder messageString;
    string msgBatchId = string.Empty;

    try
    {
        //load original message
        originalMessageDoc.LoadXml(originalDataString);
                
        //fetch namespace and root element
        string namespaceURI = originalMessageDoc.DocumentElement.NamespaceURI;
        string rootElement = originalMessageDoc.DocumentElement.Name;

        //start batching messages
        messageString = new StringBuilder();
        messageString.Append("<" + rootElement + " xmlns:ns0='" + namespaceURI + "'>");
        string rowId = string.Empty;

        foreach (XmlNode childNode in originalMessageDoc.DocumentElement.ChildNodes)
        {
            messageString.Append("<" + childNode.Name + ">");
            foreach (XmlNode rows in childNode.ChildNodes)
            {
                rowId = rows.ChildNodes[this.GroupingElement].InnerText;

                if (msgBatchId == string.Empty)
                    msgBatchId = rowId;

                if (msgBatchId != rowId)
                {
                    messageString.Append("</" + childNode.Name + ">");
                    messageString.Append("</" + rootElement + ">");

                    //Queue message
                    CreateOutgoingMessage(pc, messageString.ToString(), namespaceURI, rootElement);

                    msgBatchId = rowId;

                    messageString.Remove(0, messageString.Length);
                    messageString.Append("<" + rootElement + " xmlns:ns0='" + namespaceURI + "'>");
                    messageString.Append("<" + childNode.Name + ">");
                    messageString.Append(rows.OuterXml);
                }
                else
                {
                    messageString.Append(rows.OuterXml);
                }
            }
            messageString.Append("</" + childNode.Name + ">");
        }

        messageString.Append("</" + rootElement + ">");

        CreateOutgoingMessage(pc, messageString.ToString(), namespaceURI, rootElement);
    }
    catch (Exception ex)
    {
        throw new ApplicationException("Error in writing outgoing messages: " + ex.Message);
    }
    finally
    {
        messageString = null;
        originalMessageDoc = null;
    }


    // _msgs.Enqueue(inmsg);
}
        #endregion

        private void CreateOutgoingMessage(IPipelineContext pContext, String messageString, string namespaceURI, string rootElement)
        {
            IBaseMessage outMsg;

            try
            {
                //create outgoing message
                outMsg = pContext.GetMessageFactory().CreateMessage();
                outMsg.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);
                outMsg.Context.Promote("MessageType", systemPropertiesNamespace, namespaceURI + "#" + rootElement.Replace("ns0:", ""));
                byte[] bufferOoutgoingMessage = System.Text.ASCIIEncoding.ASCII.GetBytes(messageString);
                outMsg.BodyPart.Data = new MemoryStream(bufferOoutgoingMessage);
                qOutputMsgs.Enqueue(outMsg);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in queueing outgoing messages: " + ex.Message);
            }
        }

        #region IProbeMessage members
        /// <summary>
        /// called by the messaging engine when a new message arrives
        /// checks if the incoming message is in a recognizable format
        /// if the message is in a recognizable format, only this component
        /// within this stage will be execute (FirstMatch equals true)
        /// </summary>
        /// <param name="pc">the pipeline context</param>
        /// <param name="inmsg">the actual message</param>
        public bool Probe(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {
            // 
            // TODO: check whether you're interested in the given message
            // 
            return true;
        }
        #endregion
    }
}
