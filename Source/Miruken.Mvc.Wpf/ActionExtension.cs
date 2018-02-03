namespace Miruken.Mvc.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Xaml;
    using ExpressionEvaluator;

    public class ActionExtension : MarkupExtension
    {
        private readonly string _action;

        public class Scope
        {
            public string a { get; set; }
            public object c { get; set; }
            public object t { get; set; }
            public object v { get; set; }
            public object p { get; set; }
        }

        public class ActionBinding
        {
            public Action<Scope>     Execute;
            public Func<Scope, bool> CanExecute;
            public EventInfo         CanExecuteChanged;
        }

        private static readonly Dictionary<string, ActionBinding>
            ActionCache = new Dictionary<string, ActionBinding>();

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

            binding.Converter = new ActionCommandValueConverter(new Scope
            {
                a = action,
                t = target,
                v = rootObjectProvider?.RootObject
            });

            return binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var ipvt = (IProvideValueTarget)serviceProvider
                .GetService(typeof(IProvideValueTarget));
            var target = ipvt?.TargetObject as DependencyObject ?? DesignObject;
            if (DesignerProperties.GetIsInDesignMode(target))
                return "Designer Mode not supported";

            var binding = ProcessAction(target, serviceProvider);
            return binding.ProvideValue(serviceProvider);
        }

        #region ActionCommandValueConverter

        private class ActionCommandValueConverter : IValueConverter
        {
            private readonly Scope _scope;

            public ActionCommandValueConverter(Scope scope)
            {
                _scope = scope;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null) return null;
                _scope.c = value;
                return new ActionCommand(_scope);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ActionCommand

        private class ActionCommand : ICommand
        {
            private readonly Scope _scope;
            private readonly ActionBinding _action;

            public ActionCommand(Scope scope)
            {
                _scope  = scope;
                _action = GetAction();
            }

            public event EventHandler CanExecuteChanged
            {
                add => _action.CanExecuteChanged?.AddEventHandler(_scope.c, value);
                remove => _action.CanExecuteChanged?.RemoveEventHandler(_scope.c, value);
            }

            public bool CanExecute(object parameter)
            {
                _scope.p = parameter;
                return _action.CanExecute?.Invoke(_scope) != false;
            }

            public void Execute(object parameter)
            {
                _scope.p = parameter;
                _action.Execute(_scope);
            }

            private ActionBinding GetAction()
            {
                var action         = _scope.a;
                var controllerType = _scope.c.GetType();
                var controllerKey  = GetTypeKey(controllerType);
                var targetType     = _scope.t?.GetType();
                var targetKey      = GetTypeKey(targetType);
                var viewType       = _scope.v?.GetType();
                var viewKey        = GetTypeKey(viewType);

                if (!action.EndsWith(")"))
                    action += "()";
                else if (!action.EndsWith("()"))
                {
                    if (targetType != null)
                        action = action.Replace("@target", $"(({targetKey})t)");
                    if (viewType != null)
                        action = action.Replace("@view", $"(({viewKey})v)");
                    action = action.Replace("@param", "(p)");
                }

                var executeExpr = $"(({controllerKey})c).{action}";

                if (!ActionCache.TryGetValue(executeExpr, out var binding))
                {
                    var types = new TypeRegistry();
                    types.RegisterType(controllerKey, controllerType);
                    if (targetType != null)
                        types.RegisterType(targetKey, targetType);
                    if (viewType != null)
                        types.RegisterType(viewKey, viewType);

                    var execute = new CompiledExpression(executeExpr)
                    {
                        TypeRegistry = types
                    };

                    binding = new ActionBinding
                    {
                        Execute = execute.ScopeCompileCall<Scope>()
                    };

                    try
                    {
                        var canAction      = "Can" + char.ToUpper(action[0]) + action.Substring(1);
                        var canExecuteExpr = $"(({controllerKey})c).{canAction}";
                        var canExecute     = new CompiledExpression<bool>(canExecuteExpr)
                        {
                            TypeRegistry = types
                        };
                        binding.CanExecute = canExecute.ScopeCompile<Scope>();

                        var startParen            = canAction.IndexOf("(", StringComparison.Ordinal);
                        var canActionChanged      = canAction.Substring(0, startParen) + "Changed";
                        binding.CanExecuteChanged = controllerType.GetEvent(canActionChanged);
                    }
                    catch
                    {
                        // canExecute not available
                    }

                    ActionCache.Add(executeExpr, binding);
                }

                return binding;
            }

            private static string GetTypeKey(Type type)
            {
                return type == null ? null
                     : $"{type.FullName}".Replace(".", "_").Replace('+', '_');
            }
        }

        #endregion

        private static readonly DependencyObject DesignObject = new DependencyObject();
    }
}
