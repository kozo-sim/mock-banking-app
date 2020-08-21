using System;

namespace MiBank_A3.Models
{
    public class ErrorViewModel
    {
        public int? HttpCode { get; set; }
        public ErrorViewModel(int? httpCode)
        {
            HttpCode = httpCode;
        }
    }
}
