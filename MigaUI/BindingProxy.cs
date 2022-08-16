namespace Acorisoft.Miga.UI
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(object),
            typeof(BindingProxy),
            new PropertyMetadata(default(object)));
        protected sealed override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
    
    public class BindingProxy<T> : Freezable
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(T),
            typeof(BindingProxy<T>),
            new PropertyMetadata(default(T)));
        
        protected sealed override Freezable CreateInstanceCore()
        {
            return new BindingProxy<T>();
        }
        public T ViewModel
        {
            get => (T)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}