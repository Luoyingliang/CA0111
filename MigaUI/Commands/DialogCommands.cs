namespace Acorisoft.Miga.UI.Commands
{
    public static class DialogCommands
    {
        public static RoutedUICommand Completed { get; } =
            Xaml.CreateCommand(nameof(Completed), typeof(DialogCommands));
        
        
        public static RoutedUICommand Cancel { get; } =
            Xaml.CreateCommand(nameof(Cancel), typeof(DialogCommands));
        
        public static RoutedUICommand Show { get; } =
            Xaml.CreateCommand(nameof(Show), typeof(DialogCommands));
    }
}