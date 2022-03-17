using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
/*
 * Description  : Utility Class
 * Author       : Randy Paulo
*/

namespace BizTalk.Archiving.Common
{
    public class Utility
    {
        /// <summary>
        /// Returns the Zipped Stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="input"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Stream GetZipStream(string fileName, Stream input, string password)
        {
            MemoryStream output = new MemoryStream();
            using (var zip = new ZipFile())
            {
                if (!string.IsNullOrEmpty(password))
                    zip.Password = password;

                zip.AddEntry(fileName, input);
                zip.Save(output);
            }
            return output; 
        }
    }
}