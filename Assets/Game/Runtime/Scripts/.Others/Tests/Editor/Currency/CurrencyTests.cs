using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    [Category("Currency")]
    public class CurrencyTests
    {
        private const int _5 = 5;
        private const int _7 = 7;
        private const int _13 = 13;
        private const int _17 = 17;
        
        private Currency coins5 = new Currency(_5, Currency.UBE_COI);
        private Currency coins7 = new Currency(_7, Currency.UBE_COI);
        private Currency dia17 = new Currency(_17, Currency.UBE_DIA);

        [Test]
        public void Addition()
        {
            Currency result;
            
            result = coins5 + coins7;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == 12);

            result = _13 + dia17;
            Assert.That(() => result.code == Currency.UBE_DIA && result.amount == 30);

            result = coins5 + _7;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == 12);

            Assert.That(() => coins5 + dia17, Throws.Exception);
        }
        
        [Test]
        public void Subtraction()
        {
            Currency result;
            
            result = coins5 - coins7;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == -2);

            result = _13 - dia17;
            Assert.That(() => result.code == Currency.UBE_DIA && result.amount == -4);

            result = coins5 - _7;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == -2);

            Assert.That(() => coins5 - dia17, Throws.Exception);
        }
        
        [Test]
        public void Multiplication()
        {
            Currency result;
            
            result = coins5 * coins7;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == 35);

            result = _13 * dia17;
            Assert.That(() => result.code == Currency.UBE_DIA && result.amount == _13 * _17);

            result = coins5 * _7;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == 35);

            Assert.That(() => coins5 * dia17, Throws.Exception);
        }
        
        [Test]
        public void Division()
        {
            Currency result;

            var coins20 = new Currency(20, Currency.UBE_COI);
            var coins60 = new Currency(60, Currency.UBE_COI);

            var dia50 = new Currency(50, Currency.UBE_DIA);

            result = coins60 / coins20;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == 3);

            result = 100 / dia50;
            Assert.That(() => result.code == Currency.UBE_DIA && result.amount == 2);

            result = coins60 / 7;
            Assert.That(() => result.code == Currency.UBE_COI && result.amount == 8);

            Assert.That(() => coins5 / dia17, Throws.Exception);
        }

        [Test]
        public void Negation()
        {
            Currency result;

            result = -coins5;
            
            Assert.That(result.amount, Is.EqualTo(-5));
            Assert.That(result.code, Is.EqualTo(Currency.UBE_COI));
        }

        [Test]
        public void Equality()
        {
            var a = new Currency(20, Currency.UBE_DIA);
            var b = new Currency(20, Currency.UBE_DIA);
            var c = new Currency(30, Currency.UBE_COI);
            var d = new Currency(30, Currency.UBE_DIA);
            
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(c == d, Is.False);
            Assert.That(c != d, Is.True);
        }

        [Test]
        public void Comparison()
        {
            Assert.That(coins7 > coins5, Is.True);
            Assert.That(coins7 < coins5, Is.False);
            Assert.That(20 > dia17, Is.True);
            Assert.That(20 < dia17, Is.False);
            Assert.That(dia17 > 10, Is.True);
            Assert.That(dia17 < 10, Is.False);
            
            Assert.That(coins7 < coins5, Is.False);
            Assert.That(coins7 > coins5, Is.True);
            Assert.That(20 < dia17, Is.False);
            Assert.That(20 > dia17, Is.True);
            Assert.That(dia17 < 10, Is.False);
            Assert.That(dia17 > 10, Is.True);
            
            Assert.That(coins7 >= coins7, Is.True);
            Assert.That(coins7 >= _7, Is.True);
            Assert.That(coins7 <= coins7, Is.True);
            Assert.That(coins7 <= _7, Is.True);
            
            Assert.That(coins7 >= coins5, Is.True);
            Assert.That(coins7 <= coins5, Is.False);
            Assert.That(20 >= dia17, Is.True);
            Assert.That(20 <= dia17, Is.False);
            Assert.That(dia17 >= 10, Is.True);
            Assert.That(dia17 <= 10, Is.False);
            
            Assert.That(coins7 <= coins5, Is.False);
            Assert.That(coins7 >= coins5, Is.True);
            Assert.That(20 <= dia17, Is.False);
            Assert.That(20 >= dia17, Is.True);
            Assert.That(dia17 <= 10, Is.False);
            Assert.That(dia17 >= 10, Is.True);
        }
    }
}
