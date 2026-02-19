using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.Models.Merchant
{
    public class CreateOfferViewModel
    {
        [Required(ErrorMessage = "სათაური სავალდებულოა")]
        [Display(Name = "სათაური")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "აღწერა სავალდებულოა")]
        [Display(Name = "აღწერა")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "სურათის ლინკი სავალდებულოა")]
        [Display(Name = "სურათის URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 10000, ErrorMessage = "ფასი უნდა იყოს 0-ზე მეტი")]
        [Display(Name = "საწყისი ფასი")]
        public decimal OriginalPrice { get; set; }

        [Display(Name = "ფასდაკლებული ფასი")]
        public decimal DiscountPrice { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "კუპონების რაოდენობა უნდა იყოს 1-დან 1000-მდე")]
        [Display(Name = "კუპონების რაოდენობა")]
        public int CouponsCount { get; set; }

        [Required]
        [Display(Name = "დაწყების თარიღი")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "დასრულების თარიღი")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "გთხოვთ აირჩიოთ კატეგორია")]
        [Display(Name = "კატეგორია")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
