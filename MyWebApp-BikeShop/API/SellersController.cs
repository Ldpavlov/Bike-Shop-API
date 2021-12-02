namespace MyWebApp_BikeShop.API
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyWebApp_BikeShop.DATA;
    using MyWebApp_BikeShop.DATA.Models;
    using MyWebApp_BikeShop.Services.Sellers;
    using System.Linq;

    [Route("api/[controller]")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private ISellersService sellersService;
        private readonly BikeShopDbContext data;


        public SellersController(ISellersService sellersService, BikeShopDbContext data)
        {
            this.sellersService = sellersService;
            this.data = data;
        }

        [HttpPost]
        [Authorize]
        public IActionResult BecomeSeller()
        {
            var newSeller = new Seller
            {
                Id = 1,
                Name = "Simon Ruud",
                PhoneNumber = "088765354"
            };
            return Ok(newSeller);
        }

        //[HttpGet]
        //[Authorize]
        //public IActionResult IsValidSeller(string userId)
        //{
        //    bool isUserValid = this.data.Sellers.Any(s => s.UserId = userId);

        //    return Ok();
        //}


    }
}
