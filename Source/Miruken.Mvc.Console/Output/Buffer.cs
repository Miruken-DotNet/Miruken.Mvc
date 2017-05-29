namespace Miruken.Mvc.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Forms;

    public class Buffer : FrameworkElement
    {
        public List<Output> Outputs     { get; set; }
        public Point        NextContent { get; set; }
        public IConsole     Console     { get; set; } = new ConsoleWrapper();

        public Buffer()
        {
            Outputs = new List<Output>();
        }

        public Buffer Write(string text)
        {
            Outputs.Add(new Output(text, OutputType.Write));
            return this;
        }

        public Buffer WriteLine()
        {
            return WriteLine(string.Empty);
        }

        public Buffer WriteLine(string text)
        {
            Outputs.Add(new Output(text, OutputType.WriteLine));
            return this;
        }

        public Buffer ReplaceLastLine(string text)
        {
            if (Outputs.Any())
                Outputs.Remove(Outputs.Last());

            Outputs.Add(new Output(text, OutputType.WriteLine));
            return this;
        }

        public Buffer Wrap(string text)
        {
            Outputs.Add(new Output(text, OutputType.Wrap));
            return this;
        }

        public override void Render(Cells cells)
        {
            base.Render(cells);
            new RenderBuffer()
                .Handle(this, cells);
        }

        private Point SetInputLocation(InputLocation inputLocation = InputLocation.Bottom)
        {
            var x = ContentBoundry.Start.X;
            int y;
            switch (inputLocation)
            {
                case InputLocation.Bottom:
                    y = Boundry.End.Y - Margin.Bottom - Border.Bottom - Padding.Bottom - 1;
                    break;
                default:
                    y = NextContent.Y;
                    break;
            }
            Console.SetCursorPosition(x, y);
            return new Point(x, y);
        }

        public string Prompt(string prompt, InputLocation inputLocation = InputLocation.Bottom)
        {
            var point = SetInputLocation(inputLocation);
            var message = $"? {prompt} ";
            WriteToConsole(message);
            return ReadLine(new Point(point.X + message.Length, point.Y));
        }

        public void Warn(string warning, InputLocation inputLocation = InputLocation.Bottom)
        {
            SetInputLocation(inputLocation);
            WriteToConsole($"! {warning} ");
            NextContent.Y++;
        }

        public string Edit(string prompt, string value, InputLocation inputLocation = InputLocation.Inline)
        {
            var message = $"? {prompt} ";

            var length = message.Length + value.Length;
            if (length > ContentBoundry.Width)
            {
                if ( ContentBoundry.Width >= 3)
                {
                    SetInputLocation(inputLocation);
                    WriteToConsole("...");
                }
                return value;
            }

            var point = SetInputLocation(inputLocation);
            WriteToConsole(message);
            return ReadLine(new Point(point.X + message.Length, point.Y), value);
        }

        private void WriteToConsole(string message)
        {
            var length = ContentBoundry.Width;
            Console.Write(message.PadRight(length));
        }

        private string tempFilePath => Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");

        public string EditWithDefaultProgram(string value)
        {
            var filePath = tempFilePath;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
            };
            return EditInSeperatProcess(value, filePath, process);
        }

        public string EditWithVim(string value)
        {
            var filePath = tempFilePath;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("vim", filePath)
                {
                    UseShellExecute = false
                }
            };
            return EditInSeperatProcess(value, filePath, process);
        }

        public string EditInPlace(string value)
        {
            var point = SetInputLocation(InputLocation.Inline);
            return ReadLine(point, value);
        }

        private string EditInSeperatProcess(string value, string filePath, Process p)
        {
            try
            {
                File.WriteAllText(filePath, value);
                p.Start();
                p.WaitForExit();
                var result = File.ReadAllText(filePath).Trim();
                return result;
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private string ReadLine(Point point)
        {
            Console.SetCursorPosition(point.X, point.Y);
            Console.CursorVisible = true;
            var line = Console.ReadLine()?.Trim();
            Console.CursorVisible = false;
            return line;
        }

        private string ReadLine(Point point, string Default)
        {
            var quit   = false;
            var startX = point.X;
            var endX   = ContentBoundry.End.X;
            var chars  = new List<char> (endX - startX);

            Console.SetCursorPosition(point.X, point.Y);
            Console.CursorVisible = true;
            Console.Write(Default);

            if (!string.IsNullOrEmpty(Default))
                chars.AddRange(Default.ToCharArray());

            do
            {
                var info = Console.ReadKey(true);
                var placeCursorX = Console.CursorLeft;
                var overwriteX = placeCursorX;

                if (info.Modifiers == ConsoleModifiers.Alt || info.Modifiers == ConsoleModifiers.Control)
                    continue;

                if (info.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft <= startX) continue;

                    var index = placeCursorX - startX - 1;
                    if (index < chars.Count)
                        chars.RemoveAt(index);

                    placeCursorX--;
                    overwriteX--;
                }
                else if (info.Key == ConsoleKey.Delete)
                {
                    var index = placeCursorX - startX + 1;
                    if (index < chars.Count)
                        chars.RemoveAt(index);
                }
                else if (info.Key == ConsoleKey.Enter)
                {
                    quit = true;
                }
                else if (info.Key == ConsoleKey.LeftArrow)
                {
                    if (Console.CursorLeft <= startX)
                        continue;

                    placeCursorX--;
                }
                else if (info.Key == ConsoleKey.RightArrow)
                {
                    if (placeCursorX >= endX)
                        continue;

                    placeCursorX++;
                }
                else
                {
                    if (info.KeyChar == (char) 0)
                        continue;

                    if (placeCursorX >= endX)
                        continue;

                    if (chars.Count >= ContentBoundry.Width)
                        continue;

                    var index = placeCursorX - startX;
                    if (index > chars.Count)
                    {
                        var spacesToAdd = index - chars.Count;
                        for (var i = 0; i < spacesToAdd; i++)
                            chars.Add(' ');
                    }

                    chars.Insert(index, info.KeyChar);
                    placeCursorX++;
                }

                var overwriteStart = overwriteX - startX;
                var pad = ContentBoundry.End.X - placeCursorX;
                var value = overwriteStart < chars.Count
                                ? string.Join("", chars).Substring(overwriteStart).PadRight(pad)
                                : new string(' ', pad);

                Console.CursorLeft = overwriteX;
                Console.Write(value);

                Console.CursorLeft = placeCursorX;
            } while (!quit);

            Console.CursorVisible = false;
            return new string(chars.ToArray());
        }
    }

    public static class OutputBufferExtensions
    {
        public static Buffer Header(this Buffer buffer, string title)
        {
            Seperator(buffer);
            buffer.WriteLine($"{title}");
            Seperator(buffer);
            return buffer;
        }

        public static Buffer Seperator(this Buffer buffer, char character = '-')
        {
           return Seperator(buffer, Console.WindowWidth, character);
        }

        public static  Buffer Seperator(this Buffer buffer, int length, char character = '-')
        {
            buffer.WriteLine(new string(character, length));
            return buffer;
        }

        public static Buffer Block(this Buffer buffer, string text)
        {
            buffer.WriteLine();
            buffer.WriteLine(text);
            buffer.WriteLine();
            return buffer;
        }
    }
}