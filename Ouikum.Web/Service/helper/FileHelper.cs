using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Drawing;
using System.Web.Mvc;
using System.Globalization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
namespace Prosoft.Service
{

    //#region FileHelper Local
    //public class FileHelper
    //{
    //    #region Member
    //    private string root = "~/Upload/Prosoft/";
    //    private string _DirPath;
    //    private string _DirTempPath;
    //    private string _PathFile;

    //    public int? FullWidth { get; set; }
    //    public int? FullHeight { get; set; }
    //    public string ImageName { get; set; }
    //    public string ImageThumbName { get; set; }
    //    public int? ThumbWidth { get; set; }
    //    public int? ThumbHeight { get; set; }
    //    public string DirPath
    //    {
    //        get
    //        {
    //            return _DirPath;
    //        }
    //        set
    //        {
    //            _DirPath = Path.Combine(root, value);
    //        }
    //    }
    //    public string DirTempPath
    //    {
    //        get
    //        {
    //            return _DirTempPath;
    //        }
    //        set
    //        {
    //            _DirTempPath = Path.Combine(root, value);
    //        }
    //    }
    //    public string PathFile
    //    {
    //        get
    //        {
    //            return _PathFile;
    //        }
    //        set
    //        {
    //            _PathFile = Path.Combine(root, value);
    //        }
    //    }
    //    public string FileType { get; set; }
    //    public bool IsSuccess { get; set; }
    //    public string ErrorMsg { get; set; }
    //    public HttpPostedFileBase File { get; set; }
    //    public List<string> listFile { get; set; }


    //    #endregion

    //    #region Methods
    //    #region Private Method
    //    private void CreateFolder(string path)
    //    {
    //        DirectoryInfo DirInfo = new DirectoryInfo(MapPath(path));
    //        if (!DirInfo.Exists) { DirInfo.Create(); }
    //    }

    //    private string RandomCharecter(int Size)
    //    {
    //        Random ran = new Random();
    //        string chars = "ABCDEFGHIJKLMNOPQESTUVWXYZ0123456789";
    //        char[] buffer = new char[Size];
    //        for (int i = 0; i < Size; i++)
    //        {
    //            buffer[i] = chars[ran.Next(chars.Length)];
    //        }
    //        return new string(buffer);
    //    }

    //    private string GetFileType(string name)
    //    {
    //        string[] arrStr = name.Split('.');
    //        return arrStr[arrStr.Length - 1];
    //    }

    //    private string MapPath(string path)
    //    {
    //        return System.Web.HttpContext.Current.Server.MapPath(path);
    //    }
    //    #endregion

    //    #region Image Methods
    //    public string ReNameFile(string PathFile)
    //    {
    //        string dir = Path.GetDirectoryName(PathFile);
    //        string name = Path.GetDirectoryName(PathFile);
    //        string newName = RandomCharecter(15) + "." + GetFileType(name);
    //        var physicalPath = Path.Combine(MapPath(DirPath), name);
    //        var physicalNewPath = Path.Combine(MapPath(DirPath), newName);

    //        #region Check Same Old File
    //        while (System.IO.File.Exists(physicalNewPath))
    //        {
    //            ImageName = RandomCharecter(15) + "." + GetFileType(name);
    //            physicalNewPath = Path.Combine(DirPath, ImageName);
    //        };
    //        #endregion

    //        #region rename
    //        if (!string.IsNullOrEmpty(physicalPath))
    //        {
    //            System.IO.File.Move(physicalPath, physicalNewPath);
    //        }
    //        #endregion

    //        // return ชื่อไฟล์ใหม่
    //        return Path.GetFileName(physicalNewPath);
    //    }

    //    #region Validate
    //    public bool ValidateImage(int MaxSize, int MaxWidth, int MaxHeight, string PathFile)
    //    {
    //        if (File != null)
    //        {
    //            #region Check File Size
    //            int size = File.ContentLength;
    //            int maxSize = (MaxSize > 0) ? MaxSize : 150000;
    //            if (size < maxSize)
    //            {
    //                IsSuccess = true;
    //            }
    //            else
    //            {
    //                IsSuccess = false;
    //                ErrorMsg += " ขนาดไฟล์มีขนาดใหญ่เกินกว่ากำหนด | ";
    //            }
    //            #endregion

