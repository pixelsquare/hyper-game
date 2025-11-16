using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public struct Currency
    {
        public const string UBE_DIA = "UDI";
        public const string UBE_COI = "UCO";
        
        public string code;
        public int amount;

        public Currency(int amount, string code)
        {
            this.amount = amount;
            this.code = code;
        }

        public Currency ConvertToCurrency(string newCurrencyCode)
        {
            return new Currency { amount = amount, code = newCurrencyCode };
        }

        public override bool Equals(object obj) 
        {
            if (!(obj is Currency currency))
            {
                return false;
            }

            var other = currency;
            return amount == other.amount && code == other.code;
        }

        public override string ToString()
        {
            return $"{code}{amount:n0}";
        }

        #region Operator overrides

        // negation operations
        public static Currency operator -(Currency currency)
        {
            return new Currency(-currency.amount, currency.code);
        }
        
        // int and currency operations
        public static Currency operator +(Currency left, int right)
        {
            return new Currency(left.amount + right, left.code);
        }
        
        public static Currency operator +(int left, Currency right)
        {
            return new Currency(left + right.amount, right.code);
        }
        
        public static Currency operator -(Currency left, int right)
        {
            return new Currency(left.amount - right, left.code);
        }
        
        public static Currency operator -(int left, Currency right)
        {
            return new Currency(left - right.amount, right.code);
        }
        
        public static Currency operator *(Currency left, int right)
        {
            return new Currency(left.amount * right, left.code);
        }
        
        public static Currency operator *(int left, Currency right)
        {
            return new Currency(left * right.amount, right.code);
        }
        
        public static Currency operator /(Currency left, int right)
        {
            return new Currency(left.amount / right, left.code);
        }
        
        public static Currency operator /(int left, Currency right)
        {
            return new Currency(left / right.amount, right.code);
        }

        public static bool operator >(Currency left, int right)
        {
            return left.amount > right;
        }

        public static bool operator >(int left, Currency right)
        {
            return left > right.amount;
        }
        
        public static bool operator <(Currency left, int right)
        {
            return left.amount < right;
        }
        
        public static bool operator <(int left, Currency right)
        {
            return left < right.amount;
        }
        
        public static bool operator >=(Currency left, int right)
        {
            return left.amount >= right;
        }

        public static bool operator >=(int left, Currency right)
        {
            return left >= right.amount;
        }
        
        public static bool operator <=(Currency left, int right)
        {
            return left.amount <= right;
        }
        
        public static bool operator <=(int left, Currency right)
        {
            return left <= right.amount;
        }
        
        // Currency & currency operations
        public static Currency operator +(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return new Currency(left.amount + right.amount, left.code);
        }
        
        public static Currency operator -(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return new Currency(left.amount - right.amount, left.code);
        }
        
        public static Currency operator *(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return new Currency(left.amount * right.amount, left.code);
        }
        
        public static Currency operator /(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return new Currency(left.amount / right.amount, left.code);
        }

        public static bool operator >(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return left.amount > right.amount;
        }
        
        public static bool operator <(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return left.amount < right.amount;
        }
        
        public static bool operator >=(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return left.amount >= right.amount;
        }
        
        public static bool operator <=(Currency left, Currency right)
        {
            if (left.code != right.code)
            {
                throw new Exception($"Incompatible currencies! A:{left.code}, b:{right.code}");
            }

            return left.amount <= right.amount;
        }

        // Equality operations
        public static bool operator ==(Currency left, Currency right)
        {
            return left.code == right.code && left.amount == right.amount;
        }

        public static bool operator !=(Currency left, Currency right)
        {
            return left.code != right.code || left.amount != right.amount;
        }

        #endregion
    }
}
