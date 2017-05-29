namespace Miruken.Mvc.Console
{
    using System;
    using System.Threading.Tasks;

    public class MenuItem
    {
        public MenuItem(string text, ConsoleKey key, Func<Task> action)
        {
            Text   = text;
            Key    = key;
            Action = action;
        }

        public string     Text   { get; set; }
        public ConsoleKey Key    { get; set; }
        public Func<Task> Action { get; set; }
    }
}