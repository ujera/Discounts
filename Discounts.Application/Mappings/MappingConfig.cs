// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Merchant;
using Discounts.Application.DTOs.Offer;
using Discounts.Domain.Entities;
using Mapster;

namespace Discounts.Application.Mappings
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Offer, OfferDto>.NewConfig()
                .Map(dest => dest.CategoryName, src => src.Category.Name)
                .Map(dest => dest.MerchantName, src => src.Merchant.FirstName)
                .Map(dest => dest.Status, src => src.Status.ToString());

            TypeAdapterConfig<Coupon, MerchantSalesDto>.NewConfig()
                .Map(dest => dest.CustomerName, src => src.Customer.FirstName + " " + src.Customer.LastName)
                .Map(dest => dest.CustomerEmail, src => src.Customer.Email)
                .Map(dest => dest.OfferTitle, src => src.Offer.Title)
                .Map(dest => dest.CouponCode, src => src.Code)
                .Map(dest => dest.SoldAt, src => src.SoldAt)
                .Map(dest => dest.PricePaid, src => src.Offer.DiscountPrice);
        }
    }
}
