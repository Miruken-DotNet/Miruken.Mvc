namespace Miruken.Mvc.Console
{
    using System;

    public class DockChildSettings
    {
        public DockChildSettings()
        {
        }

        public DockChildSettings(Dock dock, decimal percent = 100m)
        {
            Dock    = dock;
            Percent = percent;
        }

        public Dock             Dock    { get; set; }

        private decimal _percent;
        public decimal Percent
        {
            get
            {
                return _percent;
            }
            set
            {
                if(value < 0 || value > 100)
                    throw new ArgumentException("Must be between 0 and 100 inclusive.");
                _percent = value;
            }
        }
    }
}