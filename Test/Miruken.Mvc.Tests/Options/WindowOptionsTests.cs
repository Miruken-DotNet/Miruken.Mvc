#if NETFULL
namespace Miruken.Mvc.Tests.Options
{
    using System.Windows.Forms;
    using Mvc.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WindowOptionsTests
    {
        [TestMethod]
        public void Should_Determine_Virtual_Screen()
        {
            var options      = new WindowOptions {FillScreen = ScreenFill.VirtualScreen};
            var frame        = options.GetFrame();
            var virtualFrame = SystemInformation.VirtualScreen;
            Assert.AreEqual(virtualFrame, frame);
        }
    }
}
#endif

