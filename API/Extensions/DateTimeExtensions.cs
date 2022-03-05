using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
      public static int CalculateAge(this DateTime DateOfBirth)
      {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;

        /*check if the person's birthday has
         already happened before the current
         date*/

        if(DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
      }

    }
}
