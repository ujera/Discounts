// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Exceptions
{
    public class AlreadyReservedException : BadRequestException
    {
        public AlreadyReservedException()
            : base("You already have an active reservation for this offer.")
        {
        }
    }
}
