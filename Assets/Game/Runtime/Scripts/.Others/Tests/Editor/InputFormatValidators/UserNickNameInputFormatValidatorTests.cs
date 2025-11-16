using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    public class UserNickNameInputFormatValidatorTests
    {
        private UserNickNameInputFormatValidator validator = new UserNickNameInputFormatValidator();
        
        [Test]
        public void IsValid_ReturnsTrue_ForValidNickName()
        {
            Assert.That(validator.IsValid("Donald McRonald", out var d), Is.True);
            Assert.That(d.IsWithinCharLimits, Is.True);
            Assert.That(d.HasNoSpecialChars, Is.True);
            Assert.That(d.HasNoTrailingOrLeadingWhitespaces, Is.True);
        }

        [Test]
        public void IsValid_ReturnsFalse_ForInvalidNickName()
        {
            // Null
            Assert.That(validator.IsValid(null, out var d0), Is.False);
            Assert.That(d0.HasNoTrailingOrLeadingWhitespaces, Is.False);
            Assert.That(d0.HasNoSpecialChars, Is.False);
            Assert.That(d0.IsWithinCharLimits, Is.False);
            
            // Leading spaces
            Assert.That(validator.IsValid("  Donald McRonald", out var d1), Is.False);
            Assert.That(d1.HasNoTrailingOrLeadingWhitespaces, Is.False);
            
            // Trailing spaces
            Assert.That(validator.IsValid("  Donald McRonald", out var d2), Is.False);
            Assert.That(d2.HasNoTrailingOrLeadingWhitespaces, Is.False);
            
            // Special characters
            Assert.That(validator.IsValid("Donald_McRonald", out var d3), Is.False);
            Assert.That(d3.HasNoSpecialChars, Is.False);
            
            // Too short
            Assert.That(validator.IsValid("", out var d4), Is.False);
            Assert.That(d4.IsWithinCharLimits, Is.False);
            
            // Too long
            Assert.That(validator.IsValid("Donald McRonaldDonald McRonaldDonald McRonaldDonald McRonaldDonald McRonald", out var d5), Is.False);
            Assert.That(d5.IsWithinCharLimits, Is.False);
            
            // Null
            Assert.That(validator.IsValid(null, out var d6), Is.False);
        }
    }
}
