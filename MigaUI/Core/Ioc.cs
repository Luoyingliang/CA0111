
using Disposable = Acorisoft.Miga.Utils.Disposable;
using Container = DryIoc.Container;

namespace Acorisoft.Miga.UI.Core
{
    public class Ioc : Disposable
    {
        private readonly Container _container;
        private readonly ConcurrentDictionary<Type, MvvmInfo> _factories;
        private readonly ConcurrentDictionary<Guid, MvvmInfo> _mapping;
        private readonly object _sync;
    
        public class MvvmInfo
        {
            public Type View { get; init; }
            public Type ViewModel { get; init; }
        }

        public Ioc()
        {
            _container = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArguments).WithTrackingDisposableTransients());
            _factories = new ConcurrentDictionary<Type, MvvmInfo>();
            _mapping = new ConcurrentDictionary<Guid, MvvmInfo>();
            _sync = new object();

            RegisterInstance<PageInfoCollection, IPageInfoCollection>(new PageInfoCollection());
        }

        protected override void ReleaseUnmanagedResources()
        {
            _container.Dispose();
        }

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="TImpl">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface">注册的服务类型。</typeparam>
        public void RegisterInstance<TImpl, TInterface>(TImpl instance) where TImpl : TInterface
        {
            
            _container.RegisterInstance(instance);
            _container.Use(typeof(TInterface), instance);
        }

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="TImpl">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface2">注册的服务类型。</typeparam>
        public void RegisterInstance<TImpl, TInterface, TInterface2>(TImpl instance) where TImpl : TInterface, TInterface2
        {
            
            _container.RegisterInstance(instance);
            _container.Use(typeof(TInterface), instance);
            _container.Use(typeof(TInterface2), instance);
        }
        
        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="TImpl">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface2">注册的服务类型。</typeparam>
        /// <typeparam name="TInterface3">注册的服务类型。</typeparam>
        public void RegisterInstance<TImpl, TInterface, TInterface2, TInterface3>(TImpl instance) where TImpl : TInterface, TInterface2, TInterface3
        {
            
            _container.RegisterInstance(instance);
            _container.Use(typeof(TInterface), instance);
            _container.Use(typeof(TInterface2), instance);
            _container.Use(typeof(TInterface3), instance);
        }

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public void Register<T>() => _container.Register(typeof(T));
    
        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <param name="type">注册的服务类型。</param>
        public void Register(Type type) => _container.Register(type);
    
        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public void RegisterInstance<T>() => _container.RegisterInstance(typeof(T));
    
        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <param name="type">注册的服务类型。</param>
        public void RegisterInstance(Type type) => _container.RegisterInstance(type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterInstance<T>(T instance) => _container.RegisterInstance(instance);

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public T Resolve<T>() => (T)_container.Resolve(typeof(T));
        
        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public T Resolve<T>(Type type) => (T)_container.Resolve(type);
    
        /// <summary>
        /// 注册类型。
        /// </summary>
        public object Resolve(Type type) => _container.Resolve(type);

        /// <summary>
        /// 注册类型。
        /// </summary>
        /// <typeparam name="T">注册的服务类型。</typeparam>
        public T Locate<T>(Guid id)
        {
            if (_mapping.TryGetValue(id, out var info))
            {
                return (T)Resolve(info.ViewModel);
            }

            return default(T);
        }
    
        /// <summary>
        /// 注册视图
        /// </summary>
        /// <typeparam name="TView">视图类型。</typeparam>
        /// <typeparam name="TViewModel">视图模型类型。</typeparam>
        public void RegisterView<TView, TViewModel>() where TView : FrameworkElement where TViewModel : class
        {
            var vType = typeof(TView);
            var vmType = typeof(TViewModel);
            var info = new MvvmInfo
            {
                View = vType,
                ViewModel = vmType
            };
        
            if (_factories.TryAdd(vType, info) && _factories.TryAdd(vmType, info))
            {
                lock (_sync)
                {
                    _container.Register(vType);
                    _container.Register(vmType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vType"></param>
        /// <param name="vmType"></param>
        public void RegisterView(Type vType, Type vmType)
        {
            if (vType is null || vmType is null)
            {
                return;
            }
        
            var info = new MvvmInfo
            {
                View = vType,
                ViewModel = vmType
            };
        
            if (_factories.TryAdd(vType, info) && _factories.TryAdd(vmType, info))
            {
                
                lock (_sync)
                {
                    if (vType.IsDefined(typeof(GuidAttribute)) &&
                        vType.GetCustomAttribute<GuidAttribute>() is { } compileAttribute && 
                        Guid.TryParse(compileAttribute.Value, out var vid))
                    {
                        _mapping.TryAdd(vid, info);
                        
                    }
                    _container.Register(vType);
                    _container.Register(vmType);
                }
            }
        }

        /// <summary>
        /// 注册静态视图
        /// </summary>
        /// <typeparam name="TView">视图类型。</typeparam>
        /// <typeparam name="TViewModel">视图模型类型。</typeparam>
        public void RegisterInstanceView<TView, TViewModel>() where TView : FrameworkElement where TViewModel : class
        {
            var vType = typeof(TView);
            var vmType = typeof(TViewModel);
            var info = new MvvmInfo
            {
                View = vType,
                ViewModel = vmType
            };
        
            if (_factories.TryAdd(vType, info) && _factories.TryAdd(vmType, info))
            {
                lock (_sync)
                {
                    _container.Register(vType);
                    _container.Register(vmType, Reuse.Singleton);
                }
            }
        }

        /// <summary>
        /// 注册静态视图
        /// </summary>
        /// <typeparam name="TView">视图类型。</typeparam>
        /// <typeparam name="TViewModel">视图模型类型。</typeparam>
        /// <param name="viewModel">指定的视图模型类型。</param>
        public void RegisterInstanceView<TView, TViewModel>(TViewModel viewModel)
            where TView : FrameworkElement where TViewModel : class
        {
            if (viewModel is null)
            {
                return;
            }
        
            var vType = typeof(TView);
            var vmType = typeof(TViewModel);
            var info = new MvvmInfo
            {
                View = vType,
                ViewModel = vmType
            };
        
            if (_factories.TryAdd(vType, info) && _factories.TryAdd(vmType, info))
            {
                lock (_sync)
                {
                    _container.Register(vType);
                    _container.RegisterInstance(viewModel);
                }
            }
        }
    
        /// <summary>
        /// 定位视图
        /// </summary>
        /// <param name="viewModel">指定的视图模型。要求不能为空</param>
        /// <returns>返回指定的视图。</returns>
        public object Locate(object viewModel)
        {
            if (viewModel is null)
            {
                throw new InvalidOperationException($"arg {nameof(viewModel)} is null");
            }
        
            var type = viewModel.GetType();
        
            if (!_factories.TryGetValue(type, out var info) || 
                info?.ViewModel != type)
            {
                return null;
            }
        
            var v = _container.Resolve(info.View);

            if (v is null)
            {
                return null;
            }

            if (v is IDataContextBridge dcBridge)
            {
                dcBridge.DataContext = viewModel;
            }
            else if (v is FrameworkElement element)
            {
                element.DataContext = viewModel;
            }

            return v;
        }

        /// <summary>
        /// 定位视图
        /// </summary>
        /// <param name="type">指定的视图模型。要求不能为空</param>
        /// <returns>返回指定的视图。</returns>
        public object Locate(Type type)
        {
            if (type is null)
            {
                throw new InvalidOperationException($"arg {nameof(type)} is null");
            }
        
            if (!_factories.TryGetValue(type, out var info) ||
                info?.View != type)
            {
                return null;
            }
        
            var v = _container.Resolve(info.View);
            var vm = _container.Resolve(info.ViewModel);

            if (v is null || vm is null)
            {
                return null;
            }

            if (v is IDataContextBridge dcBridge)
            {
                dcBridge.DataContext = vm;
            }
            else if (v is FrameworkElement element)
            {
                element.DataContext = vm;
            }

            return v;
        }

        /// <summary>
        /// 定位视图
        /// </summary>
        /// <typeparam name="TViewModel">指定的视图模型类型。</typeparam>
        /// <returns>返回指定的视图。</returns>
        public TViewModel Locate<TViewModel>() where TViewModel : class
        {
            return Locate(typeof(TViewModel)) as TViewModel;
        }

        // /// <summary>
        // /// 注册指定程序集中的所有视图与视图模型。
        // /// </summary>
        // /// <param name="assemblies">操作的目标程序集列表。</param>
        // /// <param name="onlyReflectEntryAssembly">是否反射应用程序集。</param>
        // public void RegisterAllViews(IEnumerable<Assembly> assemblies, bool onlyReflectEntryAssembly)
        // {
        //     if (assemblies is null)
        //     {
        //         return;
        //     }
        //
        //     if (onlyReflectEntryAssembly)
        //     {
        //         assemblies ??= new[]
        //         {
        //             Assembly.GetExecutingAssembly(),
        //         };
        //     }
        //     else
        //     {
        //         assemblies ??= new[]
        //         {
        //             Assembly.GetExecutingAssembly(),
        //         };
        //     }
        //
        //     foreach (var assembly in assemblies)
        //     {
        //         RegisterAllViews(assembly);
        //     }
        // }

        /// <summary>
        /// 注册指定程序集中的所有视图与视图模型。
        /// </summary>
        /// <param name="assembly">操作的目标程序集列表。</param>
        public void RegisterAllViews(Assembly assembly)
        {
            if (assembly is null)
            {
                return;
            }

            foreach (var maybeTarget in assembly.GetTypes().Where(x => x.IsClass))
            {
                if (maybeTarget.GetCustomAttribute<PageTokenAttribute>() is { } page)
                {
                    RegisterView(page.View, page.ViewModel);
                }
            }
        }
    
        /// <summary>
        /// 注册指定程序集中的所有视图与视图模型。
        /// </summary>
        public void RegisterAllViews()
        {
            var implType = Classes.FindInterfaceImplmentation<IViewModelRegister>(Assembly.GetEntryAssembly());
        
            if (implType is null)
            {
                return;
            }

            var impl = (IViewModelRegister)Activator.CreateInstance(implType);
            impl?.Register(this);
        }
    
        /// <summary>
        /// 注册指定程序集中的所有视图与视图模型。
        /// </summary>
        /// <param name="assemblies">操作的目标程序集列表。</param>
        public void RegisterAllViewsFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies is null)
            {
                return;
            }

            foreach (var assembly in assemblies)
            {
                var implType = Classes.FindInterfaceImplmentation<IViewModelRegister>(assembly);
        
                if (implType is null)
                {
                    return;
                }

                var impl = (IViewModelRegister)Activator.CreateInstance(implType);
                impl?.Register(this);
            }
        }
    }
}