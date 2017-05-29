namespace Miruken.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Callback;
    using Context;
    using Infrastructure;
    using Options;
    using Views;

    public delegate IHandler FilterBuilder(IHandler handler);
    internal delegate object MemorizeAction(IHandler handler);

    public class Controller : ContextualHandler,
        IController, ISupportInitialize, INotifyPropertyChanged, IDisposable
    {
        internal IHandler _io;
        private ControllerPolicy _policy;
        internal MemorizeAction _lastAction;
        internal MemorizeAction _retryAction;
        protected bool _disposed;

        public static FilterBuilder GlobalPrepare;
        public static FilterBuilder GlobalExecute;

        #region Context

        protected IHandler IO => _io ?? Context;

        protected void EndContext()
        {
            var context = Context;
            context?.End();
        }

        protected void EndContext(object sender, EventArgs e)
        {
            EndContext();
        }

        protected void EndCallingContext()
        {
            var context = Composer.Resolve<IContext>();
            if ((context != null) && (context != Context))
                context.End();
        }

        protected void EndCallingContext(object sender, EventArgs e)
        {
            EndCallingContext();
        }

        #endregion

        #region Policy

        public ControllerPolicy Policy
        {
            get { return _policy ?? (_policy = new ControllerPolicy(this)); }
            set { _policy = value; }
        }

        #endregion

        #region Protocol

        protected TProto P<TProto>() where TProto : class
        {
            return Protocol.P<TProto>(IO);
        }

        protected TProto P<TProto>(IHandler handler) where TProto : class
        {
            return Protocol.P<TProto>(handler);
        }

        #endregion

        #region Render

        protected IViewLayer Show<V>(Action<V> init = null)
            where V : IView
        {
            return Show(IO, init);
        }

        protected IViewLayer Show<V>(IHandler handler, Action<V> init = null)
            where V : IView
        {
            return Region(handler).Show(init);
        }

        protected IViewLayer Overlay<V>(Action<V> init = null)
            where V : IView
        {
            return Overlay(IO, init);
        }

        protected IViewLayer Overlay<V>(IHandler handler, Action<V> init = null)
             where V : IView
        {
            return Region(handler.PushLayer()).Show(init);
        }

        protected IViewRegion Region(IHandler handler)
        {
            return P<IViewRegion>(handler);
        }

        protected IContext AddRegion(IViewRegion region, Action<IViewRegion> init = null)
        {
            var context = Context.AddRegion(region);
            init?.Invoke(Region(context));
            return context;
        }

        #endregion

        #region Navigate

        protected C Next<C>() where C : class, IController
        {
            return Next<C>(IO);
        }

        protected C Next<C>(IHandler handler) 
            where C : class, IController
        {
            return handler.Next<C>();
        }

        protected C Push<C>()
            where C : class, IController
        {
            return Push<C>(IO);
        }

        protected C Push<C>(IHandler handler) 
            where C : class, IController
        {
            return handler.Push<C>();
        }

        protected C Navigate<C>(NavigationStyle style) 
            where C : class, IController
        {
            return Push<C>(IO);
        }

        protected C Navigate<C>(IHandler handler, NavigationStyle style)
            where C : class, IController
        {
            return handler.Navigate<C>(style);
        }

        public object GoBack()
        {
            return GoBack(IO);
        }

        protected object GoBack(IHandler handler)
        {
            return handler.GoBack();
        }

        #endregion

        #region Initialize

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
        }

        #endregion

        #region Property Change

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

        #endregion

        #region Dispose

        protected bool IsDisposed => _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Policy      = null;
            Context     = null;
            _io         = null;
            _lastAction = _retryAction = null;
        }

        ~Controller()
        {
            Dispose(false);
        }

        #endregion
    }
}
