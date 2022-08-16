using Acorisoft.Miga.UI.Services;

namespace Acorisoft.Miga.UI
{
    public class MGViewHost : MGViewHostBase
    {
        private readonly Stack<ViewModelBase> _lastStack;
        private readonly Stack<ViewModelBase> _nextStack;
        private ViewModelBase _current;

        static MGViewHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MGViewHost), new FrameworkPropertyMetadata(typeof(MGViewHost)));
        }

        public MGViewHost()
        {
            _lastStack = new Stack<ViewModelBase>(32);
            _nextStack = new Stack<ViewModelBase>(32);
            MGApp.Resolve<IRouterAmbient>().SetHost(this);
        }

        //
        //
        private void Snapshot(ViewModelBase current)
        {
            if (ReferenceEquals(current, _current))
            {
                return;
            }
            
            _lastStack.Push(_current);
            _current = current;
        }
        
        /// <summary>
        /// 重写该方法来决定是否导航
        /// </summary>
        /// <param name="vm">将要导航到的视图模型。</param>
        protected override void OnViewModelChanged(ViewModelBase vm)
        {
            if (_current is null)
            {
                _current = vm;
            }
            else
            {
                Snapshot(vm);
            }

            //
            // 获得页面
            var page = MGApp.Locate(vm);

            //
            //
            if (page is null)
            {
                return;
            }

            //
            // 获得命令服务
            var cs = MGApp.Resolve<ICommandService>();
            
            //
            // 注册命令
            cs.RegisterViewCommands(page);
            
            //
            // 设置数据上下文
            page.DataContext = vm;
            vm.OnStart();

            //
            // 设置内容
            Content = page;
        }

        #region IRouter Interface Members
        
        public void Route(ViewModelBase vm)
        {
            ViewModel = vm;
        }

        public bool CanGoForward() => _nextStack.Count > 0;

        public bool CanGoBack() => _lastStack.Count > 0;

        public PageAware GoForward()
        {
            if (!CanGoForward())
            {
                return null;
            }
            _lastStack.Push(_current);
            _current = _nextStack.Pop();
            Route(_current);
            return _current as PageAware;
        }

        public PageAware GoBack()
        {
            if (!CanGoBack())
            {
                return null;
            }
            _nextStack.Push(_current);
            
            //
            //
            _current = _lastStack.Pop();

            //
            //
            if (_current.SkipDisposePass)
            {
                _current.SkipDisposePass = false;
            }
            
            //
            //
            if (_current.IsTemporaryEntry)
            {
                _current = MGApp.Resolve(_current.GetType()) as PageAware;
            }
            
            Route(_current);
            return _current as PageAware;
        }
        
        public void Route(PageTokenAttribute attribute, Guid id)
        {
            MGApp.Locate(id);
        }

        public void Route(PageTokenAttribute attribute, Guid id, object parameter)
        {
            MGApp.Locate(id, parameter);
        }

        #endregion
        
        

        public override string ToString()
        {
            return Content?.GetType().Name ?? "无内容";
        }
    }
}