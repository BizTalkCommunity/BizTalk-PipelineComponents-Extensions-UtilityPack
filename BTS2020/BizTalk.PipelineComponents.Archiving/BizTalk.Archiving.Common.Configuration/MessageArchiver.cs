/*
 * Description  : Message Archiver
 * Author       : Randy Paulo
*/

using System;
using System.Data.Common;
using System.IO;
using System.Xml;
using System.Security;

namespace BizTalk.Archiving.Common.Configuration
{
    public class MessageArchiver : IMessageArchiveProvider
    {
        /// <summary>
        /// Method to archive the msg to a database by calling Stored Procedure
        /// The stored procedure should match the parameter names
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="providerName"></param>
        /// <param name="storedProcName">Name of Stored procedure</param>
        /// <param name="msgId">Message Id</param>
        /// <param name="msgStream">Message Body Stream</param>
        /// <param name="xmlProperties">Message Properties</param>
        /// <param name="size">Size uncompressed</param>
        /// <param name="isCompressed">Is Compressed</param>
        /// <param name="compressedSize">compressed size</param>
        /// <returns></returns>
        public virtual bool ArchiveToDb(string connectionString, string providerName, string storedProcName, Guid msgId, Stream msgStream, string xmlProperties, long size, bool isCompressed, long compressedSize)
        {
            DbProviderFactory database = DbProviderFactories.GetFactory(providerName);

            using (DbConnection dbConn = database.CreateConnection())
            {
                dbConn.ConnectionString = connectionString;

                using (DbCommand command = dbConn.CreateCommand())
                {
                    command.Connection = dbConn;
                    command.CommandText = storedProcName;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    DbParameter paramId = command.CreateParameter();
                    paramId.ParameterName = "MessageId";
                    paramId.DbType = System.Data.DbType.Guid; 
                    paramId.Value = msgId;

                    DbParameter paramProperty = command.CreateParameter();
                    paramProperty.ParameterName = "Property";
                    paramProperty.DbType = System.Data.DbType.Xml;
                    paramProperty.Value = xmlProperties;
                    
                    var memStream = new MemoryStream();
                    msgStream.Position = 0;
                    msgStream.CopyTo(memStream);

                    DbParameter paramBody = command.CreateParameter();
                    paramBody.ParameterName = "Body";
                    paramBody.DbType = System.Data.DbType.Binary;
                    paramBody.Value = memStream.ToArray();

                    DbParameter paramSize = command.CreateParameter();
                    paramSize.ParameterName = "Size";
                    paramSize.DbType = System.Data.DbType.Int64; 
                    paramSize.Value = size;

                    DbParameter paramIsCompressed = command.CreateParameter();
                    paramIsCompressed.ParameterName = "IsCompressed";
                    paramIsCompressed.DbType = System.Data.DbType.Boolean;
                    paramIsCompressed.Value = isCompressed;

                    DbParameter paramCompressedSize = command.CreateParameter();
                    paramCompressedSize.ParameterName = "CompressedSize";
                    paramCompressedSize.DbType = System.Data.DbType.Int64;
                    paramCompressedSize.Value = compressedSize;

                    command.Parameters.Add(paramId);
                    command.Parameters.Add(paramProperty);
                    command.Parameters.Add(paramBody);
                    command.Parameters.Add(paramSize);
                    command.Parameters.Add(paramCompressedSize);
                    command.Parameters.Add(paramIsCompressed);

                    dbConn.Open();
                    command.ExecuteNonQuery(); 
                }
            }
            return true;
        }

