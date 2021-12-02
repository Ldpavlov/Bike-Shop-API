namespace MyWebApp_BikeShop.Services.Bikes
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyWebApp_BikeShop.DATA;
    using MyWebApp_BikeShop.DATA.Models;
    using MyWebApp_BikeShop.Services.Bikes.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BikeService : IBikeService
    {
        private readonly BikeShopDbContext data;
        private readonly IConfigurationProvider mapper;

        public BikeService(BikeShopDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper.ConfigurationProvider;
        }

        public async Task<IEnumerable<AllBikeServiceModel>> AllBikes()
            => await data.Bikes
                .OrderByDescending(c => c.Id)
                .Include(c => c.Category)
                .Include(c => c.Seller)
                .Select(c => new AllBikeServiceModel()
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    ImageUrl = c.ImageUrl,
                    Year = c.Year,
                    Description = c.Description,
                    Category = c.Category.Name,
                    UserId = c.Seller.UserId
                })
            .ToListAsync();

        public string GetCategoryName(int id)
            => data.Categories.FirstOrDefault(c => c.Id == id).Name;

        public async Task<int> GetSellerId(string userId)
            => await this.data
                .Sellers
                .Where(s => s.UserId == userId)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

        public void Add(AddBikeServiceModel bike)
        {
            var bikeData = new Bike
            {
                Brand = bike.Brand,
                Model = bike.Model,
                Description = bike.Description,
                ImageUrl = bike.ImageUrl,
                CategoryId = bike.CategoryId,
                Year = bike.Year,
                SellerId = bike.SellerId
            };

            this.data.Bikes.Add(bikeData);
            this.data.SaveChanges();
        }

        public bool CheckCategoryId(int id)
            => this.data.Categories.Any(c => c.Id == id);

        public int TotalBikes()
            => this.data.Bikes.Count();

        public IEnumerable<string> Brands()
            => this.data
                .Bikes
                .Select(b => b.Brand)
                .Distinct()
                .OrderBy(br => br)
                .ToList();

        public async Task<IEnumerable<BikeCategoryServiceModel>> GetAllCategories() 
            => await this.data
                .Categories
                .Select(c => new BikeCategoryServiceModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
            .ToListAsync();


        public DetailsServiceModel Details(int id)
            => this.data
                .Bikes
                .Where(b => b.Id == id)
                .ProjectTo<DetailsServiceModel>(this.mapper)
                .FirstOrDefault();
       
        public bool Edit(int id,
            string brand, 
            string model, 
            string description, 
            string imageUrl, 
            int year, 
            int categoryId)
        {
            var bike = this.data.Bikes.Find(id);

            if(bike == null)
            {
                return false;
            }

            bike.Brand = brand;
            bike.CategoryId = categoryId;
            bike.Model = model;
            bike.Description = description;
            bike.ImageUrl = imageUrl;
            bike.Year = year;

            this.data.SaveChanges();

            return true;
        }

        public async Task<string> GetUserId(int id) 
            =>  await this.data
               .Bikes
               .Include(b => b.Seller)
               .Where(b => b.Id == id)
               .Select(b => b.Seller.UserId)
               .FirstOrDefaultAsync();

        public bool IsSeller(int bikeId, int sellerId)
            => this.data
                .Bikes
                .Any(c => c.Id == bikeId && c.SellerId == sellerId);


    }
}
