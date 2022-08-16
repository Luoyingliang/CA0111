using System.Threading.Tasks;
using Acorisoft.Miga.UI.Services;
// ReSharper disable InvertIf

namespace Acorisoft.Miga.UI
{
    public class MGDialogHost : ContentControl
    {
        static MGDialogHost()
        {
            ViewModelProperty = DependencyProperty.Register(
                "ViewModel", 
                typeof(object), 
                typeof(MGDialogHost),
                new PropertyMetadata(default(object), OnViewModelChanged));
        
            DialogPropertyKey = DependencyProperty.RegisterReadOnly(
                "Dialog",
                typeof(object),
                typeof(MGDialogHost),
                new PropertyMetadata(default(object)));
            DialogProperty = DialogPropertyKey.DependencyProperty;
            
            TitlePropertyKey = DependencyProperty.RegisterReadOnly(
                "Title",
                typeof(string),
                typeof(MGDialogHost),
                new PropertyMetadata(string.Empty));
            
            TitleProperty = TitlePropertyKey.DependencyProperty;
            IsOpenedPropertyKey = DependencyProperty.RegisterReadOnly(
                "IsOpened",
                typeof(bool),
                typeof(MGDialogHost),
                new PropertyMetadata(Xaml.False));
            
            IsOpenedProperty = IsOpenedPropertyKey.DependencyProperty;
        }

        public static readonly DependencyProperty ViewModelProperty;
        public static readonly DependencyPropertyKey DialogPropertyKey;
        public static readonly DependencyProperty DialogProperty;
        public static readonly DependencyPropertyKey TitlePropertyKey;
        public static readonly DependencyProperty TitleProperty;
        public static readonly DependencyPropertyKey IsOpenedPropertyKey;
        public static readonly DependencyProperty IsOpenedProperty;

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dh = (MGDialogHost)d;
            dh.Focus();
            if (e.NewValue is not null)
            {
                var v = (MGUserControl)MGApp.Locate(e.NewValue);
                var vm = (DialogAware)e.NewValue;

                dh.PushDialog(vm, v);
                return;
            }
            
            if (dh._lastStack.Count > 0)
            {
                var vm = dh._lastStack.Pop();
                dh.PushDialog(vm);
            }
            else
            {
                d.ClearValue(DialogPropertyKey);
                d.ClearValue(IsOpenedPropertyKey);
                d.ClearValue(TitlePropertyKey);
                dh._current = null;
            }
        }

        private void PushDialog(DialogAware vm)
        {
            var v = (MGUserControl)MGApp.Locate(vm);
            PushDialog(vm, v);
        }
        
        private void PushDialog(DialogAware vm, MGUserControl v)
        {
            if (_current is not null)
            {
                _lastStack.Push(_current);
            }
            
            _current = vm;
            vm.OnStart();
            v.Focus();
            
            SetValue(DialogPropertyKey, v);
            SetValue(IsOpenedPropertyKey, Xaml.True);
            SetValue(TitlePropertyKey, vm.Title);
            
        }
        
        private void PushDialogNoPush(DialogAware vm)
        {
            _current = vm;
            
            var v = (MGUserControl)MGApp.Locate(vm);
            PushDialogWithoutRecord(vm, v);
            
        }
        
        private void PushDialogWithoutRecord(DialogAware vm, MGUserControl v)
        {
            _current = vm;
            v.Focus();

            SetValue(DialogPropertyKey, v);
            SetValue(IsOpenedPropertyKey, Xaml.True);
            SetValue(TitlePropertyKey, vm.Title);
            
        }

        private readonly Stack<DialogAware> _lastStack;
        private readonly Stack<DialogAware> _nextStack;
        private DialogAware _current;

        public MGDialogHost()
        {
            _lastStack = new Stack<DialogAware>();
            _nextStack = new Stack<DialogAware>();
            _current = null;
            
            CommandBindings.Add(new CommandBinding(DialogCommands.Completed, Executed_Complete, CanExecute_Complete));
            CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, Executed_Cancel));
            
            var ds = MGApp.Resolve<IDialogAmbient>();
            ds?.SetHost(this);
        }

        private void CanExecute_Complete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel is DialogAware { CanFinish : true };
        }

        private void Executed_Complete(object sender, ExecutedRoutedEventArgs e)
        {
            Completed();
        }
        
        private void Executed_Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel();
        }

        public bool CanGoBack() => _lastStack.Count > 0;
        public bool CanGoForward() => _nextStack.Count > 0;

        public void Completed()
        {
            if (_current is DialogAware dvm)
            {
                dvm.Completed();
                IsOpened = false;
            }

            _current = null;
            if (_lastStack.Count > 0)
            {
                _current = _lastStack.Pop();
                PushDialogNoPush(_current);
            }
        }

        public void Cancel()
        {
            if (ViewModel is DialogAware dvm)
            {
                dvm.Cancel();
                IsOpened = false;
            }
            
            _current = null;
            if (_lastStack.Count > 0)
            {
                _current = _lastStack.Pop();
                PushDialogNoPush(_current);
            }
        }

        public void GoBack()
        {
            if (CanGoBack())
            {
                _nextStack.Push(_current);
                _current = _lastStack.Pop();
                PushDialog(_current);
            }
        }

        public void GoForward()
        {
            if (CanGoBack())
            {
                _lastStack.Push(_current);
                _current = _nextStack.Pop();
                PushDialog(_current);
            }
        }
        
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            private set => SetValue(TitlePropertyKey, value);
        }
        
        public bool IsOpened
        {
            get => (bool)GetValue(IsOpenedProperty);
            private set => SetValue(IsOpenedPropertyKey, Xaml.Box(value));
        }
        
        public object Dialog
        {
            get => GetValue(DialogProperty);
            private set => SetValue(DialogProperty, value);
        }

        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        
        

        public static async Task<IsCompleted<T>> ShowDialog<T>(DialogAware vm)
        {
            if (vm is null)
            {
                return new IsCompleted<T>
                {
                    IsFinished = false
                };
            }
            
            return await MGApp.ShowDialog<T>(vm);
        }
    }
}