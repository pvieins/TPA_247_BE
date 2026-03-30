using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Office.Interop.Word;
using System.Net.Mime;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using System.Reflection;
using System;
using PdfSharpCore.Drawing.BarCodes;
using PdfSharpCore.Pdf.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text;
using System.util;

namespace PVI.Helper
{
    public class PDFEdit
    {

        private readonly Serilog.ILogger _logger;
        public PDFEdit( Serilog.ILogger logger)
        {
            _logger = logger;
        }
        public bool ReplaceTextInPDF(string sourceFile, string descFile, Dictionary<string, string> listword, bool isTop)
        {
           return ReplaceText(listword, descFile, sourceFile, isTop);
        }
        public static void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();
            content = Regex.Replace(content, searchText, replaceText);
            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }
        private static void ChangeLocaText(string key_first, PdfContentByte cb, string value_first, string value_last, iTextSharp.text.Rectangle p)
        {
            if (key_first.Equals("LD_TAT"))
            {
                cb.ShowTextAligned(1, value_first, p.Left + 10 * 30, p.Top - 6, 0);
                cb.ShowTextAligned(1, value_last, p.Left + 10 * 30, p.Top - 30, 0);
            }
            else if (key_first.Equals("CB_TAT"))
            {
                cb.ShowTextAligned(1, value_first, p.Left + 10 * 18, p.Top - 4, 0);
                cb.ShowTextAligned(1, value_last, p.Left + 10 * 18, p.Top - 28, 0);
            }
            else if (key_first.Equals("GD_TAT"))
            {
                cb.ShowTextAligned(1, value_first, p.Left + 10 * 30, p.Top - 6, 0);
                cb.ShowTextAligned(1, value_last, p.Left + 10 * 30, p.Top - 30, 0);
            }
            else
            {
                cb.ShowTextAligned(1, value_first, p.Left + 10 * 43, p.Top - 4, 0);
                cb.ShowTextAligned(1, value_last, p.Left + 10 * 43, p.Top - 28, 0);
            }
        }

