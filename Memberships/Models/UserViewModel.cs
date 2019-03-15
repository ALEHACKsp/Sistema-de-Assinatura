using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Memberships.Models
{
    public class UserViewModel
    {
        [Display(Name ="User ID")]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }


        [Required]
        [Display(Name ="First name")]
        [StringLength(30,ErrorMessage =" O {0} não pode ultrapassar o maximo de {1} caracteres")]
        public string FirstName { get; set; }


        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string Password { get; set; }

    }
}