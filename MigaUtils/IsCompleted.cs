namespace Acorisoft.Miga.Utils
{
    public class IsCompleted
    {
        public string Message { get; init; }
        public bool IsFinished { get; init; }
    }
    
    public class IsCompleted<T>
    {
        public T Result { get; init; }
        public string Message { get; init; }
        public bool IsFinished { get; init; }
    }
    
    public static class IsCompletedExtension
    {
        public static IsCompleted<T> AsCompleted<T>(T value)
        {
            return new IsCompleted<T> { IsFinished = true, Result = value };
        }
    }
    
}