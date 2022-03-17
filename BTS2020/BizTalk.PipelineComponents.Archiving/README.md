# BizTalk Archive Pipeline Component
The BizTalk Archive Pipeline Component is based on the initial work of [Randy Paulo](https://randypaulo.wordpress.com/2012/02/13/biztalk-archiving-sql-and-file-documentation/) BizTalk Archiving SQL and File component that my team and I adjust and migrate to BizTalk Server 2020. This component was initially available on CodePlex which is unreachable since it was closed.

This component has the availability to archive incoming/outgoing messages from any adapters to either a folder (local, shared, network) or in a SQL Server database. 

The project will also include a SQL script file called CreateDatabase.sql that you can use to create the supported resources in SQL Server to be used with this component:
* Database called BizTalkArchiveDb with a table named Messages
* And stored procedure called InsMessages

This is the list of properties that you can set up on the archive pipeline component:

| Property Name  | Description  | Use for  | Sample Values |
| -------------  | -----------  | -------  | ------------------ |
| CompressionPassword  | The password that will be used in the zip file | File & SQL  |   P@ssw0rd  |
| DbConnProviderConnection | Provider | SQL | System.Data.SqlClient |
| DbConnStrDatabase | connection string | SQL  | Data Source=localhost;Initial Catalog=BizTalkArchiveDb;Integrated Security=SSPI; |
| DbFileExtensions | File extension to be used | SQL | .xml |
| DbPropList | This is a set of delimited values of name & namespace of message context properties that will be passed to the stored procedure to be used for custom logging. | SQL | ReceivedFileName;http://schemas.microsoft.com/BizTalk/2003/file-properties|ReceivedPortName;http://schemas.microsoft.com/BizTalk/2003/system-properties |
| DbSPName | Stored procedure name | SQL | Built-In: InsMessages |
| FileArchiveBackFolder | Folder / Shared Location / Network location wherein the files will be archived | File | \\archive\Test\Archive |
| FileArchiveFileName | Name of the archive file | File | %SourceFileName%_%datetime% |
| FileArchiveIsOverwriteFiles | Overwrite Flag | File | True |
| FileArchiveUserDomain | The domain name of user credentials | File | DOMAIN |
| FileArchiveUserName | User name | File | BizTalkUser |
| FileArchiveUserPwd | Password | File | P@ssword |
| IsArchiveToDb | Archive to database flag | SQL | True |
| IsArchiveToFile | Archive to file flag | File |True |
| IsCompressFile | Compress file flag | SQL & File | True |

THIS PIPELINE COMPONENT IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND.

# About Us
**Sandro Pereira** | [Blog](https://randypaulo.wordpress.com/2012/02/13/biztalk-archiving-sql-and-file-documentation/)

**Sandro Pereira** | [DevScope](http://www.devscope.net/) | MVP & MCTS BizTalk Server 2010 | [https://blog.sandro-pereira.com/](https://blog.sandro-pereira.com/) | [@sandro_asp](https://twitter.com/sandro_asp)

**Diogo Formosinho** | [DevScope](http://www.devscope.net/) | BizTalk Server Developer | [https://www.linkedin.com/in/diogo-formosinho-242b221a2/](https://www.linkedin.com/in/diogo-formosinho-242b221a2/)
