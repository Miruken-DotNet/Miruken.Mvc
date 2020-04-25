namespace Miruken.Mvc.Wpf
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Xaml;
    using NReco.Linq;

    public class ActionExtension : MarkupExtension
    {
        private readonly string _action;

        public ActionExtension()
        {
        }

        public ActionExtension(string action)
        {
            _action = action;
        }

        public Binding ProcessAction(object target, IServiceProvider serviceProvider)
        {
            var binding = new Binding();
            if (string.IsNullOrWhiteSpace(_action)) return binding;

            var action = _action.Trim();
            var rootObjectProvider = (IRootObjectProvider)serviceProvider
                .GetService(typeof(IRootObjectProvider));

            binding.Converter = new ActionCommandValueConverter(
                new ActionBuilder(action, target, rootObjectProvider?.RootObject));

            return binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provider = (IProvideValueTarget)serviceProvider
                .GetService(typeof(IProvideValueTarget));
            var target = provider?.TargetObject as DependencyObject ?? DesignObject;

            if (DesignerProperties.GetIsInDesignMode(target))
                return "Designer Mode not supported";

            var binding = ProcessAction(target, serviceProvider);
            return binding.ProvideValue(serviceProvider);
        }

        #region ActionBuilder

        private class ActionBuilder
        {
            private readonly string _action;
            private readonly object _target;
            private readonly object _view;

            public ActionBuilder(string action, object target, object view)
            {
                _action = action;
                _target = target;
                _view   = view;
            }

            public ActionBinding Build(object controller) =>
                new ActionBinding(_action, controller, _target, _view);
        }

        #endregion

        #region ActionBinding

        private class ActionBinding
        {
            private readonly object _controller;
            private readonly object _target;
            private readonly object _view;
            private readonly string _actionExpr;
            private EventInfo _canActionChangedEvent;
            private string _canActionExpr;

            public ActionBinding(string action, object controller, object target, object view)
            {
                _controller = controller;
                _target     = target;
                _view       = view;

                if (!action.EndsWith(")")) action += "()";
                var canAction = "Can" + char.ToUpper(action[0]) + action.Substring(1);

                _actionExpr    = $"@ctrl.{action}";
                _canActionExpr = $"@ctrl.{canAction}";

                var startParen       = canAction.IndexOf("(", StringComparison.Ordinal);
                var canActionChanged = canAction.Substring(0, startParen) + "Changed";
                _canActionChangedEvent = _controller.GetType().GetEvent(canActionChanged);
            }

            public event EventHandler CanExecuteChanged
            {
                add => _canActionChangedEvent?.AddEventHandler(_controller, value);
                remove => _canActionChangedEvent?.RemoveEventHandler(_controller, value);
            }

            public bool CanExecute()
            {
                if (_canActionExpr == null) return true;
                try
                {
                    return (bool) Parser.Eval(_canActionExpr, GetVariable);
                }
                catch
                {
                    _canActionExpr         = null;
                    _canActionChangedEvent = null;
                    return true;
                }
            }

            public void Execute() => Parser.Eval(_actionExpr, GetVariable);

            private object GetVariable(string name) => name switch
                {
                    "@ctrl"   => _controller,
                    "@target" => _target,
                    "@view"   => _view,
                    _ => throw new ArgumentException($"Unknown variable '{name}'")
                };
        }

        #endregion

        #region ActionCommand

        private class ActionCommand : ICommand
        {
            private readonly ActionBinding _binding;

            public ActionCommand(ActionBinding binding)
            {
                _binding = binding;
            }

            public event EventHandler CanExecuteChanged
            {
                add => _binding.CanExecuteChanged += value;
                remove => _binding.CanExecuteChanged -= value;
            }

            public bool CanExecute(object parameter) => _binding.CanExecute();
            public void Execute(object parameter) => _binding.Execute();
        }

        #endregion

        #region ActionCommandValueConverter

        private class ActionCommandValueConverter : IValueConverter
        {
            private readonly ActionBuilder _builder;

            public ActionCommandValueConverter(ActionBuilder builder)
            {
                _builder = builder;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value != null
                     ? new ActionCommand(_builder.Build(value))
                     : null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        private static readonly LambdaParser     Parser       = new LambdaParser { UseCache = true };
        private static readonly DependencyObject DesignObject = new DependencyObject();
    }
}
