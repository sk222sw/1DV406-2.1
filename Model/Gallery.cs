using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApplication1.Model
{
    public class Gallery
    {
        public static readonly string PhysicalUploadedImagesPath;
        public static readonly string ThumbnailPath;
        public static readonly Regex ApprovedExtensions;
        public static readonly Regex SantizePath;


        static Gallery()
        {
            PhysicalUploadedImagesPath = Path.Combine(
                AppDomain.CurrentDomain.GetData("APPBASE").ToString(),
                "Content", "images");

            ThumbnailPath = Path.Combine(PhysicalUploadedImagesPath, "thumbnails");
            ApprovedExtensions = new Regex(@"^.*\.(gif|jpg|png)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var invalidChars = new string(Path.GetInvalidFileNameChars());
            SantizePath = new Regex(string.Format("[0]", Regex.Escape(invalidChars)));

        }

        public static IEnumerable<string> GetImageNames()
        {
            var di = new DirectoryInfo(PhysicalUploadedImagesPath);

            return (from fi in di.GetFiles()
                    select fi.Name
                        ).AsEnumerable();
        }

        public IEnumerable<string> GetCachedFiles()
        {
            var files = HttpContext.Current.Cache["Files"] as IEnumerable<string>;
            if (files == null)
            {
                files = GetImageNames();
                HttpContext.Current.Cache.Insert("Files", files, null, DateTime.Now.AddMinutes(1),
                    TimeSpan.Zero);
            }

            return files;
        }

        //returnera true eller false beroende på om det redan finns en bild med samma namn
        public static bool ImageExists(string fileName) 
        {
            return File.Exists(Path.Combine(PhysicalUploadedImagesPath, fileName));
        }

        public static string SaveImage(Stream stream, string fileName)
        {
            //kolla om det finns en fil att ladda upp
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                //validera extension på filen
                if (!ApprovedExtensions.IsMatch(fileName))
                {
                    throw new ArgumentException("Otillåten filtyp.");
                }

                var image = System.Drawing.Image.FromStream(stream);

                //kasta undantag om det är fel mime-typ
                if (!IsValidImage(image))
                {
                    throw new ArgumentException("Otillåten MIME-typ");
                }

                //ta bort ogiltiga tecken
                fileName = SantizePath.Replace(fileName, "");
                
                //kör RenameFile om det redan existerar en fil med samma namn
                if (ImageExists(fileName))
                {
                    fileName = RenameFile(fileName);
                }


                //spara
                var saveUrl = Path.Combine(PhysicalUploadedImagesPath, fileName);
                image.Save(saveUrl);
                
                var thumbnail = image.GetThumbnailImage(60, 50, null, System.IntPtr.Zero);
                thumbnail.Save(Path.Combine(ThumbnailPath, fileName));

                return fileName;
            }
            else
            {
                throw new ArgumentException("Ingen fil vald.");
            }

        }

        //validera MIME-typ
        public static bool IsValidImage(Image Image)
        {
            return Image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid ||
                Image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid ||
                Image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid;
        }

        //om det finns en eller flera filer med samma namn loopas de igenom
        //och får ett index-tillägg i slutet.
        //http://stackoverflow.com/questions/13049732/automatically-rename-a-file-if-it-already-exists-in-windows-way
        public static string RenameFile(string fileName)
        {
            var count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string path = Path.GetDirectoryName(fileName);
            string newFileName = fileName;

            while (ImageExists(newFileName))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFileName = tempFileName + extension;
            }

            return newFileName;
        }

    }
}