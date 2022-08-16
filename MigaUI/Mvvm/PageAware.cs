using System.Reactive.Concurrency;

namespace Acorisoft.Miga.UI.Mvvm
{
    public abstract class PageAware : ViewModelBase
    {
        protected PageAware()
        {
            Disposable = new DisposableCollector();
        }

        protected override void ReleaseManagedResources()
        {
            Disposable.Dispose();
        }

        protected Result<T> ToProperty<T>(string name, IObservable<T> publisher)
        {
            return new Result<T>(this, publisher, name);
        }

#pragma warning disable CA1822
        protected internal IScheduler Scheduler => MGApp.MainThreadScheduler;
#pragma warning restore CA1822

        protected internal DisposableCollector Disposable { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool QueryByLeaving { get; protected set; }
    }
}