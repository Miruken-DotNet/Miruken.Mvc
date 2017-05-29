namespace Miruken.Mvc.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Forms;

    public class Window
    {
        private static readonly List<FrameworkElement> frameworkElements = new List<FrameworkElement>();
        private static readonly object                 _updateLock       = new object();

        public static IConsole   Console { get; set; } = new ConsoleWrapper();
        public static bool       Quit    { get; set; } = false;
        public static ViewRegion Region  { get; }      = new ViewRegion();

        static Window()
        {
            Listen();
        }

        public static void Update()
        {
            lock (_updateLock)
            {
                var size = new Size(Console.WindowWidth - 1, Console.WindowHeight - 2);
                var cells = new Cells(size);
                Region.Measure(size);
                Region.Arrange(new Rectangle(new Point(0, 0), size));
                Region.Render(cells);
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(cells.ToString());
            }
        }

        public static void ElementLoaded(FrameworkElement element)
        {
            if (frameworkElements.Contains(element)) return;
            frameworkElements.Add(element);
            Update();
        }

        public static void ElementUnloaded(FrameworkElement element)
        {
            if (!frameworkElements.Contains(element)) return;
            frameworkElements.Remove(element);
            Update();
        }

        private static void Listen()
        {
            Task.Factory.StartNew(() =>
            {
                do
                {
                    var keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.F5:
                            Console.Clear();
                            Update();
                            break;

                        default:
                            if (keyInfo.Modifiers == ConsoleModifiers.Alt && keyInfo.Key == ConsoleKey.Q)
                            {
                                Quit = true;
                                break;
                            }

                            if (!frameworkElements.Any()) continue;

                            //Going to need something better than this eventually
                            var elements = frameworkElements.ToArray();
                            foreach (var element in elements)
                            {
                                element.KeyPressed(keyInfo);
                            }
                            Update();
                            break;
                    }
                } while (!Quit);
            });
        }
    }
}
