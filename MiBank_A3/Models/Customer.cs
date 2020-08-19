using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MiBank_A3.Data;

namespace MiBank_A3.Models
{
    public class Customer
    {
        //properties

        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        [StringLength(50)]
        [Required]
        public string CustomerName { get; set; }

        [StringLength(11)]
        public string TFN { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public State State { get; set; }

        [DataType(DataType.PostalCode)]
        [RegularExpression("\\d\\d\\d\\d", ErrorMessage = "Must be a 4 digit postcode")]
        [StringLength(4)]
        public string PostCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(Common.AUS_PHONE_REGEX, ErrorMessage = "Must be a valid phone number")]
        public string Phone { get; set; }


        //navigation properties

        public List<Account> Accounts { get; set; }
        public Login LoginDetails { get; set; }
    }
}
