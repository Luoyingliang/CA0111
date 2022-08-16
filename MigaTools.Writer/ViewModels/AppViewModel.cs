using Acorisoft.Miga.UI.Mvvm;

namespace Acorisoft.MigaTools.Writer.ViewModels
{
    public class AppViewModel : AppViewModelBase
    {
        public override void Navigated(PageAware vm)
        {
            IsGoBackVisibility = vm is not HomeViewModel ? Visibility.Collapsed : Visibility.Visible; 
            base.Navigated(vm);
        }

        private Visibility _isGoBackVisibility;

        /// <summary>
        /// 获取或设置 <see cref="IsGoBackVisibility"/> 属性。
        /// </summary>
        public Visibility IsGoBackVisibility
        {
            get => _isGoBackVisibility;
            set => SetValue(ref _isGoBackVisibility, value);
        }
    }
}