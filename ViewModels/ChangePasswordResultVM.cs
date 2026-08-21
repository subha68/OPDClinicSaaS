using Models;

namespace ViewModels
{
    public class ChangePasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}