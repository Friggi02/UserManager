namespace Project.Dal.Entities
{
    public class Result
    {
        public bool Success { get; }
        public string? Message { get; }
        public object? Object { get; }

        public Result(bool success)
        {
            Success = success;
            Message = null;
            Object = null;
        }

        public Result(bool success, string? message)
        {
            Success = success;
            Message = message;
            Object = null;
        }

        public Result(bool success, string? message, object? obj)
        {
            Success = success;
            Message = message;
            Object = obj;
        }
    }
}