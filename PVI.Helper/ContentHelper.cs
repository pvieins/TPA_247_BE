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
using static iTextSharp.text.pdf.events.IndexEvents;

namespace PVI.Helper
{
    public class ContentHelper
    {
        private readonly DownloadSettings _downloadSettings;
        private readonly Word2PdfSettings _word2PdfSettings;
        private readonly Serilog.ILogger _logger;
        public ContentHelper(IOptions<DownloadSettings> downloadSettings, IOptions<Word2PdfSettings> word2PdfSettings, Serilog.ILogger logger)
        {
            _downloadSettings = downloadSettings.Value;
            _word2PdfSettings = word2PdfSettings.Value;
            _logger = logger;
        }
        public ContentHelper(Serilog.ILogger logger)
        {
            _logger = logger;
        }
        public static string EndcodeTo64(string toEncode)
        {
            try
            {
                byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
                string returnValue = Convert.ToBase64String(toEncodeAsBytes);
                return returnValue;
            }
            catch (Exception ex)
            {
                return toEncode;
            }
        }

        public static string formatMoney(string value)
        {
            try
            {
                string temp = string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length > 3)
                    {
                        while (true)
                        {
                            string number1 = "." + value.Remove(0, value.Length - 3);
                            temp = number1 + temp;
                            value = value.Substring(0, value.Length - 3);
                            if (value.Length <= 3)
                            {
                                temp = value + temp;
                                break;
                            }
                        }

                        return temp;
                    }

                }

