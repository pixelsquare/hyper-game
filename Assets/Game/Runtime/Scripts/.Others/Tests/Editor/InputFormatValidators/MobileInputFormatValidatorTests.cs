using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    public class MobileInputFormatValidatorTests
    {
        private MobileInputFormatValidator mobileValidator = new MobileInputFormatValidator();

        [Test]
        public void IsValid_ReturnsTrue_ForValidNumbers()
        {
            Assert.That(mobileValidator.IsValid("+639472810001"), Is.True);
        }
        
        [Test]
        public void IsValid_ReturnsFalse_ForInvalidNumbers()
        {
            Assert.That(mobileValidator.IsValid(null), Is.False);
            Assert.That(mobileValidator.IsValid("+637890000001"), Is.False);
        }
    }
}
