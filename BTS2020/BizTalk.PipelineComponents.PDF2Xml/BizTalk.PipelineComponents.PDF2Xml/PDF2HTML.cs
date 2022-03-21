using System;
using System.IO;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Globalization;

namespace BizTalk.PipelineComponents.PDF2Xml
{
    /// <summary>
    /// Parses a PDF file and extracts the text from it.
    /// </summary>
    public class PDF2HTML 
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

        #region ExtractHTML

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>
        /// <param name="outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public Stream ExtractHTML(string inFileName)
        {
            StreamReader reader = new StreamReader(inFileName);
            Stream html = ExtractHTML(reader.BaseStream);

            reader.Close();
            return html;
        }

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>
        /// <param name="outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public Stream ExtractHTML(string inFileName, string outFileName)
        {
            StreamReader reader = new StreamReader(inFileName);
            Stream html = ExtractHTML(reader.BaseStream, outFileName);

            reader.Close();
            return html;
        }

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="inFileName">the full path to the pdf file.</param>
        /// <param name="outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public Stream ExtractHTML(Stream inStream, string outFileName)
        {
            StreamWriter outFile = null;
            try
            {
                // Create a reader for the given PDF file
                PdfReader reader = new PdfReader(inStream);
                //outFile = File.CreateText(outFileName);
                outFile = new StreamWriter(outFileName, false, System.Text.Encoding.UTF8);

                outFile.Write("<HTML><BODY><TABLE border='1'>");

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    List<TextObject> lst = ExtractTextObjectsFromPDFBytes(reader.GetPageContent(page));
                    //enumTextObjects = lst.OrderBy(p => p.Y).ThenBy(p => p.X);
                    lst.Sort(new TextObjectComparer());
                    outFile.Write(String.Format("<TR><TD>{0}</TD>", lst[0].Value));
                    for (int i = 1; i < lst.Count; i++)
                    {
                        if (lst[i].Y < lst[i - 1].Y)
                        {
                            outFile.Write(String.Format("</TR><TR><TD>{0}</TD>", lst[i].Value));
                        }
                        else
                        {
                            outFile.Write(String.Format("<TD>{0}</TD>", lst[i].Value));
                        }
                    }
                    outFile.Write("</TR>");
                }
                outFile.Write("</TABLE></BODY></HTML>");

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
        public Stream ExtractHTML(Stream inStream)
        {
            StreamWriter outFile = null;
            MemoryStream mem = new MemoryStream();

            try
            {
                // Create a reader for the given PDF file
                PdfReader reader = new PdfReader(inStream);
                //outFile = File.CreateText(outFileName);
                outFile = new StreamWriter(mem,  System.Text.Encoding.UTF8);
                outFile.Write("<HTML><BODY><TABLE border='1'>");
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    List<TextObject> lst = ExtractTextObjectsFromPDFBytes(reader.GetPageContent(page));
                    //enumTextObjects = lst.OrderBy(p => p.Y).ThenBy(p => p.X);
                    lst.Sort(new TextObjectComparer());
                    outFile.Write(String.Format("<TR><TD>{0}</TD>", lst[0].Value));
                    for (int i = 1; i < lst.Count; i++)
                    {
                        if (lst[i].Y < lst[i - 1].Y)
                        {
                            outFile.Write(String.Format("</TR><TR><TD>{0}</TD>", lst[i].Value));
                        }
                        else
                        {
                            outFile.Write(String.Format("<TD>{0}</TD>", lst[i].Value));
                        }
                    }
                    outFile.Write("</TR>");
                }

                outFile.Write("</TABLE></BODY></HTML>");
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
                string resultString = "<TABLE border=\"1\"><TR><TD>";
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
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                resultString += "\n\r";
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultString += "\n";
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        resultString += " ";
                                    }
                                }
                            }
                        }

                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 &&
                            CheckToken(new string[] { "ET" }, previousCharacters))
                        {

                            inTextObject = false;
                            //resultString += " ";
                            resultString += "</TD></TR><TD>";
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
                                                resultString += c.ToString();
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

                        //txt.X = Double.Parse(x);
                        txt.X = GetDouble(x, 0);
                        x = "";

                        //Get Y
                        i++;

                        while ((char)input[i] != ' ')
                        {

                            y += (char)input[i];
                            i++;

                        }

                        //txt.Y = Double.Parse(y);
                        txt.Y = GetDouble(y, 0);
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
            foreach(string token in tokens)
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

        #region Private

        private double GetDouble(string value, double defaultValue)
        {
            double result;

            // Try parsing in the current culture
            if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                // Then try in US english
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                // Then in neutral language
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        #endregion
    }
}