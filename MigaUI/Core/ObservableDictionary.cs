using System.Collections.ObjectModel;

namespace Acorisoft.Miga.UI.Core
{
    public class ObservableDictionary
    {
        private readonly Dictionary<string, object> _appSetting;
        private readonly List<ObservableItem>       _items;
        private readonly PropertyEventPublisher     _publisher;

        public ObservableDictionary(IPropertyEventSubscriber subscriber)
        {
            _appSetting = new Dictionary<string, object>(13);
            _publisher  = new PropertyEventPublisher();
            _publisher.Subscribe(subscriber);
            _items  = new List<ObservableItem>(32);
            Storage = new ReadOnlyDictionary<string, object>(_appSetting);
        }

        internal bool Get(string key, out object value)
        {
            return _appSetting.TryGetValue(key, out value);
        }

        internal void Set(string key, object value)
        {
            if (_appSetting.ContainsKey(key))
            {
                _appSetting[key] = value;
            }
            else
            {
                _appSetting.Add(key, value);
            }
        }

        internal void Raise(string propertyName)
        {
            _publisher.Publish(new PropertyChangedEventArgs(propertyName));
        }

        public ObservableItem<T> Get<T>(string propertyName)
        {
            var item = new ObservableItem<T>(this, propertyName, default);
            _items.Add(item);
            return item;
        }

        public ObservableItem<T> Get<T>(string propertyName, T defaultValue)
        {
            var item = new ObservableItem<T>(this, propertyName, defaultValue);
            _items.Add(item);
            return item;
        }

        public ReadOnlyDictionary<string, object> Storage { get; }

        public void Install(Dictionary<string, object> dictionary)
        {
            _appSetting.Clear();

            foreach (var item in dictionary)
            {
                ((IDictionary<string, object>)_appSetting).Add(item);
            }

            foreach (var item in _items)
            {
                item.Update();
            }
        }
    }

    public abstract class ObservableItem
    {
        private protected readonly string               _propertyName;
        private protected readonly ObservableDictionary _dictionary;

        protected ObservableItem(string propertyName, ObservableDictionary dictionary)
        {
            _propertyName = propertyName;
            _dictionary   = dictionary;
        }

        protected internal void Update()
        {
            _dictionary.Raise(_propertyName);
        }
    }

    public class ObservableItem<T> : ObservableItem
    {
        private readonly T _defaultValue;

        internal ObservableItem(ObservableDictionary dictionary, string propertyName, T defaultValue) 
            : base(propertyName, dictionary)
        {
            _defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                if (_dictionary.Get(_propertyName, out var val))
                {
                    return (T)val;
                }

                _dictionary.Set(_propertyName, _defaultValue);
                return _defaultValue;
            }
            set
            {
                _dictionary.Set(_propertyName, value);
                Update();
            }
        }
    }
}