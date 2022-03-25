/*##########################################################################################################################################
 # BizTalk Server Pipeline component to use as a substitution for the out-of-the-box XmlValidator component. The default component stops   #
 # validating the XML message after encountering the fix error. It doesn't go thru the rest of the validation and provide a complete       #
 # detailed list of all errors. That means that if you have several errors, you probably will test it several times and fix one issue at a #
 # time. This component will suppress this limitation/behavior. Instead of throwing an exception the moment the first error occurs, it     #
 # will validate an entire message and then provide a complete list of the errors found.                                                   #
 ###########################################################################################################################################*/

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Messaging;
using Microsoft.BizTalk.Component.Utilities;

using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;

using XMLNORM;

namespace BizTalk.PipelineComponents.XmlCompleteValidator
{
	/// <summary>
	/// XmlValidator Pipeline component to validate an entire XML message against specified Schemas or DTDs and then provide a complete list of the errors found.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Guid("a50d5943-6c4d-4435-b60a-16b18d6015bb")]
    [ComponentCategory(CategoryTypes.CATID_Validate)]
    public class XmlCompleteValidator : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        private ResourceManager resourceManager = new ResourceManager("BizTalk.PipelineComponents.XmlCompleteValidator.XmlCompleteValidator", Assembly.GetExecutingAssembly());
		private static PropertyBase _documentSpecNameProperty;
		private string _concatDocSpecNames;
		private string _concatDocSpecTargetNamespaces;

		// Array to store validation errors that might occur when validating the processed message
		private ArrayList _validationErrors = new ArrayList();


		static XmlCompleteValidator()
		{
			XmlCompleteValidator._documentSpecNameProperty = new DocumentSpecName();
		}

		#region Properties

		private SchemaList _documentSchemas = new SchemaList();

		/// <summary>
		/// List to store and retrieve a list of schemas that this component should validate inbound XML messages against it
		/// </summary>
		public SchemaList DocumentSchemas
		{
			get
			{
				return _documentSchemas;
			}
			set
			{
				_documentSchemas = value;

				// Convert the SchemaList into two sets of strings, one for the document names
				// and one for the DocumentNamespaces the inbound message should adhere to
				StringBuilder documentNames = new StringBuilder();
				StringBuilder documentSchemas = new StringBuilder();

				// Iterate thru the Schema list
				foreach(Schema s in this._documentSchemas)
				{
					// Append the DocumentSpecName and the TargetNamespace
					documentNames.Append(s.DocSpecName);
					documentSchemas.Append(s.TargetNamespace);

					documentNames.Append('|');
					documentSchemas.Append(' ');
				}

				// And to finalize, we are removing any trailing separator characters
				documentNames.Remove(documentNames.Length - 1, 1);
				documentSchemas.Remove(documentSchemas.Length - 1, 1);

				this._concatDocSpecNames = documentNames.ToString();
				this._concatDocSpecTargetNamespaces = documentSchemas.ToString();
			}
		}

		#endregion

		#region IBaseComponent

		/// <summary>
		/// Name of the component
		/// </summary>
		[Browsable(false)]
        public string Name
        {
            get
            {
                return this.resourceManager.GetString("COMPONENTNAME", CultureInfo.InvariantCulture);
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
                return this.resourceManager.GetString("COMPONENTVERSION", CultureInfo.InvariantCulture);
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
                return this.resourceManager.GetString("COMPONENTDESCRIPTION", CultureInfo.InvariantCulture);
            }
        }

		#endregion

		#region IComponentUI

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
		/// The Validate method is called by the BizTalk Editor during the build of a BizTalk project.
		/// </summary>
		/// <param name="projectSystem">An Object containing the configuration properties.</param>
		/// <returns>The IEnumerator enables the caller to enumerate through a collection of strings containing error messages. These error messages appear as compiler error messages. To report successful property validation, the method should return an empty enumerator.</returns>
		public IEnumerator Validate(object projectSystem)
		{
			ArrayList errors = new ArrayList();
			IEnumerator errorEnumerator = null;

			if (projectSystem != null)
			{
				// Iterate all schemas to see whether they are actually still referenced from the project
				foreach (Schema s in this.DocumentSchemas)
				{
					// Check if the current schema is not referenced
					if (!SchemaExists(s, projectSystem))
					{
						errors.Add(string.Format("Unable to find document specification for Schema '{0}'", s.SchemaName));
					}
				}

				// Check whether we reference schemas in which the target namespaces collide
				CheckForDuplicateTargetNamespaces(this.DocumentSchemas, errors, "Duplicate namespace found: '{0}'");

				if (errors.Count > 0)
				{
					errorEnumerator = errors.GetEnumerator();
				}
			}

			return errorEnumerator;
		}

