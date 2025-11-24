using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    using SaveKey = GameConstants.SaveKeys;

    public static class GameUtil
    {
        public static void ExitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            UnityEngine.Application.Quit();
#endif
        }

        public static string NewGuid()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
        }

        public static double GetUnixTime()
        {
            return Math.Truncate((DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds);
        }

        public static DateTime FromUnixTime(double unixTimestamp)
        {
            return DateTime.UnixEpoch.AddSeconds(unixTimestamp);
        }

        public static bool ShouldSyncUp(ISaveManager saveManager)
        {
            // TODO: Uncomment this line when cloud server is up.
            // This will write player data to cloud (client to server) once on first install.
            // Ideally server should set the default items for the player.
            // Client should never be able to write to the sever.
            return true; // IsFirstInstall(saveManager);
        }

        public static bool IsFirstInstall(ISaveManager saveManager)
        {
            var installDate = saveManager.Load(SaveKey.InstallDateUtc);
            return string.IsNullOrEmpty(installDate);
        }

        public static void RecordFirstInstall(ISaveManager saveManager)
        {
            saveManager.Save(SaveKey.InstallDateUtc, GetUnixTime().ToString());
        }

        public static bool DidDuplicate(Event curEvent)
        {
            return curEvent != null && curEvent.commandName.Equals("Duplicate", StringComparison.OrdinalIgnoreCase);
        }

        public static float GetProgressFill(int progress, int max)
        {
            progress = Mathf.Clamp(progress, 0, max);
            return progress < max ? progress / (float)max - 0.05f : 1f;
        }

        public static string FormatString(string text, char character = '.', int length = 30)
        {
            var sb = new StringBuilder();
            sb.Append(text).Append(character, length - text.Length);
            return sb.ToString();
        }

        public static List<ItemStats> GetItemStats(IItem item)
        {
            var itemStats = new List<ItemStats>();
            GetItemStatsNonAlloc(item, itemStats);
            return itemStats;
        }

        public static void GetItemStatsNonAlloc(IItem item, List<ItemStats> itemStats)
        {
            if (itemStats == null)
            {
                throw new NullReferenceException($"{nameof(itemStats)}");
            }

            switch (item)
            {
                case EmblemChange emblemChange:
                    itemStats.Add(new ItemStats(null, FormatString("Health"), emblemChange.Health, 0));
                    itemStats.Add(new ItemStats(null, FormatString("Armor"), emblemChange.Armor, 0));
                    break;
                case EmblemDeparture emblemDeparture:
                    itemStats.Add(new ItemStats(null, FormatString("Move Speed", length: 37), emblemDeparture.MoveSpeed, 0));
                    itemStats.Add(new ItemStats(null, FormatString("Evasion Chance", length: 34), emblemDeparture.EvasionChance, 0));
                    itemStats.Add(new ItemStats(null, FormatString("Cooldown Modifier"), emblemDeparture.CooldownModifier, 0));
                    break;
                case EmblemPursuit emblemPursuit:
                    itemStats.Add(new ItemStats(null, FormatString("Critical Chance", length: 31), emblemPursuit.CriticalChance, 0));
                    itemStats.Add(new ItemStats(null, FormatString("Critical Damage"), emblemPursuit.CriticalDamage, 0));
                    itemStats.Add(new ItemStats(null, FormatString("Loot Distance", length: 32), emblemPursuit.LootDistance, 0));
                    break;
            }
        }
    }
}
