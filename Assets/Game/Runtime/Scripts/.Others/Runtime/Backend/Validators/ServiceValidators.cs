using System;
using System.Linq;
using System.Text.RegularExpressions;
using Kumu.Extensions;
using PhoneNumbers;

namespace Kumu.Kulitan.Backend
{
    public class UserNameInputFormatValidator : IInputFormatValidator<UserNameFormatDetails>
    {
        public const int MIN_LENGTH = 8;
        
        public const int MAX_LENGTH = 32;

        private readonly Regex regex = new Regex("^[a-z0-9]*$");
        
        /// <summary>
        /// Valid if:<br/>
        /// 1. 2-32 chars in length (inclusive).<br/>
        /// 2. Lowercase only.<br/>
        /// 3. No spaces.<br/>
        /// 4. No special characters.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="details">Struct with details of format validity.</param>
        /// <returns>True if all conditions are true. False otherwise.</returns>
        public bool IsValid(string value, out UserNameFormatDetails details)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                details = new UserNameFormatDetails();
                return false;
            }
            
            details = new UserNameFormatDetails
            {
                IsWithinCharLimits = value.Length >= MIN_LENGTH && value.Length <= MAX_LENGTH,
                IsLowercase = value.All(char.IsLower),
                HasNoSpaces = !value.Any(char.IsWhiteSpace),
                HasNoSpecialChars = regex.IsMatch(value)
            };
            
            return details.IsWithinCharLimits && details.IsLowercase && details.HasNoSpaces && details.HasNoSpecialChars;
        }
    }

    public struct UserNameFormatDetails
    {
        public bool IsWithinCharLimits { get; set; }
        public bool IsLowercase { get; set; }
        public bool HasNoSpaces { get; set; }
        public bool HasNoSpecialChars { get; set; }
    }

    public class UserNickNameInputFormatValidator : IInputFormatValidator<UserNickNameFormatDetails>
    {
        public const int MIN_LENGTH = 2;
        
        public const int MAX_LENGTH = 32;
        
        private readonly Regex regex = new Regex("^[a-zA-Z0-9 ]*$");
        
        /// <summary>
        /// Valid if:<br/>
        /// 1. 1-32 characters (inclusive).<br/>
        /// 2. No special characters.
        /// 3. No leading or trailing whitespaces.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="details">Struct with details of format validity.</param>
        /// <returns>True if all conditions are true. False otherwise.</returns>
        public bool IsValid(string value, out UserNickNameFormatDetails details)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                details = new UserNickNameFormatDetails();
                return false;
            }
            
            details = new UserNickNameFormatDetails
            {
                IsWithinCharLimits = value.Length >= MIN_LENGTH && value.Length <= MAX_LENGTH,
                HasNoSpecialChars = regex.IsMatch(value),
                HasNoTrailingOrLeadingWhitespaces = value.Length == 0 || (!char.IsWhiteSpace(value.First()) && !char.IsWhiteSpace(value.Last())),
            };

            return details.IsWithinCharLimits && details.HasNoSpecialChars && details.HasNoTrailingOrLeadingWhitespaces;
        }
    }

    public struct UserNickNameFormatDetails
    {
        public bool IsWithinCharLimits { get; set; }
        
        public bool HasNoSpecialChars { get; set; }
        
        public bool HasNoTrailingOrLeadingWhitespaces { get; set; }
    }

    public class OtpInputFormatValidator : IInputFormatValidator
    {
        public const int REQUIRED_LENGTH = 6;
        
        private readonly Regex regex = new Regex("^[0-9]*$");
        
        /// <summary>
        /// Valid if:<br/>
        /// 1. String only contains digits (0-9).<br/>
        /// 2. String is exactly 6 characters long.<br/>
        /// </summary>
        /// <param name="value">Password to test.</param>
        /// <returns>True if password is in the valid format. False otherwise.</returns>
        public bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            
            return value.Length == REQUIRED_LENGTH && regex.IsMatch(value);
        }
    }

    public class MobileInputFormatValidator : IInputFormatValidator
    {
        private readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

        /// <summary>
        /// Valid if:<br/>
        /// 1. Is in international number format (starts with a +).<br/>
        /// 2. Number is a mobile number.
        /// </summary>
        /// <param name="value">Phone number in string format.</param>
        /// <returns>True if valid mobile number. False otherwise.</returns>
        public bool IsValid(string value)
        {
            try
            {
                var phoneNumber = phoneNumberUtil.Parse(value, null);
                var isNumberTypeValid = phoneNumberUtil.GetNumberType(phoneNumber) == PhoneNumberType.MOBILE ||
                                        phoneNumberUtil.GetNumberType(phoneNumber) == PhoneNumberType.FIXED_LINE_OR_MOBILE;
                return phoneNumberUtil.IsValidNumber(phoneNumber) && 
                       isNumberTypeValid && phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.E164) == value;
            }
            catch (NumberParseException)
            {
                return false;
            }
            catch (Exception)
            {
                "Some other error occured trying to parse the phone number".LogError();
                return false;
            }
        }
    }
    
    /// <summary>
    /// Valid if: <br/>
    /// 1. Only alphanumerics, underscores, and periods.
    /// 2. All lowercase. 
    /// </summary>
    public class UsernameLinkValidator : IInputFormatValidator
    {
        private const int MIN_CHAR = 2;
        
        private readonly Regex regex = new Regex(@"^[a-z0-9_\.]*$");

        private readonly string[] reserved =
        {
            "kumu",
            "karlito",
            "karlita",
        };
        
        public bool IsValid(string value)
        {
            var strlen = value.Length;
            
            if (strlen > 0 && strlen < MIN_CHAR)
            {
                return false;
            }

            if (reserved.Contains(value))
            {
                return false;
            }
            
            return regex.IsMatch(value);
        }
    }
    
    public class EmailValidator : IInputFormatValidator
    {
        private readonly Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        
        public bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && regex.IsMatch(value);
        }
    }
}