    //            #region Check File Type
    //            string strType = File.ContentType.ToLower();
    //            if (strType == "image/jpeg" || strType == "image/gif" || strType == "image/png")
    //            {
    //                IsSuccess = true;
    //            }
    //            else
    //            {
    //                IsSuccess = false;
    //                ErrorMsg += "ประเภทไฟล์ไม่ถูกต้อง | ";
    //            }
    //            #endregion

    //            #region Check File Widht Height
    //            if (MaxWidth > 0 || MaxHeight > 0)
    //            {
    //                System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(Path.Combine(MapPath(PathFile)));

    //                #region check ความสูง
    //                if (MaxHeight > 0)
    //                {
    //                    if (objGraphic.Height <= MaxHeight)
    //                    {
    //                        IsSuccess = true;
    //                    }
    //                    else
    //                    {
    //                        IsSuccess = false;
    //                        ErrorMsg += "ขนาดไฟล์มี ความกว้างเกินกว่ากำหนด";
    //                    }
    //                }
    //                #endregion

    //                #region check ความกว้าง
    //                if (MaxWidth > 0)
    //                {
    //                    if (objGraphic.Width <= MaxWidth)
    //                    {
    //                        IsSuccess = true;
    //                    }
    //                    else
    //                    {
    //                        IsSuccess = false;
    //                        ErrorMsg += "ขนาดไฟล์มี ความสูงเกินกว่ากำหนด";
    //                    }
    //                }
    //                #endregion
    //            }
    //            #endregion
    //        }
    //        else
    //        {
    //            IsSuccess = false;
    //            ErrorMsg += "ไม่พบไฟล์ | ";
    //        }

    //        return IsSuccess;
    //    }

    //    #endregion

    //    #region Exists
    //    /// <summary>
    //    /// ตรวจสอบว่ามีไฟล์นี้อยู่หรือเปล่า
    //    /// </summary>
    //    /// <param name="PathFile">ตัวอย่าง ~/Upload/Company/img.jpg</param>
    //    /// <returns> boolean</returns>
    //    public bool Exists(string PathFile)
    //    {
    //        PathFile = Path.Combine(MapPath(PathFile));
    //        return System.IO.File.Exists(PathFile);
    //    }
    //    #endregion

    //    #region Exists
    //    /// <summary>
    //    /// ตรวจสอบว่ามี Directory นี้อยู่หรือเปล่า
    //    /// </summary>
    //    /// <param name="PathFile">ตัวอย่าง ~/Upload/Company </param>
    //    /// <returns></returns>
    //    public bool ExistsDirectory(string DirPath)
    //    {
    //        DirPath = Path.Combine(MapPath(DirPath));
    //        return System.IO.File.Exists(DirPath);
    //    }
    //    #endregion

    //    #region ListFile
    //    public void ListFileInDir()
    //    {
    //        DirectoryInfo DirInfo = new DirectoryInfo(MapPath(DirPath));
    //        if (DirInfo.Exists)
    //        {
    //            foreach (var f in DirInfo.GetFiles())
    //            {
    //                listFile.Add(f.FullName);
    //            }

    //        }
    //    }
    //    #endregion

    //    #region Upload
    //    /// <summary>
    //    ///  upload image setค่า DirPath , File ก่อน เรียกใช้ Method นี้
    //    /// </summary>
    //    /// <param name="DirPath">directory path เช่น ~/Upload/Company</param>
    //    /// <param name="File">ส่ง file เข้ามา</param>
    //    public void UploadImage(string DirPath, HttpPostedFileBase File)
    //    {
    //        DirPath = dirPath;
    //        var type = GetFileType(File.FileName);
    //        CreateFolder(DirPath);
    //        try
    //        {
    //            ImageName = RandomCharecter(15) + "." + type;
    //            var physicalPath = Path.Combine(MapPath(DirPath), ImageName);

    //            #region Check Same Old File
    //            while (System.IO.File.Exists(physicalPath))
    //            {
    //                ImageName = RandomCharecter(15) + "." + GetFileType(File.FileName);
    //                physicalPath = Path.Combine(MapPath(DirPath), ImageName);

    //            };
    //            #endregion

