using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;

namespace VirtualGreen.Samples.MultiDisassembler.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [System.Runtime.InteropServices.Guid("12345678-90AB-CDEF-FEDC-BA0987654321")]
    public class ExtractingXmlDisassembler : IBaseComponent, IPersistPropertyBag, IComponentUI, IDisassemblerComponent
    {
        private FFDasmComp ffDasmPC = new FFDasmComp();
        private Queue<IBaseMessage> messages = null;

        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            SetFFDasmProperties(ffDasmPC);
            ffDasmPC.Disassemble(pContext, pInMsg);
        }

        public IBaseMessage GetNext(IPipelineContext pContext)
        {
            if (messages == null)
            {
                messages = new Queue<IBaseMessage>();
                IBaseMessage msgS1 = null;
                while ((msgS1 = ffDasmPC.GetNext(pContext)) != null)
                {
                    XmlDasmComp xmlDasmPC = new XmlDasmComp();
                    SetXmlDasmProperties(xmlDasmPC);
                    xmlDasmPC.Disassemble(pContext, msgS1);
                    IBaseMessage msgS2 = null;
                    while ((msgS2 = xmlDasmPC.GetNext(pContext)) != null)
                    { messages.Enqueue(msgS2); }
                }
            }

            if (messages.Count > 0)
            { return messages.Dequeue(); }
            return null;
        }


        private void SetXmlDasmProperties(XmlDasmComp pc)
        { pc.AllowUnrecognizedMessage = true; }

        private void SetFFDasmProperties(FFDasmComp pc)
        { pc.ValidateDocumentStructure = false; }

        #region plumbing
        public void GetClassID(out Guid classID)
        { classID = Guid.Parse("12345678-90AB-CDEF-FEDC-BA0987654321"); }

        public void InitNew()
        { }

        public void Load(IPropertyBag propertyBag, int errorLog)
        { }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        { }

        public IntPtr Icon
        { get { return IntPtr.Zero; } }

        public System.Collections.IEnumerator Validate(object projectSystem)
        { return null; }

        public string Description
        { get { return "Nothing"; } }

        public string Name
        { get { return "ExtractingXmlDisassembler"; } }

        public string Version
        { get { return "1.0"; } }
        #endregion
    }
}