        private bool ReplaceText(Dictionary<string, string> listword, string outputFilePath, string inputFilePath, bool isTop)
        {
            try
            {
                using (Stream inputPdfStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Stream outputPdfStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (Stream outputPdfStream2 = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(inputPdfStream);
                            PdfStamper stamper = new PdfStamper(reader, outputPdfStream);
                            string key_first = listword.Keys.First();
                            string key_last = listword.Keys.Last();
                            string value_first = listword.Values.First();
                            string value_last = listword.Values.Last();
                            for (var i = 1; i <= reader.NumberOfPages; i++)
                            {
                                // If key_first.Equals("LD_TAT") Then
                                PdfReaderContentParser parser = new PdfReaderContentParser(reader);
                                LocationTextExtractionStrategyWithPosition strategy = parser.ProcessContent(i, new LocationTextExtractionStrategyWithPosition());
                                List<TextLocation> res = strategy.GetLocations();
                                var line = res.FirstOrDefault(x => x.Text.ToString().Contains(key_first));
                                if (line != null)
                                {
                                    iTextSharp.text.Rectangle p = new iTextSharp.text.Rectangle(line.X, line.Y + 5);
                                    PdfContentByte cb = stamper.GetOverContent(i);
                                    //string path = System.AppDomain.CurrentDomain.BaseDirectory;
                                    BaseFont bf = BaseFont.CreateFont("C:\\Windows\\Fonts\\times.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                    iTextSharp.text.Font fontbold = new iTextSharp.text.Font(bf, 10);
                                    cb.SetColorFill(BaseColor.BLACK);
                                    cb.SetFontAndSize(bf, 13);
                                    cb.BeginText();
                                    ChangeLocaText(key_first, cb, value_first, value_last, p);
                                    cb.EndText();
                                }
                            }
                            stamper.Close();
                        }
                    }
                }
                if(changeAndMoveFile(inputFilePath, outputFilePath)){
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ReplaceText " + ex.Message.ToString());
                return false;
            }
        }
        public bool changeAndMoveFile(string sourcePath, string descPath)
        {
            try
            {
                if (System.IO.File.Exists(sourcePath))
                {
                    string fileName = sourcePath;
                    string fileNameDesc = System.IO.Path.GetFileName(descPath);
                    string strNewFileName = fileName;
                    System.IO.File.Delete(sourcePath);
                    System.IO.File.Copy(descPath, fileName);
                    System.IO.File.Delete(descPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("changeAndMoveFile error: " + ex.Message.ToString());
                return false;
            }
        }
        public  Dictionary<string, string> ListKeyWord(string loai_cb, string ten_tat, string ten_daydu)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                switch (loai_cb)
                {
                    case "CB_TAT":
                        {
                            dic.Add("CB_TAT", ten_tat);
                            dic.Add("CB_DD", ten_daydu);
                            break;
                        }

                    case "TP_TAT":
                        {
                            dic.Add("TP_TAT", ten_tat);
                            dic.Add("TP_DD", ten_daydu);
                            break;
                        }

                    case "LD_TAT":
                        {
                            dic.Add("LD_TAT", ten_tat);
                            dic.Add("LD_DD", ten_daydu);
                            break;
                        }
                    case "VP_TAT":
                        {
                            dic.Add("VP_TAT", ten_tat);
                            dic.Add("VP_DD", ten_daydu);
                            break;
                        }
                    case "GD_TAT":
                        {
                            dic.Add("GD_TAT", ten_tat);
                            dic.Add("GD_DD", ten_daydu);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ListKeyWord " + ex.Message.ToString());
            }
            return dic;
        }
        public class RectAndText
        {
            public iTextSharp.text.Rectangle Rect;
            public string Text;

            public RectAndText(iTextSharp.text.Rectangle rect, string text)
            {
                this.Rect = rect;
                this.Text = text;
            }
        }

        public class MyLocationTextExtractionStrategy : LocationTextExtractionStrategy
        {
            public List<RectAndText> myPoints = new List<RectAndText>();
            public string TextToSearchFor { get; set; }
            public System.Globalization.CompareOptions CompareOptions { get; set; }

            public MyLocationTextExtractionStrategy(string textToSearchFor, System.Globalization.CompareOptions compareOptions = System.Globalization.CompareOptions.None)
            {
                this.TextToSearchFor = textToSearchFor;
                this.CompareOptions = compareOptions;
            }

            public override void RenderText(TextRenderInfo renderInfo)
            {
                base.RenderText(renderInfo);
                var startPosition = System.Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(renderInfo.GetText(), this.TextToSearchFor, this.CompareOptions);
                if (startPosition < 0)
                    return;

                var chars = renderInfo.GetCharacterRenderInfos().Skip(startPosition).Take(this.TextToSearchFor.Length).ToList();
                var firstChar = chars.First();
                var lastChar = chars.Last();
                var bottomLeft = firstChar.GetDescentLine().GetStartPoint();
                var topRight = lastChar.GetAscentLine().GetEndPoint();
                var rect = new iTextSharp.text.Rectangle(bottomLeft[Vector.I1], bottomLeft[Vector.I2], topRight[Vector.I1], topRight[Vector.I2]);
                this.myPoints.Add(new RectAndText(rect, this.TextToSearchFor));
            }
        }
        public class LocationTextExtractionStrategyWithPosition : LocationTextExtractionStrategy
        {
            private readonly List<TextChunk> locationalResult = new List<TextChunk>();
            private readonly ITextChunkLocationStrategy tclStrat;

            public LocationTextExtractionStrategyWithPosition() : this(new TextChunkLocationStrategyDefaultImp())
            {
            }

            public LocationTextExtractionStrategyWithPosition(ITextChunkLocationStrategy strat)
            {
                tclStrat = strat;
            }

            private bool StartsWithSpace(string str)
            {
                if (str.Length == 0)
                    return false;
                return str[0] == ' ';
            }

            private bool EndsWithSpace(string str)
            {
                if (str.Length == 0)
                    return false;
                return str[str.Length - 1] == ' ';
            }

            private List<TextChunk> filterTextChunks(List<TextChunk> textChunks, ITextChunkFilter filter)
            {
                if (filter == null)
                    return textChunks;

                var filtered = new List<TextChunk>();

                foreach (TextChunk textChunk in textChunks)
                {
                    if (filter.Accept(textChunk))
                        filtered.Add(textChunk);
                }

                return filtered;
            }

            public override void RenderText(TextRenderInfo renderInfo)
            {
                LineSegment segment = renderInfo.GetBaseline();

                if (renderInfo.GetRise() != 0)
                {
                    Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                    segment = segment.TransformBy(riseOffsetTransform);
                }

                TextChunk tc = new TextChunk(renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
                locationalResult.Add(tc);
            }

            public List<TextLocation> GetLocations()
            {
                // Dim filteredTextChunks = filterTextChunks(locationalResult, Nothing)
                // filteredTextChunks.Sort()
                TextChunk lastChunk = null/* TODO Change to default(_) if this is not a reference type */;
                var textLocations = new List<TextLocation>();

                foreach (TextChunk chunk in locationalResult)
                {
                    if (lastChunk == null)
                        textLocations.Add(new TextLocation()
                        {
                            Text = chunk.Text,
                            X = chunk.Location.StartLocation[0],
                            Y = chunk.Location.StartLocation[1]
                        });
                    else if (chunk.SameLine(lastChunk))
                    {
                        var text = "";
                        if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.Text) && !EndsWithSpace(lastChunk.Text))
                            text += ' ';
                        text += chunk.Text;
                        textLocations[textLocations.Count - 1].Text += text;
                    }
                    else
                        textLocations.Add(new TextLocation()
                        {
                            Text = chunk.Text,
                            X = chunk.Location.StartLocation[0],
                            Y = chunk.Location.StartLocation[1]
                        });

                    lastChunk = chunk;
                }

                return textLocations;
            }
        }
        public class TextLocation
        {
            public float X { get; set; }
            public float Y { get; set; }
            public string Text { get; set; }
        }
    }

}
