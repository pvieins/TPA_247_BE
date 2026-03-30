using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace PVI.Helper
{
    public class UtilityHelper
    {
        //private readonly UploadSettings _uploadSettings;
        private readonly Serilog.ILogger _logger;
        public UtilityHelper(Serilog.ILogger logger)
        {
            //_uploadSettings = uploadSettings.Value;
            _logger = logger;
        }
        public static string PostData(string postData, string url)
        {
            try
            {
                string post_data = postData;
                ServicePointManager.Expect100Continue = true;

                if (url.ToLower().Contains("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Tls11
                           | SecurityProtocolType.Tls12;
                }

                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);
                request.Method = "POST";
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/json";
                request.Headers.Add("PartnerCode", "PVI");
                // request.Timeout = 150000;
                //  request.KeepAlive = true;
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
                    StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
                    string responseString = responseReader.ReadToEnd();

                    return responseString;
                }

                return string.Empty;
            }

            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return string.Empty;
            }
        }

        public static string SysApplicationClient()
        {
            try
            {

                Thread thread = new Thread(new ThreadStart(WorkerSys));
                thread.Start();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private static void WorkerSys()
        {
            //string url = _configuration.GetValue<string>("URLRedirect:SysPermisstionURL");
            //HttpGetData(url);
        }


        public static string HttpGetData(string url)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                if (url.ToLower().Contains("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Tls11
                           | SecurityProtocolType.Tls12;
                }

                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (request.HaveResponse)
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
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
        public string UploadFile_ToAPI(string fileSize, string extension, string folderUpload,string url_upload, bool isImage)
        {
            try
            {
                string[] allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".jfif" };
                string[] allowedFileExtensions = { ".xml", ".pdf", ".docx" };

                if (isImage)
                {
                    if (!allowedImageExtensions.Contains(extension.ToLower()))
                    {
                        _logger.Information($"UploadFile_ToAPI failed: Invalid image extension {extension}");
                        return string.Empty;
                    }
                }
                else
                {
                    if (!allowedFileExtensions.Contains(extension.ToLower()))
                    {
                        _logger.Information($"UploadFile_ToAPI failed: Invalid file extension {extension}");
                        return string.Empty;
                    }
                }
                // string uploadPath = _uploadSettings.FolderUpload;
                //string fileName = Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                string fileName = Guid.NewGuid().ToString().Replace("-", "") + extension;
                AttachFileContent objAttach = new AttachFileContent();
                objAttach.Extension = extension;
                objAttach.FilePath = folderUpload + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString() + "\\" + DateTime.Now.Day.ToString() + "\\";
                objAttach.FileName = fileName;
                objAttach.FileSize = fileSize;
                objAttach.Sign = MD5("c36b2014aabc2527c778e955241f5434" + objAttach.FilePath + objAttach.FileSize);
                string dataUpload = JsonConvert.SerializeObject(objAttach);

                //objLog.Info("Gia post UploadFile_ToAPI: " + dataUpload);

                string resultUpload = PostData(dataUpload, url_upload);

                _logger.Information("UploadFile_ToAPI resultUpload = " + resultUpload);
                ResultUpload objData = (ResultUpload)JsonConvert.DeserializeObject<ResultUpload>(resultUpload);
                if (objData.Status == 1)
                {
                    return objData.FilePath;
                }
            }
            catch (Exception ex)
            {
                _logger.Information("UploadFile_ToAPI error = " + ex);
            }

            return string.Empty;
        }
        public string UploadFileOld_ToAPI(string fileSize, string filePath, string folderUpload, string url_upload)
        {
            try
            {
                AttachFileContent objAttach = new AttachFileContent();
                objAttach.Extension = Path.GetExtension(filePath);
                objAttach.FilePath = filePath.Replace(Path.GetFileName(filePath),""); 
                objAttach.FileName = Path.GetFileName(filePath); 
                objAttach.FileSize = fileSize;
                objAttach.Sign = MD5("c36b2014aabc2527c778e955241f5434" + objAttach.FilePath + objAttach.FileSize);
                string dataUpload = JsonConvert.SerializeObject(objAttach);

                //objLog.Info("Gia post UploadFile_ToAPI: " + dataUpload);

                string resultUpload = PostData(dataUpload, url_upload);

                _logger.Information("UploadFile_ToAPI resultUpload = " + resultUpload);
                ResultUpload objData = (ResultUpload)JsonConvert.DeserializeObject<ResultUpload>(resultUpload);
                if (objData.Status == 1)
                {
                    return objData.FilePath;
                }
            }
            catch (Exception ex)
            {
                _logger.Information("UploadFile_ToAPI error = " + ex);
            }

            return string.Empty;
        }
        private static string GenerateMd5Hash(string input)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] computeHash = Encoding.UTF8.GetBytes(input);
                computeHash = md5.ComputeHash(computeHash);

                var stringBuilder = new StringBuilder();
                foreach (byte b in computeHash)
                {
                    stringBuilder.Append(b.ToString("x2").ToLower());
                }
                return stringBuilder.ToString();
            }
        }

        //public static string CopyFile(string sourcePath, string targetPath)
        //{
        //    var objAttach = new CopyFileContent();
        //    string MEDIAFILE = "http://mediafile1.pvi.com.vn";
        //    string MEDIAFILE1 = "http://mediafile3.pvi.com.vn";
        //    string resultValue;

        //    try
        //    {
        //        objAttach.SourcePath = sourcePath;
        //        objAttach.TargetPath = targetPath;
        //        objAttach.Type = "DIRECTORY";
        //        string mediafile_ = MEDIAFILE;

        //        if (sourcePath.Contains(@"GCNDT_Upload\TCD"))
        //        {
        //            mediafile_ = MEDIAFILE1;
        //        }

        //        objAttach.Sign = GenerateMd5Hash("c36b2014aabc2527c778e955241f5434" + objAttach.SourcePath + objAttach.TargetPath + objAttach.Type);
        //        resultValue = PostData(JsonConvert.SerializeObject(objAttach), mediafile_ + "/Home/CopyFileDifferentDirectory");

               
        //        var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultValue);
        //        if (jsonResult != null && jsonResult.ContainsKey("Status") && jsonResult["Status"].ToString() != "1")
        //        {
        //            return string.Empty;
        //        }

                
        //        if (jsonResult != null && jsonResult.ContainsKey("Message"))
        //        {
        //            return jsonResult["Message"].ToString();
        //        }

        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
                
        //        return string.Empty;
        //    }
        //}
        public static string CopyFile(string sourcePath, string targetPath)
        {
            var objAttach = new CopyFileContent();
            string MEDIAFILE1 = "http://mediafile1.pvi.com.vn";
            string MEDIAFILE = "http://mediafile3.pvi.com.vn";
            string resultValue;

            try
            {
                // Set source and target paths in the object
                objAttach.SourcePath = sourcePath;
                objAttach.TargetPath = targetPath;
                objAttach.Type = "DIRECTORY";
                string mediafile_ = MEDIAFILE;

                // Check if the source path contains "GCNDT_Upload\TCD", switch to MEDIAFILE1 if true
                if (sourcePath.Contains(@"GCNDT_Upload\TCD"))
                {
                    mediafile_ = MEDIAFILE1;
                }

                // Generate MD5 hash for signing
                objAttach.Sign = GenerateMd5Hash("c36b2014aabc2527c778e955241f5434" + objAttach.SourcePath + objAttach.TargetPath + objAttach.Type);

                // Send POST request with serialized object and get response
                resultValue = PostData(JsonConvert.SerializeObject(objAttach), mediafile_ + "/Home/CopyFileDifferentDirectory");

              
                // Deserialize the result into a dictionary
                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultValue);

                // Check if the status is not "1", return empty string if failure
                if (jsonResult != null && jsonResult.ContainsKey("Status") && jsonResult["Status"].ToString() != "1")
                {
                    return string.Empty;
                }

                // If the response contains a message, return it
                if (jsonResult != null && jsonResult.ContainsKey("Message"))
                {
                    return jsonResult["Message"].ToString();
                }

                // Return empty string in case no message is found
                return string.Empty;
            }
            catch (Exception ex)
            {
               

                // Return an empty string in case of exception
                return string.Empty;
            }
        }
        public static string getPathAndCopyTempServer(string path_origin,string url_download,string file_name ="")
        {
            string ketqua = "";
            try
            {
                if (!string.IsNullOrEmpty(path_origin))
                {
                    var AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
                    //string url_download = _configuration.GetValue<string>("DownloadSettings:DownloadServer") ?? "";
                    var fileResult = UtilityHelper.DownloadFile_ToAPI(path_origin, url_download);
                    var FileInfo = new FileInfo(path_origin);
                    var fileName = "";
                    if (string.IsNullOrEmpty(file_name)) {
                        fileName = FileInfo.Name;
                    }
                    else
                    {
                        fileName = file_name;
                    }
                    var path_file = AppPath + "\\Temps_Bao_Lanh\\";
                    if (!System.IO.Directory.Exists(path_file))
                    {
                        System.IO.Directory.CreateDirectory(path_file);
                    }
                    var dest_byte = System.Convert.FromBase64String(fileResult.Data);
                    //byte[] dest_byte = Encoding.UTF8.GetBytes(fileResult.Data);
                    File.WriteAllBytes(path_file + fileName, dest_byte);
                    ketqua = path_file + fileName;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return ketqua;
        }
        public static byte[] getFileBase64Server(string path,string url_download)
        {
            string resultValue;
            DownloadFileContent objDownload = new DownloadFileContent();
            string MEDIAFILE = url_download;
            try
            {
                objDownload.FilePath = path;
                objDownload.Sign = GenerateMd5Hash("c36b2014aabc2527c778e955241f5434" + objDownload.FilePath);
                resultValue = PostData(JsonConvert.SerializeObject(objDownload), MEDIAFILE);
                if (GetValueJsonObject(resultValue, "Status") != "1")
                    return null;
                return System.Convert.FromBase64String(GetValueJsonObject(resultValue, "FileSize"));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string GetValueJsonObject(string jsonString, string key)
        {
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                var valueItem = jsonResult[key].ToString();
                return valueItem;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public class CopyFileContent
        {
            public string SourcePath { get; set; }
            public string TargetPath { get; set; }
            public string Type { get; set; }
            public string Sign { get; set; }
        }
        public static string GetMimeType(string extension)
        {
            var mimeTypes = new Dictionary<string, string>
    {
        {".txt", "text/plain"},
        {".pdf", "application/pdf"},
        {".doc", "application/vnd.ms-word"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".csv", "text/csv"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".png", "image/png"},
        {".gif", "image/gif"},
        {".jfif", "image/jpeg"},
    };
            return mimeTypes.ContainsKey(extension) ? mimeTypes[extension] : "application/octet-stream";
        }

        public static DownloadFileResult DownloadFile_ToAPI(string pathFile, string url_download)
        {
            DownloadFileResult result = new DownloadFileResult();
            try
            {
                DownloadFileContent objDownload = new DownloadFileContent();
                objDownload.FilePath = pathFile;
                string extension = Path.GetExtension(pathFile);
                var contentType = GetMimeType(extension);
                string dataDownload = JsonConvert.SerializeObject(objDownload);
                string resultDownload = PostData(dataDownload, url_download);
                ResultDownload objResult = (ResultDownload)JsonConvert.DeserializeObject<ResultDownload>(resultDownload);
                if (objResult.Status == 1)
                {
                    result.Status = "00";
                    result.Message = "thanh cong";
                    result.MimeType = contentType;
                    result.Data = objResult.FileSize;
                }
            }
            catch (Exception ex)
            {
                 
                result.Status = "-500";
                result.Message = "File khong ton tai hoac khong co quyen truy cap ex: "+ ex.ToString();
               
            }

            return result; 
        }
        public static string MergePdfBase64Files(List<string> listBase64)
        {
            if (listBase64 == null || listBase64.Count == 0)
                throw new ArgumentException("Danh sách file PDF trống!");

            using (var outputDocument = new PdfDocument())
            {
                foreach (var base64 in listBase64)
                {
                    if (string.IsNullOrEmpty(base64))
                        continue;

                    // Chuyển base64 sang byte[]
                    byte[] pdfBytes = Convert.FromBase64String(base64);

                    using (var stream = new MemoryStream(pdfBytes))
                    {
                        // Mở file PDF từ memory stream
                        PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

                        // Duyệt từng trang trong file gốc và copy vào file kết quả
                        for (int i = 0; i < inputDocument.PageCount; i++)
                        {
                            outputDocument.AddPage(inputDocument.Pages[i]);
                        }
                    }
                }

                // Lưu kết quả ra memory stream
                using (var outputStream = new MemoryStream())
                {
                    outputDocument.Save(outputStream);
                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }
        }
        public string MergeBase64FilesToPdf(List<string> listBase64, int minKb = 80, int maxKb = 100)
        {
            if (listBase64 == null || listBase64.Count == 0)
                throw new ArgumentException("Danh sách file trống!");
                
                using (var outputDocument = new PdfDocument())
                {
                    try
                    {
                    //_logger.Information("ResizeImageToTargetSize 0 ");                    
                        foreach (var base64 in listBase64)
                        {
                            
                            if (string.IsNullOrEmpty(base64))
                                continue;

                            byte[] fileBytes = Convert.FromBase64String(base64);

                            // Xác định loại file qua header
                            string header = GetBase64Header(base64);

                            if (header.StartsWith("/9j")) // JPG
                            {
                            //_logger.Information("ResizeImageToTargetSize 1 ");
                                byte[] compressed = ResizeImageToTargetSize(fileBytes, minKb, maxKb);
                                //File.WriteAllBytes(@"D:\WebServices\API_247\New folder\"+ Guid.NewGuid().ToString("N") + ".jpg", compressed);
                                AddImageToPdf(outputDocument, compressed);
                            }
                            else if (header.StartsWith("iVBOR")) // PNG
                            {
                                byte[] compressed = ResizeImageToTargetSize(fileBytes, minKb, maxKb);
                                AddImageToPdf(outputDocument, compressed);
                            }
                            else
                            {
                                // PDF
                                using (var stream = new MemoryStream(fileBytes))
                                {
                                    PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                                    for (int i = 0; i < inputDocument.PageCount; i++)
                                    {
                                        outputDocument.AddPage(inputDocument.Pages[i]);
                                    }
                                }
                            }
                               
                        }
                    //_logger.Information("ResizeImageToTargetSize 2 ");
                    // Xuất PDF ra Base64
                    using (var outputStream = new MemoryStream())
                        {
                            outputDocument.Save(outputStream);
                                //_logger.Information("ResizeImageToTargetSize 3 ");
                                // Đặt lại con trỏ về đầu stream để đọc hoặc ghi ra file
                                //outputStream.Position = 0;
                                // Lưu ra file tạm để kiểm tra
                                //string filePath = @"D:\WebServices\API_247\New folder\output_test.pdf";  // hoặc đường dẫn bạn muốn
                                //File.WriteAllBytes(filePath, outputStream.ToArray());
                            return Convert.ToBase64String(outputStream.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Information("MergeBase64FilesToPdf error = " + ex);
                        return "";
                    }
                }
           
            
        }
        
        // Lấy phần đầu để xác định loại file
        private static string GetBase64Header(string base64)
        {
            return base64.Length > 10 ? base64.Substring(0, 10) : base64;
        }

        // Thêm ảnh vào PDF
        private static void AddImageToPdf(PdfDocument outputDocument, byte[] imageBytes)
        {
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageBytes))
            {
                PdfPage page = outputDocument.AddPage();
                page.Width = image.Width;
                page.Height = image.Height;

                using (var gfx = XGraphics.FromPdfPage(page))
                using (var ms = new MemoryStream())
                {
                    // Lưu ảnh tạm sang stream
                    image.Save(ms, new JpegEncoder());
                    ms.Position = 0;
                    // Nạp vào PDF qua lambda
                    using (var xImage = XImage.FromStream(() => new MemoryStream(ms.ToArray())))
                    {
                        gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                    }
                }
            }
        }


        //private static byte[] ResizeImageToTargetSize(byte[] originalBytes, int minKb, int maxKb)
        //{
        //    //SixLabors.ImageSharp 1.0.4 – 2.1.3. phiên bản cao hơn là bị lỗi không resize được ảnh
        //    using (var image = SixLabors.ImageSharp.Image.Load(originalBytes))
        //    {
        //        // Resize ảnh theo tỉ lệ
        //        int targetWidth = 1600;
        //        int targetHeight = (int)((double)image.Height / image.Width * targetWidth);

        //        image.Mutate(x => x.Resize(targetWidth, targetHeight));

        //        byte[] resultBytes;
        //        int quality = 90; // Bắt đầu với chất lượng 90%

        //        // Vòng lặp điều chỉnh chất lượng để đạt kích thước mong muốn
        //        do
        //        {
        //            using (var ms = new MemoryStream())
        //            {
        //                var encoder = new JpegEncoder { Quality = quality };
        //                image.Save(ms, encoder);
        //                resultBytes = ms.ToArray();
        //            }

        //            if (resultBytes.Length < minKb * 1024 && quality < 95)
        //            {
        //                quality += 5;
        //            }
        //            else if (resultBytes.Length > maxKb * 1024 && quality > 30)
        //            {
        //                quality -= 5;
        //            }
        //            else
        //            {
        //                break;
        //            }

        //        } while (true);

        //        return resultBytes;
        //    }
        //}
        private static byte[] ResizeImageToTargetSize(byte[] originalBytes, int minKb = 80, int maxKb = 100)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(originalBytes))
            {
                // Xóa metadata để giảm dung lượng
                image.Metadata.ExifProfile = null;

                int targetWidth = image.Width;
                int targetHeight = image.Height;
                int quality = 90;
                byte[] result = originalBytes;

                for (int attempt = 0; attempt < 15; attempt++) // Cho phép nhiều vòng để tối ưu
                {
                    using (var clone = image.Clone(x => x.Resize(targetWidth, targetHeight)))
                    {
                        using (var ms = new MemoryStream())
                        {
                            var encoder = new JpegEncoder { Quality = quality };
                            clone.Save(ms, encoder);
                            result = ms.ToArray();
                        }
                    }

                    int sizeKb = result.Length / 1024;

                    // ✅ Kiểm tra kích thước hiện tại
                    if (sizeKb > maxKb)
                    {
                        // Ảnh quá lớn → giảm dần kích thước vật lý và chất lượng
                        if (sizeKb > 1500)
                        {
                            targetWidth = (int)(targetWidth * 0.65);
                            targetHeight = (int)(targetHeight * 0.65);
                            quality -= 15;
                        }
                        else if (sizeKb > 800)
                        {
                            targetWidth = (int)(targetWidth * 0.75);
                            targetHeight = (int)(targetHeight * 0.75);
                            quality -= 10;
                        }
                        else if (sizeKb > 400)
                        {
                            targetWidth = (int)(targetWidth * 0.85);
                            targetHeight = (int)(targetHeight * 0.85);
                            quality -= 5;
                        }
                        else
                        {
                            targetWidth = (int)(targetWidth * 0.9);
                            targetHeight = (int)(targetHeight * 0.9);
                            quality -= 5;
                        }
                    }
                    else if (sizeKb < minKb && quality < 95)
                    {
                        // Ảnh nhỏ hơn mức tối thiểu → tăng nhẹ chất lượng
                        quality += 5;
                    }
                    else
                    {
                        break;
                    }                    
                    if (targetWidth < 500 || quality < 30)
                        break;
                }

                return result;
            }
        }

        public static string ReplaceSqlInjection(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string sanitized = input;

            // Bước 1: loại bỏ khoảng trắng thừa và chuẩn hóa
            sanitized = sanitized.Trim();

            // Bước 2: thay thế các ký tự nguy hiểm
            sanitized = sanitized
                .Replace("'", "''")           // tránh chuỗi đóng sớm '
                .Replace("--", "")            // tránh comment SQL
                .Replace(";", "")             // tránh kết thúc lệnh
                .Replace("/*", "")            // tránh mở comment
                .Replace("*/", "")
                .Replace("@@", "@")           // tránh truy cập biến hệ thống
                .Replace("char(", "", StringComparison.OrdinalIgnoreCase)
                .Replace("nchar(", "", StringComparison.OrdinalIgnoreCase)
                .Replace("varchar(", "", StringComparison.OrdinalIgnoreCase)
                .Replace("nvarchar(", "", StringComparison.OrdinalIgnoreCase);

            // Bước 3: loại bỏ các từ khóa SQL phổ biến (có thể mở rộng thêm)
            string[] blacklist = {
            "drop", "delete", "update", "insert", "exec", "execute",
            "alter", "create", "shutdown", "grant", "revoke", "union", "select"
        };

            foreach (var word in blacklist)
            {
                sanitized = System.Text.RegularExpressions.Regex.Replace(
                    sanitized,
                    "\\b" + word + "\\b",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }

            return sanitized;
        }
    }

    public class AttachFileContent
    {
        public string? Extension { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileSize { get; set; }
        public string? Sign { get; set; }
    }
    public class UploadSettings
    {
        public string FolderUpload { get; set; }
    }

    public class ResultDownload
    {
        public int Status { get; set; }
        public string FileSize { get; set; }
        public string Message { get; set; }
    }

    public class ResultUpload
    {
        public int Status { get; set; }
        public string? FilePath { get; set; }
        public string Message { get; set; }
    }
    public class DataContentCP
    {
        public string type { get; set; }
        public string To { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public string requestid { get; set; }
        public string sign { get; set; }
        //cap nhat them thong tin quan ly

        public string ma_donvi { get; set; }

        public string ma_doitac { get; set; }

        public string nguon_tao { get; set; }

        public string mang { get; set; }
        public string text_sms { get; set; }
    }
}
