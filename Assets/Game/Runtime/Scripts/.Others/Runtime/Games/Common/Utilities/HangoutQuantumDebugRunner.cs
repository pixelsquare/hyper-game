using System;
using System.Collections;
using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Multiplayer;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    // Class is derived from `QuantumRunnerLocalDebug`. Cleaned up according to our standards with only a minor change
    // in the Start method
    public class HangoutQuantumDebugRunner : QuantumCallbacks
    {
        private const string MATCH_ID = "LOCALDEBUG";
        
        [SerializeField] private RecordingFlags recordingFlags = RecordingFlags.Default;
        [SerializeField] private InstantReplaySettings instantReplayConfig = InstantReplaySettings.Default;
        [SerializeField] private float simulationSpeedMultiplier = 1.0f;
        [SerializeField] private RuntimeConfig config;
        [SerializeField] public RuntimePlayer[] players;

        private bool isReload;
        
        public void OnGUI()
        {
            if (QuantumRunner.Default != null && QuantumRunner.Default.Id == MATCH_ID)
            {
                if (GUI.Button(new Rect(Screen.width - 150, 10, 140, 25), "Save And Reload"))
                {
                    StartCoroutine(SaveAndReload());
                }
            }
        }
        
        public override void OnGameStart(QuantumGame game)
        {
            if (isReload == false && QuantumRunner.Default.Id == MATCH_ID)
            {
                for (var i = 0; i < players.Length; ++i)
                {
                    game.SendPlayerData(i, players[i]);
                }
            }
        }

        private void StartWithFrame(int frameNumber = 0, byte[] frameData = null)
        {
            isReload = frameNumber > 0 && frameData != null;

            Debug.Log("### Starting quantum in local debug mode ###".WrapColor(Color.green));

            var mapdata = FindObjectOfType<MapData>();
            if (mapdata)
            {
                // set map to this maps asset
                config.Map = mapdata.Asset.Settings;

                var playerCount = Math.Max(1, players.Length);

                // create start game parameter
                var param = new QuantumRunner.StartParameters
                {
                    RuntimeConfig = config,
                    DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                    ReplayProvider = null,
                    GameMode = DeterministicGameMode.Local,
                    InitialFrame = 0,
                    RunnerId = MATCH_ID,
                    PlayerCount = playerCount,
                    LocalPlayerCount = playerCount,
                    RecordingFlags = recordingFlags,
                    InstantReplayConfig = instantReplayConfig
                };

                param.InitialFrame = frameNumber;
                param.FrameData = frameData;

                // start with debug config
                QuantumRunner.StartGame(MATCH_ID, param);
            }
            else
            {
                throw new Exception("No MapData object found, can't debug start scene");
            }
        }

        private IEnumerator SaveAndReload()
        {
            var frameNumber = QuantumRunner.Default.Game.Frames.Verified.Number;
            var frameData = QuantumRunner.Default.Game.Frames.Verified.Serialize(DeterministicFrameSerializeMode.Blit);

            Log.Info($"Serialized Frame size: {frameData.Length} bytes");

            QuantumRunner.ShutdownAll();

            while (QuantumRunner.ActiveRunners.Any())
            {
                yield return null;
            }

            StartWithFrame(frameNumber, frameData);
        }
        
        public void Start()
        {
            // The check `QuantumRunner.Default != null` alone doesn't work for our scene loading pattern
            // as we load the scene first before we start the simulation which guarantees the condition to always fail.
            // A check if we are in a room should be enough to tell that we started the game from Bootstrap and navigated
            // to the game in the expected flow
            if (QuantumRunner.Default != null || ConnectionManager.Client.CurrentRoom != null)
            {
                enabled = false;
                return;
            }
            
#if !RELEASE
            StartWithFrame();
#endif
        }

        public void Update()
        {
            if (QuantumRunner.Default != null && QuantumRunner.Default.Session != null)
            {
                // Set the session ticking to manual to inject custom delta time.
                QuantumRunner.Default.OverrideUpdateSession = simulationSpeedMultiplier != 1.0f;
                if (QuantumRunner.Default.OverrideUpdateSession)
                {
                    var deltaTime = QuantumRunner.Default.DeltaTime ?? Time.unscaledDeltaTime;
                    QuantumRunner.Default.Session.Update(deltaTime * simulationSpeedMultiplier);
                    UnityDB.Update();
                }
            }
        }
    }
}