    //            File.SaveAs(physicalPath);
    //            IsSuccess = true;
    //        }
    //        catch
    //        {
    //            IsSuccess = false;
    //            ErrorMsg = "เกิดข้อผิดพลาดในการอัพโหลดไฟล์";
    //        }

    //    }
    //    #endregion

    //    #region Remove
    //    /// <summary>
    //    /// ลบรูป
    //    /// </summary>
    //    /// <param name="DirPath">directory path เช่น ~/Upload/Company</param>
    //    /// <param name="ImageName">ชื่อรูป เช่น img-prosoft.jpg</param>
    //    public void RemoveImage(string DirPath, string ImageName)
    //    {
    //        try
    //        {
    //            var fileName = Path.GetFileName(ImageName);
    //            var physicalPath = Path.Combine(MapPath(DirPath), fileName);
    //            if (System.IO.File.Exists(physicalPath))
    //            {
    //                System.IO.File.Delete(physicalPath);
    //            }

    //            IsSuccess = true;

    //        }
    //        catch
    //        {
    //            IsSuccess = false;
    //            ErrorMsg = "เกิดข้อผิดพลาดในการลบไฟล์";
    //        }

    //    }
    //    #endregion

    //    #region Copy
    //    /// <summary>
    //    /// copy file
    //    /// </summary>
    //    /// <param name="dirFromPath">กรอก  directory path ต้นทาง เช่น ~/Upload/Temp/Company</param>
    //    /// <param name="dirTargetPath">กรอก directory path ปลายทาง เช่น ~/Upload/Company </param>
    //    /// <param name="Name">ชื่อไฟล์</param>
    //    public void CopyFile(string dirFromPath, string dirTargetPath, string Name)
    //    {
    //        var physicalFromPath = Path.Combine(MapPath(dirFromPath), Name);
    //        var physicalTargetPath = Path.Combine(MapPath(dirTargetPath), Name);

    //        #region Copy File
    //        System.IO.File.Copy(physicalFromPath, physicalTargetPath, true);
    //        #endregion
    //    }

    //    /// <summary>
    //    /// copy file
    //    /// </summary>
    //    /// <param name="dirFromPath">กรอก path file  ต้นทาง  เช่น ~/Upload/Temp/Company/img.jpg </param>
    //    /// <param name="dirTargetPath">กรอก path file ปลายทาง เช่น ~/Upload/Company/img.jpg </param>
    //    public void CopyFile(string physicalFromPath, string physicalTargetPath)
    //    {
    //        #region Copy File
    //        physicalFromPath = Path.Combine(MapPath(physicalFromPath));
    //        physicalTargetPath = Path.Combine(MapPath(physicalTargetPath));
    //        System.IO.File.Copy(physicalFromPath, physicalTargetPath, true);
    //        #endregion
    //    }
    //    #endregion

    //    #region
    //    /// <summary>
    //    /// copy file ข้างใน directory มาทัังหมด
    //    /// </summary>
    //    /// <param name="dirFromPath">กรอก path file  ต้นทาง เช่น ~/Upload/Temp/Company </param>
    //    /// <param name="dirTargetPath">กรอก path file ปลายทาง เช่น ~/Upload/Temp/Company  </param>
    //    private void CopyDirAllFile(string dirFromPath, string dirTargetPath)
    //    {
    //        List<string> files = new List<string>();
    //        List<string> dir = new List<string>();
    //        DirectoryInfo DirInfo = new DirectoryInfo(MapPath(dirFromPath));
    //        if (DirInfo.Exists)
    //        {
    //            foreach (var f in DirInfo.GetFiles())
    //            {
    //                files.Add(Path.GetFileName(f.FullName));
    //            }
    //            foreach (string file in files)
    //            {
    //                CopyFile(dirFromPath, dirTargetPath, file);
    //            }
    //            IsSuccess = true;
    //        }
    //    }
    //    #endregion


    //    #region CreatImageFromTemp

    //    #region CreateObjImage
    //    public void CreateObjImage(string DirFromPath, string DirTargetPath, string from_NameImg, string to_NameImg, int Width, int Height)
    //    {
    //        #region Create
    //        CreateObjImage(DirFromPath + "/" + from_NameImg, DirTargetPath + "/" + to_NameImg, Width, Height);
    //        #endregion

    //    }