		#endregion

		#region IPersistPropertyBag

		/// <summary>
		/// Gets class Id of component for usage from unmanaged code.
		/// </summary>
		/// <param name="classid">Class Id of the component</param>
		public void GetClassID(out System.Guid classid)
		{
			classid = new System.Guid("a50d5943-6c4d-4435-b60a-16b18d6015bb");
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
			string concatDocSpecNames = ReadPropertyBag(pb, "DocSpecNames") as string;
			string concatDocSpecTargetNamespaces = ReadPropertyBag(pb, "DocSpecTargetNamespaces") as string;

			if (concatDocSpecNames != null && concatDocSpecNames.Length > 0)
			{
				string[] docSpecNames = concatDocSpecNames.Split(new char[] { '|' });
				string[] docSpecTargetNamespaces = null;
				if (concatDocSpecTargetNamespaces != null && concatDocSpecTargetNamespaces.Length > 0)
				{
					docSpecTargetNamespaces = concatDocSpecTargetNamespaces.Split(new char[] { ' ' });

					for (int i = 0; i < docSpecNames.Length; i++)
					{
						Schema tempSchema = new Schema(docSpecNames[i]);
						if (docSpecTargetNamespaces != null && docSpecTargetNamespaces.Length >= i)
						{
							tempSchema.TargetNamespace = docSpecTargetNamespaces[i];
						}

						this._documentSchemas.Add(tempSchema);
					}

					this._concatDocSpecNames = concatDocSpecNames;
					this._concatDocSpecTargetNamespaces = concatDocSpecTargetNamespaces;
				}
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
			WritePropertyBag(pb, "DocSpecNames", this._concatDocSpecNames);
			WritePropertyBag(pb, "DocSpecTargetNamespaces", this._concatDocSpecTargetNamespaces);
		}

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
			catch (System.ArgumentException)
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
		private void WritePropertyBag(IPropertyBag pb, string propName, object val)
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

		#region Private Methods

		private bool SchemaExists(Schema usedSchema, object projectSystem)
		{
			bool retVal = false;

			if(projectSystem != null)
			{
				// start off by retrieving all available schemas within the project
				Schema[] schemas = SchemaRetriever.GetSchemas(projectSystem);

				// iterate the retrieved schemas
				foreach(Schema schema in schemas)
				{
					// if we find a match, we return true
					if(schema.SchemaName == usedSchema.SchemaName)
					{
						retVal = true;

						break;
					}
				}
			}

			return retVal;
		}


		/// <param name="schemas">the collection of <see cref="Schema"/> objects to check for duplicates</param>
		/// <param name="errors">the collection of error strings to be added to in case of duplicates</param>
		/// <param name="errorMessage">a configurable error message to be added to the collection</param>
		private void CheckForDuplicateTargetNamespaces(SchemaList schemas, ArrayList errors, string errorMessage)
		{
			// stored all encountered target namespaces
			ArrayList usedSchemas = new ArrayList();

			// if we have schemas defined within the project
			if(schemas != null && schemas.Count > 0)
			{
				// iterate over all schemas within this project
				foreach(Schema s in schemas)
				{
					// if the current schema has a target namespace specified,
					if(s.TargetNamespace != null && s.TargetNamespace.Length > 0)
					{
						// if we encountered the specified target namespace before,
						if(usedSchemas.Contains(s.TargetNamespace))
						{
							// default to a simple error message if nothing specified
							if(errorMessage == null || errorMessage.Length == 0)
							{
								errorMessage = "duplicate namespace encountered: '{0}'";
							}

							// add the encountered duplicate to the errors to be reported
							errors.Add(string.Format(errorMessage, s.TargetNamespace));
						}
						else
						{
							// add the target namespace as being in use
							usedSchemas.Add(s.TargetNamespace);
						}
					}
				}
			}
		}

		/// <summary>
		/// Deep XML message valitation method. This method copies the inbound message, processes
		/// the inbound message onto the copied message and returns the copied message if all went well
		/// </summary>
		/// <param name="pc">the <see cref="IPipelineContext"/></param>
		/// <param name="inmsg">the inbound message</param>
		/// <returns>the copied message instance, if all went well</returns>
		private IBaseMessage FullyValidateMessage(IPipelineContext pc, IBaseMessage inmsg)
		{
			IBaseMessage copiedMessage = null;

			if (pc != null && inmsg != null)
			{
				#region duplicate inbound message
				IBaseMessageFactory messageFactory = pc.GetMessageFactory();
				copiedMessage = messageFactory.CreateMessage();
				IBaseMessageContext copiedContext = inmsg.Context;
				IBaseMessagePart copiedMessagePart = inmsg.BodyPart;
				copiedMessage.Context = copiedContext;
				#endregion

				this.ProcessPart(pc, inmsg, copiedMessage, copiedMessagePart);
			}

			return copiedMessage;
		}

		#endregion

		#region IComponent

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
			IBaseMessage validatedMessage = this.FullyValidateMessage(pc, inmsg);

            return validatedMessage;
        }

