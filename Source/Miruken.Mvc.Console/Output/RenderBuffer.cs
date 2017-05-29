namespace Miruken.Mvc.Console
{
    public class RenderBuffer
    {
        private const int tabSpaces = 8;

        private Buffer _buffer;
        private int    _x;
        private int    _y;
        private int    _xStart;
        private int    _yStart;
        private int    _xEnd;
        private int    _yEnd;
        private Cells  _cells;

        public Cells Handle(Buffer buffer, Cells cells)
        {
            if (buffer.ActualSize.Height < 1) return cells;

            _buffer = buffer;
            _cells  = cells;

            _xStart = _buffer.Point.X + _buffer.Margin.Left + _buffer.Border.Left + _buffer.Padding.Left;
            _yStart = _buffer.Point.Y + _buffer.Margin.Top  + _buffer.Border.Top  + _buffer.Padding.Top;

            var _contentWidth = _buffer.ActualSize.Width
                                - _buffer.Margin.Left - _buffer.Border.Left - _buffer.Padding.Left
                                - _buffer.Margin.Right - _buffer.Border.Right - _buffer.Padding.Right;
            var _contentHeight = _buffer.ActualSize.Height
                                 - _buffer.Margin.Top - _buffer.Border.Top - _buffer.Padding.Top
                                 - _buffer.Margin.Bottom - _buffer.Padding.Bottom - _buffer.Border.Bottom;

            _x    = _xStart;
            _y    = _yStart;
            _xEnd = _xStart + _contentWidth;
            _yEnd = _yStart + _contentHeight;

            foreach (var item in _buffer.Outputs)
            {
                switch (item.OutputType)
                {
                    case OutputType.Wrap:
                        Wrap(item);
                        break;
                    case OutputType.Write:
                        Write(item);
                        break;
                    case OutputType.WriteLine:
                        WriteLine(item);
                        break;
                }
                if (_y >= _yEnd)
                    break;
            }
            buffer.NextContent = new Point(_x, _y);
            return _cells;
        }

        private void WriteLine(Output item)
        {
            foreach (var t in item.Text)
            {
                switch (t)
                {
                    case '\n':
                        NextLine();
                        break;
                    case '\r':
                        _x = _xStart;
                        break;
                    case '\t':
                        Tab();
                        break;
                    default:
                        if (_y < _cells.Height && _x < _cells.Width)
                        {
                            _cells[_y][_x] = t;
                            _x++;
                        }
                        break;
                }
                if (_x >= _xEnd || _y >= _yEnd )
                    break;
            }
            NextLine();
        }

        private void Write(Output output)
        {
            foreach (var t in output.Text)
            {
                if (_y >= _yEnd)
                    break;

                switch (t)
                {
                    case '\n':
                        NextLine();
                        break;
                    case '\r':
                        _x = _xStart;
                        break;
                    case '\t':
                        Tab();
                        break;
                    default:
                        _cells[_y][_x] = t;
                        _x++;
                        break;
                }

                if (_x >= _xEnd)
                    NextLine();
            }
        }

        private void Wrap(Output output)
        {
            Write(output);
            NextLine();
        }

        private void Tab()
        {
            var nextTab = tabSpaces - (_x % tabSpaces);
            if (_x + nextTab > _xEnd)
            {
                _x = _xStart;
                _y++;
            }
            else
                _x = _x + nextTab;
        }

        private void NextLine()
        {
            _x = _xStart;
            _y++;
        }
    }
}