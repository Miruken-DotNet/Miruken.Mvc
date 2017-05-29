namespace Miruken.Mvc.Console.Forms
{
    using System;

    public interface IConsole
    {
        void           Clear();
        int            CursorLeft { get; set; }
        bool           CursorVisible { get; set; }
        ConsoleKeyInfo ReadKey(bool intercept);
        ConsoleKeyInfo ReadKey();
        string         ReadLine();
        void           SetCursorPosition(int left, int top);
        int            WindowHeight { get; set; }
        int            WindowWidth  { get; set; }
        void           Write(string value);
        void           WriteLine(string value);
    }
}