		/// <summary>
		/// processes the inbound message part
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="inmsg"></param>
		/// <param name="outmsg"></param>
		/// <param name="part"></param>
		private void ProcessPart(IPipelineContext pc, IBaseMessage inmsg, IBaseMessage outmsg, IBaseMessagePart part)
		{
			IDocumentSpec docSpec = null;

			Stream dataStream = part.GetOriginalDataStream();
			MarkableForwardOnlyEventingReadStream eventingDataStream = new MarkableForwardOnlyEventingReadStream(dataStream);

			XmlSchemaCollection schemaCollection = new XmlSchemaCollection(new NameTable());
			schemaCollection.ValidationEventHandler += new ValidationEventHandler(this.ValidationCallBack);

			// retrieve the assigned document schemas to validate against
			SchemaList docSchemas = this.DocumentSchemas;

			// retrieve the namespace this document adheres to
			string contextProperty = (string) outmsg.Context.Read(XmlCompleteValidator._documentSpecNameProperty.Name.Name, XmlCompleteValidator._documentSpecNameProperty.Name.Namespace);

			// if the inbound message has a namespace,
			if(contextProperty != null && contextProperty.Length > 0)
			{
				// clear the original schemas to validate against
				docSchemas.Clear();

				string[] contextSchemas = contextProperty.Split(new char[] { '|' });
				
				// set it's schemas
				foreach(string schemaName in contextSchemas)
				{
					docSchemas.Add(new Schema(schemaName));
				}
			}

			#region retrieve validation schemas, shamelessly copied from the original XmlValidator pipeline component
			bool validateSchemas = this.DocumentSchemas != null && this.DocumentSchemas.Count > 0;
			if(validateSchemas && this.DocumentSchemas.Count == 1 && this.DocumentSchemas[0].SchemaName.Length == 0)
			{
				validateSchemas = false;
			}
			
			if(validateSchemas)
			{
				foreach(Schema s in docSchemas)
				{
					try
					{
						docSpec = pc.GetDocumentSpecByName(s.SchemaName);
					}
					catch(COMException e)
					{
						throw new XmlCompleteValidatorException(
							ExceptionType.CANNOT_GET_DOCSPEC_BY_NAME, 
							e.ErrorCode.ToString("X") + ": " + e.Message,
							new string[] { s.SchemaName });
					}

					if(docSpec == null)
					{
						throw new XmlCompleteValidatorException(
							ExceptionType.CANNOT_GET_DOCSPEC_BY_NAME, 
							string.Empty,
							new string[] { s.SchemaName });
					}

					XmlSchemaCollection coll = docSpec.GetSchemaCollection();

					schemaCollection.Add(coll);
				}
			}
			else
			{
				try
				{
					docSpec = pc.GetDocumentSpecByType(Utils.GetDocType(eventingDataStream));
				}
				catch(COMException e)
				{
					throw new XmlCompleteValidatorException(
						ExceptionType.CANNOT_GET_DOCSPEC_BY_TYPE, 
						e.ErrorCode.ToString("X") + ": " + e.Message,
						new string[] { Utils.GetDocType(eventingDataStream) });
				}

				if(docSpec == null)
				{
					throw new XmlCompleteValidatorException(
						ExceptionType.CANNOT_GET_DOCSPEC_BY_TYPE, 
						string.Empty,
						new string[] { Utils.GetDocType(eventingDataStream) });
				}

				schemaCollection = docSpec.GetSchemaCollection();
			}
			#endregion

			// the most critical line within this component, assign an
			// XmlEventingValidationStream to ensure the inbound messagestream is validated
			// and events can be assigned which allow us to capture any erros that might occur
			XmlEventingValidationStream validatingStream = new XmlEventingValidationStream(eventingDataStream);
			
			// add the schemas we'd like to validate the inbound message against
			validatingStream.Schemas.Add(schemaCollection);

			// assign a validation event which will accumulate any errors within the inbound message
			validatingStream.ValidatingReader.ValidationEventHandler += new ValidationEventHandler(XmlMessageValidationCallBack);

			// and assign the AfterLastReadEvent, which fires upon reading the last piece of information
			// from the inbound message stream and pushes all accumulated error information out into
			// the eventviewer and onto the HAT context by throwing an exception which contains the errors
			validatingStream.AfterLastReadEvent += new AfterLastReadEventHandler(validatingStream_AfterLastReadEvent);

			// duplicate the inbound message part by creating a new one and copying it's properties
			IBaseMessageFactory messageFactory = pc.GetMessageFactory();
			IBaseMessagePart messagePart = messageFactory.CreateMessagePart();
			
			// if the inbound message exists and has a body part, copy the part properties
			// into the outbound messagepart
			if (inmsg != null && inmsg.BodyPart != null)
			{
				messagePart.PartProperties = PipelineUtil.CopyPropertyBag(inmsg.BodyPart.PartProperties, messageFactory);
			}

			// set the outbound charset
			messagePart.Charset = "UTF-8";

			// set the outbound content type
			messagePart.ContentType = "text/xml";

			// and assign the outbound datastream
			messagePart.Data = validatingStream;

			// finally, copy existing message parts
			CopyMessageParts(pc, inmsg, outmsg, messagePart, false);
		}

