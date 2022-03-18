//---------------------------------------------------------------------
// File: PdfTransformer.cs
// 
// Summary: Pipeline component that converts pdf files to XML/HTML.
//
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.ComponentModel;
using System.Collections;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.Win32;
using System.Reflection;
using System.Globalization;

namespace BizTalk.PipelineComponents.PDF2Xml
{
	/// <summary>
    /// converts pdf files to XML/HTML
	/// </summary>
	/// <remarks>
	/// PdfTransformer class implements pipeline components that can be used in receive pipelines
	/// to convert PDF messages to XML/HTML format. Component can
	/// be placed only in decode stage of receive pipeline
	/// </remarks>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Decoder)]
    [ComponentCategory(CategoryTypes.CATID_Validate)]
    [System.Runtime.InteropServices.Guid("7C0760D3-8706-4864-AB34-7D97EB7A82E5")]
	public class PDFConverter		: Microsoft.BizTalk.Component.Interop.IBaseComponent, 
									  Microsoft.BizTalk.Component.Interop.IComponent, 
		                              Microsoft.BizTalk.Component.Interop.IPersistPropertyBag,
		                              Microsoft.BizTalk.Component.Interop.IComponentUI
	{
		private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.PDF2Xml.Resource", Assembly.GetExecutingAssembly());

        #region Properties

        private bool isToApplyTransformation = false;
		private bool internalProcessToHTML = false;
		private string xsltPath = null;

		/// <summary>
		/// Location of Xsl transform file.
		/// </summary>
		public string XsltFilePath
		{
			get { return xsltPath; }
			set { xsltPath = value; }
		}

		public bool IsToApplyTransformation
		{
			get { return isToApplyTransformation; }
			set { isToApplyTransformation = value; }
		}

		public bool InternalProcessToHTML
		{
			get { return internalProcessToHTML; }
			set { internalProcessToHTML = value; }
		}

		#endregion

		#region IBaseComponent Members

		public string Description
		{
			get
			{
				return resourceManager.GetString("COMPONENTDESCRIPTION", System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		public string Name
		{
			get
			{
				return resourceManager.GetString("COMPONENTNAME", System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		public string Version
		{
			get
			{
				return resourceManager.GetString("COMPONENTVERSION", System.Globalization.CultureInfo.InvariantCulture);
			}
		}


		#endregion

		#region IPersistPropertyBag Members

		/// <summary>
		/// Gets class ID of component for usage from unmanaged code.
		/// </summary>
		/// <param name="classid">Class ID of the component.</param>
		public void GetClassID(out Guid classid)
		{
			classid = new System.Guid("7C0760D3-8706-4864-AB34-7D97EB7A82E5");
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public void InitNew()
		{
		}

		/// <summary>
		/// Loads configuration properties for the component
		/// </summary>
		/// <param name="pb">Configuration property bag</param>
		/// <param name="errlog">Error status</param>
		public void Load(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, Int32 errlog)
		{
			object val = null;
			val = this.ReadPropertyBag(pb, "IsToApplyTransformation");
			if ((val != null))
			{
				this.IsToApplyTransformation = ((bool)(val));
			}
			val = this.ReadPropertyBag(pb, "InternalProcessToHTML");
			if ((val != null))
			{
				this.InternalProcessToHTML = ((bool)(val));
			}
			val = this.ReadPropertyBag(pb, "XsltFilePath");
			if ((val != null))
			{
				this.XsltFilePath = ((string)(val));
			}
		}

		/// <summary>
		/// Saves the current component configuration into the property bag.
		/// </summary>
		/// <param name="pb">Configuration property bag.</param>
		/// <param name="fClearDirty">Not used.</param>
		/// <param name="fSaveAllProperties">Not used.</param>
		public void Save(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, Boolean fClearDirty, Boolean fSaveAllProperties)
		{
			this.WritePropertyBag(pb, "XsltFilePath", this.XsltFilePath);
			this.WritePropertyBag(pb, "IsToApplyTransformation", this.IsToApplyTransformation);
			this.WritePropertyBag(pb, "InternalProcessToHTML", this.InternalProcessToHTML);
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
			if (obj == null)
				throw new System.ArgumentNullException("No project system");

			IEnumerator enumerator = null;
			ArrayList strList = new ArrayList();

			try
			{
				if(this.IsToApplyTransformation == true)
					GetValidXsltPath(xsltPath);
			}
			catch (Exception e)
			{
				strList.Add(e.Message);
				enumerator = strList.GetEnumerator();
			}

			return enumerator;
		}

		#endregion

		#region IComponent members

		/// <summary>
		/// Implements IComponent.Execute method.
		/// </summary>
		/// <param name="pc">Pipeline context</param>
		/// <param name="inmsg">Input message.</param>
		/// <returns>Converted to HTML input message.</returns>
		/// <remarks>
		/// IComponent.Execute method is used to convert pdf messages
		/// to XML messages using provided Xslt file and the itextsharp component.
		/// </remarks>
		public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, 
			Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
		{
			Stream outStream = null;

			if (this.InternalProcessToHTML)
			{
				PDF2HTML pdfParser = new PDF2HTML();
				outStream = pdfParser.ExtractHTML(inmsg.BodyPart.Data);
			}
			else
			{
				PDF2XML pdfParser = new PDF2XML();
				outStream = pdfParser.ExtractXML(inmsg.BodyPart.Data);
			}
			outStream.Seek(0, SeekOrigin.Begin);
			if (IsToApplyTransformation == true)
				inmsg.BodyPart.Data = TransformMessage(outStream);
			else inmsg.BodyPart.Data = outStream;
			return inmsg;
		}

		#endregion

		#region Helper function

		/// <summary>
		/// Transforms XML message in input stream to HTML message
		/// </summary>
		/// <param name="stm">Stream with input XML message</param>
		/// <returns>Stream with output HTML message</returns>
		private Stream TransformMessage(Stream stm)
		{
			MemoryStream ms = null;
			string validXsltPath = null;
			
			try 
			{
				// Get the full path to the Xslt file
				validXsltPath = GetValidXsltPath(xsltPath);
				
				// Load transform
				XslTransform transform = new XslTransform();
				transform.Load(validXsltPath);
				
				//Load Xml stream in XmlDocument.
				XmlDocument doc = new XmlDocument();
				doc.Load(stm);
				
				//Create memory stream to hold transformed data.
				ms = new MemoryStream();
				
				//Preform transform
				transform.Transform(doc, null, ms, null);
				ms.Seek(0, SeekOrigin.Begin);
			}
			catch(Exception e) 
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				System.Diagnostics.Trace.WriteLine(e.StackTrace);
				throw e;
			}

			return ms;
		}

		/// <summary>
		/// Get a valid full path to a Xslt file
		/// </summary>
		/// <param name="path">Path provided by user in Pipeline Designer</param>
		/// <returns>The full path</returns>
		/// <remarks>
		/// If user provides absolute path then it is used as long as the file can be opened there
		/// If user provides just a name of file or relative path then we try to open a file in 
		/// [Install foder]\Pipeline Components
		/// </remarks>
		private string GetValidXsltPath(string path)
		{
			string validPath = path;

			if (!System.IO.File.Exists(path))
			{
                		RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0");
		                string InstallPath = string.Empty;
                
                		if (null != rk)
					InstallPath = (String)rk.GetValue("InstallPath");
                        
                		validPath = InstallPath + @"Pipeline Components\" + path;
				if (!System.IO.File.Exists(validPath))
				{
					throw new ArgumentException("The XSL transformation file " + path + " can not be found");
				}
			}	

			return validPath;
		}

		#endregion	
	}
}