    //    private void CreateObjImage(string TempPathFile, string PathFile, int Width, int Height)
    //    {
    //        CreateFolder(Path.GetDirectoryName(PathFile));
    //        var physicalTempPath = Path.Combine(MapPath(TempPathFile));
    //        var physicalPath = Path.Combine(MapPath(PathFile));
    //        System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(physicalTempPath);
    //        Bitmap objBitmap;

    //        #region Create
    //        if (Width > 0 || Height > 0)
    //        {
    //            #region Resize
    //            if (objGraphic.Width > Width || objGraphic.Height > Height)
    //            {
    //                if (objGraphic.Width > objGraphic.Height)
    //                {
    //                    float ratio = ((float)objGraphic.Height / (float)objGraphic.Width) * (float)Width;
    //                    objBitmap = new Bitmap(objGraphic, Width, (int)ratio);
    //                }
    //                else
    //                {
    //                    float ratio = ((float)objGraphic.Width / (float)objGraphic.Height) * (float)Height;
    //                    objBitmap = new Bitmap(objGraphic, (int)ratio, Height);
    //                }
    //            }
    //            else
    //            {
    //                objBitmap = new Bitmap(objGraphic);
    //            }
    //            #endregion
    //        }
    //        else
    //        {
    //            objBitmap = new Bitmap(objGraphic);
    //        }

    //        objBitmap.Save(physicalPath, objGraphic.RawFormat);

    //        #region return Null and Close
    //        objGraphic.Dispose();
    //        objBitmap = null;
    //        objGraphic = null;
    //        #endregion

    //        #endregion
    //    }
    //    #endregion

    //    #region
    //    /// <summary>
    //    /// save image โดยเอาไฟล์จาก directory temp มา
    //    /// ต้องกำหนด DirTempPath,DirPath,ImageName,FullWidth,FullHeight
    //    /// ถ้ามี thumb file ด้วยก็ต้อง หนด ImageThumbName,ThumbWidth,ThumbHeight เพิ่มเข้ามาครับ
    //    /// 
    //    /// </summary>
    //    public void SaveImageFromTemp()
    //    {
    //        var physicalOldPath = Path.Combine(MapPath(DirTempPath), ImageName);
    //        var physicalPath = Path.Combine(MapPath(DirPath), ImageName);
    //        var physicalPathThumb = Path.Combine(MapPath(DirPath), "Thumb_" + ImageName);

    //        if (System.IO.File.Exists(physicalOldPath))
    //        {
    //            #region Create Thumb File
    //            if (ThumbWidth > 0)
    //            {
    //                CreateObjImage(DirTempPath + "/" + ImageName, DirPath + "/" + "Thumb_" + ImageName, (int)ThumbWidth, (int)ThumbHeight);
    //            }
    //            #endregion

    //            #region Create File
    //            CreateObjImage(DirTempPath, DirPath, ImageName, ImageName, (int)FullWidth, (int)FullHeight);
    //            #endregion
    //        }
    //    }
    //    #endregion
    //    #endregion
    //    /// <summary>
    //    /// ลบไฟล์ใน directory ทั้งหมด
    //    /// </summary>
    //    /// <param name="dirTarget">ตัวอย่าง ~/Upload/Temp/Company</param>
    //    public void DeleteFilesInDir(string dirTarget)
    //    {
    //        List<string> files = new List<string>();
    //        List<string> dir = new List<string>();
    //        DirectoryInfo DirInfo = new DirectoryInfo(MapPath(root + dirTarget));
    //        if (DirInfo.Exists)
    //        {
    //            foreach (var f in DirInfo.GetFiles())
    //            {
    //                files.Add(f.FullName);
    //            }
    //            foreach (string file in files)
    //            {
    //                System.IO.File.SetAttributes(file, FileAttributes.Normal);
    //                System.IO.File.Delete(file);
    //            }
    //            IsSuccess = true;
    //        }

    //    }

    //    #endregion

    //    #region VDO Method
    //    public void ValidateVDO(int MaxSize)
    //    {
    //        IsSuccess = false;
    //    }
    //    #endregion

    //    #region Log File

    //    public void CreateLogFiles(Exception ex)
    //    {
    //        string strSubFolder = DateTime.Now.ToString("yyyy-MM", CultureInfo.InvariantCulture);
    //        string ts = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    //        string appdata_folder = MapPath("~/Upload/Logs/ErrorLog/" + strSubFolder);

