// Copyright (C) TBC Bank. All Rights Reserved.
using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.Models.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "ელ. ფოსტა სავალდებულოა")]
        [EmailAddress(ErrorMessage = "არასწორი ელ. ფოსტის ფორმატი")]
        [Display(Name = "ელ. ფოსტა")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "პაროლი სავალდებულოა")]
        [DataType(DataType.Password)]
        [Display(Name = "პაროლი")]
        public string Password { get; set; } = string.Empty;
    }
}
