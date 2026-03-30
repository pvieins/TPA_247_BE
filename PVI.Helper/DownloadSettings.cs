using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PVI.Helper;

public  class DownloadSettings
{
    public string DownloadServer { get; set; }
    public string UploadServer { get; set; }
}
public class Word2PdfSettings
{
    public string CurrentPathWord { get; set; }
    public string PathPdf { get; set; }
    public string FilePath { get; set; }
    public string CurrentPathWordPASC { get; set; }
    public string CurrentPathWordPASC_TSK { get; set; }
    public string FileBiaHSDocx { get; set; }
    public string PathTempFile { get; set; }
    public string FileBaoLanhDocx { get; set; }
    public string FolderBaoLanhPDF { get; set; }
    public string CurrentPathWordToTrinh_TPC { get; set; }
    public string CurrentPathWordToTrinh_TPC_toanbo { get; set; }
    public string CurrentPathWord_ThongBaoBT { get; set; }
}
public class DownloadFileAPI
{
    public string? FilePath { get; set; }
    public string? Sign { get; set; }
}
public class ResultDownloadAPI
{
    public int Status { get; set; }
    public string? FileSize { get; set; }
    public string? Message { get; set; }
}
public class CustomResultModel
{
    public string? MaTtrangbt { get; set; }
    public string? MaQuyenloi { get; set; }
    public string? TenDieutri { get; set; }
    public decimal? MtnGtbh { get; set; }
    public decimal? SoTienyc { get; set; }
    public decimal? SoTientcbt { get; set; }
    public decimal? SoTienp { get; set; }
    public decimal? SoTienCl { get; set; }
    public string? TenCtyDongBh { get; set; }
    public decimal? TyleTai { get; set; }
    public int TyleTg { get; set; }
}
public class WordToPdfRequest
{
    public string CurrentPathWord { get; set; }
    public string PathPdf { get; set; }
    public List<EntityContent> ListData { get; set; }
}

public class CombinedTtrinhResult3
{
    public List<EntityContent> ThirdQueryResults { get; set; }
    public List<ThuHuong> ListThuHuong { get; set; }
}
public class CombinedPASCResult
{
    public List<EntityContent> ThirdQueryResults { get; set; }
    public List<pasc_detail> ListPascDetail { get; set; }
}

public class CombinedBaoLanhResult
{
    public List<EntityContent> ThirdQueryResults { get; set; }
}
public class CombinedTtrinhResult4
{
    public List<EntityContent> ThirdQueryResults { get; set; }
    public List<tt_giamdinh> ListGiamDinh { get; set; }
    public List<ThuHuong> ListThuHuong { get; set; }
    public bool ChkChuanopphi { get; set; }
}
public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Data { get; set; }
}