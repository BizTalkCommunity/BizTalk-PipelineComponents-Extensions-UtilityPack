using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.ComponentModel;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Data.OleDb;
using System.Data;


namespace BizTalk.PipelineComponents.ODBCTypesProcessor
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Decoder)]
    [System.Runtime.InteropServices.Guid("71A3FBC6-F5D6-4fd6-A17D-1664A58C7E68")]
    public class ODBCDecoder :
        IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        Microsoft.BizTalk.Component.Interop.IPersistPropertyBag,
        IComponentUI
    {
        private System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.ODBCTypesProcessor.Resource", Assembly.GetExecutingAssembly());

        #region enum

        public enum odbcType
        {
            Excel,
            DBF
        }

        #endregion

        #region Properties

        private string connectionString = null;
        [System.ComponentModel.Description("ODBC Connection String. Leave out the section -> Data Source=C:\\TEST.XLS; " +
                                           "\r\n" +  
                                           "This Section will be added by the Code." +
                                           "\r\n" +  
                                           "Examples of Connection Strings: " +
                                           "\r\n" +   
                                           "Excel -> Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;" +
                                           "\r\n" +
                                           "DBF and Foxpro -> Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=dBASE IV;" +
                                           "\r\n" +
                                           "For more examples please see"+
                                           "\r\n" + 
                                           "http://www.connectionstrings.com/")]
        public string ConnectionString
        { 
            get { return connectionString; }
            set { connectionString = value; }
        }

        private odbcType typeToProcess;
        [System.ComponentModel.Description("Type of file being Processed.")]
        public odbcType TypeToProcess
        {
            get { return typeToProcess; }
            set { typeToProcess = value; }
        }

        private string filter = null;
        [System.ComponentModel.Description("Filter for Select Statement" +
                                            "\r\n" +
                                           "Example: " +
                                            "\r\n" +
                                            "FirstName Like 'B%'" +
                                            "\r\n" +
                                            "This is Optional")] 
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }                                    


        private string sqlStatement = null;
        [System.ComponentModel.Description("Select Statement to Read ODBC Files." +
                                            "\r\n" +
                                           "Examples of Select Statements: " +
                                            "\r\n" +
                                            "Excel -> SELECT * FROM [sheet1$]" +
                                            "\r\n" +
                                            "DBF (Note do not need From. Table name will be part of DataSource -> Select * ")] 
        public string SqlStatement
        {
            get { return sqlStatement; }
            set { sqlStatement = value; }
        }

        private string tempDropFolderLocation = null;
        [System.ComponentModel.Description("Temp Folder for Dropping ODBC Files." +
                                            "\r\n" + 
                                           "For example C:\\Temp")]
        public string TempDropFolderLocation
        {
            get { return tempDropFolderLocation; }
            set { tempDropFolderLocation = value; }
        }

        

        private bool deleteTempMessages;
        [System.ComponentModel.Description("Delete Temp Messages after processing")]
        public bool DeleteTempMessages
        {
            get { return deleteTempMessages; }
            set { deleteTempMessages = value; }
        }

        private string fnamespace = null;
        [System.ComponentModel.Description("NameSpace for resultant XML Message, for example:" +
                                            "\r\n" +
                                            "http://Contoso.com")]
        public string NameSpace
        {
            get { return fnamespace; }
            set { fnamespace = value; }
        }

        private string rootNode = null;
        [System.ComponentModel.Description("Root Node Name for resultant XML Message, for example:" +
                                            "\r\n" +
                                            "AllEmployees")]
        public string RootNodeName
        {
            get { return rootNode; }
            set { rootNode = value; }
        }

        private string dataNode = null;
        [System.ComponentModel.Description("Data Node Name for resultant XML Message rows, for example:" +
                                            "\r\n" +
                                            "Employee")]
        public string DataNodeName
        {
            get { return dataNode; }
            set { dataNode = value; }
        }

        #endregion

        #region IBaseComponent Members

        [Browsable(false)]
        public string Description
        {
            get {
                return resourceManager.GetString("COMPONENTDESCRIPTION", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        [Browsable(false)]
        public string Name
        {
            get {
                return resourceManager.GetString("COMPONENTNAME", System.Globalization.CultureInfo.InvariantCulture); 
            }
        }
        
        [Browsable(false)]
        public string Version
        {
            get {
                return resourceManager.GetString("COMPONENTVERSION", System.Globalization.CultureInfo.InvariantCulture); 
            }
        }

        #endregion

        #region IPersistPropertyBag Members

        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classid">
        /// Class ID of the component
        /// </param>
        public void GetClassID(out System.Guid classid)
        {
            classid = new Guid("71A3FBC6-F5D6-4fd6-A17D-1664A58C7E68");
        }

        /// <summary>
        /// not implemented
        /// </summary>
        public void InitNew()
        {
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


        /// <summary>
        /// Loads configuration properties for the component
        /// </summary>
        /// <param name="pb">Configuration property bag</param>
        /// <param name="errlog">Error status</param>
        public virtual void Load(Microsoft.BizTalk.Component.Interop.IPropertyBag pb, int errlog)
        {
            object val = null;
            val = this.ReadPropertyBag(pb, "ConnectionString");
            if ((val != null))
            {
                this.ConnectionString = ((string)(val));
            }
            val = this.ReadPropertyBag(pb, "TempDropFolderLocation");
            if ((val != null))
            {
                this.TempDropFolderLocation = ((string)(val));
            }
            val = this.ReadPropertyBag(pb, "SqlStatement");
            if ((val != null))
            {
                this.SqlStatement = ((string)(val));
            }
            val = this.ReadPropertyBag(pb, "DeleteTempMessages");
            if ((val != null))
            {
                this.DeleteTempMessages = ((bool)(val));
            }
            val = this.ReadPropertyBag(pb, "TypeToProcess");
            if ((val != null))
            {
                this.TypeToProcess = ((odbcType)(val));
            }
            val = this.ReadPropertyBag(pb, "RootNodeName");
            if ((val != null))
            {
                this.RootNodeName = ((string)(val));
            }
            val = this.ReadPropertyBag(pb, "NameSpace");
            if ((val != null))
            {
                this.NameSpace = ((string)(val));
            }
            val = this.ReadPropertyBag(pb, "DataNodeName");
            if ((val != null))
            {
                this.DataNodeName = ((string)(val));
            }
            val = this.ReadPropertyBag(pb, "Filter");
            if ((val != null))
            {
                this.Filter = ((string)(val));
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
            this.WritePropertyBag(pb, "ConnectionString", this.ConnectionString);
            this.WritePropertyBag(pb, "TempDropFolderLocation", this.TempDropFolderLocation);
            this.WritePropertyBag(pb, "SqlStatement", this.SqlStatement);
            this.WritePropertyBag(pb, "DeleteTempMessages", this.DeleteTempMessages);
            this.WritePropertyBag(pb, "TypeToProcess", this.TypeToProcess);
            this.WritePropertyBag(pb, "RootNodeName", this.RootNodeName);

            this.WritePropertyBag(pb, "NameSpace", this.NameSpace);
            this.WritePropertyBag(pb, "DataNodeName", this.DataNodeName);
            this.WritePropertyBag(pb, "Filter", this.Filter);
        }

        #endregion

        #region IComponentUI Members

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

        #region IComponent Members

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
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pipelineContext, 
                    Microsoft.BizTalk.Message.Interop.IBaseMessage inputMsg)
        {
            System.Diagnostics.Debug.WriteLine("At top of Execute method for DBASE pipeline");
			IBaseMessagePart bodyPart = inputMsg.BodyPart;
            if (bodyPart != null)
            {
                try
                {

                    // First write the ODBC file to disk so can query it.
                    BinaryReader binaryReader = new BinaryReader(bodyPart.Data);
                    string folderName = this.TempDropFolderLocation;
                    if (folderName.Substring(folderName.Length - 1, 1) != "\\")
                        folderName += "\\";
                    string extension = (this.TypeToProcess == odbcType.Excel) ? ".xls" : ".dbf";
                    string filename = System.IO.Path.GetRandomFileName();
                    filename = filename.Remove(8);
                    filename += extension;
                    string folderNameAndFileName = folderName + filename;
                    FileStream fileStream = new FileStream(folderNameAndFileName, FileMode.CreateNew);
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                    binaryWriter.Write(binaryReader.ReadBytes(Convert.ToInt32(binaryReader.BaseStream.Length)));
                    binaryWriter.Close();
                    binaryReader.Close();
                    
                    // Create the Connection String for the ODBC File
                    string dataSource;
                    if (this.TypeToProcess == odbcType.Excel)
                        dataSource = "Data Source=" + folderNameAndFileName + ";";
                    else // dbf
                        dataSource = "Data Source=" + folderName + ";";
                    string odbcConnectionString = this.connectionString;
                    if (odbcConnectionString.Substring(odbcConnectionString.Length -1, 1) != ";")
                        odbcConnectionString += ";";
                    odbcConnectionString += dataSource;
                    OleDbConnection oConn = new OleDbConnection();
                    oConn.ConnectionString = odbcConnectionString;

                    // Create the Select Statement for the ODBC File
                    OleDbDataAdapter oCmd;
                    // Get the filter if there is one
                    string whereClause = "";
                    if (Filter.Trim() != "")
                        whereClause = " Where " + Filter.Trim();
                    if (this.TypeToProcess == odbcType.Excel)
                        oCmd = new OleDbDataAdapter(this.SqlStatement.Trim() + whereClause, oConn);
                    else // dbf
                        oCmd = new OleDbDataAdapter(this.SqlStatement.Trim() + " From " + filename + whereClause, oConn);
                    oConn.Open();
                    // Perform the Select statement from above into a dataset, into a DataSet. 
                    DataSet odbcDataSet = new DataSet();
                    oCmd.Fill(odbcDataSet, this.DataNodeName);
                    oConn.Close();
                    // Delete the message 
                    if (this.DeleteTempMessages)
                        System.IO.File.Delete(folderNameAndFileName);

                    // Write the XML From this DataSet into a String Builder
                    System.Text.StringBuilder stringBuilder = new StringBuilder();
                    System.IO.StringWriter stringWriter = new System.IO.StringWriter(stringBuilder);
                    odbcDataSet.Tables[0].WriteXml(stringWriter);

                    System.Xml.XmlDocument fromDataSetXMLDom = new System.Xml.XmlDocument();
                    fromDataSetXMLDom.LoadXml(stringBuilder.ToString());

                    // Create the Final XML Document. Root Node Name and Target Namespace
                    // come from properties set on the pipeline
                    System.Xml.XmlDocument finalMsgXmlDom = new System.Xml.XmlDocument();
                    System.Xml.XmlElement xmlElement;
                    xmlElement = finalMsgXmlDom.CreateElement("ns0", this.RootNodeName, this.NameSpace);
                    finalMsgXmlDom.AppendChild(xmlElement);

                    // Add the XML to the finalMsgXmlDom from the DataSet XML, 
                    // After this the XML Message will be complete
                    finalMsgXmlDom.FirstChild.InnerXml = fromDataSetXMLDom.FirstChild.InnerXml;

                    Stream strm = new MemoryStream();
                    // Save final XML Document to Stream
                    finalMsgXmlDom.Save(strm);
                    strm.Position = 0; 
                    bodyPart.Data = strm;
                    pipelineContext.ResourceTracker.AddResource(strm);
                }
                catch (System.Exception ex)
                {
                    throw ex;

                }

            }
            return inputMsg;
        }

        #endregion
    }
}