		/// <summary>
		/// called by the schema reader upon encountering errors within inbound schemas
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			this._validationErrors.Add(args);
		}

		/// <summary>
		/// called by the stream upon validation errors within the message
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void XmlMessageValidationCallBack(object sender, ValidationEventArgs args)
		{
			this._validationErrors.Add(args);
		}

		/// <summary>
		/// fires when the complete stream has been read, logs any errors that occured during
		/// validation
		/// </summary>
		/// <param name="src"></param>
		/// <param name="args"></param>
		private void validatingStream_AfterLastReadEvent(object src, EventArgs args)
		{
			// if any validation errors were encountered,
			if(this._validationErrors.Count > 0)
			{
				// dump the entire validation arraylist to string
				StringBuilder sb = new StringBuilder();
				foreach(ValidationEventArgs arg in this._validationErrors)
				{
					sb.Append(arg.Severity.ToString());
					sb.Append(": ");
					sb.Append(arg.Exception.Message);
					sb.Append("\n");
				}

				// write an eventlog entry stating the errors this message caused
				EventLog.WriteEntry("XmlCompleteValidator", sb.ToString(), EventLogEntryType.Error, 1001);
				
				// throw a new exception
				throw new XmlCompleteValidatorException(ExceptionType.VALIDATOR_FAILED, string.Empty, new string[] { sb.ToString() });
			}		
		}

		#endregion

		#region Utility functions

		/// <summary>
		/// copies all message parts part of the inbound message onto the outbound message
		/// </summary>
		/// <param name="pc">the <see cref="IPipelineContext"/> this message belongs to</param>
		/// <param name="inmsg">the inbound message</param>
		/// <param name="outmsg">the outbound message</param>
		/// <param name="bodyPart">the body part</param>
		/// <param name="allowUnrecognizeMessage">whether to allow unrecognized messages</param>
		public static void CopyMessageParts(
			IPipelineContext pc, 
			IBaseMessage inmsg, 
			IBaseMessage outmsg, 
			IBaseMessagePart bodyPart, 
			bool allowUnrecognizeMessage)
		{
			string text1 = inmsg.BodyPartName;
			for (int num1 = 0; num1 < inmsg.PartCount; num1++)
			{
				string text2 = null;
				IBaseMessagePart part1 = inmsg.GetPartByIndex(num1, out text2);
				if ((part1 == null) && !allowUnrecognizeMessage)
				{
					throw new ArgumentNullException("otherOutPart[" + num1 + "]");
				}
				if (text1 != text2)
				{
					outmsg.AddPart(text2, part1, false);
				}
				else
				{
					outmsg.AddPart(text1, bodyPart, true);
				}
			}
		}

		#endregion
	}
}