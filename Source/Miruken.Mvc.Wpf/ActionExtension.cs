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
            if (string.IsNullOrWhiteSpace(_action))
                return binding;

            var action = _action.Trim();
            var rootObjectProvider = (IRootObjectProvider)serviceProvider
                .GetService(typeof(IRootObjectProvider));

            binding.Converter = new ActionCommandValueConverter(
                new ActionScope(action)
                {
                    Target = target,
                    View   = rootObjectProvider?.RootObject
                });

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

        public class ActionScope
        {
            private readonly string _actionExpr;
            private readonly string _canActionExpr;
            private readonly string _canActionChanged;
            private object _controller;
            private EventInfo _canActionChangedEvent;

            public ActionScope(string action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                if (!action.EndsWith(")")) action += "()";
                var canAction = "Can" + char.ToUpper(action[0]) + action.Substring(1);

                _actionExpr    = $"@ctrl.{action}";
                _canActionExpr = $"@ctrl.{canAction}";

                var startParen = canAction.IndexOf("(", StringComparison.Ordinal);
                _canActionChanged = canAction.Substring(0, startParen) + "Changed";
            }

            public object Controller
            {
                get => _controller;
                set
                {
                    _controller = value;
                    if (value != null)
                    {
                        _canActionChangedEvent = _controller.GetType()
                            .GetEvent(_canActionChanged);
                    }
                }
            }

            public object Target { get; set; }
            public object View   { get; set; }

            public event EventHandler CanExecuteChanged
            {
                add => _canActionChangedEvent?.AddEventHandler(Controller, value);
                remove => _canActionChangedEvent?.RemoveEventHandler(Controller, value);
            }

            public bool CanExecute()
            {
                try
                {
                    return (bool) Parser.Eval(_canActionExpr, GetVariable);
                }
                catch
                {
                    // CanExecute not available
                    return true;
                }
            }

            public void Execute()
            {
                Parser.Eval(_actionExpr, GetVariable);
            }

            private object GetVariable(string name) => name switch
                {
                    "@ctrl"   => Controller,
                    "@target" => Target,
                    "@view"   => View,
                    _ => throw new ArgumentException($"Unknown variable '{name}'")
                };
        }

        #region ActionCommandValueConverter

        private class ActionCommandValueConverter : IValueConverter
        {
            private readonly ActionScope _scope;

            public ActionCommandValueConverter(ActionScope scope)
            {
                _scope = scope;
            }

            public object Convert(
                object      value,
                Type        targetType,
                object      parameter, 
                CultureInfo culture)
            {
                if (value == null) return null;
                _scope.Controller = value;
                return new ActionCommand(_scope);
            }

            public object ConvertBack(
                object      value,
                Type        targetType,
                object      parameter,
                CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ActionCommand

        private class ActionCommand : ICommand
        {
            private readonly ActionScope _scope;

            public ActionCommand(ActionScope scope)
            {
                _scope = scope;
            }

            public event EventHandler CanExecuteChanged
            {
                add => _scope.CanExecuteChanged += value;
                remove => _scope.CanExecuteChanged -= value;
            }

            public bool CanExecute(object parameter) => _scope.CanExecute();

            public void Execute(object parameter) => _scope.Execute();
        }

        #endregion

        private static readonly LambdaParser     Parser       = new LambdaParser { UseCache = true };
        private static readonly DependencyObject DesignObject = new DependencyObject();
    }
}
