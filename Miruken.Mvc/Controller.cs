using System;
using System.ComponentModel;
using Miruken.Callback;
using Miruken.Concurrency;
using Miruken.Context;
using Miruken.Error;
using Miruken.Mvc.Views;
using Miruken.MVC;
using static Miruken.Protocol;

namespace Miruken.Mvc
{
    public class Controller : Handler, 
        IController, ISupportInitialize, INotifyPropertyChanged, IDisposable
    {
        private IContext _context;
        private ControllerPolicy _policy;
        internal MemorizeAction _lastAction;
        internal MemorizeAction _retryAction;
        protected bool _disposed;

        public delegate IHandler FilterBuilder(IHandler handler);
        internal delegate Promise<IContext> MemorizeAction(IHandler handler);

        public static FilterBuilder GlobalFilters;
        internal static IHandler _io;

        public IContext Context
        {
            get { return _context; }
            set
            {
                if (_context == value) return;
                var newContext = value;
                ContextChanging?.Invoke(this, _context, ref newContext);
                _context?.RemoveHandlers(this);
                var oldContext = _context;
                _context = newContext;
                _context?.InsertHandlers(0, this);
                ContextChanged?.Invoke(this, oldContext, _context);
            }
        }

        public event ContextChangingDelegate<IContext> ContextChanging;
        public event ContextChangedDelegate<IContext> ContextChanged;

        public ControllerPolicy Policy
        {
            get { return _policy ?? (_policy = new ControllerPolicy(this)); }
            set { _policy = value; }
        }

        protected IHandler IO
        {
            get
            {
                var io = _io ?? Context;
                if (io == null) return null;
                var filters       = Filters;
                var globalFilters = GlobalFilters;
                if (filters != null)
                    io = filters(io);
                if (globalFilters != null)
                    io = globalFilters(io);
                return io;
            }
        }

        protected INavigate Navigate(IHandler handler)
        {
            return P<INavigate>(handler.Recover());
        }

        protected Promise<IContext> Next<C>(Action<C> action) where C : IController
        {
            return Next(IO, action);
        }

        protected Promise<IContext> Next<C>(IHandler handler, Action<C> action)
            where C : IController
        {
            return Navigate(handler).Next(action);
        }

        protected Promise<IContext> Push<C>(Action<C> action) where C : IController
        {
            return Push(IO, action);
        }

        protected Promise<IContext> Push<C>(IHandler handler, Action<C> action)
            where C : IController
        {
            return Navigate(handler).Push(action);
        }

        protected Promise<IContext> Part<C>(Action<C> action) where C : IController
        {
            return Part(IO, action);
        }

        protected Promise<IContext> Part<C>(IHandler handler, Action<C> action)
            where C : IController
        {
            return Navigate(handler).Part(action);
        }

        protected FilterBuilder Filters;

        protected IContext AddRegion(IViewRegion region)
        {
            return Context.AddRegion(region);
        }

        protected void EndContext()
        {
            var context = Context;
            context?.End();
        }

        protected void EndCallingContext()
        {
            var context = Composer.Resolve<IContext>();
            if ((context != null) && (context != Context))
                context.End();
        }

        protected virtual void Reset()
        {
            Context      = null;
            Policy       = null;
            _lastAction  = null;
            _retryAction = null;
        }

        #region ISupportInitialize

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

        #region IPropertyNotifyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

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
            Reset();
        }

        ~Controller()
        {
            Dispose(false);
        }

        #endregion
    }
}
