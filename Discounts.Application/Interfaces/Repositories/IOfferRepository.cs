// Copyright (C) TBC Bank. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IOfferRepository : IBaseRepository<Offer>
    {
        // Custom method: Get offers with their Category and Merchant loaded
        Task<Offer?> GetOfferWithDetailsAsync(int id);

        // Custom method: Get all active offers for the homepage
        Task<IEnumerable<Offer>> GetActiveOffersAsync();
    }
}
