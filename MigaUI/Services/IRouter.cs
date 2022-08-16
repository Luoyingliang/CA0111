namespace Acorisoft.Miga.UI.Services
{
    public interface IRouter
    {
        
        void Route(ViewModelBase vm);
        void Route(ViewModelBase vm, object parameter);
        void Route<T>() where T : ViewModelBase;
        void Route<T>(object parameter) where T : ViewModelBase;
        void Route(Type type);
        void Route(Type type, object parameter);
        void Route(PageTokenAttribute attribute, Guid result);
        void Route(PageTokenAttribute attribute, Guid result, object parameter);
        
        bool CanGoForward();
        bool CanGoBack();
        void GoForward();
        void GoBack();
    }
    
    public interface IRouterAmbient
    {
        void SetHost(MGViewHost host);
        void SetFilter(IViewFilter filter);
        bool HasFilter { get; }
    }
    
    
    public interface IViewFilter
    {
        void Navigated(PageTokenAttribute attribute);
        void Navigated(PageAware vm);
        void Navigated(DialogAware vm);
    }

    internal class ViewService : IRouterAmbient, IRouter
    {
        private MGViewHost _host;
        private IViewFilter _filter;
        
        public void SetHost(MGViewHost host)
        {
            _host = host;
        }

        public void SetFilter(IViewFilter filter)
        {
            _filter = filter;
        }

        public void Route(ViewModelBase vm)
        {
            _host?.Route(vm);
            _filter?.Navigated(vm as PageAware);
        }

        public void Route(ViewModelBase vm, object parameter)
        {
            //
            //
            vm?.OnParameterReceived(parameter);
            
            //
            //
            Route(vm);
        }

        public void Route<T>() where T : ViewModelBase
        {
            var vm = MGApp.Resolve<ViewModelBase>(typeof(T));
            Route(vm);
            _filter?.Navigated(vm as PageAware);
        }

        public void Route<T>(object parameter) where T : ViewModelBase
        {
            var vm = MGApp.Resolve<ViewModelBase>(typeof(T));
            vm.OnParameterReceived(parameter);
            Route(vm);
            _filter?.Navigated(vm as PageAware);
        }

        public void Route(Type vmType)
        {
            if (vmType is null)
            {
                return;
            }

            try
            {
                var vm = MGApp.Resolve<ViewModelBase>(vmType);
                Route(vm);
            }
            catch
            {
                //
            }
        }

        public void Route(Type vmType, object parameter)
        {
            if (vmType is null)
            {
                return;
            }

            try
            {
                var vm = MGApp.Resolve<ViewModelBase>(vmType);
                vm?.OnParameterReceived(parameter);
                Route(vm);
            }
            catch
            {
                //
            }
        }

        public void Route(PageTokenAttribute attribute, Guid id)
        {
            if (attribute is null || id == Guid.Empty)
            {
                return;
            }
            
            _filter?.Navigated(attribute);
            _host?.Route(attribute, id);
        }

        public void Route(PageTokenAttribute attribute, Guid result, object parameter)
        {
            _host?.Route(attribute, result, parameter);
        }

        public bool CanGoForward()
        {
            return _host?.CanGoForward() ?? false;
        }

        public bool CanGoBack()
        {
            return _host?.CanGoBack() ?? false;
        }

        public void GoForward()
        {
            var vm = _host?.GoForward();
            _filter?.Navigated(vm);
        }

        public void GoBack()
        {

            var vm = _host?.GoBack();
            _filter?.Navigated(vm);
        }

        public bool HasFilter => _filter is not null;
    }
}