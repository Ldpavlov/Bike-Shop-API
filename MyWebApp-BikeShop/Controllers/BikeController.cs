﻿namespace MyWebApp_BikeShop.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyWebApp_BikeShop.DATA;
    using MyWebApp_BikeShop.Infrastructure;
    using MyWebApp_BikeShop.Models;
    using MyWebApp_BikeShop.Models.Bikes;
    using MyWebApp_BikeShop.Services.Bikes;
    using MyWebApp_BikeShop.Services.Bikes.Models;
    using MyWebApp_BikeShop.Services.Sellers;
    using System.Linq;
    using System.Threading.Tasks;

    public class BikeController : Controller
    {
        private readonly BikeShopDbContext data;
        private IBikeService bikeService;
        private ISellersService sellerService;
        private readonly IMapper mapper;
        private readonly IConfigurationProvider selectionMapper;

        public BikeController(BikeShopDbContext data, IBikeService bikeService, IMapper mapper, ISellersService sellersService)
        {
            this.data = data;
            this.bikeService = bikeService;
            this.mapper = mapper;
            this.selectionMapper = mapper.ConfigurationProvider;
            this.sellerService = sellersService;
        }

        public async Task<IActionResult> All([FromQuery] AllBikesViewModel query)
        {
            ViewBag.UserId = this.User.GetId();            

            var bikesQuery = await bikeService.AllBikes();

            if (!string.IsNullOrEmpty(query.Brand))
            {
                bikesQuery = bikesQuery.Where(b => b.Brand == query.Brand);
            }

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                bikesQuery = bikesQuery.Where(b =>
                (b.Brand + " " + b.Model).ToLower().Contains(query.SearchTerm.ToLower()) ||
                b.Description.ToLower().Contains(query.SearchTerm.ToLower()));
            }            

            var totalBikes = bikesQuery.Count();

            var bikes = bikesQuery
                .Skip((query.CurrentPage - 1) * AllBikesViewModel.BikesPerPage)
                .Take(AllBikesViewModel.BikesPerPage)
                .AsQueryable()
                .ProjectTo<BikeListingViewModel>(this.selectionMapper);            

            var bikeBrands = bikeService.Brands();

            query.TotalBikesCount = totalBikes;
            query.Brands = bikeBrands;
            query.Bikes = bikes;

            return View(query);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            if (!this.sellerService.IsValidSeller(this.User.GetId()))
            {
                return RedirectToAction(nameof(SellersController.Become), "Sellers");
            }

            return View(new AddBikeFormModel
            {
                Categories = await bikeService.GetAllCategories()
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AddBikeFormModel bike)
        {
            var sellerId = await bikeService.GetSellerId(this.User.GetId());

            if (sellerId == 0)
            {
                return RedirectToAction(nameof(SellersController.Become), "Sellers");
            }

            if (!bikeService.CheckCategoryId(bike.CategoryId))
            {
                this.ModelState.AddModelError(nameof(bike.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                bike.Categories = await bikeService.GetAllCategories();
                return View(bike);
            }

            var bikeServiceModel = 
                mapper.Map<AddBikeFormModel, AddBikeServiceModel>(bike);

            bikeServiceModel.SellerId = sellerId;

            bikeService.Add(bikeServiceModel);

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var bike = this.bikeService.Details(id);
            return View(bike);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = this.User.GetId();

            if (!this.sellerService.IsValidSeller(userId))
            {
                return RedirectToAction(nameof(SellersController.Become), "Dealers");
            }

            var bike = this.bikeService.Details(id);

            if(await bikeService.GetUserId(id) != userId)
            {
                return Unauthorized();
            }

            var bikeData = this.mapper.Map<DetailsServiceModel, AddBikeServiceModel>(bike);

            bikeData.Categories = await this.bikeService.GetAllCategories();

            return View(bikeData);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, AddBikeFormModel bike)
        {
            var sellerId = this.sellerService.IdUser(this.User.GetId());

            if(sellerId == 0)
            {
                return RedirectToAction(nameof(SellersController.Become), "Dealers");
            }

            if (!this.bikeService.CheckCategoryId(bike.CategoryId))
            {
                this.ModelState.AddModelError(nameof(bike.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                bike.Categories = await this.bikeService.GetAllCategories();

                return View(bike);
            }

            if (!this.bikeService.IsSeller(id, sellerId))
            {
                return BadRequest();
            }

            var bikeEdited = this.bikeService.Edit(
                id,
            bike.Brand,
            bike.Model,
            bike.Description,
            bike.ImageUrl,
            bike.Year,
            bike.CategoryId);

            if (!bikeEdited)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }

}
