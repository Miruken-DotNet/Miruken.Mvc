namespace ConsoleTestApp.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ColumnLayout
    {
        private readonly List<List<string>> _columns;

        public ColumnLayout(int columns)
        {
            if(columns < 2)
                throw new ArgumentException($"{nameof(ColumnLayout)} must have atleast 2 columns");

            _columns = new List<List<string>>();
            for (var i = 0; i < columns; i++)
            {
                _columns.Add(new List<string>());
            }
        }

        public void Add(int column, string text)
        {
            if(column > _columns.Count - 1)
                throw new ArgumentException($"{nameof(ColumnLayout)} only has {_columns.Count} columns. Columns are 0 based.");

            _columns[column].Add(text);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var maxRows = _columns.Max(x => x.Count);

            var lengths = _columns
                .Select(column => column.Max(text => text.Length))
                .ToArray();

            for (var i = 0; i < maxRows; i++)
            {
                for (var j = 0; j < _columns.Count; j++)
                {
                    if (i >= _columns[j].Count) continue;

                    var text = _columns[j][i];
                    builder.Append(text.PadRight(lengths[j], ' '));
                    builder.Append("\t");

                }
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }
}