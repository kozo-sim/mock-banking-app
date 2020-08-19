using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Data
{
    public static class Common
    {
        //copied from https://stackoverflow.com/questions/39990179/regex-for-australian-phone-number-validation
        public const string AUS_PHONE_REGEX = "(\\(+61\\)|\\+61|\\(0[1-9]\\)|0[1-9])?( ?-?[0-9]){6,9}";
    }
}
