using MiBank_A3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{
    public enum State
    {
        VIC = 1,
        NSW = 2,
        QLD = 3,
        SA = 4,
        WA = 5,
        NT = 6,
        TAS = 7
    }
    public class Payee
    {
        //properties

        public int PayeeId { get; set; }

        [Display(Name = "Payee")]
        [Required]
        public string PayeeName { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        public State State { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        [RegularExpression("\\d\\d\\d\\d", ErrorMessage = "Must be a 4 digit postcode")]
        public string Postcode { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(Common.AUS_PHONE_REGEX, ErrorMessage = "Must be a valid phone number")]
        public string Phone { get; set; }


        //navigation properties

        public List<BillPay> Bills { get; set; }
    }
}