    //        #region Create Folder
    //        DirectoryInfo DirInfo = new DirectoryInfo(appdata_folder);
    //        if (!DirInfo.Exists) { DirInfo.Create(); }
    //        #endregion

    //        StreamWriter logfile = System.IO.File.AppendText(String.Format(@"{0}\B2BThai_ErrorLog_{1}.log", appdata_folder, ts));

    //        logfile.WriteLine(" ");
    //        logfile.WriteLine("+ Start Error Logs @ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture));
    //        logfile.WriteLine("|- DateTime       : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture));
    //        logfile.WriteLine("|- From           :  ");
    //        logfile.WriteLine("|- WebUrl         : ");
    //        logfile.WriteLine("|- Message        : " + ex.Message);
    //        logfile.WriteLine("|- Source         : " + ex.Source);
    //        logfile.WriteLine("|- Data     : " + ex.Data.Keys + ex.Data.Values);
    //        logfile.WriteLine("|- StackTrace     : ");
    //        logfile.WriteLine(ex.StackTrace);
    //        logfile.WriteLine("|- TargetSite     : " + ex.TargetSite);
    //        logfile.WriteLine("|- InnerException : " + ex.InnerException);
    //        logfile.WriteLine("|- HelpLink       : " + ex.HelpLink);
    //        logfile.WriteLine("|- Name           : " + ex.GetType().Name);
    //        logfile.WriteLine("|- An error occured : " + ex.GetType().ToString());
    //        logfile.WriteLine("|- FullName           : " + ex.GetType().FullName);
    //        logfile.WriteLine("|- End Error Logs @ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture));
    //        logfile.WriteLine(" ");
    //        logfile.WriteLine("+--------------------------------------------------------------------------------------");

    //        logfile.Flush();
    //        logfile.Close();
    //    }
    //    #endregion

    //    #endregion
    //}

    //#endregion

    #region FileHelper Cloud

