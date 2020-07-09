using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing.Imaging;
 
using System.Collections;
using System.Drawing;

namespace Prosoft.Service
{ 

    public class FileHelper : Controller
    {
        #region Member
        private string root = "~/Upload/";
        private string _DirPath;
        private string _DirTempPath;
        private string _PathFile;

        public int? FullWidth { get; set; }
        public int? FullHeight { get; set; }
        public string ImageName { get; set; }
        public string ImageThumbName { get; set; }
        public int? ThumbWidth { get; set; }
        public int? ThumbHeight { get; set; }
        public string DirPath
        {
            get
            {
                return _DirPath;
            }
            set
            {
                _DirPath = Path.Combine(root, value);
            }
        }
        public string DirTempPath
        {
            get
            {
                return _DirTempPath;
            }
            set
            {
                _DirTempPath = Path.Combine(root, value);
            }
        }
        public string PathFile
        {
            get
            {
                return _PathFile;
            }
            set
            {
                _PathFile = Path.Combine(root, value);
            }
        }
        public string FileType { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMsg { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<string> listFile { get; set; }


        #endregion

        #region Methods
        #region Private Method
        private void CreateFolder(string path)
        {
            DirectoryInfo DirInfo = new DirectoryInfo(MapPath(path));
            if (!DirInfo.Exists) { DirInfo.Create(); }
        }

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
            return arrStr[arrStr.Length-1];
        }

        private string MapPath(string path)
        {
            return System.Web.HttpContext.Current.Server.MapPath(path);
        }
        #endregion

        #region Image Methods
        public string ReNameFile(string PathFile)
        {
            string dir = Path.GetDirectoryName(PathFile);
            string name = Path.GetDirectoryName(PathFile);
            string newName = RandomCharecter(15) + "." + GetFileType(name);
            var physicalPath = Path.Combine(MapPath(DirPath), name);
            var physicalNewPath = Path.Combine(MapPath(DirPath), newName);

            #region Check Same Old File
            while (System.IO.File.Exists(physicalNewPath))
            {
                ImageName = RandomCharecter(15) + "." + GetFileType(name);
                physicalNewPath = Path.Combine(DirPath, ImageName);
            };
            #endregion

            #region rename
            if (!string.IsNullOrEmpty(physicalPath))
            {
                System.IO.File.Move(physicalPath, physicalNewPath);
            }
            #endregion

            // return ชื่อไฟล์ใหม่
            return Path.GetFileName(physicalNewPath);
        } 

        #region Validate
        public bool ValidateImage(int MaxSize,int MaxWidth,int MaxHeight,string PathFile)
        { 
            if (File != null)
            {
                #region Check File Size
                int size = File.ContentLength; 
                int maxSize = (MaxSize > 0) ? MaxSize : 150000;
                if (size < maxSize)
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                    ErrorMsg += " ขนาดไฟล์มีขนาดใหญ่เกินกว่ากำหนด | ";
                }
                #endregion

                #region Check File Type
                string strType = File.ContentType.ToLower();
                if (strType == "image/jpeg" || strType == "image/gif" || strType == "image/png")
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                    ErrorMsg += "ประเภทไฟล์ไม่ถูกต้อง | ";
                }
                #endregion

                #region Check File Widht Height
                if (MaxWidth > 0 || MaxHeight > 0)
                { 
                    System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(Path.Combine(MapPath(PathFile)));

                    #region check ความสูง
                    if (MaxHeight > 0)
                    {
                        if (objGraphic.Height <= MaxHeight)
                        {
                            IsSuccess = true;
                        }
                        else
                        {
                            IsSuccess = false;
                            ErrorMsg += "ขนาดไฟล์มี ความกว้างเกินกว่ากำหนด";
                        }
                    }
                    #endregion

                    #region check ความกว้าง
                    if (MaxWidth > 0)
                    {
                        if (objGraphic.Width <= MaxWidth)
                        {
                            IsSuccess = true;
                        }
                        else
                        {
                            IsSuccess = false;
                            ErrorMsg += "ขนาดไฟล์มี ความสูงเกินกว่ากำหนด";
                        }
                    }
                    #endregion
                } 
                #endregion
            }
            else
            {
                IsSuccess = false;
                ErrorMsg += "ไม่พบไฟล์ | ";
            }

            return IsSuccess;
        }

        #endregion

        #region Exists
        /// <summary>
        /// ตรวจสอบว่ามีไฟล์นี้อยู่หรือเปล่า
        /// </summary>
        /// <param name="PathFile">ตัวอย่าง ~/Upload/Company/img.jpg</param>
        /// <returns> boolean</returns>
        public bool Exists(string PathFile)
        {
            PathFile = Path.Combine(MapPath(PathFile)); 
            return System.IO.File.Exists(PathFile);
        }
        #endregion

