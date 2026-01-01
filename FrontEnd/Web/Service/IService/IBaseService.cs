 
using  Web.Models;
using  Web.Models.Dto;

namespace  Web.Service.IService
{
    public interface IBaseService
    {



        Task<ResponseDto?> SendAsync(RequestDto requestDto);
    }
}
