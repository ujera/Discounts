

namespace Discounts.Application.DTOs.Common
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ActiveOffersCount { get; set; }
    }
}