        #region Exists
        /// <summary>
        /// ตรวจสอบว่ามี Directory นี้อยู่หรือเปล่า
        /// </summary>
        /// <param name="PathFile">ตัวอย่าง ~/Upload/Company </param>
        /// <returns></returns>
        public bool ExistsDirectory(string DirPath)
        {
            DirPath = Path.Combine(MapPath(DirPath));
            return System.IO.File.Exists(DirPath);
        }
        #endregion

        #region ListFile
        public void ListFileInDir()
        {
            DirectoryInfo DirInfo = new DirectoryInfo(MapPath(DirPath)); 
            if (DirInfo.Exists)
            {
                foreach (var f in DirInfo.GetFiles())
                {
                    listFile.Add(f.FullName);
                }
              
            }
        }
        #endregion

        #region Upload
        /// <summary>
        ///  upload image setค่า DirPath , File ก่อน เรียกใช้ Method นี้
        /// </summary>
        /// <param name="DirPath">directory path เช่น ~/Upload/Company</param>
        /// <param name="File">ส่ง file เข้ามา</param>
        public void UploadImage(string DirPath,HttpPostedFileBase File)
        {
            DirPath = root + DirPath;
            var type = GetFileType(File.FileName);
            CreateFolder(DirPath);
            try
            {
                ImageName = RandomCharecter(15) + "." + type;
                var physicalPath = Path.Combine(MapPath(DirPath), ImageName);

                #region Check Same Old File 
                while (System.IO.File.Exists(physicalPath))
                {
                    ImageName = RandomCharecter(15) + "." + GetFileType(File.FileName);
                    physicalPath = Path.Combine(MapPath(DirPath), ImageName); 

                };
                #endregion

                File.SaveAs(physicalPath);
                IsSuccess = true;
            }
            catch
            {
                IsSuccess = false;
                ErrorMsg = "เกิดข้อผิดพลาดในการอัพโหลดไฟล์";
            }
             
        }
        #endregion

