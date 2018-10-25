# BizTalk Pipeline Components Extensions Utility Pack
BizTalk Pipeline Components Extensions Utility Pack is a set of custom pipeline components (libraries) with several custom pipeline components that can be used in received and sent pipelines, which will provide an extension of BizTalk out-of-the-box pipeline capabilities.

![BizTalk Pipeline Components Extensions Utility Pack](media/BizTalk-PipelineComponents-Extensions-UtilityPack.png)

## Content-Based Routing Operations
### CBRIdocOperationPromotionEncode

* Content Based Routing Component to promote IDOC Operation property.
  * This component requires one configuration that is the MessageType string to be ignored. Then it will take the last string (word) from the MessageType Message Context Property and promote it to the Operation Message Context Property.

### CBROperationPromotionEncode

* Content Based Routing Component to promote Operation property.
  * This component doesn't requires any configuration. Then it will take the value (word) which lies ahead of the cardinal (#) from the MessageType message context property and promote it to the Operation Message Context Property.

### Carry SOAPHeader To WCF-BasicHttp Pipeline Component

* Content Based Routing Component to carry forward the received SOAP Header to the ougoing message.
  * This component will access the original SOAP Header received in the source message and add it to the destination messages: OutboundCustomHeaders property (used by the WCF-BasicHTTP Adapter)
  * This component requires one configuration that is the SOAPHeaderName. A string that descrives the custom SOAP Header name to be copied to the ougoing message.
  
## Compression Operations
### Multi-Part Message Attachments Zipper Pipeline Component

* The BizTalk Multi-Part Message Attachments Zipper is a pipeline component for BizTalk Server which can be used in a send pipeline and is intended to replace all attachments of a multi-part message for its zipped equivalent.
* The capabilities are similar to those available in compression software such as WinZip or 7-zip:
  * Attachments Compression – Extracts, in a send pipeline, all message parts include in a multi-part message that are not included in the message body (Message Body Part = False), compresses it and attaches the compressed attachment back to the message..
  * This component requires one configuration that is the FileExtension where you can specified if you want for example a .zip or .gz file.

### Zip Pipeline Component

* The Zip pipeline component is a pipeline component for BizTalk Server which can be used in a send pipeline (encode stage) and is intended to compress (zip/gzip) outgoing messages..
* The capabilities are similar to those available in compression software such as WinZip or 7-zip:
  * This component requires two configuration that is the "FileExtension" where you can specified if you want for example a .zip or .gz file and "Enabled" that is a true or false value to activate the compression.
  
  ### UnZip File Pipeline Component

* This UnZip File Pipeline Component for BizTalk Server which can be used in a Received pipeline (Disassemble stage) and it allows you to receive a compress (zip/gzip) file and extract its contents into different XML messages.
* The capabilities are similar to those available in compression software such as WinZip or 7-zip:
  * This component doesn't requires any configurations.
  
## XML Namespace Operations
### Remove Xml Namespace Pipeline Component

* The RemoveXmlNamespace is a pipeline component for BizTalk Server made by Johan Hedberg which can be used to remove Xml namespaces from Xml documents inside custom pipelines.
  * This have the availability to transform this <ns0:Blah xmlns:ns0="http://RemoveXmlNamespace.BTS.BlahMessage"> into this <Blah>:

## Deploying Pipeline Components
All the .NET pipeline component assemblies (native and custom) must be located in the <installation directory>\Pipeline Components folder to be executed by the server. If the pipeline with a custom component will be deployed across several servers, the component's binaries must be present in the specified folder on every server.

You do not need to add a custom pipeline component to be used by the BizTalk Runtime to the Global Assembly Cache (GAC).

To know more about Deploying Pipeline Components, please see: [Deploying Pipeline Components](https://docs.microsoft.com/en-us/biztalk/core/deploying-pipeline-components)
