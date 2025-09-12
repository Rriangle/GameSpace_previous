using System.Threading.Tasks;

namespace GameSpace.Services.Validation
{
    public interface IValidationService
    {
        Task<ValidationResult> ValidateEmailAsync(string email);
        Task<ValidationResult> ValidatePasswordAsync(string password);
        Task<ValidationResult> ValidatePhoneNumberAsync(string phoneNumber);
        Task<ValidationResult> ValidateIdNumberAsync(string idNumber);
        Task<ValidationResult> ValidateInputAsync(string input, InputType inputType);
        Task<ValidationResult> SanitizeHtmlAsync(string html);
        Task<ValidationResult> ValidateFileUploadAsync(IFormFile file, FileType fileType);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SanitizedValue { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }

    public enum InputType
    {
        Text,
        Email,
        Phone,
        IdNumber,
        Password,
        Html,
        File
    }

    public enum FileType
    {
        Image,
        Document,
        Video,
        Audio
    }
}