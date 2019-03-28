using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Memberships.Models
{
    public class RegisterUserModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email  { get; set; }


        [Required]
        [StringLength(30,ErrorMessage ="O {0} deve ser maior que {1} caracter",MinimumLength =2)]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required]
        [StringLength(100)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool AceitarTermoDeUso { get; set; }
    }
}