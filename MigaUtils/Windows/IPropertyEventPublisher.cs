using EventSubArray =
    System.Collections.Generic.List<Acorisoft.Miga.Utils.IPropertyEventSubscriber>;
// ReSharper disable ForCanBeConvertedToForeach

namespace Acorisoft.Miga.Utils
{
    /// <summary>
    /// <see cref="IPropertyEventPublisher"/> 表示一个属性事件发布者。
    /// </summary>
    public interface IPropertyEventPublisher
    {
        /// <summary>
        /// 发布属性变更完成事件参数。
        /// </summary>
        /// <param name="e">事件参数</param>
        void Publish(PropertyChangedEventArgs e);


        /// <summary>
        /// 发布属性正在变更事件参数。
        /// </summary>
        /// <param name="e">事件参数</param>
        void Publish(PropertyChangingEventArgs e);
    }

    internal interface IObservableIPropertyEventSubscriber
    {
        IDisposable Subscribe(IPropertyEventSubscriber subscriber);
    }

    public class PropertyEventPublisher : IPropertyEventPublisher, IObservableIPropertyEventSubscriber
    {
        private readonly object        _sync        = new object();
        private readonly EventSubArray _array = new EventSubArray();
        
        private class RemoveWhenDispose : IDisposable
        {
            public void Dispose()
            {
                Array._array.Remove(Item);
            }
            
            public PropertyEventPublisher Array { get; init; }
            public IPropertyEventSubscriber Item { get; init; }
        }
        
        public IDisposable Subscribe(IPropertyEventSubscriber subscriber)
        {
            if (subscriber is null)
            {
                return null;
            }

            lock (_sync)
            {
                if (_array.Any(x => ReferenceEquals(x, subscriber)))
                {
                    return null;
                }
                
                _array.Add(subscriber);
                return new RemoveWhenDispose { Array = this, Item = subscriber };
            }
        }

        public void Publish(PropertyChangedEventArgs e)
        {
            lock (_sync)
            {
                for (var i = 0; i < _array.Count; i++)
                {
                    var sub = _array[i];
                    sub.Received(e);
                }
            }
        }

        public void Publish(PropertyChangingEventArgs e)
        {
            lock (_sync)
            {
                for (var i = 0; i < _array.Count; i++)
                {
                    var sub = _array[i];
                    sub.Received(e);
                }
            }
        }
    }
}