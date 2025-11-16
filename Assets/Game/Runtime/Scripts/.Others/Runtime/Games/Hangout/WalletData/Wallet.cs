using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Contains an array of currencies.
    /// </summary>
    public class Wallet
    {
        public static readonly Wallet Default = new(new[]
        {
            new Currency { code = Currency.UBE_COI, amount = 0 },
            new Currency { code = Currency.UBE_DIA, amount = 0 }
        });

        protected Dictionary<string, int> walletBalanceMap = new();

        public Wallet(params string[] initialCurrencyCodes)
        {
            foreach (var c in initialCurrencyCodes)
            {
                this[c] = 0;
            }
        }

        public Wallet(IEnumerable<Currency> currencies)
        {
            AccumulateCurrencies(currencies);
        }

        /// <summary>
        /// Sets all currencies in the wallet. If there are duplicate currency codes, the value of the last one is used.
        /// </summary>
        /// <param name="currencies">Currencies to set in the wallet.</param>
        public virtual void SetCurrencies(IEnumerable<Currency> currencies)
        {
            foreach (var c in currencies)
            {
                if (!walletBalanceMap.ContainsKey(c.code))
                {
                    walletBalanceMap.Add(c.code, c.amount);
                    continue;
                }

                walletBalanceMap[c.code] = c.amount;
            }
        }
        
        public virtual void AccumulateCurrencies(Currency currency)
        {
            if (!walletBalanceMap.ContainsKey(currency.code))
            {
                walletBalanceMap.Add(currency.code, currency.amount);
                return;
            }

            walletBalanceMap[currency.code] += currency.amount;
        }

        public virtual void AccumulateCurrencies(IEnumerable<Currency> currencies)
        {
            foreach (var c in currencies)
            {
                AccumulateCurrencies(c);
            }
        }

        public virtual int this[string code]
        {
            get => walletBalanceMap[code];
            set
            {
                if (!walletBalanceMap.ContainsKey(code))
                {
                    walletBalanceMap.Add(code, 0);
                }
                
                walletBalanceMap[code] = value;
            }
        }

        public virtual void AddCurrencyCode(string code)
        {
            walletBalanceMap.TryAdd(code, 0);
        }

        public Currency GetCurrency(string code)
        {
            var hasCurrency = walletBalanceMap.TryGetValue(code, out var value);

            if (!hasCurrency)
            {
                throw new Exception($"This currency code was not found in a the wallet: {code}");
            }

            return new Currency(value, code);
        }

        public bool ContainsCurrencyCode(string code)
        {
            return walletBalanceMap.ContainsKey(code);
        }

        public IEnumerable<Currency> GetCurrencies()
        {
            return walletBalanceMap.Select(kvp => new Currency(kvp.Value, kvp.Key));
        }

        public override string ToString()
        {
            return string.Join(", ", GetCurrencies().Select(c => $"{c.amount} {c.code}"));
        }

        #region operator overloads

        public static Wallet operator -(Wallet self)
        {
            var result = new Wallet();

            foreach (var c in self.GetCurrencies())
            {
                result[c.code] = -c.amount;
            }

            return result;
        }

        public static Wallet operator +(Wallet left, Wallet right)
        {
            var result = new Wallet();

            foreach (var c in left.GetCurrencies())
            {
                result.AccumulateCurrencies(c);
            }

            foreach (var c in right.GetCurrencies())
            {
                result.AccumulateCurrencies(c);
            }

            return result;
        }
        
        public static Wallet operator -(Wallet left, Wallet right)
        {
            var result = new Wallet();

            foreach (var c in left.GetCurrencies())
            {
                result.AccumulateCurrencies(c);
            }

            foreach (var c in right.GetCurrencies())
            {
                result.AccumulateCurrencies(-c);
            }

            return result;
        }
        
        public static Wallet operator *(Wallet left, int right)
        {
            var result = new Wallet(left.GetCurrencies().Select(c => c.code).ToArray());

            foreach (var c in left.GetCurrencies())
            {
                result[c.code] = c.amount * right;
            }

            return result;
        }
        
        public static Wallet operator *(int left, Wallet right)
        {
            var result = new Wallet(right.GetCurrencies().Select(c => c.code).ToArray());

            foreach (var c in right.GetCurrencies())
            {
                result[c.code] = left * c.amount;
            }

            return result;
        }
        
        public static Wallet operator /(Wallet left, int right)
        {
            var result = new Wallet(left.GetCurrencies().Select(c => c.code).ToArray());

            foreach (var c in left.GetCurrencies())
            {
                result[c.code] = c.amount / right;
            }

            return result;
        }
        
        public static Wallet operator /(int left, Wallet right)
        {
            var result = new Wallet(right.GetCurrencies().Select(c => c.code).ToArray());

            foreach (var c in right.GetCurrencies())
            {
                result[c.code] = left / c.amount;
            }

            return result;
        }

        #endregion
    }
}
