namespace WpfTestApp.Features.About
{
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;

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
