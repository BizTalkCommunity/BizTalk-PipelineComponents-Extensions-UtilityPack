# Message Archive Pipeline Component
The BizTalk Message Archive Pipeline Component is a component has the availability to archive incoming/outgoing messages from any adapters to a folder.

This is the list of properties that you can set up on the archive pipeline component:

| Property Name  | Description  | Sample Values |
| -------------  | -----------  | ------------- |
| OverwriteExistingFile  | Define if archive file is to be overwrite if already exists | true/false |
| ArchivingEnabled | Define if archive capabitities are enable or disables | true/false |
| ArchiveFilePath | Archive folder path. You can use macros to dinamically define the path  | C:\temp |
| ArchiveFilenameMacro | File name template. If emptry the source file name or Message Id will be used.  You can use macros to dinamically define the filename | .xml |
| AdditionalMacroIfExists | If file already exist and OverwriteExistingFile is set to false, a sufix can be added. If nothisng specify the MessageId will be used. You can use macros to dinamically define this sufix. | _%time% |
| OptimizeForPerformance | Setting to apply high performance on the archive | true/false |

## Available macros

* %datetime% : Coordinated Universal Time (UTC) date time in the format YYYY-MM-DDThhmmss (for example, 1997-07-12T103508).
* %MessageID% : Globally unique identifier (GUID) of the message in BizTalk Server. The value comes directly from the message context property BTS.MessageID.
* %FileName% : Name of the file from which the File adapter read the message. The file name includes the extension and excludes the file path, for example, Sample.xml. When substituting this property, the File adapter extracts the file name from the absolute file path stored in the FILE.ReceivedFileName context property. If the context property does not have a value the MessageId will be used.
* %FileNameWithoutExtension% : Same of the %FileName% but without extension
* %FileNameExtension% : Same of the %FileName% but in this case only the extension with dot: .xml
* %Day% : UTC Current day
* %Month% : UTC Current month
* %Year% : UTC Current year
* %time% : UTC time in the format hhmmss.
* %ReceivePort% : Receive port name
* %ReceiveLocation% : Receive location name
* %SendPort% : Send port
* %InboundTransportType% : Inbound Transport Type
* %InterchangeID% : InterchangeID

THIS PIPELINE COMPONENT IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND.

# About Us
**Sandro Pereira** | [DevScope](http://www.devscope.net/) | MVP & MCTS BizTalk Server 2010 | [https://blog.sandro-pereira.com/](https://blog.sandro-pereira.com/) | [@sandro_asp](https://twitter.com/sandro_asp)