namespace Miruken.Mvc.Console.Test
{
    using System;
    using System.Text;
    using System.Threading;
    using Forms;

    public class MockConsole : IConsole
    {
        private string         LastLine;

        public StringBuilder Builder { get; set; }= new StringBuilder();

        public AutoResetEvent WaitForKeyPress = new AutoResetEvent(false);
        public AutoResetEvent WaitForReadLine = new AutoResetEvent(false);

        public ConsoleKeyInfo[] ReadKeys { get; set; }
        private int _readKeyIndex { get; set; } = 0;

        #region IConsole

        public void Clear()
        {
            Builder = new StringBuilder();
        }

        public int CursorLeft     { get; set; }

        public bool CursorVisible { get; set; }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return ReadKeys[_readKeyIndex++];
        }

        public string ReadLine()
        {
            WaitForReadLine.WaitOne();
            return LastLine;
        }

        public ConsoleKeyInfo ReadKey()
        {
            return ReadKey(false);
        }

        public void SetCursorPosition(int left, int top)
        {
            CursorLeft = left;
            Top        = top;
        }

        public int Top { get; set; }

        public int WindowHeight { get; set; } = 50;

        public int WindowWidth { get; set; } = 50;

        public void Write(string value)
        {
            if (CursorLeft > Builder.Length)
            {
                var spacesToAdd = CursorLeft - Builder.Length;
                for (int i = 0; i < spacesToAdd; i++)
                {
                    Builder.Append(' ');
                }
            }

            Builder.Insert(CursorLeft, value);
            //Builder.Append(value);
            CursorLeft = CursorLeft + value.Length;
        }

        public void WriteLine(string value)
        {
            Builder.AppendLine(value);
        }

        #endregion

        public void InputForReadLine(string line)
        {
            LastLine = line;
            WaitForReadLine.Set();
        }
    }
}
