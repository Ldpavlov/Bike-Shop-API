namespace MyWebApp_BikeShop.Api
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyWebApp_BikeShop.DATA;
    using MyWebApp_BikeShop.DATA.Models;
    using MyWebApp_BikeShop.Models.Bikes;
    using MyWebApp_BikeShop.Services.Bikes;
    using MyWebApp_BikeShop.Services.Bikes.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BikeController : ControllerBase
    {
        private IBikeService bikeService;
        private readonly AutoMapper.IConfigurationProvider selectionMapper;

        public BikeController(IBikeService bikeService, IMapper mapper)
        {
            this.bikeService = bikeService;
            this.selectionMapper = mapper.ConfigurationProvider;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var bikes = (await this.bikeService.AllBikes()).AsQueryable().ProjectTo<BikeListingViewModel>(this.selectionMapper);
            
            return Ok(bikes);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TotalBikes()
        {
            var allBikes = await this.bikeService.TotalBikes();
            return Ok(allBikes);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserId(int id)
        {
            var user = await this.bikeService.GetUserId(id);
            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var allCategories = await this.bikeService.GetAllCategories();
            return Ok(allCategories);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AllBrands()
        {
            var brands = this.bikeService.Brands();
            return Ok(brands);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddBike()
        {
            var bike = new Bike
            {
                Id = 1,
                Brand = "Scott Ronsom",
                Model = "Enduro",
                Year = 1999,
                Description = "Enduro bike for mountain trails",                
            };

            return Ok(bike);
        }

        [HttpGet]
        [Authorize]
        public IActionResult TotalBikesCount()
        {
            var count = this.bikeService.TotalBikes();

            return Ok(count);
        }
    }
}
