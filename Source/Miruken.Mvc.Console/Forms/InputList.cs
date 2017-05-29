namespace Miruken.Mvc.Console.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Buffer = Buffer;

    public class InputList : Input
    {
        private readonly List<ListItem> _items = new List<ListItem>();
        private readonly List<char>     _filter = new List<char>();

        private int  _index;
        private bool _quit;

        private string Selected => _items.FirstOrDefault(x => x.Selected)?.Text;

        public IConsole Console     { get; set; } = new ConsoleWrapper();
        public Buffer   Buffer      { get; set; }
        public int      LongestText { get; set; }

        public List<ListItem> FilteredItems => _filter.Any()
            ? _items.Where(x => x.Text.ToLower().Contains(new string(_filter.ToArray()).ToLower())).ToList()
            : _items.ToList();

        public InputList(string text, string[] items, Action<string> selectedAction)
            : this(text, null, items, selectedAction)
        {
        }

        public InputList(string text, string value, string[] items, Action<string> selectedAction)
        {
            Text   = text;
            foreach (var item in items)
                _items.Add(new ListItem(item));

            if (string.IsNullOrEmpty(value))
                Select(0);
            else
                Select(_items.FindIndex(x => x.Text == value));

            _selectedAction    = selectedAction;
        }

        public override void Handle(Buffer buffer, int longestText, InputLocation inputLocation)
        {
            Buffer        = buffer;
            LongestText   = longestText;

            Render(true);
            do
            {
                KeyPressed(Console.ReadKey(true));
            } while (!_quit);
        }

        public void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    Up();
                    break;
                case ConsoleKey.DownArrow:
                    Down();
                    break;
                case ConsoleKey.Enter:
                    Choose();
                    break;
                default:
                    Filter(keyInfo);
                    break;
            }
        }

        public string Render(bool firstTime = false)
        {
            var builder = new StringBuilder();
            builder.Append($"? {Text} ");
            builder.AppendLine(firstTime ? "(Use arrow keys)" : string.Empty);
            builder.AppendLine();
            builder.Append("filter: ");
            builder.AppendLine(_filter.Any() ? new string(_filter.ToArray()) : "(Type to filter)");

            var items = FilteredItems;
            if (items.Any())
            {
                foreach (var item in items)
                {
                    builder.Append(item.Selected ? "> " : "  ");
                    builder.AppendLine(item.Text);
                }
            }
            else
                builder.AppendLine("  0 Items");

            var content = builder.ToString();

            if (firstTime)
                Buffer.WriteLine(content);
            else
                Buffer.ReplaceLastLine(content);

            Window.Update();

            return content;
        }

        private void Up()
        {
            if (_index <= 0) return;
            Select(_index - 1);
            Render();
        }

        private void Down()
        {
            if (_index >= FilteredItems.Count - 1) return;
            Select(_index + 1);
            Render();
        }

        private void Select(int index)
        {
            _index = index;
            foreach (var item in _items)
                item.Selected = false;

            if (_index <= FilteredItems.Count - 1)
            {
                var item = FilteredItems[_index];
                item.Selected = true;

            }
        }

        private void Choose()
        {
            _selectedAction?.Invoke(Selected);
            Buffer.ReplaceLastLine($"? {Text.PadRight(LongestText)} {Selected}");
            _quit = true;
            Window.Update();
        }

        private void Filter(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Modifiers != 0) return;

            switch (keyInfo.Key)
            {
                case ConsoleKey.Backspace:
                    if(_filter.Count > 0)
                        _filter.Remove(_filter.Last());
                    break;
                default:
                    if (keyInfo.KeyChar != (char) 0)
                    {
                        _filter.Add(keyInfo.KeyChar);
                        Select(0);
                    }
                    break;
            }
            Render();
        }

        public class ListItem
        {
            public ListItem(string item)
            {
                Text = item;
            }
            public bool   Selected { get; set; }
            public string Text     { get; set; }
        }
    }
}
