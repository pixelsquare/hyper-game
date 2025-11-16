using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Similar to the base <see cref="Wallet"/> class but hooked up to send a GlobalNotifier message when values are
    /// modified.
    /// </summary>
    public class UserWallet : Wallet
    {
        public new static readonly UserWallet Default = new(new[]
        {
            new Currency { code = Currency.UBE_COI, amount = 0 },
            new Currency { code = Currency.UBE_DIA, amount = 0 }
        });

        public UserWallet(params string[] initialCurrencyCodes) : base(initialCurrencyCodes)
        {
            GlobalNotifier.Instance.Trigger(new UserWalletUpdatedEvent(GetCurrenciesAsArray()));
        }

        public UserWallet(IEnumerable<Currency> currencies) : base(currencies)
        {
            GlobalNotifier.Instance.Trigger(new UserWalletUpdatedEvent(GetCurrenciesAsArray()));
        }

        public override void SetCurrencies(IEnumerable<Currency> currencies)
        {
            base.SetCurrencies(currencies);
            GlobalNotifier.Instance.Trigger(new UserWalletUpdatedEvent(GetCurrenciesAsArray()));
        }
        
        public override void AccumulateCurrencies(Currency currency)
        {
            base.AccumulateCurrencies(currency);
            GlobalNotifier.Instance.Trigger(new UserWalletUpdatedEvent(GetCurrenciesAsArray()));
        }
        
        public override void AccumulateCurrencies(IEnumerable<Currency> currencies)
        {
            base.AccumulateCurrencies(currencies);
            GlobalNotifier.Instance.Trigger(new UserWalletUpdatedEvent(GetCurrenciesAsArray()));;
        }

        public override int this[string code]
        {
            get => base[code];
            set
            {
                base[code] = value;
                GlobalNotifier.Instance.Trigger(new UserWalletUpdatedEvent(GetCurrenciesAsArray()));
            }
        }

        public override void AddCurrencyCode(string code)
        {
            base.AddCurrencyCode(code);
            GlobalNotifier.Instance.Trigger(new UserWalletUpdatedEvent(GetCurrenciesAsArray()));
        }
       
        public Currency[] GetCurrenciesAsArray()
        {
            return GetCurrencies().ToArray();
        }
    }
}
