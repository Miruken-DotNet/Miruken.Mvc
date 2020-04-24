namespace Miruken.Mvc.Wpf.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Options;
    using WpfScreenHelper;

    [TestClass]
    public class WpfWindowOptionsTests
    {
        [TestMethod]
        public void Should_Determine_Virtual_Screen()
        {
            var options      = new WpfWindowOptions();
            var frame        = options.GetFrame(ScreenFill.VirtualScreen);
            var virtualFrame = SystemInformation.VirtualScreen;
            Assert.AreEqual(virtualFrame, frame);
        }
    }
}

