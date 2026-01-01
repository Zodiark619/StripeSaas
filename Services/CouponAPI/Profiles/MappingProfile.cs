using AutoMapper;
using CouponAPI.Models;
using CouponAPI.Models.Dto; 

namespace  CouponAPI.Profiles
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<CouponDto, Coupon>().ReverseMap();
        }
    }
}
