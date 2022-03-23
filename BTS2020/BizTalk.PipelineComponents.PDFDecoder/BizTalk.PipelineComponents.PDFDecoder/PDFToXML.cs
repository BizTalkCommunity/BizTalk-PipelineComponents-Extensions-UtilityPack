using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalk.PipelineComponents.PDFDecoder
{
    public class PDFToXML
    {
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
                outFile.Write(@"<ns0:PDFDocument xmlns:ns0=""http://internal.dvs.components.pdf"">");

                int numPages = reader.NumberOfPages;

                for (int i = 1; i <= numPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());

                    string[] words = text.Split('\n');
                    for (int j = 0, len = words.Length; j < len; j++)
                    {
                        outFile.Write(String.Format("<Line><Value>{0}</Value></Line>", Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]))));
                    }
                }

                outFile.Write("</ns0:PDFDocument>");
                outFile.Flush();
                return outFile.BaseStream;
            }
            catch(Exception e)
            {
                throw  e;
            }
        }
    }
}
