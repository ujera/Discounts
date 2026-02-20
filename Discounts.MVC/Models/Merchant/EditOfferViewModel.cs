using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.Models.Merchant
{
    public class EditOfferViewModel
    {
        public int Id { get; set; }

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
        [Display(Name = "საწყისი ფასი")]
        public decimal OriginalPrice { get; set; }

        [Required]
        [Display(Name = "ფასდაკლებული ფასი")]
        public decimal DiscountPrice { get; set; }

        [Required]
        [Display(Name = "კუპონების რაოდენობა")]
        public int CouponsCount { get; set; }

        [Required]
        [Display(Name = "დაწყების თარიღი")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "დასრულების თარიღი")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "კატეგორია")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
