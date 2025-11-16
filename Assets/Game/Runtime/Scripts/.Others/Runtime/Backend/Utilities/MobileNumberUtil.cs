using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Extensions;
using PhoneNumbers;

namespace Kumu.Kulitan.Backend
{
    public static class MobileNumberUtil
    {
        private static CountryCode currentCountryCode = new()
        {
            code = "63",
            alpha = "PH"
        };

        private static string[] unsupportedCountryCodes =
        {
            "PK",
            "AF",
            "TL",
            "TJ",
            "TM",
            "KG",
            "UZ"
        };
        
        private static readonly CountryCode[] CountryData = GetInitCountryCodes();

        public static CountryCode[] GetCountryCodes()
        {
            return CountryData;
        }

        public static CountryCode GetCurrentCountryCode()
        {
            return currentCountryCode;
        }

        public static void SetCurrentCountryCode(CountryCode newCurrentCountryCode)
        {
            currentCountryCode = newCurrentCountryCode;
        }

        public static void SetPHAsCurrentCountryCode()
        {
            var phCode = Array.Find(CountryData, cc => cc.alpha.Equals("PH"));
            SetCurrentCountryCode(phCode);
        }

        public static int GetNationalNumberLimit(string countryCode)
        {
            try
            {
                return GetExampleNationalNumber(countryCode).Length;
            }
            catch(Exception)
            {
                "Error trying to parse the national number!".LogError();
                return 0;
            }
        }

        public static string GetExampleNationalNumber(string countryCode)
        {
            try
            {
                return PhoneNumberUtil.GetInstance()
                                      .GetExampleNumberForType($"{countryCode}", PhoneNumberType.MOBILE)
                                      .NationalNumber
                                      .ToString();
            }
            catch(Exception)
            {
                "Error trying to get an example national number!".LogError();
                return default;
            }
        }

        public static string Sanitize(string mobile)
        {
            return $"+{GetCurrentCountryCode().code}{mobile}";
        }
        
        private static CountryCode[] GetInitCountryCodes()
        {
            var codes = PhoneNumberUtil.GetInstance().GetSupportedRegions();
            var newCountryData = new List<CountryCode>();
            foreach (var c in codes)
            {
                if (unsupportedCountryCodes.Contains(c))
                {
                    continue;
                }

                var newCountryCode = new CountryCode()
                {
                    code = PhoneNumberUtil.GetInstance().GetCountryCodeForRegion(c).ToString(),
                    alpha = c
                };
                
                newCountryData.Add(newCountryCode);
            }

            // ensure PH is on top of the list
            // sort entries alphabetically
            var phEntry = newCountryData.Find(x => x.alpha.Equals("PH"));
            newCountryData.Remove(phEntry);
            newCountryData.Sort((x, y) => string.Compare(x.alpha, y.alpha));
            newCountryData.Insert(0, phEntry);
            return newCountryData.ToArray();
        }
    }
}
