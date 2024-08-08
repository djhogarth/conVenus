namespace API.Extensions
{
    public static class DateTimeExtensions
    {
      public static int CalculateAge(this DateOnly DateOfBirth)
      {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - DateOfBirth.Year;

        /*check if the person's birthday has
         already happened before the current
         date*/

        if(DateOfBirth > today.AddYears(-age)) age--;
        return age;
      }

    }
}
