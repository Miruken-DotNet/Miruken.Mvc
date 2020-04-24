namespace Miruken.Mvc.Wpf.TestApp.Features.About
{
    using Mvc;
    using Mvc.Animation;

    public class AboutController: Controller
    {
        private bool _back;

        public void About()
        {
            Show<About>().Disposed += EndContext;
        }

        public void AboutBack()
        {
            _back = true;
            Show<About>();
        }

        public void Back()
        {
            if (_back)
                GoBack(IO.Push(Origin.MiddleRight));
            else
                GoBack();
        }
    }
}
