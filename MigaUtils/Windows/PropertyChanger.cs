using System.Runtime.CompilerServices;

namespace Acorisoft.Miga.Utils
{
    public abstract class PropertyChanger : Disposable, IPropertyEventSubscriber, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private PropertyChangedEventHandler PropertyChangedHandler;
        private PropertyChangingEventHandler PropertyChangingHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected internal void RaiseUpdated([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaiseUpdating([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            PropertyChangingHandler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected bool SetValue<T>(ref T source, T value, [CallerMemberName] string name = "")
        {
            if (string.IsNullOrEmpty(name)) return false;
            
            if (EqualityComparer<T>.Default.Equals(source, value))
            {
                return false;
            }

            RaiseUpdating(name);
            source = value;
            RaiseUpdated(name);
            return true;
        }

        public void Received(PropertyChangedEventArgs e)
        {
            if(e is null)return;
            
            PropertyChangedHandler?.Invoke(this, e);
        }

        public void Received(PropertyChangingEventArgs e)
        {
            if(e is null)return;

            PropertyChangingHandler?.Invoke(this, e);
        }
        
        
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => PropertyChangedHandler += value;
            remove => PropertyChangedHandler -= value;
        }
        
        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingHandler += value;
            remove => PropertyChangingHandler -= value;
        }
    }
}