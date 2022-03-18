using System;
using System.IO;
using iTextSharp.text.pdf;
using System.Collections.Generic;

namespace BizTalk.PipelineComponents.PDF2Xml
{
    public class PDF2XML
    {
        /// BT = Beginning of a text object operator 
        /// ET = End of a text object operator
        /// Td move to the start of next line
        ///  5 Ts = superscript
        /// -5 Ts = subscript

        #region Properties

        /// <summary>
        /// The number of characters to keep, when extracting text.
        /// </summary>
        private static int _numberOfCharsToKeep = 15;

        #endregion

        #region ExtractXML

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>
        /// <param name="outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public Stream ExtractXML(string inFileName)
        {
            StreamReader reader = new StreamReader(inFileName);
            Stream html = ExtractXML(reader.BaseStream);

            reader.Close();
            return html;
        }

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>
        /// <param name="outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public Stream ExtractXML(string inFileName, string outFileName)
        {
            StreamReader reader = new StreamReader(inFileName);
            Stream html = ExtractXML(reader.BaseStream, outFileName);

            reader.Close();
            return html;
        }

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>
        /// <param name="outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public Stream ExtractXML(Stream inStream, string outFileName)
        {
            StreamWriter outFile = null;
            try
            {
                // Create a reader for the given PDF file
                PdfReader reader = new PdfReader(inStream);
                //outFile = File.CreateText(outFileName);
                outFile = new StreamWriter(outFileName, false, System.Text.Encoding.UTF8);

                outFile.Write(@"<ns0:GenericPDF xmlns:ns0=""http://BizTalk_Server_Project1.GenericPDF"">");

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    List<TextObject> lst = ExtractTextObjectsFromPDFBytes(reader.GetPageContent(page));
                    //enumTextObjects = lst.OrderBy(p => p.Y).ThenBy(p => p.X);
                    lst.Sort(new TextObjectComparer());
                    outFile.Write(String.Format("<Line><Value>{0}</Value>", lst[0].Value));
                    for (int i = 1; i < lst.Count; i++)
                    {
                        if (lst[i].Y < lst[i - 1].Y)
                        {
                            outFile.Write(String.Format("</Line><Line><Value>{0}</Value>", lst[i].Value));
                        }
                        else
                        {
                            outFile.Write(String.Format("<Value>{0}</Value>", lst[i].Value));
                        }
                    }
                    outFile.Write("</Line>");
                }
                outFile.Write("</ns0:GenericPDF>");

                return outFile.BaseStream;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (outFile != null) outFile.Close();
            }
        }

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>

        /// <returns>the stream</returns>
        public Stream ExtractXML(Stream inStream)
        {
            StreamWriter outFile = null;
            MemoryStream mem = new MemoryStream();

            try
            {
                // Create a reader for the given PDF file
                PdfReader reader = new PdfReader(inStream);
                //outFile = File.CreateText(outFileName);
                outFile = new StreamWriter(mem, System.Text.Encoding.UTF8);
                outFile.Write(@"<ns0:GenericPDF xmlns:ns0=""http://BizTalk_Server_Project1.GenericPDF"">");
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    List<TextObject> lst = ExtractTextObjectsFromPDFBytes(reader.GetPageContent(page));
                    //enumTextObjects = lst.OrderBy(p => p.Y).ThenBy(p => p.X);
                    lst.Sort(new TextObjectComparer());
                    outFile.Write(String.Format("<Line><Value>{0}</Value>", lst[0].Value));
                    for (int i = 1; i < lst.Count; i++)
                    {
                        if (lst[i].Y < lst[i - 1].Y)
                        {
                            outFile.Write(String.Format("</Line><Line><Value>{0}</Value>", lst[i].Value));
                        }
                        else
                        {
                            outFile.Write(String.Format("<Value>{0}</Value>", lst[i].Value));
                        }
                    }
                    outFile.Write("</Line>");
                }

                outFile.Write("</ns0:GenericPDF>");
                outFile.Flush();
                return outFile.BaseStream;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region ExtractTextObjectsFromPDFBytes

        /// <summary>
        /// This method processes an uncompressed Adobe (text) object 
        /// and extracts text.
        /// </summary>
        /// <param name="input">uncompressed</param>
        /// <returns></returns>
        private List<TextObject> ExtractTextObjectsFromPDFBytes(byte[] input)
        {
            if (input == null || input.Length == 0) return null;

            List<TextObject> lst = new List<TextObject>();
            TextObject txt = new TextObject();

            try
            {
                string value = "";
                string x = "";
                string y = "";

                // Flag showing if we are we currently inside a text object
                bool inTextObject = false;
                // Flag showing if the next character is literal 
                // e.g. '\\' to get a '\' character or '\(' to get '('
                bool nextLiteral = false;
                // () Bracket nesting level. Text appears inside ()
                int bracketDepth = 0;
                // Keep previous chars to get extract numbers etc.:
                char[] previousCharacters = new char[_numberOfCharsToKeep];
                for (int j = 0; j < _numberOfCharsToKeep; j++) previousCharacters[j] = ' ';

                for (int i = 0; i < input.Length; i++)
                {
                    char c = (char)input[i];

                    if (inTextObject)
                    {
                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 &&
                            CheckToken(new string[] { "ET" }, previousCharacters))
                        {
                            inTextObject = false;
                            value = "";
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                    txt.Value = value;
                                    lst.Add(txt);
                                    value = "";
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what. 
                                        // Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) ||
                                                ((c >= 128) && (c < 255)))
                                            {
                                                value += c.ToString();
                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Store the recent characters for 
                    // when we have to go back for a checking
                    for (int j = 0; j < _numberOfCharsToKeep - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[_numberOfCharsToKeep - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inTextObject = true;

                        txt = new TextObject();

                        //egen forech som tar fram x y
                        //Get X
                        i++;

                        while ((char)input[i] != ' ')
                        {
                            x += (char)input[i];
                            i++;
                        }
                        txt.X = Double.Parse(x);
                        x = "";
                        //Get Y
                        i++;

                        while ((char)input[i] != ' ')
                        {
                            y += (char)input[i];
                            i++;
                        }

                        txt.Y = Double.Parse(y);
                        y = "";

                        //Fill text object x and y then empty the variables
                    }
                }
                return lst;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region CheckToken

        /// <summary>
        /// Check if a certain 2 character token just came along (e.g. BT)
        /// </summary>
        /// <param name="search">the searched token</param>
        /// <param name="recent">the recent character array</param>
        /// <returns></returns>
        private bool CheckToken(string[] tokens, char[] recent)
        {
            foreach (string token in tokens)
            {
                if ((recent[_numberOfCharsToKeep - 3] == token[0]) &&
                    (recent[_numberOfCharsToKeep - 2] == token[1]) &&
                    ((recent[_numberOfCharsToKeep - 1] == ' ') ||
                    (recent[_numberOfCharsToKeep - 1] == 0x0d) ||
                    (recent[_numberOfCharsToKeep - 1] == 0x0a)) &&
                    ((recent[_numberOfCharsToKeep - 4] == ' ') ||
                    (recent[_numberOfCharsToKeep - 4] == 0x0d) ||
                    (recent[_numberOfCharsToKeep - 4] == 0x0a))
                    )
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}