using System.IO;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Acorisoft.Miga.UI.Builtin;
using Acorisoft.Miga.UI.Services;
using NLog;
using NLog.Config;
using NLog.Targets;

// ReSharper disable InvertIf

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Acorisoft.Miga.UI
{
    // ReSharper disable once InconsistentNaming
    public abstract class MGApp : Application
    {
        private static readonly Ioc                       IocValue;
        private static readonly Lazy<IRouter>             RouterValue   = Get<IRouter>();
        private static readonly Lazy<IDialogService>      DialogValue   = Get<IDialogService>();
        private static readonly Lazy<IPageInfoCollection> PageInfoValue = Get<IPageInfoCollection>();
        private static          IScheduler                _scheduler;

        private static Lazy<T> Get<T>() => new Lazy<T>(() => IocValue.Resolve<T>());

        private readonly Mutex _singleton;

        static MGApp()
        {
            IocValue = new Ioc();

            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = "${basedir}/Logs/${shortdate}-${time}.txt",
                Layout   = "${level} ${time} | ${message}  ${exception} ${event-properties:myProperty}"
            };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;

            //
            //
            AppLogger = LogManager.GetLogger("App");
        }

        protected MGApp()
        {
            ConstructTimeInitializingIntern();
            AppDomain.CurrentDomain.UnhandledException       += UnhandledException;
            Current.DispatcherUnhandledException += UnhandledException;
        }

        private static void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG
#else
            e.Handled = true;
#endif
            AppLogger.Warn(e.Exception);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppLogger.Warn(e.ExceptionObject);
        }

        protected MGApp(string name) : this()
        {
            _singleton = new Mutex(true, name, out var createdNew);

            if (!createdNew)
            {
                MessageBox.Show("应用程序已经启动");
                Environment.Exit(0);
            }
        }

        #region Initializing

        #region ConstructTimeInitializing

        private void ConstructTimeInitializingIntern()
        {
            ConstructTimeInitializing();
            RegisterInstance<PageInfoCollection, IPageInfoCollection>(new PageInfoCollection());
            RegisterInstance<DialogService, IDialogAmbient, IDialogService>(new DialogService());
            RegisterInstance<ViewService, IRouter, IRouterAmbient>(new ViewService());
            RegisterInstance<CommandCenter, ICommandService, ICommandAmbient, ICommandCenter>(new CommandCenter());
            IocValue.RegisterAllViews();
            RegisterCommonDialogs();
        }

        /// <summary>
        /// 构造函数调用期间的初始化
        /// </summary>
        protected virtual void ConstructTimeInitializing()
        {
        }

        #endregion

        protected sealed override void OnStartup(StartupEventArgs e)
        {
            //
            //
            Args = e.Args;

            //
            //
            RuntimeInitializingIntern();
            base.OnStartup(e);
        }

        #region RuntimeInitializing

        private void RuntimeInitializingIntern()
        {
            _scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current!);
            RegisterAppViewModel();
            RuntimeInitializing();
        }

        protected virtual void RuntimeInitializing()
        {
        }

        #endregion

        #endregion

        /// <summary>
        /// 构造函数调用期间的初始化
        /// </summary>
        protected virtual void RegisterCommonDialogs()
        {
            IocValue.RegisterView<NotifyView, NotifyViewModel>();
            IocValue.RegisterView<ConfirmView, ConfirmViewModel>();
        }

        protected abstract void RegisterAppViewModel();


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            //
            // 释放
            IocValue.Dispose();
            _singleton?.ReleaseMutex();
            _singleton?.Dispose();
        }

        #region Scheduler

        public static IScheduler MainThreadScheduler => _scheduler;

        #endregion

        #region PageInfo

        public static IPageInfoCollection PageInfo => PageInfoValue.Value;

        #endregion

        #region Ioc

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="TImpl">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface">注册的服务类型。</typeparam>
        public static void RegisterInstance<TImpl, TInterface>(TImpl instance) where TImpl : TInterface
        {
            IocValue.RegisterInstance<TImpl, TInterface>(instance);
        }

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="TImpl">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface2">注册的服务类型。</typeparam>
        public static void RegisterInstance<TImpl, TInterface, TInterface2>(TImpl instance)
            where TImpl : TInterface, TInterface2
        {
            IocValue.RegisterInstance<TImpl, TInterface, TInterface2>(instance);
        }

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="TImpl">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface2">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface3">注册的服务类型。</typeparam>
        public void RegisterInstance<TImpl, TInterface, TInterface2, TInterface3>(TImpl instance)
            where TImpl : TInterface, TInterface2, TInterface3
        {
            IocValue.RegisterInstance<TImpl, TInterface, TInterface2, TInterface3>(instance);
        }

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public static void Register<T>() => IocValue.Register<T>();

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <param name="type">注册的服务类型。</param>
        public static void Register(Type type) => IocValue.Register(type);

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public static void RegisterInstance<T>() => IocValue.RegisterInstance(typeof(T));

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <param name="type">注册的服务类型。</param>
        public static void RegisterInstance(Type type) => IocValue.RegisterInstance(type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInstance<T>(T instance) => IocValue.RegisterInstance(instance);

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public static T Resolve<T>() => (T)IocValue.Resolve(typeof(T));

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public static T Resolve<T>(Type type) => (T)IocValue.Resolve(type);

        /// <summary>
        /// 注册类型。
        /// </summary>
        public static object Resolve(Type type) => IocValue.Resolve(type);

        /// <summary>
        /// 注册类型。
        /// </summary>
        public static void Locate(Guid id)
        {
            var vm = IocValue.Locate<ViewModelBase>(id);
            Route(vm);
        }

        public static void Locate(PageInfo pageInfo)
        {
            if (pageInfo is null)
            {
                return;
            }

            if (!Guid.TryParse(pageInfo.Id, out var id))
            {
                return;
            }

            var vm = IocValue.Locate<ViewModelBase>(id);
            Route(vm);
        }

        /// <summary>
        /// 注册静态视图
        /// </summary>
        /// <typeparam name="TView">视图类型。</typeparam>
        /// <typeparam name="TViewModel">视图模型类型。</typeparam>
        /// <param name="viewModel">指定的视图模型类型。</param>
        public static void RegisterInstanceView<TView, TViewModel>(TViewModel viewModel)
            where TView : FrameworkElement where TViewModel : class
        {
            IocValue.RegisterInstanceView<TView, TViewModel>(viewModel);
        }

        /// <summary>
        /// 注册类型。
        /// </summary>
        public static void Locate(Guid id, object parameter)
        {
            var vm = IocValue.Locate<ViewModelBase>(id);
            vm?.OnParameterReceived(parameter);
            Route(vm);
        }

        /// <summary>
        /// 定位视图
        /// </summary>
        /// <param name="viewModel">指定的视图模型。要求不能为空</param>
        /// <returns>返回指定的视图。</returns>
        public static FrameworkElement Locate(object viewModel)
        {
            return (FrameworkElement)IocValue.Locate(viewModel);
        }

        /// <summary>
        /// 定位视图
        /// </summary>
        /// <param name="type">指定的视图模型。要求不能为空</param>
        /// <returns>返回指定的视图。</returns>
        public static FrameworkElement Locate(Type type)
        {
            return (FrameworkElement)IocValue.Locate(type);
        }

        /// <summary>
        /// 定位视图
        /// </summary>
        /// <typeparam name="TViewModel">指定的视图模型类型。</typeparam>
        /// <returns>返回指定的视图。</returns>
        public TViewModel Locate<TViewModel>() where TViewModel : FrameworkElement
        {
            return Locate(typeof(TViewModel)) as TViewModel;
        }

        /// <summary>
        /// 容器
        /// </summary>
        public static Ioc Ioc => IocValue;

        #endregion

        #region Router

        /// <summary>
        /// 
        /// </summary>
        public static IRouter Router => RouterValue.Value;

        public static void Route(ViewModelBase vm) => RouterValue.Value.Route(vm);
        public static void Route(ViewModelBase vm, object parameter) => RouterValue.Value.Route(vm, parameter);
        public static void Route<T>() where T : ViewModelBase => RouterValue.Value.Route<T>();

        public static void Route<T>(object parameter) where T : ViewModelBase
        {
            RouterValue.Value.Route<T>(parameter);
        }

        public static void Route(Type type) => RouterValue.Value.Route(type);

        public static void Route(PageTokenAttribute attribute, Guid result) =>
            RouterValue.Value.Route(attribute, result);

        #endregion

        #region Dialog

        /// <summary>
        /// 
        /// </summary>
        public static IDialogService DialogService => DialogValue.Value;

        public static Task<IsCompleted<object>> ShowDialog(Type vmType)
        {
            var vm = Ioc.Resolve<DialogAware>(vmType);
            return DialogValue.Value.ShowDialog(vm);
        }

        public static Task<IsCompleted<object>> ShowDialog(DialogAware vm) => DialogValue.Value.ShowDialog(vm);

        public static Task<IsCompleted<T>> ShowDialog<T, TViewModel>() where TViewModel : DialogAware =>
            DialogValue.Value.ShowDialog<T, TViewModel>();

        public static Task<IsCompleted<T>> ShowDialog<T>(Type dialogType) =>
            DialogValue.Value.ShowDialog<T>(dialogType);

        public static Task<IsCompleted<T>> ShowDialog<T>(DialogAware vm) => DialogValue.Value.ShowDialog<T>(vm);

        public static async Task<bool> Confirm(string title, string content)
        {
            var confirm = new ConfirmViewModel
            {
                Title   = title,
                Content = content
            };

            var result = await ShowDialog(confirm);
            return result.IsFinished;
        }

        public static async Task Notify(string content)
        {
            var notify = new NotifyViewModel()
            {
                Title   = "提示",
                Content = content
            };

            await ShowDialog(notify);
        }

        public static async Task Notify(string title, string content)
        {
            var notify = new NotifyViewModel()
            {
                Title   = title,
                Content = content
            };

            await ShowDialog(notify);
        }

        #endregion

        #region NLog

        public static ILogger AppLogger { get; }

        #endregion

        #region Directory

        public static string GetDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        #endregion

        public string[] Args { get; private set; }
    }
}