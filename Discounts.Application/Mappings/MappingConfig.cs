// Copyright (C) TBC Bank. All Rights Reserved.

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
        }
    }
}
