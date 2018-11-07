using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name ="E-mail")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Graph User Role")]
        public IdentityRole AppUserRole { get; set; }

        [Display(Name ="Graph User Role")]
        public SelectList ApplicationRoles { get; set; }

        //new 
        [Required]
        [Display(Name = "Graph User Role")]
        public GrafRole GrafRole { get; set; }

        [Display(Name = "Graph User Role")]
        public SelectList GrafRoles { get; set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
    }
}
