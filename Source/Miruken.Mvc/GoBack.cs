namespace Miruken.Mvc
{
    public class GoBack
    {
        private object _result;

        public void SetResult(object result)
        {
            _result = result;
        }

        public object ClearResult()
        {
            var result = _result;
            _result = null;
            return result;
        }
    }
}
