using Mvc.Identity101.Services.Abstract;
using Mvc.Identity101.Services.Data;

namespace Mvc.Identity101.Services.Concrete;

public class ProfileImageService : IProfileImageService
{
    private readonly IWebHostEnvironment _environment;

    public ProfileImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }


    public async Task DeleteImageAsync(string id, string relativePath, ImageType imgType)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return;

        // relativePath başındaki / kaldır, '/' karakterlerini OS uygun yapıya dönüştür
        var filePath = Path.Combine(_environment.WebRootPath,
            relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }


    public async Task<string> SaveImageAsync(string userId, IFormFile file, ImageType ImgType)
    {
        //  simdi eger bir profil fotosu var ise imageTypei Profil fotosu ise ayri bir kontrol yapacağiz buda su oalcak
        // / eski fotoyu sileceğiz ondan sonra ekleyeceğiz egerki imageType.Profile fotosu ise 

        bool haveOld = false;
        string OldImgPath = null;
        if (ImgType == ImageType.ProfilePhoto)
        {
            try
            {
                var UploadPath =
                    Path.Combine(_environment.WebRootPath, "uploads", $"{ImgType}", userId); //her id için yeni klasor ?
                if (Directory.Exists(UploadPath))
                {
                    haveOld = true;
                    var files = Directory.GetFiles(UploadPath);
                    // Directory.Delete(UploadPath+"/", true);

                    foreach (var imgFile in files)
                    {
                        File.Delete(imgFile);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        try
        {
            // userId yi controllerde check edebiliriz aslında buraya signin manager eklemeyi pek mantıklı bulmadım acıkcası 
            // burda sadece bir dosya pathi dondureceğim
            if (file != null)
            {
                if (file.Length != 0)
                {
                    if (string.IsNullOrWhiteSpace(file.FileName))
                    {
                        return "error";
                    }
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", "gif" };
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new Exception("Invalid image extension");
                }

                var UploadPath =
                    // "wwwroot"
                    Path.Combine(_environment.WebRootPath, "uploads", $"{ImgType}", userId); //her id için yeni klasor ?
                if (!Directory.Exists(UploadPath))
                    Directory.CreateDirectory(UploadPath);
                // ismi guid yapmazsan path traversal ile büyük sıkıntı altına girebilirsin
                // var guidName = Guid.NewGuid().ToString() + extension; // burda acik biraktim 
                var guidName = file.FileName;

                var filePath = Path.Combine(UploadPath, guidName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                //vtye kaydedecğeimiz path 
                var RelativePath = Path.Combine("uploads", $"{ImgType}", userId, guidName).Replace("\\", "/");
                return "/" + RelativePath;
            }
            return "error";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}