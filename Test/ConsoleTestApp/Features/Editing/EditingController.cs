namespace ConsoleTestApp.Features.Editing
{
    using Miruken.Concurrency;

    public class EditingController : FeatureController
    {
        public string  Phrase { get; set; } = "Round the rough and rugged rocks...";

        public Promise ShowEditing()
        {
            EditingView view = null;
            Show<EditingView>(v => view = v);
            view.AskQuestions();
            return Promise.Empty;
        }
    }
}
