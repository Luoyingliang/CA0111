using System.Reactive.Linq;

namespace Acorisoft.Miga.UI.Core
{
    public class Result<T>
    {
        private T _value;

        internal Result(PageAware pageAware, IObservable<T> publisher, string propertyName)
        {
            var disposable = publisher.ObserveOn(MGApp.MainThreadScheduler)
                .Subscribe(x =>
            {
                ((IPropertyEventSubscriber)pageAware).Received(new PropertyChangedEventArgs(propertyName));
                _value = x;
            });
            
            pageAware.Disposable.Add(disposable);
        }

        public T Value => _value;
    }
}