namespace Miruken.Mvc.Console
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SelectList<T> where T : class
    {
        private readonly Action<T> _selected;
        private readonly List<char> _selection = new List<char>();
        private readonly List<SelectListItem> _items = new List<SelectListItem>();
        private int _id = 65;
        private int _firstId = 65;
        private int _lastId = 90;
        private int _position = 1;

        public SelectList(IEnumerable<T> items, Action<T> selected)
        {
            _selected = selected;
            foreach (var item in items)
            {
                _items.Add(new SelectListItem(item, NextId()));
            }
        }

        public void Listen(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    _selection.Clear();
                    return;
                default:
                    if(keyInfo.Modifiers == 0 && char.IsLetter(keyInfo.KeyChar))
                        _selection.Add(keyInfo.KeyChar);
                    break;
            }

            foreach (var item in _items)
            {
                if (string.Equals(new string(_selection.ToArray()), item.Id, StringComparison.CurrentCultureIgnoreCase))
                {
                    _selected?.Invoke(item.Item);
                    break;
                }
            }
        }

        public string NextId()
        {
            var id = new char[_position];
            for (var i = 0; i < _position; i++)
            {
                id[i] = (char) _id;
            }
            _id++;
            if (_id > _lastId)
            {
                _position++;
                _id = _firstId;
            }

            return new string(id);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var item in _items)
            {
                builder.Append($"[{item.Id}] ");
                builder.AppendLine(item.Item.ToString());
            }

            return builder.ToString();
        }

        private class SelectListItem
        {
            public string Id   { get; }
            public T      Item { get; }

            public SelectListItem(T item, string id)
            {
                Item = item;
                Id   = id;
            }
        }
    }

}
