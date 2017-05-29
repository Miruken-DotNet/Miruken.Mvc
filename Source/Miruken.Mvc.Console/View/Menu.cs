namespace Miruken.Mvc.Console
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Menu
    {
        private readonly string _menu;

        public MenuItem[] Items { get; }
        public bool TopSeperator;
        public bool BottomSeperator;

        public Menu(params MenuItem[] items)
        {
            Items = items;
            var _menuBuilder = new StringBuilder();
            _menuBuilder.Append(" alt + | ");
            var itemsLength = Items.Length;
            for (var i = 0; i < itemsLength; i++)
            {
                var item = Items[i];
                _menuBuilder.Append($"{item.Text}({item.Key})");
                if (i < itemsLength - 1)
                    _menuBuilder.Append(" | ");
            }

            var menu = _menuBuilder.ToString();
            var separatorLength = menu.Length + 1;
            var separator = new string('-', separatorLength);

            var formatBuilder = new StringBuilder();

            if (TopSeperator)
                formatBuilder.AppendLine(separator);

            formatBuilder.AppendLine(menu);

            if (BottomSeperator)
                formatBuilder.AppendLine(separator);

            _menu = formatBuilder.ToString();
        }

        public async Task Listen(ConsoleKeyInfo keyInfo)
        {
            var item = Items.FirstOrDefault(x => x.Key == keyInfo.Key);
            if (keyInfo.Modifiers == ConsoleModifiers.Alt)
            {
                var itemAction = item?.Action();
                if (itemAction != null) await itemAction;
            }
        }

        public override string ToString()
        {
            return _menu;
        }
    }
}