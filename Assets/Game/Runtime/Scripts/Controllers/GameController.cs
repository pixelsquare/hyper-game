using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class GameController : Controller<GameView>
    {
        [SerializeField] private TextAsset gameDataCsv;

        private GameConfig gameConfig;
        private UniTaskCompletionSource userInputTcs;
        private CancellationTokenSource roundCts;

        private void OnEnable()
        {
            view.OnChoiceSelected += HandleChoiceSelected;
        }

        private void OnDisable()
        {
            view.OnChoiceSelected -= HandleChoiceSelected;
        }

        private void Start()
        {
            gameConfig = Resources.Load<GameConfig>(nameof(GameConfig));
            StartGame();
        }

        public void StartGame()
        {
            var roundData = Utils.ParseGameData(gameDataCsv.text);
            StartRound(roundData).Forget();
        }

        private async UniTaskVoid StartRound(RoundData[] rounds)
        {
            foreach (var round in rounds)
            {
                using (roundCts = new CancellationTokenSource())
                {
                    view.Hints.Value = round.Hints;
                    view.Choices.Value = round.Choices;
                    view.SetChoicesEnabled(true);
                
                    ShowHints().Forget();
                
                    var totalRoundTime = round.Hints.Length * gameConfig.RoundIntervalSec + 1f;
                    _ = await WaitForTimeOrInput(totalRoundTime);

                    roundCts?.Cancel();
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                    view.ShowCorrectAnswer(round.AnswerIdx);
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                    view.SetRoundCleared();
                }
            }
        }

        private async UniTaskVoid ShowHints()
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(gameConfig.RoundIntervalSec), cancellationToken: roundCts.Token);
                
                if (!view.TryShowNextHint())
                {
                    break;
                }
            }
            
            Debug.Log("Round End");
        }

        private async UniTask StartTimer(float totalTime)
        {
            view.TimerProgress.Value = totalTime;
            view.TimerMinMax.Value = (0, totalTime);

            await DOTween.To(() => view.TimerProgress.Value, x => view.TimerProgress.Value = x, 0f, totalTime)
                .SetEase(Ease.Linear)
                .Play()
                .WithCancellation(roundCts.Token);
        }

        private UniTask<int> WaitForTimeOrInput(float totalTime)
        {
            var timerTask = StartTimer(totalTime);
            userInputTcs = new UniTaskCompletionSource();
            return UniTask.WhenAny(timerTask, userInputTcs.Task);
        }

        private void HandleChoiceSelected(int choiceIdx, string choiceString)
        {
            Debug.Log($"Selected choice: {choiceString} | {choiceIdx}");
            view.SetChoicesEnabled(false);
            view.SetChoiceColor(choiceIdx, Color.paleGoldenRod);
            userInputTcs.TrySetResult();
        }
    }
}