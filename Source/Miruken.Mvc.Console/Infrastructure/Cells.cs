namespace Miruken.Mvc.Console
{
    using System.Text;

    public class Cells
    {
        public const char SpaceChar = ' ';
        public const char NullChar  = (char)0;
        public readonly char[][] _cells;

        public int   Height { get;  }
        public int   Width  { get;  }

        public Cells(Size size)
            : this(size.Height, size.Width)
        {
        }

        public Cells(int height, int width)
            : this (height, width, SpaceChar)
        {
        }

        public Cells(int height, int width, char defaultChar)
        {
            _cells = new char[height][];
            for (var i = 0; i < height; i++)
            {
                _cells[i] = new char[width];
                for (var j = 0; j < width; j++)
                    _cells[i][j] = defaultChar;
            }
            Height = height;
            Width  = width;
        }

        public char[] this[int index] => _cells[index];

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var row in _cells)
                builder.AppendLine(new string(row));
            return builder.ToString();
        }
    }
}