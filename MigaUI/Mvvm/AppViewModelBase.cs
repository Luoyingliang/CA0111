using Acorisoft.Miga.UI.Services;

namespace Acorisoft.Miga.UI.Mvvm
{
    public abstract class AppViewModelBase : ViewModelBase, IViewFilter
    {
        public virtual void Navigated(PageTokenAttribute attribute)
        {
        }

        public virtual void Navigated(PageAware vm)
        {
            CurrentViewModel = vm;
        }
        
        public virtual void Navigated(DialogAware vm)
        {
            vm.OwnerPage = CurrentViewModel;
        }

        #region OnStop
        
        protected internal sealed override void OnStop()
        {
            OnStopImpl();
        }

        private void OnStopImpl()
        {
            CurrentViewModel?.OnStop();
            StopOverride();
        }
        
        protected virtual void StopOverride()
        {
        }

        #endregion

        protected PageAware CurrentViewModel { get; private set; }
    }
}