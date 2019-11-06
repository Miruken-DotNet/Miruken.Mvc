namespace Miruken.Mvc.Console.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SelectListTest
    {
        [TestMethod]
        public void GereratesIds()
        {
            var list = new SelectList<Car>(new Car[0], car => {} );

            Assert.AreEqual("A", list.NextId());
            for (var i = 0; i < 25; i++)
            {
                list.NextId();
            }
            Assert.AreEqual("AA", list.NextId());
            for (var i = 0; i < 25; i++)
            {
                list.NextId();
            }
            Assert.AreEqual("AAA", list.NextId());
        }

        [TestMethod]
        public void AppendsIdToTheItem()
        {
            var list = new SelectList<Car>(new []
               {
                   new Car("Honda")
               }, car => {} );

            Assert.IsTrue(list.ToString().Contains("[A] Honda"));
        }


    }

    public class Car
    {
        public string Name { get; set; }

        public Car(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
