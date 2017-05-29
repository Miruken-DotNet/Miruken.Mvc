namespace Miruken.Mvc.Console
{
    public class RenderElement
    {
        private FrameworkElement _element;

        private Boundry margin;
        private Boundry border;
        private Boundry padding;
        private Boundry content;
        private Cells   _cells;

        public void Handle(FrameworkElement element, Cells cells)
        {
            _element = element;
            _cells   = cells;

            if (!CanRenderBorderAndPadding()) return;

            //margin
            var marginXStart = _element.Point.X;
            var marginYStart = _element.Point.Y;

            var marginXEnd = marginXStart + _element.ActualSize.Width;
            if (marginXEnd > cells.Width)
                marginXEnd = cells.Width;

            var marginYEnd   = marginYStart + _element.ActualSize.Height;
            if (marginYEnd > cells.Height)
                marginYEnd = cells.Height;

            margin = new Boundry(new Point(marginXStart, marginYStart), new Point(marginXEnd, marginYEnd));

            //border
            var borderXStart = marginXStart + _element.Margin.Left;
            var borderYStart = marginYStart + _element.Margin.Top;
            var borderXEnd   = marginXEnd   - _element.Margin.Right;
            var borderYEnd   = marginYEnd   - _element.Margin.Bottom;

            border = new Boundry(new Point(borderXStart, borderYStart), new Point(borderXEnd, borderYEnd));

            //padding
            var paddingXStart = borderXStart + _element.Border.Left;
            var paddingYStart = borderYStart + _element.Border.Top;
            var paddingXEnd   = borderXEnd   - _element.Border.Right;
            var paddingYEnd   = borderYEnd   - _element.Border.Bottom;

            padding = new Boundry(new Point(paddingXStart, paddingYStart), new Point(paddingXEnd, paddingYEnd));

            //content
            var contentXStart = paddingXStart + _element.Padding.Left;
            var contentYStart = paddingYStart + _element.Padding.Top;
            var contentXEnd   = paddingXEnd   - _element.Padding.Right;
            var contentYEnd   = paddingYEnd   - _element.Padding.Bottom;

            content = new Boundry(new Point(contentXStart, contentYStart), new Point(contentXEnd, contentYEnd));

            _element.ContentBoundry = content;

            RenderMargin();
            RenderBorder();
            RenderPadding();
            RenderContent();
        }

        private bool CanRenderBorderAndPadding()
        {
            return _element.ActualSize.Width >=
                       _element.Margin.Left  + _element.Margin.Right +
                       _element.Border.Left  + _element.Border.Right  +
                       _element.Padding.Left + _element.Padding.Right
                   &&
                   _element.ActualSize.Height >=
                       _element.Margin.Top  + _element.Margin.Bottom  +
                       _element.Border.Top  + _element.Border.Bottom  +
                       _element.Padding.Top + _element.Padding.Bottom;
        }

        private void RenderPadding()
        {
            RenderPaddingLeft();
            RenderPaddingRight();
            RenderPaddingTop();
            RenderPaddingBottom();
        }

        private void RenderPaddingLeft()
        {
            for (var i = 0; i < _element.Padding.Left; i++)
                for (var y = padding.Start.Y; y < padding.End.Y; y++)
                    _cells[y][padding.Start.X + i] = Cells.SpaceChar;
        }

        private void RenderPaddingRight()
        {
            for (var i = 0; i < _element.Padding.Right; i++)
                for (var y = padding.Start.Y; y < padding.End.Y; y++)
                    _cells[y][padding.End.X - 1 - i] = Cells.SpaceChar;
        }

        private void RenderPaddingTop()
        {
            for (var i = 0; i < _element.Padding.Top; i++)
                for (var x = padding.Start.X; x < padding.End.X; x++)
                    _cells[padding.Start.Y + i][x] = Cells.SpaceChar;
        }

        private void RenderPaddingBottom()
        {
            for (var i = 0; i < _element.Padding.Bottom; i++)
                for (var x = padding.Start.X; x < padding.End.X; x++)
                    _cells[padding.End.Y - 1 - i][x] = Cells.SpaceChar;
        }

        private void RenderBorder()
        {
            RenderBorderLeft();
            RenderBorderRight();
            RenderBorderTop();
            RenderBorderBottom();
        }

        private void RenderBorderLeft()
        {
            for (var i = 0; i < _element.Border.Left; i++)
                for (var y = border.Start.Y; y < border.End.Y; y++)
                    _cells[y][i + border.Start.X] =  '|';
        }

        private void RenderBorderRight()
        {
            for (var i = 0; i < _element.Border.Right; i++)
                for (var y = border.Start.Y; y < border.End.Y; y++)
                    _cells[y][border.End.X - 1 - i] = '|';
        }

        private void RenderBorderTop()
        {
            for (var i = 0; i < _element.Border.Top; i++)
                for (var x = border.Start.X; x < border.End.X; x++)
                    _cells[i + border.Start.Y][x] = '-';
        }

        private void RenderBorderBottom()
        {
            for (var i = 0; i < _element.Border.Bottom; i++)
                for (var x = border.Start.X; x < border.End.X; x++)
                    _cells[border.End.Y - 1 -i][x] = '-';
        }


        private void RenderMargin()
        {
            RenderLeftMargin();
            RenderTopMargin();
            RenderRightMargin();
            RenderBottomMargin();
        }

        private void RenderLeftMargin()
        {
            for (var i = 0; i < _element.Margin.Left; i++)
                for (var y = margin.Start.Y; y < border.End.Y; y++)
                    _cells[y][i + margin.Start.X] =  Cells.SpaceChar;
        }

        private void RenderTopMargin()
        {
            for (var i = 0; i < _element.Margin.Top; i++)
                for (var x = margin.Start.X; x < margin.End.X; x++)
                    _cells[i + margin.Start.Y][x] = Cells.SpaceChar;
        }

        private void RenderRightMargin()
        {
            for (var i = 0; i < _element.Margin.Right; i++)
                for (var y = margin.Start.Y; y < border.End.Y; y++)
                    _cells[y][margin.End.X - 1 - i] =  Cells.SpaceChar;
        }

        private void RenderBottomMargin()
        {
            for (var i = 0; i < _element.Margin.Bottom; i++)
                for (var x = margin.Start.X; x < margin.End.X; x++)
                    _cells[margin.End.Y - 1 -i][x] = Cells.SpaceChar;
        }

        private void RenderContent()
        {
            for (var y = content.Start.Y; y < content.End.Y; y++)
                for (var x = content.Start.X; x < content.End.X; x++)
                    _cells[y][x] = Cells.SpaceChar;
        }
    }
}