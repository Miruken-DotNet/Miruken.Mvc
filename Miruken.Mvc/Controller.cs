namespace Miruken.Mvc
{
    using System;
    using System.ComponentModel;
    using Callback;
    using Concurrency;
    using Miruken.Context;
    using Miruken.Error;
    using Miruken.Mvc.Views;

    public class Controller : CallbackHandler, 
        IController, ISupportInitialize, INotifyPropertyChanged, IDisposable
    {
        private IContext _context;
        private ControllerPolicy _policy;
        internal MemorizeAction _lastAction;
        internal MemorizeAction _retryAction;
        protected bool _disposed;

        public delegate ICallbackHandler FilterBuilder(ICallbackHandler handler);
        internal delegate Promise<IContext> MemorizeAction(ICallbackHandler handler);

        public static FilterBuilder GlobalFilters;
        internal static ICallbackHandler _io;

        public IContext Context
        {
            get { return _context; }
            set
            {
                if (_context == value)
                    return;

                if (_context != null)
                    _context.RemoveHandlers(this);

                _context = value;

                if (_context != null)
                    _context.InsertHandlers(0, this);
            }
        }

        public ControllerPolicy Policy
        {
            get { return _policy ?? (_policy = new ControllerPolicy(this)); }
            set { _policy = value; }
        }

        protected ICallbackHandler IO
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

        protected IMVC MVC(ICallbackHandler handler)
        {
            return new IMVC(handler.MainThread().Recover());
        }

        protected Promise<IContext> Next<C>(Action<C> action) where C : IController
        {
            return Next(IO, action);
        }

        protected Promise<IContext> Next<C>(ICallbackHandler handler, Action<C> action) 
            where C : IController
        {
            return MVC(handler).Next(action);
        }

        protected Promise<IContext> Push<C>(Action<C> action) where C : IController
        {
            return Push(IO, action);
        }

        protected Promise<IContext> Push<C>(ICallbackHandler handler, Action<C> action) 
            where C : IController
        {
            return MVC(handler).Push(action);
        }

        protected Promise<IContext> Part<C>(Action<C> action) where C : IController
        {
            return Part(IO, action);
        }

        protected Promise<IContext> Part<C>(ICallbackHandler handler, Action<C> action)
            where C : IController
        {
            return MVC(handler).Part(action);
        }

        protected FilterBuilder Filters;

        protected IContext AddRegion(IViewRegion region)
        {
            return Context.AddRegion(region);
        }

        protected void EndContext()
        {
            var context = Context;
            if (context != null)
                context.End();    
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
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

        protected bool IsDisposed
        {
            get { return _disposed; }
        }

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
