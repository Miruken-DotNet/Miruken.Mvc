namespace Miruken.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Infrastructure;

    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool ChangeProperty<T>(ref T field, T value,
               IEqualityComparer<T> comparer = null,
               [CallerMemberName] string propertyName = null)
        {
            return PropertyChanged.ChangeProperty(
                ref field, value, this, comparer, propertyName);
        }

        protected bool ChangeProperty<T>(T property, T value, Action<T> set,
            IEqualityComparer<T> comparer = null,
            [CallerMemberName] string propertyName = null)
        {
            var ret = ChangeProperty(ref property, value, comparer, propertyName);
            if (ret) set?.Invoke(property);
            return ret;
        }
    }
}
