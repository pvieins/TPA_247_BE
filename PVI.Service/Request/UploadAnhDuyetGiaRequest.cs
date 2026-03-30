using PVI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVI.Service.Request
{
    public class UploadAnhDuyetGiaRequest
    {
        public int PrKey { get; set; }
        public bool LoaiDg { get; set; }
        public UploadFileContent File { get; set; }
    }

    public class UpdateURLAnhDuyetGiaRequest : UploadAnhDuyetGiaRequest
    {
        public int PrKeyDgCt { get; set; }
    }
}