    public class FileHelper
    {
        #region Properties
        public int? FullWidth { get; set; }
        public int? FullHeight { get; set; }
        public string ImageName { get; set; }
        public string ImageThumbName { get; set; }
        public int? ThumbWidth { get; set; }
        public int? ThumbHeight { get; set; }
        public string DirPath { get; set; }
        public string DirTempPath { get; set; }
        public string PathFile { get; set; }
        public string FileType { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMsg { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<string> listFile { get; set; }
        #endregion

        public BlobStorageService Blob = new BlobStorageService();

        public bool DownLoadImage(string DesinationPath, string[] filePath)
        {
            var isSuccess = false;
            try
            {
                Blob.DownLoadBlobFile(DesinationPath, filePath);
                isSuccess = true;
            }
            catch
            {

            }
            return isSuccess;
        }

        public List<string> ListFileInDir(string dirPath)
        {
            return Blob.ListBlobFile(dirPath);
        }

        public string GetFile(string dirPath)
        {
            return Blob.GetBlobFile(dirPath);
        }
        public bool CheckMaxSize(HttpPostedFileBase file, int MaxSize = 50000)
        {
            if (file.ContentLength > 0 && file.ContentLength < MaxSize)
                return true;
            else
                return false;
        }

        public void UploadImage(string DirPath, HttpPostedFileBase file)
        {
            try
            {
                Blob.UpLoadBlobFile(DirPath, file);
                ImageName = Blob.ImageName;
                IsSuccess = true;
            }
            catch
            {
                IsSuccess = false;
                ErrorMsg = "เกิดข้อผิดพลาดในการอัพโหลดไฟล์";
            }

        }

        public void RemoveImage(string DirPath, string ImageName)
        {
            IsSuccess = false;
            try
            {
                Blob.DeleteBlobFile(Path.Combine(DirPath, ImageName));
                IsSuccess = true;
            }
            catch (Exception ex)
            {
            }
        }

        public void SaveImageFromTemp()
        {
            try
            {
                // clear file in path
                Blob.DeleteBlobInDir(DirPath);
                //   create thumbna
                if (!string.IsNullOrEmpty(ImageThumbName))
                {
                    if (ThumbWidth > 0 || ThumbHeight > 0)
                    {
                        Blob.UploadBlobThumbnail(DirTempPath + "/" + ImageName, DirPath + "/" + ImageThumbName, ThumbWidth, ThumbHeight);
                    }
                    else
                    {
                        Blob.CopyBlob(DirTempPath + "/" + ImageName, DirPath + "/" + ImageThumbName);
                    }
                }
                if (!string.IsNullOrEmpty(ImageName))
                {
                    if (FullWidth > 0 || FullHeight > 0)
                    {
                        Blob.UploadBlobThumbnail(DirTempPath + "/" + ImageName, DirPath + "/" + ImageName, FullWidth, FullWidth);
                    }
                    else
                    {
                        Blob.CopyBlob(DirTempPath + "/" + ImageName, DirPath + "/" + ImageName);
                    }
                }
                Blob.DeleteBlobInDir(DirTempPath);
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                ErrorMsg = "error : " + ex;
                IsSuccess = false;
            }
        }

        public void CreateObjImage(string DirTempPath, string DirPath, string ImageName, string ImageNewName, int? Width, int? Height)
        {
            if (Width > 0 && Height > 0)
            {
                Blob.UploadBlobThumbnail(DirTempPath + "/" + ImageName, DirPath + "/" + ImageNewName, Width, Height);
            }
            else
            {
                Blob.CopyBlob(DirTempPath + "/" + ImageName, DirPath + "/" + ImageName);
            }
        }

        public void DeleteFilesInDir(string DirPath)
        {

            Blob.DeleteBlobInDir(DirPath);
        }

        public bool Exists(string FilePath)
        {
            return Blob.Exists(FilePath);
        }

        public void CreateLogFiles(Exception ex)
        {
            Blob.BlobCreateLogFiles(ex);
        }
    }
    #endregion

    #region Blob Storage Service
    public class BlobStorageService
    {
        #region default variable
        public string ConnectionString = "storageconnection";
        // public string ConnectionString = "teststorageconnection";
        public string ContainerName = "upload";
        public string root = "upload/";
        #endregion

        #region Property
        public CloudBlockBlob BlockBlob { get; set; }
        public MemoryStream MemStream { get; set; }
        #endregion

        #region Private Method
        private string RandomCharecter(int Size)
        {
            Random ran = new Random();
            string chars = "ABCDEFGHIJKLMNOPQESTUVWXYZ0123456789";
            char[] buffer = new char[Size];
            for (int i = 0; i < Size; i++)
            {
                buffer[i] = chars[ran.Next(chars.Length)];
            }
            return new string(buffer);
        }
        private string GetFileType(string name)
        {
            string[] arrStr = name.Split('.');
            return arrStr[arrStr.Length - 1];
        }
        #endregion

        #region Method



        #region List
        public List<string> ListBlobFile(string dirPath)
        {
            CloudBlobContainer blobContainer = GetCloudBlobContainer();

            CloudBlobDirectory dir = GetCloudBlobDir(dirPath);

            // Retrieve reference to a previously created container. 
            // Loop over items within the container and output the length and URI.


            List<string> blobs = new List<string>();

            // Loop over blobs within the container and output the URI to each of them
            foreach (var blobItem in dir.ListBlobs().ToList())
            {
                blobs.Add(blobItem.Uri.AbsoluteUri);
            }

            return blobs;
        }

        public string GetBlobFile(string pathFile)
        {
            CloudBlobContainer blobContainer = GetCloudBlobContainer();
            var blob = blobContainer.GetBlockBlobReference(pathFile);
            // Loop over blobs within the container and output the URI to each of them  

            return blob.Uri.AbsoluteUri;
        }
        #endregion

        #region Delete
        public void DeleteBlobInDir(string dirPath)
        {
            CloudBlobContainer blobContainer = GetCloudBlobContainer();

            CloudBlobDirectory dir = GetCloudBlobDir(dirPath);
            // Loop over blobs within the container and output the URI to each of them

            foreach (var blobItem in dir.ListBlobs())
            {
                var abPath = blobItem.Uri.AbsolutePath;
                if (blobItem.Uri.AbsolutePath.StartsWith("/"))
                {
                    abPath = abPath.Substring(8, abPath.Length - 8);
                }
                DeleteBlobFile(abPath);
            }
        }

        public bool Exists(string path)
        {
            CloudBlobContainer blobContainer = GetCloudBlobContainer();
            var blob = blobContainer.GetBlockBlobReference(path);
            return Exists(blob);
        }

        public bool Exists(CloudBlockBlob blob)
        {
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (StorageException e)
            {
                //   BlobCreateLogFiles(e);
                return false;
            }

        }
        public void DeleteBlobFile(string filePath)
        {
            CloudBlobContainer container = GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);
            try
            {
                if (Exists(blockBlob))
                {
                    blockBlob.Delete();
                }
            }
            catch (Exception ex)
            {
            }

        }

        #endregion

        #region UpLoad
        public string ImageName { get; set; }
        public bool IsSuccess { get; set; }
        public void UpLoadBlobFile(string DirPath, HttpPostedFileBase file)
        {
            var type = GetFileType(file.FileName);
            var ContentType = file.ContentType;
            ImageName = RandomCharecter(15) + "." + type;
            if (file.ContentLength > 0)
            {
                // Retrieve a reference to a container 
                CloudBlobContainer blobContainer = GetCloudBlobContainer();
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(DirPath + "/" + ImageName);

                #region Check Exist File Name
                while (Exists(blob))
                {
                    ImageName = RandomCharecter(15) + "." + GetFileType(file.FileName);
                    blob = blobContainer.GetBlockBlobReference(DirPath + "/" + ImageName);
                }
                #endregion

                // Create or overwrite the "myblob" blob with contents from a local file 
                blob.Properties.ContentType = ContentType;
                blob.UploadFromStream(file.InputStream);

            }
        }


        public void UploadBlobThumbnail(string pathFileName, string pathFileThumb, int? maxWidth, int? maxHeight)
        {
            CloudBlobContainer blobContainer = GetCloudBlobContainer();

            CloudBlockBlob content = blobContainer.GetBlockBlobReference(pathFileName);
            CloudBlockBlob thumbnail = blobContainer.GetBlockBlobReference(pathFileThumb);

            MemoryStream image = new MemoryStream();

            content.DownloadToStream(image);

            image.Seek(0, SeekOrigin.Begin);
            Stream img = CreateThumbnail(image, maxWidth, maxHeight);

            thumbnail.Properties.ContentType = content.Properties.ContentType;
            thumbnail.UploadFromStream(img);


        }
        #endregion

        #region DownLoad
        public bool DownLoadBlobFile(string destPath, string filePath)
        {
            CloudBlobContainer blobContainer = GetCloudBlobContainer();
            var isSuccess = false;
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filePath);
            string pathLocal = @"C:\DownLoad\";
            System.IO.DirectoryInfo dir = new DirectoryInfo(pathLocal + filePath);
            if (!dir.Exists)
                dir.Create();

            using (var fileStream = System.IO.File.OpenWrite(pathLocal + filePath))
            {
                blob.DownloadToStream(fileStream);
            }
            return isSuccess;

        }

