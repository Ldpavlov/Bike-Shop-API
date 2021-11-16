﻿namespace MyWebApp_BikeShop.Infrastructure
{
    using AutoMapper;
    using MyWebApp_BikeShop.DATA.Models;
    using MyWebApp_BikeShop.Models.Bikes;
    using MyWebApp_BikeShop.Models.Sellers;
    using MyWebApp_BikeShop.Services.Bikes.Models;
    using MyWebApp_BikeShop.Services.Sellers;

    public class MappingProfile : Profile
    {        
        public MappingProfile()
        {
            this.CreateMap<AllBikeServiceModel, BikeListingViewModel>();
            this.CreateMap<AddBikeFormModel, AddBikeServiceModel>();

            this.CreateMap<BecomeSellerFormModel, BecomeSellerServiceModel>();
            this.CreateMap<Bike, DetailsServiceModel>();
            this.CreateMap<DetailsServiceModel, AddBikeServiceModel>();

        }
    }
}
