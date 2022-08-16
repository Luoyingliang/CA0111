
namespace Acorisoft.Miga.UI
{
    public abstract class MGViewHostBase : Control
    {
        #region Static Methods

        
        static MGViewHostBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MGViewHostBase), new FrameworkPropertyMetadata(typeof(MGViewHostBase)));
            ViewModelProperty = DependencyProperty.Register(
                "ViewModel",
                typeof(object),
                typeof(MGViewHostBase),
                new PropertyMetadata(default(object), OnViewModelChanged));

            ContentPropertyKey = DependencyProperty.RegisterReadOnly(
                "Content",
                typeof(object),
                typeof(MGViewHostBase),
                new PropertyMetadata(default(object)));
            ContentProperty = ContentPropertyKey.DependencyProperty;
        }
        
        public static readonly DependencyProperty ViewModelProperty;
        public static readonly DependencyPropertyKey ContentPropertyKey;
        public static readonly DependencyProperty ContentProperty;

        internal static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var host = (MGViewHostBase)d;

            if (e.OldValue is ViewModelBase oldPage)
            {
                oldPage.OnStop();
            }
            
            if (e.NewValue is not null)
            {
                var waitForNavigating = e.NewValue switch
                {
                    Type maybeVMType when maybeVMType.IsAssignableTo(typeof(ViewModelBase)) => MGApp.Resolve<ViewModelBase>(maybeVMType),
                    // ReSharper disable once MergeCastWithTypeCheck
                    ViewModelBase @base => @base,
                    _ => null
                };

                if (waitForNavigating is null)
                {
                    return;
                }
                
                host.OnViewModelChanged(waitForNavigating);
            }
            else
            {
                d.ClearValue(ContentPropertyKey);
            }
        }


        #endregion
        
        protected virtual void OnViewModelChanged(ViewModelBase vm)
        {
            
        }

        /// <summary>
        /// 内容
        /// </summary>
        public object Content
        {
            get => GetValue(ContentProperty);
            protected set => SetValue(ContentPropertyKey, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public object ViewModel
        {
            get => (object)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
               
    }
}