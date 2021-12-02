namespace MyWebApp_BikeShop.Services.Bikes
{
    using MyWebApp_BikeShop.Services.Bikes.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBikeService
    {
        public Task<IEnumerable<AllBikeServiceModel>> AllBikes();
        public string GetCategoryName(int id);
        public Task<int> GetSellerId(string userId);
        public void Add(AddBikeServiceModel bike);
        public bool CheckCategoryId(int id);
        public Task<int> TotalBikes();
        public IEnumerable<string> Brands();
        public Task<IEnumerable<BikeCategoryServiceModel>> GetAllCategories();
        public DetailsServiceModel Details(int id);
        public bool Edit(int id,string brand, string model, string description, string imageUrl, int year, int categoryId);
        public Task<string> GetUserId(int id);
        public bool IsSeller(int bikeId, int sellerId);
    }
}
