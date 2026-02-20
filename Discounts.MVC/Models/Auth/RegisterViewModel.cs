using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.Models.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "სახელი სავალდებულოა")]
        [Display(Name = "სახელი")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "გვარი სავალდებულოა")]
        [Display(Name = "გვარი")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "ელ. ფოსტა სავალდებულოა")]
        [EmailAddress(ErrorMessage = "არასწორი ელ. ფოსტის ფორმატი")]
        [Display(Name = "ელ. ფოსტა")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "პაროლი სავალდებულოა")]
        [StringLength(100, ErrorMessage = "{0} უნდა შეიცავდეს მინიმუმ {2} სიმბოლოს.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "პაროლი")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "გაიმეორეთ პაროლი")]
        [DataType(DataType.Password)]
        [Display(Name = "გაიმეორეთ პაროლი")]
        [Compare("Password", ErrorMessage = "პაროლები არ ემთხვევა ერთმანეთს.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "გთხოვთ აირჩიოთ ანგარიშის ტიპი")]
        [Display(Name = "ანგარიშის ტიპი")]
        public string Role { get; set; } = "Customer";
    }
}
