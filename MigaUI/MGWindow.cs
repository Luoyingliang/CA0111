
using Acorisoft.Miga.UI.Services;

namespace Acorisoft.Miga.UI
{
    public class ClassicWindow : Window
    {
        static ClassicWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassicWindow), new FrameworkPropertyMetadata(typeof(ClassicWindow)));
        }
        
        
        protected ClassicWindow()
        {
            //
            // Event Subscribe
            Loaded += OnLoaded;
            Closing += OnUnloaded;

            //
            //
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnWindowClose));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnWindowMinimum));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnWindowRestore));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnWindowRestore));

            
            //
            //
            Style ??= Application.Current.Resources[nameof(ClassicWindow)] as Style;
        }

        protected virtual void OnUnloaded(object sender, CancelEventArgs e)
        {
            (DataContext as AppViewModelBase)?.OnStop();
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            (DataContext as AppViewModelBase)?.OnStart();
        }

        #region SystemCommands

        private void OnWindowClose(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowMinimum(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnWindowRestore(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion SystemCommands

        /// <summary>
        /// 实现 <see cref="TitleBar"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = DependencyProperty.Register(
            "TitleBar",
            typeof(object),
            typeof(ClassicWindow),
            new PropertyMetadata(default(object)));

        /// <summary>
        /// 实现 <see cref="TitleBarTemplate"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarTemplateProperty = DependencyProperty.Register(
            "TitleBarTemplate",
            typeof(DataTemplate),
            typeof(ClassicWindow),
            new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// 实现 <see cref="TitleBarTemplateSelector"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarTemplateSelectorProperty = DependencyProperty.Register(
            "TitleBarTemplateSelector",
            typeof(DataTemplateSelector),
            typeof(ClassicWindow),
            new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// 实现 <see cref="TitleBarStringFormat"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarStringFormatProperty = DependencyProperty.Register(
            "TitleBarStringFormat",
            typeof(string),
            typeof(ClassicWindow),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color",
            typeof(Brush),
            typeof(ClassicWindow),
            new PropertyMetadata(default(Brush)));

        /// <summary>
        /// 获取或设置 <see cref="TitleBarStringFormat"/> 属性。
        /// </summary>
        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBarStringFormat"/> 属性。
        /// </summary>
        public string TitleBarStringFormat
        {
            get => (string)GetValue(TitleBarStringFormatProperty);
            set => SetValue(TitleBarStringFormatProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBarTemplateSelector"/> 属性。
        /// </summary>
        public DataTemplateSelector TitleBarTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TitleBarTemplateSelectorProperty);
            set => SetValue(TitleBarTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBarTemplate"/> 属性。
        /// </summary>
        public DataTemplate TitleBarTemplate
        {
            get => (DataTemplate)GetValue(TitleBarTemplateProperty);
            set => SetValue(TitleBarTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBar"/> 属性。
        /// </summary>
        public object TitleBar
        {
            get => GetValue(TitleBarProperty);
            set => SetValue(TitleBarProperty, value);
        }
    }
    
    public partial class MGWindow : Window
    {
        static MGWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MGWindow), new FrameworkPropertyMetadata(typeof(MGWindow)));
        }

        private readonly HashSet<CommandBinding> _immutableCommandBindings;
        private readonly HashSet<InputBinding> _immutableInputBindings;

        protected MGWindow()
        {
            //
            // Event Subscribe
            Loaded += OnLoaded;
            Closing += OnUnloaded;
            DataContextChanged += OnDataContextChangedImpl;

            //
            //
            _immutableCommandBindings = new HashSet<CommandBinding>();
            _immutableInputBindings = new HashSet<InputBinding>();

            //
            //
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnWindowClose));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnWindowMinimum));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnWindowRestore));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnWindowRestore));
            
            //
            //
            Style ??= Application.Current.Resources[nameof(MGWindow)] as Style;
        }

        private void OnDataContextChangedImpl(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AppViewModelBase filter)
            {
                MGApp.Resolve<IRouterAmbient>().SetFilter(filter);
                MGApp.Resolve<IDialogAmbient>().SetFilter(filter);
            }

            OnDataContextChanged(sender, e);
        }
        
        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// 重写该方法，能够在命令调用时，修改导航结果。
        /// </summary>
        /// <returns>重写该返回值，能够决定导航是否继续。</returns>
        protected virtual bool OnBeforeNavigate() => true;

        protected virtual void OnUnloaded(object sender, CancelEventArgs e)
        {
            (DataContext as AppViewModelBase)?.OnStop();
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {

            //
            //
            Backup();
            MGApp.Resolve<ICommandCenter>().Register(this);
        }

        protected internal void Backup()
        {
            foreach (CommandBinding commandBinding in CommandBindings)
            {
                if (_immutableCommandBindings.Contains(commandBinding))
                {
                    continue;
                }

                _immutableCommandBindings.Add(commandBinding);
            }

            foreach (InputBinding inputBinding in InputBindings)
            {
                if (_immutableInputBindings.Contains(inputBinding))
                {
                    continue;
                }

                _immutableInputBindings.Add(inputBinding);
            }
        }

        protected internal void Recovery()
        {
            for (var i = 0; i < CommandBindings.Count; i++)
            {
                var binding = CommandBindings[i];
                if (!_immutableCommandBindings.Contains(binding))
                {
                    CommandBindings.RemoveAt(i);
                }
            }

            for (var i = 0; i < InputBindings.Count; i++)
            {
                var binding = InputBindings[i];
                if (!_immutableInputBindings.Contains(binding))
                {
                    InputBindings.RemoveAt(i);
                }
            }
        }

        #region SystemCommands

        private void OnWindowClose(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowMinimum(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnWindowRestore(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion SystemCommands

        /// <summary>
        /// 实现 <see cref="TitleBar"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = DependencyProperty.Register(
            "TitleBar",
            typeof(object),
            typeof(MGWindow),
            new PropertyMetadata(default(object)));

        /// <summary>
        /// 实现 <see cref="TitleBarTemplate"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarTemplateProperty = DependencyProperty.Register(
            "TitleBarTemplate",
            typeof(DataTemplate),
            typeof(MGWindow),
            new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// 实现 <see cref="TitleBarTemplateSelector"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarTemplateSelectorProperty = DependencyProperty.Register(
            "TitleBarTemplateSelector",
            typeof(DataTemplateSelector),
            typeof(MGWindow),
            new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// 实现 <see cref="TitleBarStringFormat"/> 属性的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleBarStringFormatProperty = DependencyProperty.Register(
            "TitleBarStringFormat",
            typeof(string),
            typeof(MGWindow),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color",
            typeof(Brush),
            typeof(MGWindow),
            new PropertyMetadata(default(Brush)));

        /// <summary>
        /// 获取或设置 <see cref="TitleBarStringFormat"/> 属性。
        /// </summary>
        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBarStringFormat"/> 属性。
        /// </summary>
        public string TitleBarStringFormat
        {
            get => (string)GetValue(TitleBarStringFormatProperty);
            set => SetValue(TitleBarStringFormatProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBarTemplateSelector"/> 属性。
        /// </summary>
        public DataTemplateSelector TitleBarTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TitleBarTemplateSelectorProperty);
            set => SetValue(TitleBarTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBarTemplate"/> 属性。
        /// </summary>
        public DataTemplate TitleBarTemplate
        {
            get => (DataTemplate)GetValue(TitleBarTemplateProperty);
            set => SetValue(TitleBarTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TitleBar"/> 属性。
        /// </summary>
        public object TitleBar
        {
            get => GetValue(TitleBarProperty);
            set => SetValue(TitleBarProperty, value);
        }
    }
    
    public abstract class MGWindow<TViewModel> : MGWindow where TViewModel : AppViewModelBase
    {
        protected sealed override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);

            //
            //
            var vm = MGApp.Resolve<TViewModel>();
            vm.OnStart();

            DataContext = vm;
            
            //
            //
            OnLoaded();
        }

        protected virtual void OnLoaded()
        {
            
        }
    }
    
    public abstract class MGWindow<TViewModel, TStartup> : MGWindow 
        where TViewModel : AppViewModelBase
        where TStartup : PageAware
    {
        protected sealed override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);

            //
            //
            var vm = MGApp.Resolve<TViewModel>();
            vm.OnStart();

            DataContext = vm;
            
            //
            MGApp.Route((ViewModelBase)MGApp.Resolve(typeof(TStartup)));
            
            
            //
            //
            OnLoaded();
        }

        protected virtual void OnLoaded()
        {
            
        }
    }
}