                return value;
            }
            catch (Exception ex)
            {
                return value;
            }
        }

        public static string formatMoney_new2020(string value)
        {
            try
            {
                value = value.Replace(" ", "");
                string temp = string.Empty;


                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length > 3)
                    {
                        while (true)
                        {
                            //string number1 = "." + value.Remove(0, value.Length - 3);
                            string number1 = " " + value.Remove(0, value.Length - 3);
                            temp = number1 + temp;
                            value = value.Substring(0, value.Length - 3);
                            if (value.Length <= 3)
                            {
                                temp = value + temp;
                                break;
                            }
                        }

                        return temp.Replace(" ", ".");
                    }
                }

                return value.Replace(" ", ".");
            }
            catch (Exception ex)
            {
                return value;
            }
        }
        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace(" ", "-").ToLower().Replace(":", "").Replace("!", "").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("*", "").Replace("(", "").Replace(")", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(";", "").Replace(".", "").Replace(",", "").Replace("/", "").Replace("?", "").Replace("|", "").Replace("'", "").Replace("'", "").Replace("\"", "");
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ShowMessage(string strMessage)
        {
            string strScript = "<script language=JavaScript>";
            strScript = strScript + "alert('" + strMessage.Replace("'", " ") + "');";
            strScript = strScript + "</script>";

            return strScript;
        }


        public static string HttpGetData(string url)
        {
            try
            {
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (request.HaveResponse)
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader responseReader = new StreamReader(responseStream, Encoding.UTF8);
                    string responseString = responseReader.ReadToEnd();
                    return responseString;
                }

                return string.Empty;
            }

            catch (Exception ex)
            {
                return "-1|" + ex.ToString();
            }
        }


        public static string PostData(string postData, string url)
        {
            try
            {

                string post_data = postData;
                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);
                request.Method = "POST";
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                //Cap nhat user, password authen API truoc khi post data                
                request.ContentType = "text/html; charset=utf-8";
                //request.ContentType = "application/json";
                request.ContentLength = postBytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Flush();
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (request.HaveResponse)
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader responseReader = new StreamReader(responseStream, Encoding.UTF8);
                    string responseString = responseReader.ReadToEnd();
                    return responseString;
                }

                return string.Empty;
            }

            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string MD5(string s)
        {
            byte[] originalBytes;
            byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            //            originalBytes = ASCIIEncoding.Default.GetBytes(s);
            originalBytes = Encoding.Default.GetBytes(s);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).ToLower().Replace("-", "");
        }
        public DownloadFileResult DownloadFile(string dataPost)
        {
            DownloadFileContent objData = JsonConvert.DeserializeObject<DownloadFileContent>(dataPost);


            DownloadFileResult result = new DownloadFileResult();
            DownloadFileAPI objDownload = new DownloadFileAPI();
            string filePath = objData.FilePath;
            string extension = Path.GetExtension(filePath);
            var contentType = GetMimeType(extension);

            objDownload.FilePath = filePath;

            string dataDownload = JsonConvert.SerializeObject(objDownload);
            string resultDownload = ContentHelper.PostData(dataDownload, _downloadSettings.DownloadServer);

            ResultDownloadAPI objResultAPI = (ResultDownloadAPI)JsonConvert.DeserializeObject<ResultDownloadAPI>(resultDownload);
            if (objResultAPI.Status == 1)
            {
                result.Status = "00";
                result.Message = "thanh cong";
                result.MimeType = contentType;
                result.Data = objResultAPI.FileSize;

            }
            else
            {
                result.Status = "-500";
                result.Message = "File khong ton tai hoac khong co quyen truy cap";
            }
            return result;



        }


        public string GetMimeType(string extension)
        {
            var mimeTypes = new Dictionary<string, string>
    {
        {".txt", "text/plain"},
        {".pdf", "application/pdf"},
        {".doc", "application/vnd.ms-word"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".csv", "text/csv"}
    };

            return mimeTypes.ContainsKey(extension) ? mimeTypes[extension] : "application/octet-stream";
        }


        public static void FindAndReplace(Microsoft.Office.Interop.Word.Application WordApp, object findText, object replaceWithText)
        {
            try
            {


                object matchCase = true;
                object matchWholeWord = true;
                object matchWildCards = false;
                object matchSoundsLike = false;
                object nmatchAllWordForms = false;
                object forward = true;
                object format = false;
                object matchKashida = false;
                object matchDiacritics = false;
                object matchAlefHamza = false;
                object matchControl = false;
                object read_only = false;
                object visible = true;
                object replace = 2;
                object wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindContinue;
                object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
                WordApp.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord, ref matchWildCards, ref matchSoundsLike,
                ref nmatchAllWordForms, ref forward,
                ref wrap, ref format, ref replaceWithText,
                ref replaceAll, ref matchKashida,
                ref matchDiacritics, ref matchAlefHamza,
                ref matchControl);
            }
            catch (Exception ex)
            {

            }
        }

        public static void FindAndReplaceCRM(Microsoft.Office.Interop.Word.Application WordApp, string findText, string replaceWithText)
        {
            try
            {
                const int maxLength = 255;
                if (replaceWithText.Length <= maxLength)
                {
                    // If under 255 chars, do a single replacement
                    Find find = WordApp.Selection.Find;
                    find.ClearFormatting();
                    find.Text = findText;
                    find.Replacement.ClearFormatting();
                    find.Replacement.Text = replaceWithText;

                    find.Execute(
                        Replace: WdReplace.wdReplaceAll,
                        MatchCase: true,
                        MatchWholeWord: true,
                        MatchWildcards: false,
                        MatchSoundsLike: false,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: WdFindWrap.wdFindContinue,
                        Format: false
                    );
                }
                else
                {
                    // Split the text into chunks and replace incrementally
                    string tempPlaceholder = Guid.NewGuid().ToString(); // Unique temporary placeholder
                    Find find = WordApp.Selection.Find;
                    find.ClearFormatting();
                    find.Text = findText;
                    find.Replacement.ClearFormatting();
                    find.Replacement.Text = tempPlaceholder;

                    // Step 1: Replace findText with a temporary placeholder
                    find.Execute(
                        Replace: WdReplace.wdReplaceAll,
                        MatchCase: true,
                        MatchWholeWord: true,
                        MatchWildcards: false,
                        MatchSoundsLike: false,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: WdFindWrap.wdFindContinue,
                        Format: false
                    );

                    // Step 2: Replace the placeholder with chunks of the long text
                    int chunkSize = maxLength;
                    for (int i = 0; i < replaceWithText.Length; i += chunkSize)
                    {
                        string chunk = replaceWithText.Substring(i, Math.Min(chunkSize, replaceWithText.Length - i));
                        find.Text = i == 0 ? tempPlaceholder : ""; // Replace placeholder only on first chunk
                        find.Replacement.Text = chunk;
                        find.Execute(
                            Replace: i == 0 ? WdReplace.wdReplaceOne : WdReplace.wdReplaceNone, // Replace only first occurrence of placeholder
                            Forward: true,
                            Wrap: WdFindWrap.wdFindContinue
                        );

                        // Move cursor to end of inserted chunk for next insertion
                        if (i + chunkSize < replaceWithText.Length)
                        {
                            WordApp.Selection.MoveRight(Unit: WdUnits.wdCharacter, Count: chunk.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FindAndReplaceCRM: " + ex.Message);
            }
        }
        public static void FindAndReplaceFooter(Microsoft.Office.Interop.Word.Sections docSection, string findText, string replaceWithText)
        {
            try
            {


                object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
                object missing = System.Reflection.Missing.Value;
                foreach (Microsoft.Office.Interop.Word.Section section in docSection)
                {
                    Microsoft.Office.Interop.Word.Range footerRange = section.Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    footerRange.Find.Text = findText;
                    footerRange.Find.Replacement.Text = replaceWithText;
                    footerRange.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceAll, ref missing, ref missing, ref missing, ref missing);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static bool Word2PDF(object Source, object Target, Microsoft.Office.Interop.Word.Application MSdoc)
        {
            //Creating the instance of Word Application
            object Unknown = Type.Missing;
            // Microsoft.Office.Interop.Word.Application MSdoc;
            if (MSdoc == null) MSdoc = new Microsoft.Office.Interop.Word.Application();

            try
            {
                MSdoc.Visible = false;
                MSdoc.Documents.Open(ref Source, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                MSdoc.Application.Visible = false;
                MSdoc.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMinimize;

                object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                MSdoc.ActiveDocument.SaveAs(ref Target, ref format,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                       ref Unknown, ref Unknown);



                return true;

            }
            catch (Exception e)
            {

            }

            return false;
        }

        public static void CreateTableInWordDocument_Serial_CNKH_All(Microsoft.Office.Interop.Word._Document objDoc, List<CustomResultModel> lstData_KBTT)
        {
            try
            {
                //int targetPageNumber = 2;

                //// Go to the desired page
                //objDoc.GoTo(Microsoft.Office.Interop.Word.WdGoToItem.wdGoToPage, Microsoft.Office.Interop.Word.WdGoToDirection.wdGoToAbsolute, targetPageNumber);

                //// Set the starting point for your table
                //object oStart = objDoc.Bookmarks.get_Item("\\Page").Range.End;
                //object oEnd = objDoc.Bookmarks.get_Item("\\Page").Range.End;

                //Microsoft.Office.Interop.Word.Range tableStart = objDoc.Range(ref oStart, ref oEnd);
                //string strSelect = " Select * from tcd_bht_seri where tcd_bht_seri.fr_key=" + dPr_key;
                //DataSet ds_the = Ws_pias.SelectSQL_TCD(DateTime.Now.Year.ToString(), strSelect, "nvu_bht_the");
                if (lstData_KBTT != null)
                {

                    if (lstData_KBTT.Count > 0)
                    {
                        int iRowCount = lstData_KBTT.Count();
                        int iColCount = 6;

                        int i = 1;
                        object oMissing = System.Reflection.Missing.Value;
                        object oEndOfDoc = "TinhToanTraTienBh";

                        int check_book = objDoc.Bookmarks.Count;
                        if (check_book < 1)
                        {
                            return;
                        }

                        Microsoft.Office.Interop.Word.Table objTable;
                        Microsoft.Office.Interop.Word.Range wrdRng = objDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        objTable = objDoc.Tables.Add(wrdRng, iRowCount + 1, iColCount, ref oMissing, ref oMissing);
                        //objTable = objDoc.Tables.Add(tableStart, iRowCount + 1, iColCount, ref oMissing, ref oMissing);
                        objTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        objTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;

                        int check = 1;
                        foreach (var objData in lstData_KBTT)
                        {
                            i++;

                            if (check == 1)
                            {
                                //Add ten cot
                                objTable.Cell(1, 1).Range.Text = "Tên quyền lợi";
                                objTable.Cell(1, 2).Range.Text = "Mức trách nhiệm";
                                objTable.Cell(1, 3).Range.Text = "Số tiền YCBT";
                                objTable.Cell(1, 4).Range.Text = "Số tiền TCBT";
                                objTable.Cell(1, 5).Range.Text = "Số tiền BT";
                                objTable.Cell(1, 6).Range.Text = "MTN còn lại";

                                //objTable.Cell(1, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            }

                            check = 2;


                            objTable.Cell(i, 1).Range.Text = objData.TenDieutri.ToString();
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                            objTable.Cell(i, 2).Range.Text = Math.Round((decimal)objData.MtnGtbh).ToString("N0", new CultureInfo("is-IS"));
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;


                            objTable.Cell(i, 3).Range.Text = Math.Round((decimal)objData.SoTienyc).ToString("N0", new CultureInfo("is-IS"));
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;

                            objTable.Cell(i, 4).Range.Text = Math.Round((decimal)objData.SoTientcbt).ToString("N0", new CultureInfo("is-IS"));
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Text = Math.Round((decimal)objData.SoTienp).ToString("N0", new CultureInfo("is-IS"));
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            if (objData.MtnGtbh == 0)
                            {
                                objData.SoTienCl = 0;
                            }
                            objTable.Cell(i, 6).Range.Text = Math.Round((decimal)objData.SoTienCl).ToString("N0", new CultureInfo("is-IS"));
                            objTable.Cell(i, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                        }


                        // Add a new row for totals
                        Microsoft.Office.Interop.Word.Row totalRow = objTable.Rows.Add(ref oMissing);
                        totalRow.Range.Font.Bold = 1;

                        // Add headers for the total row
                        totalRow.Cells[1].Range.Text = "Tổng cộng";
                        totalRow.Cells[1].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                        // Calculate and add the sum for each column to the total row
                        decimal? sumColumn2 = 0, sumColumn3 = 0, sumColumn4 = 0, sumColumn5 = 0, sumColumn6 = 0;

                        foreach (var objData in lstData_KBTT)
                        {
                            sumColumn2 += objData.MtnGtbh;
                            sumColumn3 += objData.SoTienyc;
                            sumColumn4 += objData.SoTientcbt;
                            sumColumn5 += objData.SoTienp;
                            sumColumn6 += objData.SoTienCl;
                        }

                        //totalRow.Cells[1].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        //totalRow.Cells[2].Range.Text = Math.Round((decimal)sumColumn2).ToString("N0", new CultureInfo("is-IS"));
                        totalRow.Cells[2].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                        totalRow.Cells[3].Range.Text = Math.Round((decimal)sumColumn3).ToString("N0", new CultureInfo("is-IS"));
                        totalRow.Cells[3].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                        totalRow.Cells[4].Range.Text = Math.Round((decimal)sumColumn4).ToString("N0", new CultureInfo("is-IS")); ;
                        totalRow.Cells[4].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                        totalRow.Cells[5].Range.Text = Math.Round((decimal)sumColumn5).ToString("N0", new CultureInfo("is-IS"));
                        totalRow.Cells[5].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                        //totalRow.Cells[6].Range.Text = Math.Round((decimal)sumColumn6).ToString("N0", new CultureInfo("is-IS"));
                        //totalRow.Cells[6].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                        objTable.Rows[1].Range.Font.Bold = 1;
                        objTable.Rows[1].Range.Font.Name = "Times New Roman";
                        objTable.Rows[1].Range.Font.Size = (float)10;
                        objTable.Rows[1].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;


                        Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        Single firstColAutoWidth = 120;
                        objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol3 = objTable.Columns[3];
                        Single firstColAutoWidth3 = 70;
                        firstCol3.SetWidth(firstColAutoWidth3, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public DownloadFileResult ConvertFileWordToPdf(List<EntityContent> listData,List<tt_giamdinh> tt_giamdinh,List<ThuHuong> hsgd_totrinh_tt,bool ChkChuanopphi)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.FilePath}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                var path_pdf = _word2PdfSettings.PathPdf;
                var currentPathWord = _word2PdfSettings.CurrentPathWord;
                string fullPathLocal = file_dest + nameFileCopy;

                File.Copy(currentPathWord, fullPathLocal, true);

                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }


                doc = app.Documents.Open(fullPathLocal);

                app.Visible = false;
                doc.Activate();

                if (listData != null)
                {
                    foreach (var obj in listData)
                    {
                        FindAndReplace(app, obj.KeyCode, obj.Value);
                        if (obj.KeyCode == "[SO_HSBT]" || obj.KeyCode == "[SO_HSGD]")
                        {
                            FindAndReplaceFooter(doc.Sections, obj.KeyCode, obj.Value);
                        }
                    }
                }

                foreach (Microsoft.Office.Interop.Word.Table table in doc.Tables)
                {
                    for (int rowIndex = table.Rows.Count; rowIndex > 0; rowIndex--)
                    {
                        Microsoft.Office.Interop.Word.Row row = table.Rows[rowIndex];
                        bool rowContainsText = false;

                        // Check each cell in the row for the text "NgayNhapVien"
                        foreach (Microsoft.Office.Interop.Word.Cell cell in row.Cells)
                        {
                            if (ChkChuanopphi)
                            {
                                if (cell.Range.Text.Contains("[CHK_DAYDU_CO]") || cell.Range.Text.Contains("[CHK_DUNGHAN_CO]") || cell.Range.Text.Contains("[CHK_DAYDU_KHONG]") || cell.Range.Text.Contains("[CHK_DUNGHAN_KHONG]"))
                                {
                                    rowContainsText = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (cell.Range.Text.Contains("[CHK_CHUANOPPHI]"))
                                {
                                    rowContainsText = true;
                                    break;
                                }
                            }
                            if (cell.Range.Text.Contains("[TTOAN_BTVCX0]") || cell.Range.Text.Contains("[TTOAN_BTHHOA0]") || cell.Range.Text.Contains("[TTOAN_BTLPX0]") || cell.Range.Text.Contains("[TTOAN_BTNT30]") || cell.Range.Text.Contains("[CHI_KHAC]"))
                            {
                                rowContainsText = true;
                                break;
                            }
                           
                        }

                        // Delete the row if it contains the text
                        if (rowContainsText)
                        {
                            row.Delete();
                        }
                    }
                }
                CreateTableInWordDocument_GiamDinh(doc, tt_giamdinh);
                CreateTableInWordDocument_ThuHuong(doc, hsgd_totrinh_tt);
                doc.Save();
                doc.Close(false);

                //string clonedFilePath = file_dest + Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                //File.Copy(fullPathLocal, clonedFilePath);

                //doc = null;
                path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                bool status = Word2PDF(fullPathLocal, path_pdf, app);
                //bool status = Word2PDF(clonedFilePath, path_pdf, app);
                app.Quit(false);

                if (doc != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (app != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                doc = null;
                app = null;
                GC.Collect();

                //try
                //{
                //    foreach (Process clsProcess in Process.GetProcesses())
                //    {
                //        //_logger.Information("ConvertFileWordToPdf Kill clsProcess.Id:" + clsProcess.Id);

                //        if (Process.GetCurrentProcess().Id == clsProcess.Id)
                //        {
                //            _logger.Information("ConvertFileWordToPdf " + clsProcess.Id);

                //            if (clsProcess.ProcessName.ToUpper().Contains("WINWORD"))
                //            {
                //                _logger.Information("ConvertFileWordToPdf Kill");
                //                clsProcess.Kill();
                //                break;
                //            }
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{

                //}


                if (status)
                {
                    //Return file duoi dang base64
                    byte[] pdfBytes = File.ReadAllBytes(path_pdf);
                    string base64String = Convert.ToBase64String(pdfBytes);
                    string extension = Path.GetExtension(path_pdf);
                    var contentType = GetMimeType(extension);
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "00",
                        Message = "Thanh cong",
                        MimeType = contentType,
                        Data = base64String,
                    };
                    try
                    {
                        // File.Delete(clonedFilePath);
                        //File.Delete(path_pdf);
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception ee)
                    {
                        _logger.Information("PrintToTrinh xóa file thất bại " + ee);
                    }
                    return downloadFileResult;
                }
                else
                {
                    try
                    {
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception eee) { _logger.Information("PrintToTrinh xóa file thất bại " + eee); }
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "-500",
                        Message = "Error",
                    };
                    return downloadFileResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Information("PrintToTrinh thất bại " + ex);
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);                    
                }

                doc = null;
                app = null;
                GC.Collect();
            }

            return null;
        }
        public DownloadFileResult ConvertFileWordToPdf_BiaHs(List<EntityContent> listData)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.PathTempFile}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                //var path_pdf = _word2PdfSettings.PathTempFile;
                var currentPathWord = _word2PdfSettings.FileBiaHSDocx;
                string fullPathLocal = file_dest + nameFileCopy;

                File.Copy(currentPathWord, fullPathLocal, true);

                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }


                doc = app.Documents.Open(fullPathLocal);

                app.Visible = false;
                doc.Activate();

                if (listData != null)
                {
                    foreach (var obj in listData)
                    {
                        FindAndReplace(app, obj.KeyCode, obj.Value);
                    }
                }
                doc.Save();
                doc.Close(false);
               // path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                //bool status = Word2PDF(fullPathLocal, path_pdf, app);
                app.Quit(false);

                if (doc != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (app != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                doc = null;
                app = null;
                GC.Collect();
                //Return file duoi dang base64
                byte[] pdfBytes = File.ReadAllBytes(fullPathLocal);
                string base64String = Convert.ToBase64String(pdfBytes);
                string extension = Path.GetExtension(fullPathLocal);
                var contentType = GetMimeType(extension);
                DownloadFileResult downloadFileResult = new DownloadFileResult
                {
                    Status = "00",
                    Message = "Thanh cong",
                    MimeType = contentType,
                    Data = base64String,
                };
                try
                {
                    // File.Delete(clonedFilePath);
                    //File.Delete(path_pdf);
                    //File.Delete(fullPathLocal);
                }
                catch (Exception ee)
                {
                    _logger.Information("CreateBiaHS xóa file thất bại " + ee);
                }
                return downloadFileResult;
                //if (status)
                //{
                //    //Return file duoi dang base64
                //    byte[] pdfBytes = File.ReadAllBytes(path_pdf);
                //    string base64String = Convert.ToBase64String(pdfBytes);
                //    string extension = Path.GetExtension(path_pdf);
                //    var contentType = GetMimeType(extension);
                //    DownloadFileResult downloadFileResult = new DownloadFileResult
                //    {
                //        Status = "00",
                //        Message = "Thanh cong",
                //        MimeType = contentType,
                //        Data = base64String,
                //    };
                //    try
                //    {
                //        // File.Delete(clonedFilePath);
                //        //File.Delete(path_pdf);
                //        //File.Delete(fullPathLocal);
                //    }
                //    catch (Exception ee)
                //    {
                //        _logger.Information("CreateBiaHS xóa file thất bại " + ee);
                //    }
                //    return downloadFileResult;
                //}
                //else
                //{
                //    try
                //    {
                //        //File.Delete(fullPathLocal);
                //    }
                //    catch (Exception eee) { _logger.Information("CreateBiaHS xóa file thất bại " + eee); }
                //    DownloadFileResult downloadFileResult = new DownloadFileResult
                //    {
                //        Status = "-500",
                //        Message = "Error",
                //    };
                //    return downloadFileResult;
                //}

            }
            catch (Exception ex)
            {
                _logger.Information("CreateBiaHS thất bại " + ex);
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }

                doc = null;
                app = null;
                GC.Collect();
            }

            return null;
        }
        public DownloadFileResult ConvertFileWordToPdf_PASC(List<EntityContent> listData,List<pasc_detail> list_pasc_detail,int loai_dx)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.FilePath}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                var path_pdf = _word2PdfSettings.PathPdf;
                var currentPathWord = _word2PdfSettings.CurrentPathWordPASC;
                if (loai_dx == 3)
                {
                    currentPathWord = _word2PdfSettings.CurrentPathWordPASC_TSK;
                } 
                string fullPathLocal = file_dest + nameFileCopy;

                File.Copy(currentPathWord, fullPathLocal, true);

                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }


                doc = app.Documents.Open(fullPathLocal);

                app.Visible = false;
                doc.Activate();

                if (listData != null)
                {
                    foreach (var obj in listData)
                    {
                        FindAndReplace(app, obj.KeyCode, obj.Value);
                        if (obj.KeyCode == "[SO_HSGD]")
                        {
                            FindAndReplaceFooter(doc.Sections, obj.KeyCode, obj.Value);
                        }
                    }
                }
                if (loai_dx == 0 || loai_dx == 1)
                {
                    CreateTableInWordDocument_PASC_VCX_DETAIL(doc, list_pasc_detail);
                }
                else 
                {
                    CreateTableInWordDocument_PASC_TSK_DETAIL(doc, list_pasc_detail);
                }


                doc.Save();
                doc.Close(false);

                //string clonedFilePath = file_dest + Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                //File.Copy(fullPathLocal, clonedFilePath);

                //doc = null;
                path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                bool status = Word2PDF(fullPathLocal, path_pdf, app);
                //bool status = Word2PDF(clonedFilePath, path_pdf, app);
                app.Quit(false);

                if (doc != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (app != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                doc = null;
                app = null;
                GC.Collect();


                if (status)
                {
                    //Return file duoi dang base64
                    byte[] pdfBytes = File.ReadAllBytes(path_pdf);
                    string base64String = Convert.ToBase64String(pdfBytes);
                    string extension = Path.GetExtension(path_pdf);
                    var contentType = GetMimeType(extension);
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "00",
                        Message = "Thanh cong",
                        MimeType = contentType,
                        Data = base64String,
                    };
                    try
                    {
                        // File.Delete(clonedFilePath);
                        //File.Delete(path_pdf);
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception ee)
                    {
                        _logger.Information("PrintPASC xóa file thất bại " + ee);
                    }
                    return downloadFileResult;
                }
                else
                {
                    try
                    {
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception eee) { _logger.Information("PrintPASC xóa file thất bại " + eee); }
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "-500",
                        Message = "Error",
                    };
                    return downloadFileResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Information("PrintPASC thất bại " + ex);
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }

                doc = null;
                app = null;
                GC.Collect();
            }

            return null;
        }
        public DownloadFileResult ConvertFileWordToPdf_ToTrinh_TPC(List<EntityContent> listData,int loai_tt)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.FilePath}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                var path_pdf = _word2PdfSettings.PathPdf;
                var currentPathWord = "";
                if (loai_tt == 0)
                {
                    currentPathWord = _word2PdfSettings.CurrentPathWordToTrinh_TPC;
                }
                else
                {
                    currentPathWord = _word2PdfSettings.CurrentPathWordToTrinh_TPC_toanbo;
                } 
                string fullPathLocal = file_dest + nameFileCopy;

                File.Copy(currentPathWord, fullPathLocal, true);

                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }


                doc = app.Documents.Open(fullPathLocal);

                app.Visible = false;
                doc.Activate();

                if (listData != null)
                {
                    foreach (var obj in listData)
                    {
                        FindAndReplace(app, obj.KeyCode, obj.Value);
                    }
                }

              
                doc.Save();
                doc.Close(false);

                //string clonedFilePath = file_dest + Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                //File.Copy(fullPathLocal, clonedFilePath);

                //doc = null;
                path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                bool status = Word2PDF(fullPathLocal, path_pdf, app);
                //bool status = Word2PDF(clonedFilePath, path_pdf, app);
                app.Quit(false);

                if (doc != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (app != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                doc = null;
                app = null;
                GC.Collect();

                //try
                //{
                //    foreach (Process clsProcess in Process.GetProcesses())
                //    {
                //        //_logger.Information("ConvertFileWordToPdf Kill clsProcess.Id:" + clsProcess.Id);

                //        if (Process.GetCurrentProcess().Id == clsProcess.Id)
                //        {
                //            _logger.Information("ConvertFileWordToPdf " + clsProcess.Id);

                //            if (clsProcess.ProcessName.ToUpper().Contains("WINWORD"))
                //            {
                //                _logger.Information("ConvertFileWordToPdf Kill");
                //                clsProcess.Kill();
                //                break;
                //            }
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{

                //}


                if (status)
                {
                    //Return file duoi dang base64
                    byte[] pdfBytes = File.ReadAllBytes(path_pdf);
                    string base64String = Convert.ToBase64String(pdfBytes);
                    string extension = Path.GetExtension(path_pdf);
                    var contentType = GetMimeType(extension);
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "00",
                        Message = "Thanh cong",
                        MimeType = contentType,
                        Data = base64String,
                    };
                    try
                    {
                        // File.Delete(clonedFilePath);
                        //File.Delete(path_pdf);
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception ee)
                    {
                        _logger.Information("PrintToTrinhTPC xóa file thất bại " + ee);
                    }
                    return downloadFileResult;
                }
                else
                {
                    try
                    {
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception eee) { _logger.Information("PrintToTrinhTPC xóa file thất bại " + eee); }
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "-500",
                        Message = "Error",
                    };
                    return downloadFileResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Information("PrintToTrinhTPC thất bại " + ex);
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }

                doc = null;
                app = null;
                GC.Collect();
            }

            return null;
        }
        public DownloadFileResult ConvertFileWord_ToTrinh_TPC(List<EntityContent> listData, int loai_tt)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.FilePath}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                var path_pdf = _word2PdfSettings.PathPdf;
                var currentPathWord = "";
                if (loai_tt == 0)
                {
                    currentPathWord = _word2PdfSettings.CurrentPathWordToTrinh_TPC;
                }
                else
                {
                    currentPathWord = _word2PdfSettings.CurrentPathWordToTrinh_TPC_toanbo;
                }
                string fullPathLocal = file_dest + nameFileCopy;

                File.Copy(currentPathWord, fullPathLocal, true);

                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }


                doc = app.Documents.Open(fullPathLocal);

                app.Visible = false;
                doc.Activate();

                if (listData != null)
                {
                    foreach (var obj in listData)
                    {
                        FindAndReplace(app, obj.KeyCode, obj.Value);
                    }
                }


                doc.Save();
                doc.Close(false);

                //string clonedFilePath = file_dest + Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                //File.Copy(fullPathLocal, clonedFilePath);

                //doc = null;
               // path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                //bool status = Word2PDF(fullPathLocal, path_pdf, app);
                //bool status = Word2PDF(clonedFilePath, path_pdf, app);
                app.Quit(false);

                if (doc != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (app != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                doc = null;
                app = null;
                GC.Collect();

                
                    //Return file duoi dang base64
                    byte[] pdfBytes = File.ReadAllBytes(fullPathLocal);
                    string base64String = Convert.ToBase64String(pdfBytes);
                    string extension = Path.GetExtension(fullPathLocal);
                    var contentType = GetMimeType(extension);
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "00",
                        Message = "Thanh cong",
                        MimeType = contentType,
                        Data = base64String,
                    };
                    try
                    {
                        // File.Delete(clonedFilePath);
                        //File.Delete(path_pdf);
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception ee)
                    {
                        _logger.Information("PrintToTrinhTPC xóa file thất bại " + ee);
                    }
                    return downloadFileResult;
               
            }
            catch (Exception ex)
            {
                _logger.Information("PrintToTrinhTPC thất bại " + ex);
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }

                doc = null;
                app = null;
                GC.Collect();
            }

            return null;
        }
        // ConvertWordToPDF cho bảo lãnh:
        public DownloadFileResult ConvertFileWordToPdf_BaoLanh(List<EntityContent> listData)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.FolderBaoLanhPDF}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                var path_pdf = _word2PdfSettings.FolderBaoLanhPDF;
                var currentPathWord = _word2PdfSettings.FileBaoLanhDocx;
                string fullPathLocal = file_dest + nameFileCopy;

                File.Copy(currentPathWord, fullPathLocal, true);

                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }


                doc = app.Documents.Open(fullPathLocal);

                app.Visible = false;
                doc.Activate();

                if (listData != null)
                {
                    foreach (var obj in listData)
                    {
                        FindAndReplace(app, obj.KeyCode, obj.Value);
                        if (obj.KeyCode == "[ma_user_duyet_bl]" || obj.KeyCode == "[ten_user_duyet_bl]")
                        {
                            FindAndReplaceFooter(doc.Sections, obj.KeyCode, ""); // Trên 247 gốc đang không hiện những thông tin này.
                        }
                        else if (obj.KeyCode == "[so_hsgd]" || obj.KeyCode == "[ten_tru_so]")
                        {
                            FindAndReplaceFooter(doc.Sections, obj.KeyCode, obj.Value);
                        }
                    }
                }

                doc.Save();
                doc.Close(false);

                path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                bool status = Word2PDF(fullPathLocal, path_pdf, app);
              
                app.Quit(false);

                if (doc != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                if (app != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                doc = null;
                app = null;
                GC.Collect();

                if (status)
                {
                    //Return file duoi dang base64
                    byte[] pdfBytes = File.ReadAllBytes(path_pdf);
                    string base64String = Convert.ToBase64String(pdfBytes);
                    string extension = Path.GetExtension(path_pdf);
                    var contentType = GetMimeType(extension);
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "00",
                        Message = "Thanh cong",
                        MimeType = contentType,
                        Data = base64String,
                    };
                    try
                    {
                        // Xoá file temp local sau khi đã tạo xong
                        File.Delete(path_pdf);
                        File.Delete(fullPathLocal); 
                    }
                    catch (Exception err)
                    {
                        _logger.Information("Print Bao Lanh xóa file thất bại " + err);
                    }
                    return downloadFileResult;
                }
                else
                {
                    try
                    {
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception err) { _logger.Information("Print Bao Lanh xóa file thất bại " + err); }
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "-500",
                        Message = "Error",
                    };
                    return downloadFileResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Information("Print Bao Lanh thất bại " + ex);
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }

                doc = null;
                app = null;
                GC.Collect();
            }

            return null;
        }

        public DownloadFileResult ConvertFileWordUploadToPdf(string FileData)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.FilePath}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                string fullPathLocal = file_dest + nameFileCopy;
                File.WriteAllBytes(fullPathLocal, Convert.FromBase64String(FileData));
                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }

                app.Visible = false;


                //doc = null;
                string path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                bool status = Word2PDF(fullPathLocal, path_pdf, app);
                app.Quit(false);


                if (app != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                app = null;
                GC.Collect();


                if (status)
                {
                    //Return file duoi dang base64
                    byte[] pdfBytes = File.ReadAllBytes(path_pdf);
                    string base64String = Convert.ToBase64String(pdfBytes);
                    string extension = Path.GetExtension(path_pdf);
                    var contentType = GetMimeType(extension);
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "00",
                        Message = "Thanh cong",
                        MimeType = contentType,
                        Data = base64String,
                    };
                    try
                    {
                        // File.Delete(clonedFilePath);
                        //File.Delete(path_pdf);
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception ee)
                    {
                        _logger.Information("ConvertFileWordUploadToPdf xóa file thất bại " + ee);
                    }
                    return downloadFileResult;
                }
                else
                {
                    try
                    {
                        //File.Delete(fullPathLocal);
                    }
                    catch (Exception eee) { _logger.Information("ConvertFileWordUploadToPdf xóa file thất bại " + eee); }
                    DownloadFileResult downloadFileResult = new DownloadFileResult
                    {
                        Status = "-500",
                        Message = "Error",
                    };
                    return downloadFileResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Information("ConvertFileWordUploadToPdf thất bại " + ex);
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }
                app = null;
                GC.Collect();
            }

            return null;
        }
        public static void CreateTableInWordDocument_PASC_VCX_DETAIL(Microsoft.Office.Interop.Word._Document objDoc, List<pasc_detail> list_pasc_detail)
        {
            try
            {
                if (list_pasc_detail != null)
                {

                    if (list_pasc_detail.Count > 0)
                    {
                        // lấy thêm cột stt
                        var data = list_pasc_detail.Select((r, i) => new
                        {
                            pr_key_dx = r.pr_key_dx,
                            ma_hmuc = r.ma_hmuc,
                            ten_hmuc = r.ten_hmuc,
                            so_tientt = r.so_tientt,
                            so_tienph = r.so_tienph,
                            so_tienson = r.so_tienson,
                            vat_sc = r.vat_sc,
                            giam_tru_bt = r.giam_tru_bt,
                            thu_hoi_ts = r.thu_hoi_ts,
                            vat_so_tientt = r.vat_so_tientt,
                            vat_so_tienph = r.vat_so_tienph,
                            vatso_tienson = r.vatso_tienson,
                            so_tientt_gomVAT = r.so_tientt_gomVAT,
                            so_tienph_gomVAT = r.so_tienph_gomVAT,
                            so_tienson_gomVAT = r.so_tienson_gomVAT,
                            ghi_chudv = r.ghi_chudv,
                            so_tien_vat = r.so_tien_vat,
                            sum_tt_ph_son_gomVAT = r.sum_tt_ph_son_gomVAT,
                            sum_giamtru_bt = r.sum_giamtru_bt,
                            sum_so_tienggsc = r.sum_so_tienggsc,
                            Stt = i + 1
                        }).ToList();
                        int iRowCount = list_pasc_detail.Count();
                        int iColCount = 9;

                        object oMissing = System.Reflection.Missing.Value;
                        object oEndOfDoc = "TABLEPASC";

                        int check_book = objDoc.Bookmarks.Count;
                        if (check_book < 1)
                        {
                            return;
                        }

                        Microsoft.Office.Interop.Word.Table objTable;
                        Microsoft.Office.Interop.Word.Range wrdRng = objDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        objTable = objDoc.Tables.Add(wrdRng, iRowCount + 2, iColCount, ref oMissing, ref oMissing);
                        //objTable = objDoc.Tables.Add(tableStart, iRowCount + 1, iColCount, ref oMissing, ref oMissing);
                        objTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        objTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        objTable.Range.Font.Size = (float)11;

                        //Add ten cot
                        objTable.Rows[1].Range.Font.Bold = 1;
                        objTable.Rows[1].Range.Font.Name = "Times New Roman";
                        objTable.Rows[1].Range.Font.Size = (float)11;
                        objTable.Rows[1].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        objTable.Rows[1].Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        objTable.Cell(1, 1).Range.Text = "TT";
                        objTable.Cell(1, 2).Range.Text = "Hạng mục tổn thất";
                        objTable.Cell(1, 3).Range.Text = "Phương án sửa chữa";
                        objTable.Cell(1, 6).Range.Text = "Thuế VAT (%)";
                        objTable.Cell(1, 7).Range.Text = "Giảm trừ BT (%)";
                        objTable.Cell(1, 8).Range.Text = "Thu hồi tài sản";
                        objTable.Cell(1, 9).Range.Text = "Ghi chú";

                        objTable.Rows[2].Range.Font.Bold = 1;
                        objTable.Rows[2].Range.Font.Name = "Times New Roman";
                        objTable.Rows[2].Range.Font.Size = (float)11;
                        objTable.Rows[2].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        objTable.Rows[2].Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        objTable.Cell(2, 3).Range.Text = "Phụ tùng";
                        objTable.Cell(2, 4).Range.Text = "Nhân công";
                        objTable.Cell(2, 5).Range.Text = "Sơn";

                        ////merge cột
                        //objTable.Cell(1, 1).Merge(objTable.Cell(2, 1));
                        //objTable.Cell(1, 2).Merge(objTable.Cell(2, 2));
                        //objTable.Cell(1, 6).Merge(objTable.Cell(2, 6));
                        //objTable.Cell(1, 7).Merge(objTable.Cell(2, 7));
                        //objTable.Cell(1, 8).Merge(objTable.Cell(2, 8));
                        //objTable.Cell(1, 9).Merge(objTable.Cell(2, 9));
                        //objTable.Cell(1, 3).Merge(objTable.Cell(1, 5));

                        int i = 2;
                        char checkMark = '\u2612';
                        char emptyBox = '\u2610';
                        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                        foreach (var objData in data)
                        {
                            i++;
                            objTable.Cell(i, 1).Range.Text = objData.Stt.ToString();
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = objData.ten_hmuc.ToString();
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = objData.so_tientt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = objData.so_tienph.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 5).Range.Text = objData.so_tienson.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 6).Range.Text = objData.vat_sc.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 6).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 7).Range.Text = objData.giam_tru_bt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 7).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            if (objData.thu_hoi_ts)
                            {
                                objTable.Cell(i, 8).Range.Text = checkMark.ToString();
                            }
                            else
                            {
                                objTable.Cell(i, 8).Range.Text = emptyBox.ToString();
                            }
                            objTable.Cell(i, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 8).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 9).Range.Text = objData.ghi_chudv.ToString();
                            objTable.Cell(i, 9).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 9).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        }
                        //sum data
                        var data_sum = list_pasc_detail.GroupBy(g => 1 == 1)
                            .Select(s => new
                            {
                                sum_so_tientt = s.Sum(x => x.so_tientt),
                                sum_so_tienph = s.Sum(x => x.so_tienph),
                                sum_so_tienson = s.Sum(x => x.so_tienson),
                                sum_so_tien_vat = s.Sum(x => x.so_tien_vat),
                                sum_vat_so_tientt = s.Sum(x => x.vat_so_tientt),
                                sum_vat_so_tienph = s.Sum(x => x.vat_so_tienph),
                                sum_vatso_tienson = s.Sum(x => x.vatso_tienson)
                            }).FirstOrDefault();
                        if (data_sum != null)
                        {
                            // Add a new row for totals chi phí
                            objTable.Rows.Add(ref oMissing);
                            i++;
                            objTable.Rows[i].Range.Font.Bold = 1;
                            objTable.Cell(i, 1).Range.Text = "I";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = "Tổng chi phí:";
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = data_sum.sum_so_tientt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = data_sum.sum_so_tienph.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 5).Range.Text = data_sum.sum_so_tienson.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 6).Range.Text = data_sum.sum_so_tien_vat.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 6).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Add a new row for totals vat
                            objTable.Rows.Add(ref oMissing);
                            i++;
                            objTable.Rows[i].Range.Font.Bold = 1;
                            objTable.Cell(i, 1).Range.Text = "II";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = "Thuế VAT:";
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = data_sum.sum_vat_so_tientt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = data_sum.sum_vat_so_tienph.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 5).Range.Text = data_sum.sum_vatso_tienson.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Add a new row for totals all
                            objTable.Rows.Add(ref oMissing);
                            i++;
                            objTable.Rows[i].Range.Font.Bold = 1;
                            objTable.Cell(i, 1).Range.Text = "III";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = "Tổng cộng (I + II):";
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = (data_sum.sum_so_tientt + data_sum.sum_vat_so_tientt).ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = (data_sum.sum_so_tienph + data_sum.sum_vat_so_tienph).ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 5).Range.Text = (data_sum.sum_so_tienson + data_sum.sum_vatso_tienson).ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 6).Range.Text = data_sum.sum_so_tien_vat.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 6).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        }


                        Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        Single firstColAutoWidth = 30;
                        //objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol2 = objTable.Columns[2];
                        Single firstColAutoWidth2 = 100;
                        firstCol2.SetWidth(firstColAutoWidth2, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol3 = objTable.Columns[3];
                        Single firstColAutoWidth3 = 70;
                        firstCol3.SetWidth(firstColAutoWidth3, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol4 = objTable.Columns[4];
                        Single firstColAutoWidth4 = 70;
                        firstCol4.SetWidth(firstColAutoWidth4, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol5 = objTable.Columns[5];
                        Single firstColAutoWidth5 = 70;
                        firstCol5.SetWidth(firstColAutoWidth5, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol6 = objTable.Columns[6];
                        Single firstColAutoWidth6 = 70;
                        firstCol6.SetWidth(firstColAutoWidth6, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol7 = objTable.Columns[7];
                        Single firstColAutoWidth7 = 40;
                        firstCol7.SetWidth(firstColAutoWidth7, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol8 = objTable.Columns[8];
                        Single firstColAutoWidth8 = 40;
                        firstCol8.SetWidth(firstColAutoWidth8, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol9 = objTable.Columns[9];
                        Single firstColAutoWidth9 = 80;
                        firstCol9.SetWidth(firstColAutoWidth9, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        //objTable.AllowAutoFit = true;
                        //Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        //firstCol.AutoFit(); // force fit sizing
                        //Single firstColAutoWidth = firstCol.Width; // store autofit width
                        //objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        //firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn); // reset width keeping right table margin

                        //merge cột
                        objTable.Cell(1, 1).Merge(objTable.Cell(2, 1));
                        objTable.Cell(1, 2).Merge(objTable.Cell(2, 2));
                        objTable.Cell(1, 6).Merge(objTable.Cell(2, 6));
                        objTable.Cell(1, 7).Merge(objTable.Cell(2, 7));
                        objTable.Cell(1, 8).Merge(objTable.Cell(2, 8));
                        objTable.Cell(1, 9).Merge(objTable.Cell(2, 9));
                        objTable.Cell(1, 3).Merge(objTable.Cell(1, 5));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void CreateTableInWordDocument_PASC_TSK_DETAIL(Microsoft.Office.Interop.Word._Document objDoc, List<pasc_detail> list_pasc_detail)
        {
            try
            {
                if (list_pasc_detail != null)
                {

                    if (list_pasc_detail.Count > 0)
                    {
                        // lấy thêm cột stt
                        var data = list_pasc_detail.Select((r, i) => new
                        {
                            pr_key_dx = r.pr_key_dx,
                            ten_hmuc = r.ten_hmuc,
                            so_tientt = r.so_tientt,
                            so_tiensc = r.so_tiensc,
                            vat_sc = r.vat_sc,
                            giam_tru_bt = r.giam_tru_bt,
                            thu_hoi_ts = r.thu_hoi_ts,
                            vat_so_tientt = r.vat_so_tientt,
                            vat_so_tiensc = r.vat_so_tiensc,
                            so_tientt_gomVAT = r.so_tientt_gomVAT,
                            so_tiensc_gomVAT = r.so_tiensc_gomVAT,
                            ghi_chudv = r.ghi_chudv,
                            so_tien_vat = r.so_tien_vat,
                            sum_tt_sc_gomVAT = r.sum_tt_sc_gomVAT,
                            sum_giamtru_bt = r.sum_giamtru_bt,
                            Stt = i + 1
                        }).ToList();
                        int iRowCount = list_pasc_detail.Count();
                        int iColCount = 8;

                        object oMissing = System.Reflection.Missing.Value;
                        object oEndOfDoc = "TABLEPASC";

                        int check_book = objDoc.Bookmarks.Count;
                        if (check_book < 1)
                        {
                            return;
                        }

                        Microsoft.Office.Interop.Word.Table objTable;
                        Microsoft.Office.Interop.Word.Range wrdRng = objDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        objTable = objDoc.Tables.Add(wrdRng, iRowCount + 2, iColCount, ref oMissing, ref oMissing);
                        //objTable = objDoc.Tables.Add(tableStart, iRowCount + 1, iColCount, ref oMissing, ref oMissing);
                        objTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        objTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                        objTable.Range.Font.Size = (float)11;

                        //Add ten cot
                        objTable.Rows[1].Range.Font.Bold = 1;
                        objTable.Rows[1].Range.Font.Name = "Times New Roman";
                        objTable.Rows[1].Range.Font.Size = (float)11;
                        objTable.Rows[1].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        objTable.Rows[1].Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        objTable.Cell(1, 1).Range.Text = "TT";
                        objTable.Cell(1, 2).Range.Text = "Hạng mục tổn thất";
                        objTable.Cell(1, 3).Range.Text = "Phương án sửa chữa";
                        objTable.Cell(1, 5).Range.Text = "Thuế VAT (%)";
                        objTable.Cell(1, 6).Range.Text = "Giảm trừ BT (%)";
                        objTable.Cell(1, 7).Range.Text = "Thu hồi tài sản";
                        objTable.Cell(1, 8).Range.Text = "Ghi chú";

                        objTable.Rows[2].Range.Font.Bold = 1;
                        objTable.Rows[2].Range.Font.Name = "Times New Roman";
                        objTable.Rows[2].Range.Font.Size = (float)11;
                        objTable.Rows[2].Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        objTable.Rows[2].Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        objTable.Cell(2, 3).Range.Text = "Thay thế";
                        objTable.Cell(2, 4).Range.Text = "Sửa chữa";


                        int i = 2;
                        char checkMark = '\u2612';
                        char emptyBox = '\u2610';
                        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                        foreach (var objData in data)
                        {
                            i++;
                            objTable.Cell(i, 1).Range.Text = objData.Stt.ToString();
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = objData.ten_hmuc.ToString();
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = objData.so_tientt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = objData.so_tiensc.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 5).Range.Text = objData.vat_sc.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 6).Range.Text = objData.giam_tru_bt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 6).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            if (objData.thu_hoi_ts)
                            {
                                objTable.Cell(i, 7).Range.Text = checkMark.ToString();
                            }
                            else
                            {
                                objTable.Cell(i, 7).Range.Text = emptyBox.ToString();
                            }
                            objTable.Cell(i, 7).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 7).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 8).Range.Text = objData.ghi_chudv.ToString();
                            objTable.Cell(i, 8).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 8).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        }
                        //sum data
                        var data_sum = list_pasc_detail.GroupBy(g => 1 == 1)
                            .Select(s => new
                            {
                                sum_so_tientt = s.Sum(x => x.so_tientt),
                                sum_so_tiensc = s.Sum(x => x.so_tiensc),
                                sum_so_tien_vat = s.Sum(x => x.so_tien_vat),
                                sum_vat_so_tientt = s.Sum(x => x.vat_so_tientt),
                                sum_vat_so_tiensc = s.Sum(x => x.vat_so_tiensc)
                            }).FirstOrDefault();
                        if (data_sum != null)
                        {
                            // Add a new row for totals chi phí
                            objTable.Rows.Add(ref oMissing);
                            i++;
                            objTable.Rows[i].Range.Font.Bold = 1;
                            objTable.Cell(i, 1).Range.Text = "I";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = "Tổng chi phí:";
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = data_sum.sum_so_tientt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = data_sum.sum_so_tiensc.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 5).Range.Text = data_sum.sum_so_tien_vat.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Add a new row for totals vat
                            objTable.Rows.Add(ref oMissing);
                            i++;
                            objTable.Rows[i].Range.Font.Bold = 1;
                            objTable.Cell(i, 1).Range.Text = "II";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = "Thuế VAT:";
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = data_sum.sum_vat_so_tientt.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = data_sum.sum_vat_so_tiensc.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Add a new row for totals all
                            objTable.Rows.Add(ref oMissing);
                            i++;
                            objTable.Rows[i].Range.Font.Bold = 1;
                            objTable.Cell(i, 1).Range.Text = "III";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            objTable.Cell(i, 1).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 2).Range.Text = "Tổng cộng (I + II):";
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 3).Range.Text = (data_sum.sum_so_tientt + data_sum.sum_vat_so_tientt).ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 3).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 4).Range.Text = (data_sum.sum_so_tiensc + data_sum.sum_vat_so_tiensc).ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 4).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            objTable.Cell(i, 5).Range.Text = data_sum.sum_so_tien_vat.ToString("#,###", cul.NumberFormat);
                            objTable.Cell(i, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                            objTable.Cell(i, 5).Range.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                        }


                        Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        Single firstColAutoWidth = 30;
                       // objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol2 = objTable.Columns[2];
                        Single firstColAutoWidth2 = 120;
                        firstCol2.SetWidth(firstColAutoWidth2, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol3 = objTable.Columns[3];
                        Single firstColAutoWidth3 = 70;
                        firstCol3.SetWidth(firstColAutoWidth3, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol4 = objTable.Columns[4];
                        Single firstColAutoWidth4 = 70;
                        firstCol4.SetWidth(firstColAutoWidth4, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol5 = objTable.Columns[5];
                        Single firstColAutoWidth5 = 70;
                        firstCol5.SetWidth(firstColAutoWidth5, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol6 = objTable.Columns[6];
                        Single firstColAutoWidth6 = 70;
                        firstCol6.SetWidth(firstColAutoWidth6, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol7 = objTable.Columns[7];
                        Single firstColAutoWidth7 = 50;
                        firstCol7.SetWidth(firstColAutoWidth7, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol8 = objTable.Columns[8];
                        Single firstColAutoWidth8 = 90;
                        firstCol8.SetWidth(firstColAutoWidth8, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        //merge cột
                        objTable.Cell(1, 1).Merge(objTable.Cell(2, 1));
                        objTable.Cell(1, 2).Merge(objTable.Cell(2, 2));
                        objTable.Cell(1, 5).Merge(objTable.Cell(2, 5));
                        objTable.Cell(1, 6).Merge(objTable.Cell(2, 6));
                        objTable.Cell(1, 7).Merge(objTable.Cell(2, 7));
                        objTable.Cell(1, 8).Merge(objTable.Cell(2, 8));
                        objTable.Cell(1, 3).Merge(objTable.Cell(1, 4));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void CreateTableInWordDocument_GiamDinh(Microsoft.Office.Interop.Word._Document objDoc, List<tt_giamdinh> tt_giamdinh)
        {
            try
            {
                if (tt_giamdinh != null)
                {

                    if (tt_giamdinh.Count > 0)
                    {
                        // lấy thêm cột stt
                        int iRowCount = tt_giamdinh.Count() * 2;
                        int iColCount = 2;

                        object oMissing = System.Reflection.Missing.Value;
                        object oEndOfDoc = "GIAMDINH";

                        int check_book = objDoc.Bookmarks.Count;
                        if (check_book < 1)
                        {
                            return;
                        }

                        Microsoft.Office.Interop.Word.Table objTable;
                        Microsoft.Office.Interop.Word.Range wrdRng = objDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        objTable = objDoc.Tables.Add(wrdRng, iRowCount, iColCount, ref oMissing, ref oMissing);
                        //objTable = objDoc.Tables.Add(tableStart, iRowCount + 1, iColCount, ref oMissing, ref oMissing);
                        objTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                        objTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                        objTable.Range.Font.Size = (float)12.5;


                        int i = 1;
                        foreach (var objData in tt_giamdinh)
                        {
                            objTable.Cell(i, 1).Range.Text = "- Đơn vị giám định:";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                            objTable.Cell(i, 2).Range.Text = objData.cty_gdinh.ToString();
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                            objTable.Cell(i+1, 1).Range.Text = "- Phí giám định:";
                            objTable.Cell(i+1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                            objTable.Cell(i+1, 2).Range.Text = objData.sotien_gdinh.ToString("#,###", CultureInfo.GetCultureInfo("vi-VN").NumberFormat) + " đ.";
                            objTable.Cell(i+1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                            i = i + 2;
                        }
                       
                        //objTable.AllowAutoFit = true;
                        //Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        //firstCol.AutoFit(); // force fit sizing
                        //Single firstColAutoWidth = firstCol.Width; // store autofit width
                        //objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        //firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn); // reset width keeping right table margin
                        Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        Single firstColAutoWidth = 120;
                        objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void CreateTableInWordDocument_ThuHuong(Microsoft.Office.Interop.Word._Document objDoc, List<ThuHuong> hsgd_totrinh_tt)
        {
            try
            {
                if (hsgd_totrinh_tt != null)
                {

                    if (hsgd_totrinh_tt.Count > 0)
                    {
                        // lấy thêm cột stt
                        int iRowCount = hsgd_totrinh_tt.Count() * 6;
                        int iColCount = 2;

                        object oMissing = System.Reflection.Missing.Value;
                        object oEndOfDoc = "THANHTOAN";

                        int check_book = objDoc.Bookmarks.Count;
                        if (check_book < 1)
                        {
                            return;
                        }

                        Microsoft.Office.Interop.Word.Table objTable;
                        Microsoft.Office.Interop.Word.Range wrdRng = objDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        wrdRng.InsertAfter("\n\n");
                        objTable = objDoc.Tables.Add(wrdRng, iRowCount, iColCount, ref oMissing, ref oMissing);
                        //objTable = objDoc.Tables.Add(tableStart, iRowCount + 1, iColCount, ref oMissing, ref oMissing);
                        objTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                        objTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                        objTable.Range.Font.Size = (float)12.5;


                        int i = 1;
                        int stt = 1;
                        string stt_i = "";
                        char cham = '\u2022';
                        foreach (var objData in hsgd_totrinh_tt)
                        {
                            stt_i = "";
                            for (int j = 1; j <= stt; j++)
                            {
                                stt_i += "i";
                            }
                            objTable.Cell(i, 1).Range.Text = "(" + stt_i + ")";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 1).Range.Font.Italic= 1;

                            objTable.Cell(i, 2).Range.Text = "Thụ hưởng " + stt;
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Font.Italic = 1;

                            objTable.Cell(i + 1, 1).Range.Text = "";
                            objTable.Cell(i + 1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 1, 1).Range.Font.Italic = 1;

                            objTable.Cell(i + 1, 2).Range.Text = cham + " Tên chủ tài khoản " + stt + ": " + objData.TenChuTk;
                            objTable.Cell(i + 1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 1, 2).Range.Font.Italic = 1;
                            objTable.Cell(i + 1, 2).Range.Font.Size = 12;

                            objTable.Cell(i + 2, 1).Range.Text = "";
                            objTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 2, 1).Range.Font.Italic = 1;

                            objTable.Cell(i + 2, 2).Range.Text = cham + " Số tài khoản ngân hàng: " + objData.SoTaikhoanNh;
                            objTable.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 2, 2).Range.Font.Italic = 1;

                            objTable.Cell(i + 3, 1).Range.Text = "";
                            objTable.Cell(i + 3, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 3, 1).Range.Font.Italic = 1;

                            objTable.Cell(i + 3, 2).Range.Text = cham + " Tên ngân hàng: " + objData.TenNh;
                            objTable.Cell(i + 3, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 3, 2).Range.Font.Italic = 1;
                            objTable.Cell(i + 3, 2).Range.Font.Size = 12;

                            objTable.Cell(i + 4, 1).Range.Text ="";
                            objTable.Cell(i + 4, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 4, 1).Range.Font.Italic = 1;

                            objTable.Cell(i + 4, 2).Range.Text = cham + " Số tiền thanh toán: " + Convert.ToDecimal(objData.SotienTt).ToString("#,###", CultureInfo.GetCultureInfo("vi-VN").NumberFormat);
                            objTable.Cell(i + 4, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 4, 2).Range.Font.Italic = 1;

                            objTable.Cell(i + 5, 1).Range.Text = "";
                            objTable.Cell(i + 5, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 5, 1).Range.Font.Italic = 1;

                            objTable.Cell(i + 5, 2).Range.Text = cham + " Lý do thanh toán: " + objData.LydoTt;
                            objTable.Cell(i + 5, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 5, 2).Range.Font.Italic = 1;

                            i = i + 6;
                            stt++;
                        }

                        Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        Single firstColAutoWidth = 50;
                        objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol2 = objTable.Columns[2];
                        Single firstColAutoWidth2 = 500;
                        firstCol2.SetWidth(firstColAutoWidth2, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void CreateTableInWordDocument_ThuHuong_thongbaobt(Microsoft.Office.Interop.Word._Document objDoc, List<ThuHuong> hsgd_totrinh_tt)
        {
            try
            {
                if (hsgd_totrinh_tt ==null)
                {
                    //add mặc định 
                    ThuHuong obj = new ThuHuong
                    {
                        TenChuTk = "....................................................................",
                        SoTaikhoanNh = "........................................................................................",
                        TenNh = "...............................................................................................",
                        LydoTt = "",
                        SotienTt =0
                    };
                    hsgd_totrinh_tt = new List<ThuHuong>();
                    hsgd_totrinh_tt.Add(obj);
                }    
                //if (hsgd_totrinh_tt != null)
                //{

                //    if (hsgd_totrinh_tt.Count > 0)
                //    {
                        // lấy thêm cột stt
                        int iRowCount = hsgd_totrinh_tt.Count() * 4;
                        int iColCount = 2;

                        object oMissing = System.Reflection.Missing.Value;
                        object oEndOfDoc = "THANHTOAN";

                        int check_book = objDoc.Bookmarks.Count;
                        if (check_book < 1)
                        {
                            return;
                        }

                        Microsoft.Office.Interop.Word.Table objTable;
                        Microsoft.Office.Interop.Word.Range wrdRng = objDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        wrdRng.InsertAfter("\n\n");
                        objTable = objDoc.Tables.Add(wrdRng, iRowCount, iColCount, ref oMissing, ref oMissing);                        
                        objTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                        objTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                        objTable.Range.Font.Size = (float)13;


                        int i = 1;
                        int stt = 1;
                        string stt_i = "";
                        char cham = '\u2022';
                        foreach (var objData in hsgd_totrinh_tt)
                        {
                            stt_i = "";
                            for (int j = 1; j <= stt; j++)
                            {
                                stt_i += "i";
                            }
                            objTable.Cell(i, 1).Range.Text = "(" + stt_i + ")";
                            objTable.Cell(i, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 1).Range.Font.Italic = 0;

                            objTable.Cell(i, 2).Range.Text = "Thụ hưởng " + stt;
                            objTable.Cell(i, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i, 2).Range.Font.Italic = 0;

                            objTable.Cell(i + 1, 1).Range.Text = "";
                            objTable.Cell(i + 1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 1, 1).Range.Font.Italic = 0;

                            objTable.Cell(i + 1, 2).Range.Text = " -Tên đơn vị/cá nhân thụ hưởng: " + stt + ": " + (objData.TenChuTk !=""? objData.TenChuTk : ".............................................................................");
                            objTable.Cell(i + 1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 1, 2).Range.Font.Italic = 0;
                            objTable.Cell(i + 1, 2).Range.Font.Size = 12;

                            objTable.Cell(i + 2, 1).Range.Text = "";
                            objTable.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 2, 1).Range.Font.Italic = 0;

                            objTable.Cell(i + 2, 2).Range.Text = " -Số tài khoản: " + (objData.SoTaikhoanNh !=""? objData.SoTaikhoanNh: "...................................................................................................");
                            objTable.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 2, 2).Range.Font.Italic = 0;

                            objTable.Cell(i + 3, 1).Range.Text = "";
                            objTable.Cell(i + 3, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 3, 1).Range.Font.Italic = 0;

                            objTable.Cell(i + 3, 2).Range.Text = " -Ngân hàng: " + (objData.TenNh != "" ? objData.TenNh : "...............................................................................................................");
                            objTable.Cell(i + 3, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            objTable.Cell(i + 3, 2).Range.Font.Italic = 0;
                            objTable.Cell(i + 3, 2).Range.Font.Size = 12;
                            i = i + 4;
                            stt++;
                        }

                        Microsoft.Office.Interop.Word.Column firstCol = objTable.Columns[1];
                        Single firstColAutoWidth = 50;
                        objTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitWindow); // fill page width
                        firstCol.SetWidth(firstColAutoWidth, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);

                        Microsoft.Office.Interop.Word.Column firstCol2 = objTable.Columns[2];
                        Single firstColAutoWidth2 = 500;
                        firstCol2.SetWidth(firstColAutoWidth2, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustFirstColumn);
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
        }
        public DownloadFileResult ConvertFileWordToPdf_ThongBaoBT(List<EntityContent> listData,List<ThuHuong> hsgd_totrinh_tt,bool pdf_file)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                //1. Thuc hien copy ra file moi
                string nameFileCopy = Guid.NewGuid().ToString().Replace("-", "") + ".docx";
                string file_dest = $"{_word2PdfSettings.FilePath}{DateTime.Now.Year}\\{DateTime.Now.Month}\\";

                if (!System.IO.File.Exists(file_dest))
                {
                    Directory.CreateDirectory(file_dest);
                }
                var path_pdf = _word2PdfSettings.PathPdf;
                var currentPathWord = _word2PdfSettings.CurrentPathWord_ThongBaoBT;
                string fullPathLocal = file_dest + nameFileCopy;

                File.Copy(currentPathWord, fullPathLocal, true);

                try
                {
                    Type acType = Type.GetTypeFromProgID("Word.Application");
                    app = (Microsoft.Office.Interop.Word.Application)Activator.CreateInstance(acType, true);
                    if (app == null)
                    {
                        app = new Microsoft.Office.Interop.Word.Application();
                    }
                }
                catch (COMException ex)
                {
                    app = new Microsoft.Office.Interop.Word.Application();
                }


                doc = app.Documents.Open(fullPathLocal);

                app.Visible = false;
                doc.Activate();

                if (listData != null)
                {
                    foreach (var obj in listData)
                    {
                        FindAndReplace(app, obj.KeyCode, obj.Value);
                    }
                }
                CreateTableInWordDocument_ThuHuong_thongbaobt(doc, hsgd_totrinh_tt);
                doc.Save();
                doc.Close(false);
                //preview file pdf
                if(pdf_file)
                {
                    path_pdf = fullPathLocal.ToLower().Replace(".docx", ".pdf");
                    bool status = Word2PDF(fullPathLocal, path_pdf, app);
                    app.Quit(false);

                    if (doc != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                    if (app != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                    doc = null;
                    app = null;
                    GC.Collect();

                    if (status)
                    {
                        //Return file duoi dang base64
                        byte[] pdfBytes = File.ReadAllBytes(path_pdf);
                        string base64String = Convert.ToBase64String(pdfBytes);
                        string extension = Path.GetExtension(path_pdf);
                        var contentType = GetMimeType(extension);
                        DownloadFileResult downloadFileResult = new DownloadFileResult
                        {
                            Status = "00",
                            Message = "Thanh cong",
                            MimeType = contentType,
                            Data = base64String,
                        };
                        try
                        {                            
                            if (File.Exists(fullPathLocal))
                            {
                                File.Delete(fullPathLocal);
                            }
                            if (File.Exists(path_pdf))
                            {
                                File.Delete(path_pdf);
                            }
                        }
                        catch (Exception ee)
                        {
                            _logger.Information("PrintThongBaoBT xóa file thất bại " + ee);
                        }
                        return downloadFileResult;
                    }
                    else
                    {
                        try
                        {
                            if (File.Exists(fullPathLocal))
                            {
                                File.Delete(fullPathLocal);
                            }
                        }
                        catch (Exception eee) { _logger.Information("PrintThongBaoBT xóa file thất bại " + eee); }
                        DownloadFileResult downloadFileResult = new DownloadFileResult
                        {
                            Status = "-500",
                            Message = "Error",
                        };
                        return downloadFileResult;
                    }
                }
                else // tải file word
                {
                   app.Quit(false);
                    if (doc != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                    if (app != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                    doc = null;
                    app = null;
                    GC.Collect();

                    try
                    {
                        //Return file duoi dang base64
                        byte[] wordBytes = File.ReadAllBytes(fullPathLocal);
                        string base64String = Convert.ToBase64String(wordBytes);
                        string extension = Path.GetExtension(fullPathLocal);
                        var contentType = GetMimeType(extension);
                        DownloadFileResult downloadFileResult = new DownloadFileResult
                        {
                            Status = "00",
                            Message = "Thanh cong",
                            MimeType = contentType,
                            Data = base64String,
                        };
                        try
                        {
                            if (File.Exists(fullPathLocal))
                            {
                                File.Delete(fullPathLocal);
                            }
                            
                        }
                        catch (Exception ee)
                        {
                            _logger.Information("PrintThongBaoBT xóa file thất bại " + ee);
                        }
                        return downloadFileResult;
                    }
                    catch (Exception ex)
                    {
                        _logger.Information("PrintThongBaoBT thất bại " + ex);
                        DownloadFileResult downloadFileResult = new DownloadFileResult
                        {
                            Status = "-500",
                            Message = "Error",
                        };
                        return downloadFileResult;

                    }

                     
                }    
              
               
            }
            catch (Exception ex)
            {
                _logger.Information("PrintThongBaoBT thất bại " + ex);
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }

                doc = null;
                app = null;
                GC.Collect();
            }

            return null;
        }
        /// 
        /// Chuyển phần nguyên của số thành chữ
        /// 
        /// Số double cần chuyển thành chữ
        /// Chuỗi kết quả chuyển từ số
        public static string NumberToText(double inputNumber, bool suffix = true)
        {
            if (inputNumber == 0)
            {
                return null;
            }
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result.Substring(0, 1).ToUpper() + result.Substring(1, result.Length - 1) + (suffix ? " đồng chẵn" : "");
        }

        public static string DoiTienVND(string S, string VND)
        {
            int k;
            string ketqua = "";
            string ketqua_cent = "";
            string ma_tte;
            string DoiTienVND = "";
            if (string.IsNullOrEmpty(VND))
            {
                VND = "VND";
            }
            if (Strings.Len(S) == 0)
                DoiTienVND = "";
            else
            {
                k = S.IndexOf(".");
                if (k == -1)
                {
                    k = 0;
                }
                if (k == 0)
                    ketqua = DoiTien(S);
                else if (Strings.Mid(S, k + 2) == "")
                {
                    S = S + "0";
                    ketqua = DoiTien(Strings.Mid(S, 1, k - 1));
                    ketqua_cent = DoiTien(Strings.Mid(S, k + 1)) + "cent./.";
                }
                else
                {
                    ketqua = DoiTien(Strings.Mid(S, 1, k - 1));
                    ketqua_cent = DoiTien(Strings.Mid(S, k + 1)) + "cent./.";
                }
            }
            if (Strings.UCase(VND) == "VND")
                DoiTienVND = ketqua == "" ? "Không " : ketqua + " đồng chẵn./.";
            else
            {

                if (Strings.Len(ketqua_cent) > 7)
                    DoiTienVND = ketqua + VND + " và " + ketqua_cent;
                else
                    DoiTienVND = ketqua + VND + "./.";
            }
            return DoiTienVND;
        }

        public static string DoiTien(string S)
        {
            string[] so = new[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new[] { "", "nghìn", "triệu", "tỷ" };
            string DoiTien = "";

            int i, j, donvi, chuc, tram;
            string str;
            str = "";
            i = Strings.Len(S);
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Int32.Parse(Strings.Mid(S, i, 1));
                    i = i - 1;
                    if (i > 0)
                        chuc = Int32.Parse(Strings.Mid(S, i, 1));
                    else
                        chuc = -1;
                    i = i - 1;
                    if (i > 0)
                        tram = Int32.Parse(Strings.Mid(S, i, 1));
                    else
                        tram = -1;
                    i = i - 1;
                    if (donvi > 0 | chuc > 0 | tram > 0 | j == 3)
                        str = hang[j] + " " + str;
                    j = j + 1;
                    if (j > 3)
                        j = 1;

                    if (donvi == 1 & chuc > 1)
                        str = "mốt" + " " + str;
                    else if (donvi == 5 & chuc > 0)
                        str = "lăm" + " " + str;
                    else if (donvi > 0)
                        str = so[donvi] + " " + str;
                    if (chuc < 0)
                        break;
                    else if (chuc == 0 & donvi > 0)
                        str = "lẻ" + " " + str;
                    else if (chuc == 1)
                        str = "mười" + " " + str;
                    else if (chuc > 1)
                        str = so[chuc] + " " + "mươi" + " " + str;
                    if (tram < 0)
                        break;
                    else if (tram > 0 | chuc > 0 | donvi > 0)
                        str = so[tram] + " " + "trăm" + " " + str;
                }
            }
            DoiTien = str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1);
            return DoiTien;
        }

        public static string formatNewLine(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                return result.ToString().Replace((char)10 + "", (char)11 + "").Replace("\n", (char)11 + "");
            }
            else
            {
                return result;
            }
        }
        public static List<string> SplitString(string input, int maxLength)
        {
            List<string> segments = new List<string>();

            for (int i = 0; i < input.Length; i += maxLength)
            {
                string segment = input.Substring(i, Math.Min(maxLength, input.Length - i));
                segments.Add(segment);
            }

            return segments;
        }


    }
}
