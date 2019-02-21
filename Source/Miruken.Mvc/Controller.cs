namespace Miruken.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Callback;
    using Concurrency;
    using Context;
    using Infrastructure;
    using Options;
    using Views;

    public abstract class Controller : ContextualHandler,
        IController, ISupportInitialize, INotifyPropertyChanged, IDisposable
    {
        private IHandler _io;
        protected bool _disposed;

        #region Context

        public IHandler IO
        {
            protected get => _io ?? Context
                ?? throw new InvalidOperationException(
                    $"{GetType().FullName} is not bound to a context");
            set => _io = value;
        }

        protected void EndContext()
        {
            Context?.End(this);
        }

        protected void EndContext(object sender, EventArgs e)
        {
            EndContext();
        }

        protected void UnwindContext()
        {
            Context?.Unwind(this);
        }

        protected void Track(Promise promise)
        {
            Context?.Track(promise);
        }

        protected void Dispose(IDisposable disposable)
        {
            Context?.Dispose(disposable);
        }

        protected IHandler Async => Context.Async();

        #endregion

        #region Protocol

        protected TProto Proxy<TProto>() where TProto : class
        {
            return IO.Proxy<TProto>();
        }

        protected TProto Proxy<TProto>(IHandler handler) where TProto : class
        {
            return handler.Proxy<TProto>();
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

        protected IViewLayer Show(IView view)
        {
            return Show(IO, view);
        }

        protected IViewLayer Show(IHandler handler, IView view)
        {
            return view.Display(Region(handler));
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
            return Proxy<IViewRegion>(handler);
        }

        protected Context AddRegion(IViewRegion region, Action<IViewRegion> init = null)
        {
            var context = Context.AddRegion(region);
            init?.Invoke(Region(context));
            return context;
        }

        #endregion

        #region Navigate

        protected C Next<C>() where C : class, IController
        {
            return Next<C>(Context);
        }

        protected C Next<C>(IHandler handler) 
            where C : class, IController
        {
            return handler.Next<C>();
        }

        protected Promise<Context> Next<C>(Action<C> action)
            where C : class, IController
        {
            return Next(Context, action);
        }

        protected Promise<Context> Next<C>(IHandler handler, Action<C> action)
            where C : class, IController
        {
            return handler.Next(action);
        }

        protected TargetActionBuilder<C, Promise<Context>> NextBlock<C>()
            where C : IController
        {
            return NextBlock<C>(Context);
        }

        protected TargetActionBuilder<C, Promise<Context>> NextBlock<C>(IHandler handler)
            where C : IController
        {
            return handler.NavigateBlock<C>(NavigationStyle.Next);
        }

        protected C Push<C>()
            where C : class, IController
        {
            return Push<C>(Context);
        }

        protected C Push<C>(IHandler handler) 
            where C : class, IController
        {
            return handler.Push<C>();
        }

        protected Promise<Context> Push<C>(Action<C> action)
            where C : class, IController
        {
            return Push(Context, action);
        }

        protected Promise<Context> Push<C>(IHandler handler, Action<C> action)
            where C : class, IController
        {
            return handler.Push(action);
        }

        protected TargetActionBuilder<C, Promise<Context>> PushBlock<C>()
            where C : IController
        {
            return PushBlock<C>(Context);
        }

        protected TargetActionBuilder<C, Promise<Context>> PushBlock<C>(IHandler handler)
            where C : IController
        {
            return handler.NavigateBlock<C>(NavigationStyle.Push);
        }

        protected C Partial<C>()
            where C : class, IController
        {
            return Partial<C>(Context);
        }

        protected C Partial<C>(IHandler handler)
            where C : class, IController
        {
            return handler.Partial<C>();
        }

        protected Promise<Context> Partial<C>(Action<C> action)
            where C : class, IController
        {
            return Partial(Context, action);
        }

        protected Promise<Context> Partial<C>(IHandler handler, Action<C> action)
            where C : class, IController
        {
            return handler.Partial(action);
        }

        protected TargetActionBuilder<C, Promise<Context>> PartialBlock<C>()
            where C : IController
        {
            return PartialBlock<C>(Context);
        }

        protected TargetActionBuilder<C, Promise<Context>> PartialBlock<C>(IHandler handler)
            where C : IController
        {
            return handler.NavigateBlock<C>(NavigationStyle.Partial);
        }

        protected Promise<Context> Navigate<C>(NavigationStyle style, Action<C> action)
            where C : class, IController
        {
            return Navigate(Context, style, action);
        }

        protected Promise<Context> Navigate<C>(
            IHandler handler, NavigationStyle style, Action<C> action)
            where C : class, IController
        {
            return handler.Navigate(style, action);
        }

        protected C Navigate<C>(NavigationStyle style) 
            where C : class, IController
        {
            return Navigate<C>(Context, style);
        }

        protected C Navigate<C>(IHandler handler, NavigationStyle style)
            where C : class, IController
        {
            return handler.Navigate<C>(style);
        }

        protected TargetActionBuilder<C, Promise<Context>> NavigateBlock<C>(
            NavigationStyle style)
            where C : class, IController
        {
            return NavigateBlock<C>(Context, style);
        }

        protected TargetActionBuilder<C, Promise<Context>> NavigateBlock<C>(
            IHandler handler, NavigationStyle style)
            where C : class, IController
        {
            return handler.NavigateBlock<C>(style);
        }

        public void GoBack() => GoBack(Context);

        protected void GoBack(IHandler handler) => handler.GoBack();

        #endregion

        #region Workflow

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
            Context = null;
            _io     = null;
        }

        ~Controller()
        {
            Dispose(false);
        }

        #endregion
    }
}
