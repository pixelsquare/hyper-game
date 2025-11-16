using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    public class WalletTests
    {
        private Currency[] coins1 = { new(7, Currency.UBE_COI), new(11, Currency.UBE_COI) };
        private Currency[] coins2 = { new(17, Currency.UBE_COI), new(19, Currency.UBE_COI) };
        private Currency[] dia1 = { new(7, Currency.UBE_DIA), new(11, Currency.UBE_DIA) };
        private Currency[] dia2 = { new(17, Currency.UBE_DIA), new(19, Currency.UBE_DIA) };
        
        [Test]
        public void Constructor_InitializesCorrectly()
        {
            var currencyCodes = new[] { "x", "y", "z" };
            
            var wallet = new Wallet(currencyCodes);
            
            Assert.That(wallet["x"], Is.EqualTo(0));
            Assert.That(wallet["y"], Is.EqualTo(0));
            Assert.That(wallet["z"], Is.EqualTo(0));

            var wallet2 = new Wallet(new List<Currency>
            {
                new(7, "x"), 
                new(13, "y")
            });
            
            Assert.That(wallet2["x"], Is.EqualTo(7));
            Assert.That(wallet2["y"], Is.EqualTo(13));
        }

        [Test]
        public void Addition_WithSameCurrencies_ReturnsCorrectResult()
        {
            var wallet1 = new Wallet(coins1);
            var wallet2 = new Wallet(coins2);

            var result = wallet1 + wallet2;
            
            Assert.That(result.GetCurrencies().Count(), Is.EqualTo(1));
            Assert.That(result[Currency.UBE_COI], Is.EqualTo(7 + 11 + 17 + 19));
        }
        
        [Test]
        public void Addition_WithDifferentCurrencies_ReturnsCorrectResult()
        {
            var wallet1 = new Wallet(coins1);
            var wallet2 = new Wallet(coins2);
            var wallet3 = new Wallet(dia1);
            var wallet4 = new Wallet(dia2);

            var result = wallet1 + wallet2 + wallet3 + wallet4;
            
            Assert.That(result.GetCurrencies().Count(), Is.EqualTo(2));
            Assert.That(result.ContainsCurrencyCode(Currency.UBE_COI), Is.True);
            Assert.That(result.ContainsCurrencyCode(Currency.UBE_DIA), Is.True);
            Assert.That(result[Currency.UBE_COI], Is.EqualTo(7 + 11 + 17 + 19));
            Assert.That(result[Currency.UBE_DIA], Is.EqualTo(7 + 11 + 17 + 19));
        }
        
        [Test]
        public void Subtraction_WithSameCurrencies_ReturnsCorrectResult()
        {
            var wallet1 = new Wallet(coins1);
            var wallet2 = new Wallet(coins2);

            var result = wallet1 - wallet2;
            
            Assert.That(result.GetCurrencies().Count(), Is.EqualTo(1));
            Assert.That(result[Currency.UBE_COI], Is.EqualTo(7 + 11 - 17 - 19));
        }
        
        [Test]
        public void Subtraction_WithDifferentCurrencies_ReturnsCorrectResult()
        {
            var wallet1 = new Wallet(coins1);
            var wallet2 = new Wallet(dia2);

            var result = wallet1 - wallet2;
            
            Assert.That(result.GetCurrencies().Count(), Is.EqualTo(2));
            Assert.That(result.ContainsCurrencyCode(Currency.UBE_COI), Is.True);
            Assert.That(result.ContainsCurrencyCode(Currency.UBE_DIA), Is.True);
            Assert.That(result[Currency.UBE_COI], Is.EqualTo(7 + 11));
            Assert.That(result[Currency.UBE_DIA], Is.EqualTo(-17 - 19));
        }
        
        [Test]
        public void Multiplication_WithInt_ReturnsCorrectResult()
        {
            var wallet1 = new Wallet(coins1);
            var result1 = wallet1 * 3;
            Assert.That(result1[Currency.UBE_COI], Is.EqualTo((7 + 11) * 3));

            var wallet2 = new Wallet(coins2);
            var result2 = 3 * wallet2;
            Assert.That(result2[Currency.UBE_COI], Is.EqualTo(3 * (17 + 19)));
        }
        
        [Test]
        public void Division_WithInt_ReturnsCorrectResult()
        {
            var wallet1 = new Wallet(coins1);
            var result1 = wallet1 / 3;
            Assert.That(result1[Currency.UBE_COI], Is.EqualTo((7 + 11) / 3));

            var wallet2 = new Wallet(coins2);
            var result2 = 72 / wallet2;
            Assert.That(result2[Currency.UBE_COI], Is.EqualTo(72 / (17 + 19)));
        }
    }
}
