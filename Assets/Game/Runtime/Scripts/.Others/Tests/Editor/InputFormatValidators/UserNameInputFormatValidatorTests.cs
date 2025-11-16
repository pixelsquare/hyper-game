using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    public class UserNameInputFormatValidatorTests
    {
        private UserNameInputFormatValidator validator = new UserNameInputFormatValidator();

        [Test]
        public void IsValid_ReturnTrue_ForValidUserName()
        {
            Assert.That(validator.IsValid("myawesomename", out var d), Is.True);
            Assert.That(d.IsLowercase, Is.True);
            Assert.That(d.IsWithinCharLimits, Is.True);
            Assert.That(d.HasNoSpaces, Is.True);
            Assert.That(d.HasNoSpecialChars, Is.True);
        }
        
        [Test]
        public void IsValid_ReturnFalse_ForInvalidUserName()
        {
            // Null
            Assert.That(validator.IsValid(null, out var d0), Is.False);
            Assert.That(d0.IsWithinCharLimits, Is.False);
            Assert.That(d0.IsLowercase, Is.False);
            Assert.That(d0.HasNoSpaces, Is.False);
            Assert.That(d0.HasNoSpecialChars, Is.False);

            // Too short
            Assert.That(validator.IsValid("m", out var d1), Is.False);
            Assert.That(d1.IsWithinCharLimits, Is.False);
            
            // Special chars
            Assert.That(validator.IsValid("my_@wesome_name", out var d2), Is.False);
            Assert.That(d2.HasNoSpecialChars, Is.False);
            
            // Upper case
            Assert.That(validator.IsValid("MyAwesomeName", out var d3), Is.False);
            Assert.That(d3.IsLowercase, Is.False);
            
            // Spaces
            Assert.That(validator.IsValid("my awesome name", out var d4), Is.False);
            Assert.That(d4.HasNoSpaces, Is.False);
            
            // Too long
            Assert.That(validator.IsValid("myawesomenamemyawesomenamemyawesomenamemyawesomenamemyawesomenamemyawesomename", out var d5), Is.False); // too long
            Assert.That(d5.IsWithinCharLimits, Is.False);
        }
    }
}
