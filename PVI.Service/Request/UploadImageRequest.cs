using PVI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Service.Request
{
    public class UploadImageRequest
    {
        public int PrKey { get; set; }
        public int Stt {  get; set; }
        public string? MaHmuc {  get; set; }
        public string? DienGiai { get; set; }
        public string? MaHmucSc {  get; set; }
        public UploadFileContent File {  get; set; }
    }


    public class UpdateURLImageRequest : UploadImageRequest
    {
        public int PrKeyCt { get; set; }
    }
}
