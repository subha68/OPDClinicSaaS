using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Models;

namespace ViewModels
{
    public class FieldTypeChangeVM
    {
        public enum ApiResponseType
        {
            BadRequest, NotFound, ConfirmationRequired, Success
        }
        public bool Success { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool SupportsOptions { get; set; }
        public bool RequiresConfirmation { get; set; }
        public string Message { get; set; } = string.Empty;
        public int OptionCount { get; set; }
        public string ResponseType { get; set; } = string.Empty;

        public FieldTypeChangeVM()
        {
            
        }

        public FieldTypeChangeVM(bool success, string responseType, string message)
        {
            Success = success;
            ResponseType = responseType;
            Message = message;
            RequiresConfirmation = false;
            OptionCount = 0;
        }

        public FieldTypeChangeVM(bool success, string responseType, string message, bool requiresConfirmation)
        {
            Success = success;
            ResponseType = responseType;
            Message = message;
            RequiresConfirmation = requiresConfirmation;
            //OptionCount = optionCount;
        }

        public FieldTypeChangeVM(bool success, string responseType, string fieldCode, string fieldLabel, bool supportsOptions)
        {
            Success = success;
            ResponseType = responseType;
            Value = fieldCode;
            Text = fieldLabel;
            SupportsOptions = supportsOptions;
        }

    }

}