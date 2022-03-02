using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class Invite
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        public string Role { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true),
            Display(Name = "Expiration")]
        public DateTime ExpirationDate { get; set; }

        public string InviteCode { get; set; }

        public string Status { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public void GenerateInviteCode()
        {
            this.InviteCode = Guid.NewGuid().ToString();
        }

        public string GenerateEmail()
        {
            return String.Format("Invite Email");
        }
    }
}