        /// <summary>
        /// Archive the File to a shared folder / folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="backupPath"></param>
        /// <param name="overwrite"></param>
        /// <param name="msgStream"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="isCompressed"></param>
        /// <param name="compressionPassword"></param>
        /// <returns></returns>
        public virtual bool ArchiveToFile(string fileName, string backupPath, bool overwrite, Stream msgStream, string userName, string password, string domain, bool isCompressed, string compressionPassword)
        {
            //Create Folder
            if(!new DirectoryInfo(backupPath).Exists)
            {
                try
                {
                    Directory.CreateDirectory(backupPath); 
                }
                catch
                { 
                    try
                    {
                        using(UNCAccessWithCredentials netUse = new UNCAccessWithCredentials())
                        {
                            if(backupPath.Substring(0,backupPath.Length) == @"\")
                            {
                                //Remove \ at the end
                                backupPath = backupPath.Substring(0,backupPath.Length-1);
                            }
                            bool isSuccess = netUse.NetUseWithCredentials(backupPath,userName,domain,password);
                            if(isSuccess)
                            {
                                Directory.CreateDirectory(backupPath); 
                            }
                        }
                    }
                    catch
                    {

                    }
                
                }
            }

            //try Saving using credentials of Adapter
            try
            {
                if (isCompressed)
                {
                    SaveStreamToFile(Utility.GetZipStream(fileName, msgStream, compressionPassword),
                              Path.Combine(backupPath, fileName.Replace(new FileInfo(fileName).Extension, ".zip")),
                              overwrite);
                }
                else
                {
                    SaveStreamToFile(msgStream, Path.Combine(backupPath, fileName), overwrite);
                }
            }
            catch(Exception exc)
            {
                msgStream.Position = 0;

                //try using NetUse
                using (UNCAccessWithCredentials netUse = new UNCAccessWithCredentials())
                {
                    bool isSuccess = netUse.NetUseWithCredentials(backupPath, userName, domain, password);
                    if (isSuccess)
                    {
                        string tmpfileName = string.Empty;
                        string actualFileName = string.Empty;
                        string destinationFileName = string.Empty;
                        try
                        {
                            //Save to temporary folder first
                            if (isCompressed)
                            {
                                actualFileName = fileName.Replace(new FileInfo(fileName).Extension, ".zip");
                                tmpfileName = Path.Combine(Path.GetTempPath(), actualFileName);
                                SaveStreamToFile(Utility.GetZipStream(fileName, msgStream, compressionPassword),
                                    tmpfileName,
                                    overwrite);
                            }
                            else
                            {
                                actualFileName = fileName;
                                tmpfileName = Path.Combine(Path.GetTempPath(), fileName);
                                SaveStreamToFile(msgStream, tmpfileName, overwrite);
                                
                            }

                            destinationFileName = Path.Combine(backupPath, actualFileName);
                            //Copy file
                            File.Copy(tmpfileName, destinationFileName, overwrite);
                            try
                            {
                                File.Delete(tmpfileName);
                            }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Cannot copy file '{0}' to '{1}', Username: '{2}', Domain: '{3}' due to: '{4}', Details: '{5}'", tmpfileName, destinationFileName, userName,domain, ex.Message, ex.ToString()));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Net Use failed, Cannot copy file '{0}' to '{1}', Username: '{2}', Domain: '{3}' due to: '{4}', Details: '{5}'", 
                            fileName, 
                            backupPath, 
                            userName, 
                            domain, 
                            exc.Message, 
                            exc.ToString()));
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Saves a Stream to File
        /// </summary>
        /// <param name="msgStream"></param>
        /// <param name="fileName"></param>
        protected virtual void SaveStreamToFile(Stream msgStream, string fileName, bool overWrite)
        {
            int bufferSize = 4096;
            byte[] buffer = new byte[4096];
            int numBytesRead = 0;
            
            using (FileStream fs = new FileStream(fileName, overWrite ? FileMode.Create: FileMode.CreateNew))
            {
                // Setup the stream writter and reader 
                BinaryWriter w = new BinaryWriter(fs);
                w.BaseStream.Seek(0,
                SeekOrigin.End);
                if (msgStream != null)
                {
                    msgStream.Seek(0,
                    SeekOrigin.Begin);
                    // Copy the data from the msg to the file 
                    int n = 0;
                    do
                    {
                        n = msgStream.Read(buffer, 0, bufferSize);
                        if (n == 0) // We're at EOF 
                            break;
                        w.Write(buffer, 0, n);
                        numBytesRead += n;
                    }
                    while (n > 0);
                }
                w.Flush();
            }
        }
    }
}
