
namespace Discounts.Application.Exceptions
{
    public class SoldOutException : BadRequestException
    {
        public SoldOutException(string offerTitle)
            : base($"The offer '{offerTitle}' is completely sold out.")
        {
        }
    }
}
