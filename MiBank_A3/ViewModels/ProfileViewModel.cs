using MiBank_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.ViewModels
{
    public enum profilePostAction
    {
        UPDATE_USER = 1,
        UPDATE_LOGIN = 2
    }
    public class ProfileViewModel
    {
        public Customer Customer { get; set; }
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
        public profilePostAction Action { get; set; }
    }
}
