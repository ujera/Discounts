
namespace Discounts.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }
}
