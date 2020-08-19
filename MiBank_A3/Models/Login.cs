using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{

    public class Login
    {
        //properties

        public int Id { get; set; }

        [Required, StringLength(8)]
        [Display(Name = "Username")]
        public string LoginName { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }

        [Required, StringLength(64)]
        public string PasswordHash { get; set; }

        //navigation properties

        public Customer Customer { get; set; }
    }
}
