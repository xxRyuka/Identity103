using Mvc.Identity101.Data.Dto;
using Mvc.Identity101.Services.Data;

namespace Mvc.Identity101.Services.Abstract;

public interface IProfileImageService
{
    //userId kısmını kaldırmak mantık çerçevesinde
    //çünkü onu check etmek için signin manager ve user managera ihtiyac duyacağim
    //burdaki asıl amacim parametredden gelen imgFilenin gerekli kontrollerini sağlayip dogru yere doğru şekilde kaydetmek olacak
    Task<string> SaveImageAsync(string Id,IFormFile file, ImageType ImgType); // bir path dondureceğiz sonucunda 
    Task DeleteImageAsync(string id,string path,ImageType ImgType);
    // Task<string> SaveImageAsync(IFormFile file, string folder, string fileName);
    
}