        public bool DownLoadBlobFile(string destPath, string[] filePath)
        {
            var isSuccess = false;

            CloudBlobContainer blobContainer = GetCloudBlobContainer();
            for (var i = 0; i < filePath.Count(); i++)
            {
                // Retrieve a reference to a container 
                isSuccess = DownLoadBlobFile(destPath, filePath[i]);
            }

            return isSuccess;
        }

        #endregion

        #region Copy
        public void CopyBlob(string fromFilePath, string descFilePath)
        {
            // Get a reference to the source blob.

            CloudBlobContainer blobContainer = GetCloudBlobContainer();
            // fromPathFile => "mycontainer/myblob.txt"
            CloudBlockBlob sourceBlob = blobContainer.GetBlockBlobReference(fromFilePath);
            sourceBlob.FetchAttributes();

            // Get a reference to the destination blob.
            CloudBlockBlob destBlob = blobContainer.GetBlockBlobReference(descFilePath);

            try
            {
                // Copy source blob to destination blob.
                destBlob.StartCopyFromBlob(sourceBlob);
                destBlob.FetchAttributes();
                IsSuccess = true;
            }
            catch (StorageException e)
            {
                IsSuccess = false;
                Console.WriteLine("Error code: " + e.Message);
            }


        }
        #endregion

