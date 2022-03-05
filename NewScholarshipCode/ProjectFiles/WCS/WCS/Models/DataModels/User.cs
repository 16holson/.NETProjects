using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WCS.Data;

namespace WCS.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class User : IdentityUser, IComparable<User>
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Initial")]
        public string MiddleInitial { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Display(Name = "Last Login")]
        public DateTime LastLogin { get; set; }

        [NotMapped]
        public string Role { get; set; }

        public int? StudentProfileId { get; set; }

        public int CompareTo(User other)
        {
            if (other.Role.Equals(Role))
            {
                if (other.LastName.Equals(LastName))
                    return FirstName.CompareTo(other.FirstName);
                else
                    return LastName.CompareTo(other.LastName);
            }
            else
                return Role.CompareTo(other.Role);
        }
    }
}
