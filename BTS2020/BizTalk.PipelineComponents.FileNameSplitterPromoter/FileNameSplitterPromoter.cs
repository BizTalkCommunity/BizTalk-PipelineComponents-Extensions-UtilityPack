namespace BizTalk.PipelineComponents.FileNameSplitterPromoter
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
    [System.Runtime.InteropServices.Guid("b14d4e16-9dec-4c2d-98a0-62efb63e51da")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public class FileNameSplitterPromoter : Microsoft.BizTalk.Component.Interop.IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        
        private System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("BizTalk.PipelineComponents.FileNameSplitterPromoter.FileNameSplitterPromoter", Assembly.GetExecutingAssembly());
        
        private string _splitterChar;
        
        public string SplitterChar
        {
            get
            {
                return _splitterChar;
            }
            set
            {
                _splitterChar = value;
            }
        }
        
        private int _position;
        
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        
        private string _promoteNamespace;
        
        public string PromoteNamespace
        {
            get
            {
                return _promoteNamespace;
            }
            set
            {
                _promoteNamespace = value;
            }
        }
        
        private string _promoteProperty;
        
        public string PromoteProperty
        {
            get
            {
                return _promoteProperty;
            }
            set
            {
                _promoteProperty = value;
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
            classID = new System.Guid("b14d4e16-9dec-4c2d-98a0-62efb63e51da");
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
            val = this.ReadPropertyBag(propertyBag, "SplitterChar");
            if ((val != null))
            {
                this._splitterChar = ((string)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "Position");
            if ((val != null))
            {
                this._position = ((int)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "PromoteNamespace");
            if ((val != null))
            {
                this._promoteNamespace = ((string)(val));
            }
            val = this.ReadPropertyBag(propertyBag, "PromoteProperty");
            if ((val != null))
            {
                this._promoteProperty = ((string)(val));
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
            this.WritePropertyBag(propertyBag, "SplitterChar", this.SplitterChar);
            this.WritePropertyBag(propertyBag, "Position", this.Position);
            this.WritePropertyBag(propertyBag, "PromoteNamespace", this.PromoteNamespace);
            this.WritePropertyBag(propertyBag, "PromoteProperty", this.PromoteProperty);
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
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pContext, Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            string filename = string.Empty;
            bool validFile = true;
            string value = "";

            try
            {
                filename = pInMsg.Context.Read("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties").ToString();

                string[] listComments = null;
                listComments = filename.Split(Convert.ToChar(this.SplitterChar));
                value = listComments[this.Position];

                validFile = true;
            }
            catch
            {
                validFile = false;
            }

            if (validFile)
            {
                pInMsg.Context.Promote(this.PromoteProperty, this.PromoteNamespace, value);
            }

            return pInMsg;
        }
        #endregion
    }
}
