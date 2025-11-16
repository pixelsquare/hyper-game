using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kumu.Kulitan.Backend
{
    public static class MockedServicesUtil
    {
        public const string MOCKED_USER_PROFILE_KEY = "MockedUserProfile";

        public const string MOCKED_BLOCKED_USERS_KEY = "MockedBlockedUsers";

        public const string OWNED_ITEMS_KEY = "temp-avatar-owned";

        public const string EQUIPPED_ITEMS_KEY = "temp-avatar-equipped";

        public const string DEFAULT_MOBILE_NUMBER = "+639001234567";

        public const string DEFAULT_NAME = "noname";

        public const string DEFAULT_NICK = "NoNick";

        public static UserProfile SetMockedInitialProfileInPrefs(string mobile)
        {
            var profile = new UserProfile
            {
                accountId = Guid.NewGuid().ToString(),
                mobile = mobile
            };

            PlayerPrefs.SetString(MOCKED_USER_PROFILE_KEY, JsonUtility.ToJson(profile));

            return profile;
        }

        public static UserProfile GetMockedInitialProfileInPrefs()
        {
            var json = PlayerPrefs.GetString(MOCKED_USER_PROFILE_KEY, null);

            if (string.IsNullOrWhiteSpace(json))
            {
                return GetDefaultInitialProfile();
            }

            return JsonUtility.FromJson<UserProfile>(json);
        }

        public static UserProfile SetMockedProfileInPrefs(string name, string nickName, int ageRange, int gender)
        {
            var initialProfile = GetMockedInitialProfileInPrefs();
            var profile = new UserProfile
            {
                userName = CreateUserNameWithRandomDiscriminator(name).ToString(),
                accountId = initialProfile.accountId,
                ageRange = ageRange,
                gender = gender,
                mobile = initialProfile.mobile,
                nickName = nickName
            };

            PlayerPrefs.SetString(MOCKED_USER_PROFILE_KEY, JsonUtility.ToJson(profile));

            return profile;
        }

        public static UserProfile GetMockedProfileInPrefs()
        {
            var json = PlayerPrefs.GetString(MOCKED_USER_PROFILE_KEY, null);

            if (string.IsNullOrWhiteSpace(json))
            {
                return GetDefaultProfile();
            }

            return JsonUtility.FromJson<UserProfile>(json);
        }

        public static void ClearMockedProfileInPrefs()
        {
            PlayerPrefs.DeleteKey(MOCKED_USER_PROFILE_KEY);
        }

        private static string GetRandomDiscriminator()
        {
            return Random.Range(0, 10000).ToString("D4");
        }

        public static string GetRandomAccountId()
        {
            return Guid.NewGuid().ToString();
        }

        public static UserName CreateUserNameWithRandomDiscriminator(string name)
        {
            return new UserName
            {
                discriminator = GetRandomDiscriminator(),
                name = name.ToLower().Replace(" ", ""),
            };
        }

        private static UserProfile GetDefaultInitialProfile()
        {
            return new UserProfile
            {
                accountId = GetRandomAccountId(),
                mobile = DEFAULT_MOBILE_NUMBER
            };
        }

        public static UserProfile GetDefaultProfile()
        {
            return new UserProfile
            {
                userName = CreateUserNameWithRandomDiscriminator(DEFAULT_NAME).ToString(),
                accountId = GetRandomAccountId(),
                ageRange = 0,
                gender = 0,
                mobile = DEFAULT_MOBILE_NUMBER,
                nickName = DEFAULT_NICK
            };
        }

        public static void GetMockedNames(out string[] mockedNames)
        {
            var textAsset = Resources.Load<TextAsset>("MockedDB/mocked_names");
            mockedNames = JsonConvert.DeserializeObject<string[]>(textAsset.text);
        }

        public static void AddUserToBlocklist(string userId)
        {
            var blockedList = new HashSet<string>(GetBlockedlist());
            blockedList.Add(userId);
            WriteBlockedlist(blockedList.ToArray());
        }

        public static void RemoveUserToBlocklist(string userId)
        {
            var blockedList = new HashSet<string>(GetBlockedlist());
            blockedList.Remove(userId);
            WriteBlockedlist(blockedList.ToArray());
        }

        public static bool IsUserBlocked(string userId)
        {
            var blockedList = GetBlockedlist();
            return Array.FindIndex(blockedList, a => string.Compare(a, userId, StringComparison.CurrentCultureIgnoreCase) == 0) != -1;
        }

        public static string[] GetBlockedlist()
        {
            var blockedList = PlayerPrefs.GetString(MOCKED_BLOCKED_USERS_KEY, "[]");
            return JsonConvert.DeserializeObject<string[]>(blockedList);
        }

        private static void WriteBlockedlist(string[] blockedList)
        {
            var blockedListString = JsonConvert.SerializeObject(blockedList);
            PlayerPrefs.SetString(MOCKED_BLOCKED_USERS_KEY, blockedListString);
        }
    }
}
