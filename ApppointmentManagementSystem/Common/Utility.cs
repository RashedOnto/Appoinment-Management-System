using System.Drawing;
using System.IO;

namespace AppointmentManagementSystem.Common
{

    public static class Utility
    {
        public static IConfiguration _configuration;
        public static string GetBase64(this IFormFile formFile)
        {
            using var memoryStream = new MemoryStream();
            formFile.CopyToAsync(memoryStream);
            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            return base64;
        }
        //public static string SaveFile(this IFormFile formFile)
        //{
        //    string savePath = "";
        //    return savePath;
        //    //using var memoryStream = new MemoryStream();
        //    //formFile.CopyToAsync(memoryStream);
        //    //var base64 = Convert.ToBase64String(memoryStream.ToArray());
        //    //return base64;
        //}
        public static string SaveFile(IFormFile itemFile, string path)
        {
            if (itemFile != null)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fileExt = Path.GetExtension(itemFile.FileName);
                string uniqueFileName = Guid.NewGuid() + fileExt;
                path = Path.Combine(path, uniqueFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    itemFile.CopyTo(stream);
                }

            }
            return path;
        }
        public static byte[] PathToBytes(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            using MemoryStream m = new MemoryStream();
            try
            {
                var imageBytes = File.ReadAllBytes(path);
                return imageBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string PathToBase64(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            using (MemoryStream m = new MemoryStream())
            {
                var source = "";
                byte[] imageBytes = null;
                try
                {
                    imageBytes = File.ReadAllBytes(path);

                }
                catch (Exception ex)
                {
                    return source = "/Images/bg.jpg";
                }
                string base64String = Convert.ToBase64String(imageBytes);
                FileInfo type = new FileInfo(path);
                if (type.Extension == ".docx")
                {
                    source = "data: application / vnd.openxmlformats - officedocument.wordprocessingml.document; base64," + base64String;
                }
                else if (type.Extension == ".pdf")
                {
                    source = "data:application/pdf;base64," + base64String;
                }
                else
                {
                    source = "data: image /" + type.Extension + ";base64," + base64String;
                }

                return source;
            }
        }
        public static string PathToBase64Original(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            using (MemoryStream m = new MemoryStream())
            {
                var source = "";
                byte[] imageBytes = null;
                try
                {
                    imageBytes = File.ReadAllBytes(path);
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;

                }
                catch
                {
                    return null;
                }
               
               
            }
        }
    }

}