        #region Remove
        /// <summary>
        /// ลบรูป
        /// </summary>
        /// <param name="DirPath">directory path เช่น ~/Upload/Company</param>
        /// <param name="ImageName">ชื่อรูป เช่น img-prosoft.jpg</param>
        public void RemoveImage(string DirPath,string ImageName)
        {
            try{  
                    var fileName = Path.GetFileName(ImageName);
                    var physicalPath = Path.Combine(MapPath(DirPath), fileName);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }

                    IsSuccess = true; 

            }catch{
                    IsSuccess = false;
                    ErrorMsg = "เกิดข้อผิดพลาดในการลบไฟล์";
            }
             
        }
        #endregion

        #region Copy
        /// <summary>
        /// copy file
        /// </summary>
        /// <param name="dirFromPath">กรอก  directory path ต้นทาง เช่น ~/Upload/Temp/Company</param>
        /// <param name="dirTargetPath">กรอก directory path ปลายทาง เช่น ~/Upload/Company </param>
        /// <param name="Name">ชื่อไฟล์</param>
        public void CopyFile(string dirFromPath, string dirTargetPath, string Name)
        {
            var physicalFromPath = Path.Combine(MapPath(dirFromPath), Name);
            var physicalTargetPath = Path.Combine(MapPath(dirTargetPath), Name);

            #region Copy File
            System.IO.File.Copy(physicalFromPath, physicalTargetPath, true);
            #endregion
        }

        /// <summary>
        /// copy file
        /// </summary>
        /// <param name="dirFromPath">กรอก path file  ต้นทาง  เช่น ~/Upload/Temp/Company/img.jpg </param>
        /// <param name="dirTargetPath">กรอก path file ปลายทาง เช่น ~/Upload/Company/img.jpg </param>
        public void CopyFile(string physicalFromPath, string physicalTargetPath)
        {
            #region Copy File
            physicalFromPath = Path.Combine(MapPath(physicalFromPath));
            physicalTargetPath = Path.Combine(MapPath(physicalTargetPath));
            System.IO.File.Copy(physicalFromPath, physicalTargetPath, true);
            #endregion
        }
        #endregion

        #region
        /// <summary>
        /// copy file ข้างใน directory มาทัังหมด
        /// </summary>
        /// <param name="dirFromPath">กรอก path file  ต้นทาง เช่น ~/Upload/Temp/Company </param>
        /// <param name="dirTargetPath">กรอก path file ปลายทาง เช่น ~/Upload/Temp/Company  </param>
        private void CopyDirAllFile(string dirFromPath, string dirTargetPath)
        {
            List<string> files = new List<string>();
            List<string> dir = new List<string>();
            DirectoryInfo DirInfo = new DirectoryInfo(MapPath(dirFromPath));
            if (DirInfo.Exists)
            {
                foreach (var f in DirInfo.GetFiles())
                {  
                    files.Add(Path.GetFileName(f.FullName));
                }
                foreach (string file in files)
                {
                    CopyFile(dirFromPath, dirTargetPath, file);
                }
                IsSuccess = true;
            }
        }
        #endregion


        #region CreatImageFromTemp

        #region CreateObjImage
        public void CreateObjImage(string DirFromPath, string DirTargetPath, string from_NameImg,string to_NameImg, int Width, int Height)
        { 
            #region Create
            CreateObjImage(DirFromPath + "/" + from_NameImg, DirTargetPath + "/" + to_NameImg, Width, Height);
            #endregion

        }

        private void CreateObjImage(string TempPathFile, string PathFile, int Width, int Height)
        {
            CreateFolder(Path.GetDirectoryName(PathFile));
            var physicalTempPath = Path.Combine(MapPath(TempPathFile));
            var physicalPath = Path.Combine(MapPath(PathFile));
            System.Drawing.Image objGraphic = System.Drawing.Image.FromFile(physicalTempPath);
            Bitmap objBitmap;

            #region Create
            if (Width > 0 || Height > 0)
            {
                #region Resize
                if (objGraphic.Width > Width || objGraphic.Height > Height)
                {
                    if (objGraphic.Width > objGraphic.Height)
                    {
                        float ratio = ((float)objGraphic.Height / (float)objGraphic.Width) * (float)Width;
                        objBitmap = new Bitmap(objGraphic, Width, (int)ratio);
                    }
                    else
                    {
                        float ratio = ((float)objGraphic.Width / (float)objGraphic.Height) * (float)Height;
                        objBitmap = new Bitmap(objGraphic, (int)ratio, Height);
                    }
                }
                else
                {
                    objBitmap = new Bitmap(objGraphic);
                }
                #endregion
            }
            else
            {
                objBitmap = new Bitmap(objGraphic);
            }

            objBitmap.Save(physicalPath, objGraphic.RawFormat);

            #region return Null and Close
            objGraphic.Dispose();
            objBitmap = null;
            objGraphic = null;
            #endregion

            #endregion
        }
        #endregion

        #region
        /// <summary>
        /// save image โดยเอาไฟล์จาก directory temp มา
        /// ต้องกำหนด DirTempPath,DirPath,ImageName,FullWidth,FullHeight
        /// ถ้ามี thumb file ด้วยก็ต้อง หนด ImageThumbName,ThumbWidth,ThumbHeight เพิ่มเข้ามาครับ
        /// 
        /// </summary>
        public void SaveImageFromTemp()
        {
            var physicalOldPath = Path.Combine(MapPath(DirTempPath), ImageName);
            var physicalPath = Path.Combine(MapPath(DirPath),ImageName);
            var physicalPathThumb = Path.Combine(MapPath(DirPath),"Thumb_"+ ImageName); 

            if (System.IO.File.Exists(physicalOldPath))
            {
                #region Create Thumb File
                if (ThumbWidth > 0)
                {
                    CreateObjImage(DirTempPath + "/" + ImageName, DirPath + "/" + "Thumb_" + ImageName, (int)ThumbWidth, (int)ThumbHeight);
                }
                #endregion

                #region Create File
                CreateObjImage(DirTempPath, DirPath, ImageName, ImageName , (int)FullWidth, (int)FullHeight);
                #endregion
            }
        }
        #endregion
        #endregion
        /// <summary>
        /// ลบไฟล์ใน directory ทั้งหมด
        /// </summary>
        /// <param name="dirTarget">ตัวอย่าง ~/Upload/Temp/Company</param>
        public void DeleteFilesInDir(string dirTarget)
        {
            List<string> files = new List<string>();
            List<string> dir = new List<string>();
            DirectoryInfo DirInfo = new DirectoryInfo(MapPath(root+dirTarget));
            if (DirInfo.Exists)
            {
                foreach (var f in DirInfo.GetFiles())
                {
                    files.Add(f.FullName);
                }
                foreach (string file in files)
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                }
                IsSuccess = true;
            }
          
        }

        #endregion

        #region VDO Method
        public void ValidateVDO(int MaxSize)
        {
            IsSuccess = false;
        }
        #endregion

        #endregion
    }
}