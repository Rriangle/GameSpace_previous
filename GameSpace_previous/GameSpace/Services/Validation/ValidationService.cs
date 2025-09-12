using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameSpace.Services.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;
        private readonly HashSet<string> _allowedFileExtensions = new()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", // 圖片
            ".pdf", ".doc", ".docx", ".txt", ".rtf", // 文檔
            ".mp4", ".avi", ".mov", ".wmv", // 視頻
            ".mp3", ".wav", ".ogg", ".aac" // 音頻
        };

        private readonly Dictionary<FileType, long> _maxFileSizes = new()
        {
            { FileType.Image, 5 * 1024 * 1024 }, // 5MB
            { FileType.Document, 10 * 1024 * 1024 }, // 10MB
            { FileType.Video, 100 * 1024 * 1024 }, // 100MB
            { FileType.Audio, 20 * 1024 * 1024 } // 20MB
        };

        public ValidationService(ILogger<ValidationService> logger)
        {
            _logger = logger;
        }

        public async Task<ValidationResult> ValidateEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "電子郵件不能為空",
                        Errors = { "Email is required" }
                    };
                }

                // 基本長度檢查
                if (email.Length > 254)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "電子郵件長度不能超過 254 個字符",
                        Errors = { "Email too long" }
                    };
                }

                // 正則表達式驗證
                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                if (!emailRegex.IsMatch(email))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "電子郵件格式不正確",
                        Errors = { "Invalid email format" }
                    };
                }

                // 檢查危險字符
                if (ContainsDangerousCharacters(email))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "電子郵件包含不安全的字符",
                        Errors = { "Email contains dangerous characters" }
                    };
                }

                return new ValidationResult
                {
                    IsValid = true,
                    Message = "電子郵件驗證通過",
                    SanitizedValue = email.Trim().ToLowerInvariant()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email validation error for: {Email}", email);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "電子郵件驗證失敗",
                    Errors = { "Validation error occurred" }
                };
            }
        }

        public async Task<ValidationResult> ValidatePasswordAsync(string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "密碼不能為空",
                        Errors = { "Password is required" }
                    };
                }

                var errors = new List<string>();

                // 長度檢查
                if (password.Length < 8)
                {
                    errors.Add("密碼長度至少需要 8 個字符");
                }

                if (password.Length > 128)
                {
                    errors.Add("密碼長度不能超過 128 個字符");
                }

                // 複雜度檢查
                if (!password.Any(char.IsUpper))
                {
                    errors.Add("密碼必須包含至少一個大寫字母");
                }

                if (!password.Any(char.IsLower))
                {
                    errors.Add("密碼必須包含至少一個小寫字母");
                }

                if (!password.Any(char.IsDigit))
                {
                    errors.Add("密碼必須包含至少一個數字");
                }

                if (!password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c)))
                {
                    errors.Add("密碼必須包含至少一個特殊字符");
                }

                // 檢查常見弱密碼
                var commonPasswords = new[] { "password", "123456", "qwerty", "abc123", "admin" };
                if (commonPasswords.Any(common => password.ToLowerInvariant().Contains(common)))
                {
                    errors.Add("密碼不能包含常見的弱密碼模式");
                }

                // 檢查連續字符
                if (HasConsecutiveCharacters(password))
                {
                    errors.Add("密碼不能包含連續的字符");
                }

                return new ValidationResult
                {
                    IsValid = errors.Count == 0,
                    Message = errors.Count == 0 ? "密碼驗證通過" : "密碼不符合要求",
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password validation error");
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "密碼驗證失敗",
                    Errors = { "Validation error occurred" }
                };
            }
        }

        public async Task<ValidationResult> ValidatePhoneNumberAsync(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "手機號碼不能為空",
                        Errors = { "Phone number is required" }
                    };
                }

                // 移除所有非數字字符
                var cleanNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

                // 檢查長度
                if (cleanNumber.Length < 10 || cleanNumber.Length > 15)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "手機號碼長度不正確",
                        Errors = { "Invalid phone number length" }
                    };
                }

                // 檢查是否只包含數字
                if (!cleanNumber.All(char.IsDigit))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "手機號碼只能包含數字",
                        Errors = { "Phone number must contain only digits" }
                    };
                }

                return new ValidationResult
                {
                    IsValid = true,
                    Message = "手機號碼驗證通過",
                    SanitizedValue = cleanNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Phone number validation error for: {PhoneNumber}", phoneNumber);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "手機號碼驗證失敗",
                    Errors = { "Validation error occurred" }
                };
            }
        }

        public async Task<ValidationResult> ValidateIdNumberAsync(string idNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idNumber))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "身分證字號不能為空",
                        Errors = { "ID number is required" }
                    };
                }

                var cleanId = idNumber.Trim().ToUpperInvariant();

                // 台灣身分證字號驗證
                if (cleanId.Length == 10)
                {
                    return ValidateTaiwanIdNumber(cleanId);
                }

                // 其他格式的身分證字號
                if (cleanId.Length >= 8 && cleanId.Length <= 20)
                {
                    return new ValidationResult
                    {
                        IsValid = true,
                        Message = "身分證字號驗證通過",
                        SanitizedValue = cleanId
                    };
                }

                return new ValidationResult
                {
                    IsValid = false,
                    Message = "身分證字號格式不正確",
                    Errors = { "Invalid ID number format" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ID number validation error for: {IdNumber}", idNumber);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "身分證字號驗證失敗",
                    Errors = { "Validation error occurred" }
                };
            }
        }

        public async Task<ValidationResult> ValidateInputAsync(string input, InputType inputType)
        {
            return inputType switch
            {
                InputType.Email => await ValidateEmailAsync(input),
                InputType.Phone => await ValidatePhoneNumberAsync(input),
                InputType.IdNumber => await ValidateIdNumberAsync(input),
                InputType.Password => await ValidatePasswordAsync(input),
                InputType.Html => await SanitizeHtmlAsync(input),
                _ => ValidateTextInput(input)
            };
        }

        public async Task<ValidationResult> SanitizeHtmlAsync(string html)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(html))
                {
                    return new ValidationResult
                    {
                        IsValid = true,
                        Message = "HTML 內容為空",
                        SanitizedValue = string.Empty
                    };
                }

                // 移除危險的 HTML 標籤和屬性
                var dangerousTags = new[] { "script", "iframe", "object", "embed", "form", "input", "button" };
                var dangerousAttributes = new[] { "onclick", "onload", "onerror", "onmouseover", "onfocus", "onblur" };

                var sanitized = html;

                // 移除危險標籤
                foreach (var tag in dangerousTags)
                {
                    var regex = new Regex($@"<{tag}[^>]*>.*?</{tag}>", RegexOptions.IgnoreCase);
                    sanitized = regex.Replace(sanitized, string.Empty);
                }

                // 移除危險屬性
                foreach (var attr in dangerousAttributes)
                {
                    var regex = new Regex($@"\s{attr}\s*=\s*[""'][^""']*[""']", RegexOptions.IgnoreCase);
                    sanitized = regex.Replace(sanitized, string.Empty);
                }

                // 移除 JavaScript 協議
                sanitized = Regex.Replace(sanitized, @"javascript:", "", RegexOptions.IgnoreCase);
                sanitized = Regex.Replace(sanitized, @"vbscript:", "", RegexOptions.IgnoreCase);
                sanitized = Regex.Replace(sanitized, @"data:", "", RegexOptions.IgnoreCase);

                return new ValidationResult
                {
                    IsValid = true,
                    Message = "HTML 內容已清理",
                    SanitizedValue = sanitized
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTML sanitization error");
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "HTML 清理失敗",
                    Errors = { "Sanitization error occurred" }
                };
            }
        }

        public async Task<ValidationResult> ValidateFileUploadAsync(IFormFile file, FileType fileType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "文件不能為空",
                        Errors = { "File is required" }
                    };
                }

                var errors = new List<string>();

                // 檢查文件大小
                if (file.Length > _maxFileSizes[fileType])
                {
                    errors.Add($"文件大小不能超過 {_maxFileSizes[fileType] / (1024 * 1024)} MB");
                }

                // 檢查文件擴展名
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedFileExtensions.Contains(extension))
                {
                    errors.Add($"不支持的文件類型: {extension}");
                }

                // 檢查 MIME 類型
                if (!IsValidMimeType(file.ContentType, fileType))
                {
                    errors.Add($"文件 MIME 類型不正確: {file.ContentType}");
                }

                // 檢查文件名
                if (ContainsDangerousCharacters(file.FileName))
                {
                    errors.Add("文件名包含不安全的字符");
                }

                return new ValidationResult
                {
                    IsValid = errors.Count == 0,
                    Message = errors.Count == 0 ? "文件驗證通過" : "文件驗證失敗",
                    Errors = errors,
                    SanitizedValue = SanitizeFileName(file.FileName)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File validation error for: {FileName}", file?.FileName);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "文件驗證失敗",
                    Errors = { "Validation error occurred" }
                };
            }
        }

        private ValidationResult ValidateTextInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "輸入內容不能為空",
                    Errors = { "Input is required" }
                };
            }

            if (input.Length > 1000)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "輸入內容長度不能超過 1000 個字符",
                    Errors = { "Input too long" }
                };
            }

            if (ContainsDangerousCharacters(input))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "輸入內容包含不安全的字符",
                    Errors = { "Input contains dangerous characters" }
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                Message = "輸入驗證通過",
                SanitizedValue = input.Trim()
            };
        }

        private ValidationResult ValidateTaiwanIdNumber(string idNumber)
        {
            // 台灣身分證字號驗證邏輯
            if (idNumber.Length != 10)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "台灣身分證字號必須為 10 位",
                    Errors = { "Invalid Taiwan ID length" }
                };
            }

            var firstChar = idNumber[0];
            if (!char.IsLetter(firstChar))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "台灣身分證字號第一位必須為字母",
                    Errors = { "First character must be a letter" }
                };
            }

            var remainingChars = idNumber.Substring(1);
            if (!remainingChars.All(char.IsDigit))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "台灣身分證字號後九位必須為數字",
                    Errors = { "Last 9 characters must be digits" }
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                Message = "台灣身分證字號驗證通過",
                SanitizedValue = idNumber
            };
        }

        private bool ContainsDangerousCharacters(string input)
        {
            var dangerousChars = new[] { '<', '>', '"', '\'', '&', '\0', '\r', '\n' };
            return input.Any(c => dangerousChars.Contains(c));
        }

        private bool HasConsecutiveCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (char.IsLetter(password[i]) && char.IsLetter(password[i + 1]) && char.IsLetter(password[i + 2]))
                {
                    if (password[i + 1] == password[i] + 1 && password[i + 2] == password[i] + 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsValidMimeType(string mimeType, FileType fileType)
        {
            var validMimeTypes = fileType switch
            {
                FileType.Image => new[] { "image/jpeg", "image/png", "image/gif", "image/webp" },
                FileType.Document => new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" },
                FileType.Video => new[] { "video/mp4", "video/avi", "video/quicktime", "video/x-ms-wmv" },
                FileType.Audio => new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/aac" },
                _ => new[] { mimeType }
            };

            return validMimeTypes.Contains(mimeType);
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            return sanitized.Trim();
        }
    }
}