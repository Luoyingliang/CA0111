

namespace Acorisoft.Miga.UI.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _handler;
        private readonly Func<object, bool> _detector;
        
        public DelegateCommand(Action<object> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _detector = null;
        }
        
        public DelegateCommand(Action<object> handler, Func<object, bool> detector)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _detector = detector ?? throw new ArgumentNullException(nameof(detector));
        }
        
        public bool CanExecute(object parameter)
        {
            return _detector?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _handler?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
    
    public class DelegateRaiseUpdateCommand : ICommand
    {
        private readonly Action<object> _handler;
        private readonly Func<object, bool> _detector;
        
        public DelegateRaiseUpdateCommand(Action<object> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _detector = null;
        }
        
        public DelegateRaiseUpdateCommand(Action<object> handler, Func<object, bool> detector)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _detector = detector ?? throw new ArgumentNullException(nameof(detector));
        }
        
        public bool CanExecute(object parameter)
        {
            return _detector?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _handler?.Invoke(parameter);
        }

        public void Update()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}