        #region Thumbnail
        public Stream CreateThumbnail(Stream input)
        {
            CreateThumbnail(input, 0, 0);
            return input;
        }
        public Stream CreateThumbnail(Stream input, int? maxWidth, int? maxHeight)
        {
            Bitmap orig = new Bitmap(input);

            int width = (maxWidth > 0) ? (int)maxWidth : 128;
            int height = (maxHeight > 0) ? (int)maxHeight : 128;

            if (orig.Width > orig.Height)
            {
                height = width * orig.Height / orig.Width;
            }
            else
            {
                width = height * orig.Width / orig.Height;
            }

            Bitmap thumb = new Bitmap(width, height);
            using (Graphics graphic = Graphics.FromImage(thumb))
            {
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphic.DrawImage(orig, 0, 0, width, height);
                MemoryStream ms = new MemoryStream();
                thumb.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }
        #endregion

        #endregion

        #region Base
        public CloudBlobContainer GetCloudBlobContainer()
        {

            // Retrieve storage account from connection-string
            //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            //CloudConfigurationManager.GetSetting("StorageConnectionString"));
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
    CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container 

            CloudBlobContainer blobContainer = blobClient.GetContainerReference(ContainerName);
            blobContainer.CreateIfNotExists();

            blobContainer.SetPermissions(
             new BlobContainerPermissions
             {
                 PublicAccess = BlobContainerPublicAccessType.Blob
             });

            return blobContainer;
        }

        public CloudBlobDirectory GetCloudBlobDir(string path)
        {
            // Retrieve storage account from connection-string
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
        CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client 

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            CloudBlobDirectory blobDir = container.GetDirectoryReference(path);
            // Retrieve a reference to a CloudBlobDirectory 

            //  CloudBlobDirectory blobDir = blobClient.GetBlobDirectoryReference(path);
            //  CloudBlobDirectory blobDir = bl;
            //modify



            return blobDir;
        }
        #endregion


        public void BlobCreateLogFiles(Exception ex, string url = "", string compname = "")
        {
            string strSubFolder = DateTime.Now.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            string ts = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string filePath = "Logs/ErrorLog/" + strSubFolder + "/" + ts + ".txt";

            #region Create Log
            CloudBlobContainer blobContainer = GetCloudBlobContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filePath);

            string logText = "";
            string oldContent = "";

            if (!Exists(blob))
            {
                oldContent = "";
            }
            else
            {
                #region Read Log File
                using (StreamReader reader = new StreamReader(blob.OpenRead()))
                {
                    oldContent = reader.ReadToEnd();
                }
                #endregion
            }
            #region Write Log File
            using (StreamWriter writer = new StreamWriter(blob.OpenWrite()))
            {
                writer.Write(oldContent);
                #region text log file
                logText += "\r\n ";
                logText += "+ Start Error Logs @ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + " \r\n";
                logText += "|- DateTime       : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + " \r\n";

                if (!string.IsNullOrEmpty(compname))
                    logText += "|- From         : " + compname + "  \r\n ";
                else
                    logText += "|- From         :  \r\n ";

                if (!string.IsNullOrEmpty(url))
                    logText += "|- WebUrl         : " + url + "  \r\n ";
                else
                    logText += "|- WebUrl         :  \r\n ";

                logText += "|- Message        : " + ex.Message + "\r\n";
                logText += "|- Source         : " + ex.Source + "\r\n";
                logText += "|- Data     : " + ex.Data.Keys + ex.Data.Values + "\r\n";
                logText += "|- StackTrace     : " + ex.StackTrace + " \r\n ";
                logText += "|- TargetSite     : " + ex.TargetSite + " \r\n ";
                logText += "|- InnerException : " + ex.InnerException + " \r\n ";
                logText += "|- HelpLink       : " + ex.HelpLink + " \r\n ";
                logText += "|- Name           : " + ex.GetType().Name + " \r\n ";
                logText += "|- An error occured : " + ex.GetType().ToString() + " \r\n ";
                logText += "|- FullName           : " + ex.GetType().FullName + " \r\n ";
                logText += "|- End Error Logs @ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + " \r\n ";
                logText += " \r\n";
                logText += "+--------------------------------------------------------------------------------------";
                #endregion
                writer.Write(logText);
            }
            #endregion

            #endregion
        }
    }
    #endregion

}