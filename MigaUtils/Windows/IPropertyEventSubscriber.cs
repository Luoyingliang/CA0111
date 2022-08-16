namespace Acorisoft.Miga.Utils
{
    /// <summary>
    /// <see cref="IPropertyEventPublisher"/> 表示一个属性事件订阅者。
    /// </summary>
    public interface IPropertyEventSubscriber
    {
        public void Subscribe(IPropertyEventPublisher publisher)
        {
            var opes = (IObservableIPropertyEventSubscriber)publisher;
            opes.Subscribe(this);
        }
        
        /// <summary>
        /// 接收属性变更完成事件参数。
        /// </summary>
        /// <param name="e">事件参数</param>
        void Received(PropertyChangedEventArgs e);
        
        /// <summary>
        /// 接收属性正在变更事件参数。
        /// </summary>
        /// <param name="e">事件参数</param>
        void Received(PropertyChangingEventArgs e);
    }
}