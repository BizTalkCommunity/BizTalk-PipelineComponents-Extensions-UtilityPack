# Multi-Part Message Attachments Zipper Pipeline Component for BizTalk 2010

# Introduction
The BizTalk Multi-Part Message Attachments Zipper is a pipeline component for BizTalk Server 2010 which can be used in a send pipeline and is intended to replace all attachments of a multi-part message for its zipped equivalent.

The capabilities are similar to those available in compression software such as WinZip or 7-zip:
* Attachments Compression – Extracts, in a send pipeline, all message parts include in a multi-part message that are not included in the message body (Message Body Part = False), compresses it and attaches the compressed attachment back to the message.

# Requirements
The Multipart Message Attachments Zipper will work with:
* BizTalk Server 2010
* .NET Framework 4.0
* No compression/decompression software needs to be installed in the BizTalk Server

# Description

This component extracts, in a send pipeline, all message parts include in a multi-part message that are not included in the message body (Message Body Part = False), compresses it and attaches the compressed attachment back to the message.

This is the code inside the Execute method:  

    
    public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg) 
	{            
		int PartCount = inmsg.PartCount; 
		string BodyPartName = inmsg.BodyPartName; 
	 
		try 
		{ 
			for (int i = 0; i < inmsg.PartCount; i++) 
			{ 
				string PartName; 
				IBaseMessagePart CurrentPart = inmsg.GetPartByIndex(i, out PartName); 
	 
				if (!PartName.Equals(BodyPartName)) 
				{ 
					Stream CurrentPartSource = CurrentPart.GetOriginalDataStream(); 
					byte[] CurrentPartBuffer = new byte[CurrentPartSource.Length]; 
					CurrentPartSource.Read(CurrentPartBuffer, 0, CurrentPartBuffer.Length); 
							 
					byte[] CompressedPartBuffer; 
					using (MemoryStream TempStream = new MemoryStream()) 
					{ 
						using (GZipStream CompressedStream = new GZipStream(TempStream, CompressionMode.Compress, true)) 
						{ 
							CompressedStream.Write(CurrentPartBuffer, 0, CurrentPartBuffer.Length); 
							CompressedStream.Close(); 
						} 
						CompressedPartBuffer = TempStream.ToArray(); 
					} 
	 
					MemoryStream TempCompressedStream = new MemoryStream(CompressedPartBuffer); 
					inmsg.GetPartByIndex(i, out PartName).Data = TempCompressedStream; 
					string PropertyName = "FileName"; 
					string PropertySchema = "http://schemas.microsoft.com/BizTalk/2003/mime-properties"; 
					string SourcePartName = inmsg.GetPartByIndex(i, out PartName).PartProperties.Read(PropertyName, 
						PropertySchema).ToString(); 
					SourcePartName += ".gz"; 
	 
					inmsg.GetPartByIndex(i, out PartName).PartProperties.Write("FileName", PropertySchema, SourcePartName);                         
				} 
			} 
		} 
		catch (Exception Exception) 
		{ 
			//Exception.Message;  
		} 
	 
		return inmsg;     
	}
    

To use this pipeline component in your projects just copy the “BizTalk.PipelineComponent.MultiPartMsg.ZipAttach.dll” file into “Pipeline Components“ folder that exists in BizTalk Server Installation directory:  “..\Program Files (x86)\Microsoft BizTalk Server 2010\Pipeline Components”.

# Read more about it
You can read more about this topic here: [BizTalk Multi-part Message Attachments Zipper Pipeline Component is now available](https://blog.sandro-pereira.com/2012/06/26/biztalk-multi-part-message-attachments-zipper-pipeline-component-is-now-available/)

# About Me
**Sandro Pereira** | [DevScope](http://www.devscope.net/) | MVP & MCTS BizTalk Server 2010 | [https://blog.sandro-pereira.com/](https://blog.sandro-pereira.com/) | [@sandro_asp](https://twitter.com/sandro_asp)

