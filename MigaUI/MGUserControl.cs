namespace Acorisoft.Miga.UI
{
    public abstract class MGUserControl : UserControl
    {
        
    }
    
    
    public abstract class MGPageView<TViewModel> : UserControl where TViewModel : ViewModelBase
    {
        private TViewModel _cacheViewModel;

        protected MGPageView()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SkipDisposePass)
            {
                return;
            }

            if (ViewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public TViewModel ViewModel
        {
            get
            {
                if (_cacheViewModel is null || !ReferenceEquals(_cacheViewModel, DataContext))
                {
                    _cacheViewModel = DataContext as TViewModel;
                }

                return _cacheViewModel;
            }
        }
    }

    public abstract class MGDialogView<TViewModel> : MGUserControl where TViewModel : DialogAware
    {
        private TViewModel _cacheViewModel;
        
        protected MGDialogView()
        {
            Loaded   += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
        
        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsTemporaryEntry)
            {
                return;
            }

            if (ViewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public TViewModel ViewModel
        {
            get
            {
                if (_cacheViewModel is null || !ReferenceEquals(_cacheViewModel, DataContext))
                {
                    _cacheViewModel = DataContext as TViewModel;
                }

                return _cacheViewModel;
            }
        }
    }
}