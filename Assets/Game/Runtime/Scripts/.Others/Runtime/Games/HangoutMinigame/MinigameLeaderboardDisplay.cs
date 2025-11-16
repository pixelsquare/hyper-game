using Kumu.Kulitan.Common;
using System.Collections.Generic;
using System.Linq;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public class MinigameLeaderboardDisplay : ListView<MinigameLeaderboardUiEntry, MinigameLeaderboardEntry>
    {
        [SerializeField] private MinigameLeaderboardUiEntry[] rankPrefabs;
        [SerializeField] private GameObject screen;
        [SerializeField] private int noOfEntriesDisplayed = 10;

        public override void Display(IEnumerable<MinigameLeaderboardEntry> data)
        {
            var counter = 0;
            foreach (var datum in data)
            {
                var element = Instantiate(rankPrefabs[counter], container);
                if (counter < rankPrefabs.Length - 1)
                {
                    counter++;
                }
                element.SetRankLabel(ParseRanking(datum.rank));
                OnCreate(element, datum);
                uiElements.Add(element);
            }
        }

        private string ParseRanking(int rank)
        {
            switch (rank % 10)
            {
                case 1:
                    return rank + "st";
                case 2:
                    return rank + "nd";
                case 3:
                    return rank + "rd";
                default:
                    return rank + "th";
            }
        }

        private void OnShowLeaderboard(EventOnMinigameStateStart eventData)
        {
            if (eventData.minigameState == MinigameState.Ready)
            {
                Clear();
                screen.SetActive(false);
            }

            if (eventData.minigameState != MinigameState.Result)
            {
                return;
            }

            screen.SetActive(true);            
            var entries = GetEntries();
            Clear();
            Display(entries);
        }

        private IEnumerable<MinigameLeaderboardEntry> GetEntries()
        {
            var f = QuantumRunner.Default.Game.Frames.Predicted;
            var entries = new List<MinigameLeaderboardEntry>();

            foreach (var (entity, minigameContestant) in f.GetComponentIterator<MinigameContestant>())
            {
                var playerRef = f.Get<HangoutPlayer>(entity).player;
                var nickname = f.GetPlayerData(playerRef).nickname;
                var score = minigameContestant.score;

                var entry = new MinigameLeaderboardEntry()
                {
                    name = nickname,
                    score = score,
                };
                
                entries.Add(entry);
            }

            return GetRankedEntries(entries);
        }

        private IEnumerable<MinigameLeaderboardEntry> GetRankedEntries(List<MinigameLeaderboardEntry> entries)
        {
            var counter = 1;
            var rankedEntries = from entry in entries.OrderByDescending(entry => entry.score)
                    .Take(noOfEntriesDisplayed)
                select new MinigameLeaderboardEntry
                {
                    name = entry.name,
                    score = entry.score,
                    rank = counter++,
                };

            return rankedEntries;
        }

        [ContextMenu("Mock Populate")]
        private void MockPopulate()
        {
            var testScores = new[] { 300, 150, 200, 20, 10, 599, 800, -10, 100, 400, 1020 };
            var testEntries = new List<MinigameLeaderboardEntry>();

            for(var i = 0; i < testScores.Length; i++)
            {
                var entry = new MinigameLeaderboardEntry()
                {
                    name = "nickname"+i,
                    score = testScores[i],
                };
                
                testEntries.Add(entry);
            }

            screen.SetActive(true);            
            var entries = GetRankedEntries(testEntries);;
            Clear();
            Display(entries);
        }
        
        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnMinigameStateStart>(this, OnShowLeaderboard);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }        
    }
}
