using Acorisoft.Miga.UI.Services;

namespace Acorisoft.Miga.UI.Services
{
    public interface ICommandCenter : ICommandService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ambient"></param>
        void Register(ICommandAmbient ambient);
    }

    public interface ICommandService
    {
        /// <summary>
        /// 注册视图中的所有命令
        /// </summary>
        /// <param name="element">要注册的元素</param>
        void RegisterViewCommands(FrameworkElement element);
    }

    public interface ICommandAmbient
    {
        /// <summary>
        /// 注册视图中的所有命令
        /// </summary>
        /// <param name="element">要注册的元素</param>
        void RegisterViewCommands(FrameworkElement element);
    }

    public class CommandCenter : ICommandCenter, ICommandAmbient
    {
        private ICommandAmbient _ambient;
        
        public void RegisterViewCommands(FrameworkElement element)
        {
            _ambient?.RegisterViewCommands(element);
        }

        public void Register(ICommandAmbient ambient)
        {
            if (ambient is null)
            {
                return;
            }
            
            _ambient = ambient;
        }
    }
}

namespace Acorisoft.Miga.UI
{
    partial class MGWindow : ICommandAmbient
    {
        
        public void RegisterViewCommands(FrameworkElement element)
        {
            if (element is null)
            {
                return;
            }
            
            //
            //
            Recovery();
                
            //
            //
            CommandBindings.AddRange(element.CommandBindings);
            InputBindings.AddRange(element.InputBindings);
        }
    }
}