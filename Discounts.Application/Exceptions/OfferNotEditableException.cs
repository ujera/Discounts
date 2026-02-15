// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Exceptions
{
    public class OfferNotEditableException : BadRequestException
    {
        public OfferNotEditableException(string reason)
            : base($"Offer cannot be edited: {reason}")
        {
        }
    }
}
