namespace Discounts.Domain.Entities
{
    public class SystemSetting
    {
        public int Id { get; set; }

        public int ReservationDurationInMinutes { get; set; } = 30;

        public int MerchantEditWindowInHours { get; set; } = 24;
    }
}
