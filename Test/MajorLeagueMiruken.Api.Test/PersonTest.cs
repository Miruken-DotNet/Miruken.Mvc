namespace MajorLeagueMiruken.Api.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PersonTest
    {
        [TestMethod]
        public void FullName()
        {
            var person = new Person();
            Assert.IsNull(person.FullName);

            person.FirstName = "A";
            Assert.IsNull(person.FullName);

            person.LastName = "B";
            Assert.AreEqual("A B", person.FullName);
        }

        [TestMethod]
        public void Age()
        {
            var person = new Person();
            Assert.IsNull(person.Age);

            person.Birthdate = new DateTime(2000, 01, 01);
            Assert.AreEqual(17, person.Age);
        }
    }
}
