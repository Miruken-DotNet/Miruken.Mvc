namespace MajorLeagueMiruken.Api
{
    using System;

    public class Person
    {
        public const string DATE_FORMAT = "MM/DD/YYYY";

        public int      Id        { get; set; }
        public string   FirstName { get; set; }
        public string   LastName  { get; set; }
        public DateTime Birthdate { get; set; }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
                    return null;

                return $"{FirstName} {LastName}";
            }
        }

        public int? Age
        {
            get
            {
                if (Birthdate == DateTime.MinValue)
                    return null;

               return DateTime.Today.Year - Birthdate.Year;
            }
        }
    }
}
