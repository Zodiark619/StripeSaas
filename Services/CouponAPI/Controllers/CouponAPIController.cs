using AutoMapper;
using CouponAPI.Data;
using CouponAPI.Models;
using CouponAPI.Models.Dto; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace  CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper map;
        private ResponseDto _response;
        public CouponAPIController(AppDbContext dbContext,IMapper map)
        {
            this.dbContext = dbContext;
            this.map = map;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> obj = dbContext.Coupons.ToList();
                _response.Result = map.Map<IEnumerable<CouponDto>>(obj);
               
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon obj = dbContext.Coupons.First (x=>x.CouponId==id);
                _response.Result = map.Map<CouponDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto Get(string code)
        {
            try
            {
                Coupon obj = dbContext.Coupons.First  (x=>x.CouponCode.ToLower()==code.ToLower());
                if (obj == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = map.Map<CouponDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }
        [HttpPost]
        public ResponseDto Post([FromBody]CouponDto couponDto)
        {
            try
            {
                Coupon obj = map.Map<Coupon>(couponDto);
                dbContext.Coupons.Add(obj   );
                dbContext.SaveChanges();
                _response.Result = map.Map<CouponDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }
        [HttpPut]
        public ResponseDto Put([FromBody]CouponDto couponDto)
        {
            try
            {
                Coupon obj = map.Map<Coupon>(couponDto);
                dbContext.Coupons.Update(obj   );
                dbContext.SaveChanges();
                _response.Result = map.Map<CouponDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }
        [HttpDelete]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = dbContext.Coupons.First(x=>x.CouponId==id);
                dbContext.Coupons.Remove(obj   );
                dbContext.SaveChanges();
              
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }
    }
}
