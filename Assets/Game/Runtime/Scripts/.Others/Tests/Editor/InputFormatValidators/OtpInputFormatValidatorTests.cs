using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    public class OtpInputFormatValidatorTests
    {
        private OtpInputFormatValidator otpValidator = new OtpInputFormatValidator();

        [Test]
        public void IsValid_ReturnsTrue_ForValidOtp()
        {
            Assert.That(otpValidator.IsValid("012345"), Is.True);
        }
    
        [Test]
        public void IsValid_ReturnsFalse_ForInvalidOtp()
        {
            Assert.That(otpValidator.IsValid(null), Is.False);
            Assert.That(otpValidator.IsValid("A12345"), Is.False);
            Assert.That(otpValidator.IsValid("012"), Is.False);
        }
    }
}
