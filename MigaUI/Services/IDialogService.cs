using System.Threading.Tasks;

namespace Acorisoft.Miga.UI.Services
{
    public interface IDialogAmbient
    {
        void SetHost(MGDialogHost host);
        void SetFilter(AppViewModelBase filter);
    }
    public interface IDialogService
    {
        void Completed();
        void Cancel();
        bool CanGoBack();
        bool CanGoForward();
        void GoBack();
        void GoForward();
        Task<IsCompleted<object>> ShowDialog(DialogAware vm);
        Task<IsCompleted<T>> ShowDialog<T, TViewModel>() where TViewModel : DialogAware;
        Task<IsCompleted<T>> ShowDialog<T>(Type dialogType);
        Task<IsCompleted<T>> ShowDialog<T>(DialogAware dialogViewModel);
    }

    internal class DialogService : IDialogService, IDialogAmbient
    {
        private MGDialogHost _host;
        private IViewFilter  _filter;
        
        public void SetHost(MGDialogHost host)
        {
            _host = host;
        }

        public void SetFilter(AppViewModelBase filter)
        {
            _filter = filter;
        }

        public void Completed()
        {
            _host?.Completed();
        }

        public void Cancel()
        {
            _host?.Cancel();
        }

        public bool CanGoBack()
        {
            return _host?.CanGoBack() ?? false;
        }

        public bool CanGoForward()
        {
            return _host?.CanGoForward() ?? false;
        }

        public void GoBack()
        {
            _host?.GoBack();
        }

        public void GoForward()
        {
            _host?.GoForward();
        }

        public async Task<IsCompleted<object>> ShowDialog(DialogAware vm)
        {
            if (vm is null)
            {
                return new IsCompleted<object>{IsFinished = false};
            }
            
            
            if(_host is not null)
            {
                _host.ViewModel = vm;
                _filter?.Navigated(vm);
            }
            
            var tcs = vm._signal;
            var result = await tcs.Task;
            return new IsCompleted<object>
            {
                Result = result,
                IsFinished = vm.IsCompleted
            };
        }


        public async Task<IsCompleted<T>> ShowDialog<T, TViewModel>() where TViewModel : DialogAware
        {
            var vm = MGApp.Ioc.Resolve<DialogAware>(typeof(TViewModel));

            if (vm is null)
            {
                return new IsCompleted<T>{IsFinished = false};
            }
            
            if(_host is not null)
            {
                _host.ViewModel = vm;
                _filter?.Navigated(vm);
            }

            var tcs = vm._signal;
            var result = await tcs.Task;

            if(ReferenceEquals(result, default(T)))
            {
                return new IsCompleted<T>
                {
                    IsFinished = false
                };
            }

            return new IsCompleted<T>
            {
                Result = (T)result,
                IsFinished = vm.IsCompleted
            };
        }

        public async Task<IsCompleted<T>> ShowDialog<T>(Type dialogType)
        {
            var vm = MGApp.Ioc.Resolve<DialogAware>(dialogType);

            if (vm is null)
            {
                return new IsCompleted<T>{IsFinished = false};
            }

            if(_host is not null)
            {
                _host.ViewModel = vm;
                _filter?.Navigated(vm);
            }
            
            var tcs = vm._signal;
            var result = await tcs.Task;
            return new IsCompleted<T>
            {
                Result = (T)result,
                IsFinished = vm.IsCompleted
            };
        }

        public async Task<IsCompleted<T>> ShowDialog<T>(DialogAware dialogViewModel)
        {
            if (dialogViewModel is null)
            {
                return new IsCompleted<T>{IsFinished = false};
            }

            if(_host is not null)
            {
                _host.ViewModel = dialogViewModel;
                _filter?.Navigated(dialogViewModel);
            }

            var tcs = dialogViewModel._signal;
            var result = await tcs.Task;
            return new IsCompleted<T>
            {
                Result = (T)result,
                IsFinished = dialogViewModel.IsCompleted
            };
        }
    }
}