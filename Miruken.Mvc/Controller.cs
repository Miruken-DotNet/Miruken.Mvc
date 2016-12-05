﻿using System;
using System.ComponentModel;
using Miruken.Callback;
using Miruken.Concurrency;
using Miruken.Context;
using Miruken.Error;
using Miruken.Mvc.Views;
using Miruken.MVC;

namespace Miruken.Mvc
{
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
                if (_context == value) return;
                _context?.RemoveHandlers(this);
                _context = value;
                _context?.InsertHandlers(0, this);
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

        protected INavigate Navigate(ICallbackHandler handler)
        {
            return new INavigate(handler.Recover());
        }

        protected Promise<IContext> Next<C>(Action<C> action) where C : IController
        {
            return Next(IO, action);
        }

        protected Promise<IContext> Next<C>(ICallbackHandler handler, Action<C> action)
            where C : IController
        {
            return Navigate(handler).Next(action);
        }

        protected Promise<IContext> Push<C>(Action<C> action) where C : IController
        {
            return Push(IO, action);
        }

        protected Promise<IContext> Push<C>(ICallbackHandler handler, Action<C> action)
            where C : IController
        {
            return Navigate(handler).Push(action);
        }

        protected Promise<IContext> Part<C>(Action<C> action) where C : IController
        {
            return Part(IO, action);
        }

        protected Promise<IContext> Part<C>(ICallbackHandler handler, Action<C> action)
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
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
