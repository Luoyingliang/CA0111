namespace Acorisoft.Miga.UI.Mvvm
{
    public interface IDataContextBridge
    {
        object DataContext { get; set; }
    }

    public interface IViewModelBefore
    {
        ViewModelBase Before { get; }
    }
}