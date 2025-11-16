using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Common;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using System.Reflection;
#endif

namespace Kumu.Kulitan.Hangout
{
    [CreateAssetMenu(menuName = "Config/Hangout/AnimationConfig")]
    public class AnimationConfig : ScriptableObject, IInspectorGUI
    {
        [SerializeField] private List<AnimationDataAsset> characterAnimations;
        [SerializeField] [KeyValueDictionary] private SerializableDictionary<string, Sprite> interactableIconTags = new() { { "default", null } };

#if UNITY_EDITOR
        [Header("Settings")]
        [SerializeField] private AnimatorController animatorController;

        [Tooltip("Entry transition duration")]
        [SerializeField] private float inDuration = 0.1f;

        [Tooltip("Exit transition duration")]
        [SerializeField] private float outDuration;

        [SerializeField] private string[] animParamFilter = { "speed", "blinkSpeed", "onReset" };
        [SerializeField] private string animDataPath = "Assets/_Source/Content/QuantumAssets/Hangout/Animations";

        private readonly Dictionary<string, Dictionary<string, StateMachineBehaviour[]>> stateMachineBehaviorMap = new()
        {
            { EMOTE_KEY, new Dictionary<string, StateMachineBehaviour[]>() },
            { INTERACTABLE_KEY, new Dictionary<string, StateMachineBehaviour[]>() }
        };

        private const string EMOTE_KEY = "Emotes";
        private const string INTERACTABLE_KEY = "Interactables";
#endif

        public List<AnimationDataAsset> CharacterAnimations => characterAnimations;
        public SerializableDictionary<string, Sprite> InteractableIconTags => interactableIconTags;

#if UNITY_EDITOR
        public void OnInspectorInit()
        {
            SetAnimationsDirty();
        }

        public void OnInspectorDraw()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            DrawButton("Create Emote Data", Color.white,
                    () => CreateAnimationData<EmotesAnimationAsset>($"{animDataPath}/Emote", "New Emote Data"));
            DrawButton("Create Interactable Data", Color.white,
                    () => CreateAnimationData<InteractableAnimationAsset>($"{animDataPath}/Interactable",
                            "New Interactable Data"));
            EditorGUILayout.EndHorizontal();

            DrawButton("Apply Changes", Color.green, RefreshAnimatorFields);

            EditorGUILayout.EndVertical();
        }

        private void DrawButton(string text, Color color, Action callback = null)
        {
            var oldColor = GUI.color;
            GUI.color = color;

            if (GUILayout.Button(text, GUILayout.Height(30f)))
            {
                callback?.Invoke();
            }

            GUI.color = oldColor;
        }

        private void RefreshAnimatorFields()
        {
            if (EditorUtility.DisplayDialog("Reset Parameters", "Do you want to remove unused emote parameters?", "Ok",
                        "Cancel"))
            {
                ResetEmoteParameters();
            }

            if (EditorUtility.DisplayDialog("Reset State Machine", "Do you want to remove unused state machine?", "Ok",
                        "Cancel"))
            {
                ResetAnimationStateMachine();
            }

            var emotesLen = 0;
            var interactablesLen = 0;

            SetAnimationsDirty();
            var animLen = characterAnimations.Count;

            for (var i = 0; i < animLen; i++)
            {
                var animData = characterAnimations[i];
                var animState = default(AnimatorState);

                animData.SetAnimLengthDirty();

                if (animData is EmotesAnimationAsset)
                {
                    TryAddEmoteState(animData.DisplayName, emotesLen++, out animState);
                }
                else if (animData is InteractableAnimationAsset)
                {
                    TryAddInteractableState(animData.DisplayName, interactablesLen++, out animState);
                }
                else
                {
                    continue;
                }

                animState.motion = animData.AnimationClipEditor;

                if (!TryAddParameter(animData.AnimParamKey, animData.AnimParamType, out _))
                {
                    Debug.LogWarning("Duplicate parameter key found. Animation might not play properly." +
                            $"[{animData.name} -> {animData.AnimParamKey}]\n{AssetDatabase.GetAssetPath(animData)}");
                }

                TryConnectFromMovementState(animState, animData.AnimParamKey, out _);
                TryConnectToMovementState(animState, animData.AnimParamKey, out _);

                EditorUtility.DisplayProgressBar("Animator Update",
                        $"Generating animation nodes ... ({i + 1} / {animLen})", (float)(i + 1) / animLen);
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(animatorController);
            AssetDatabase.SaveAssetIfDirty(animatorController);
            AssetDatabase.Refresh();
        }

        private bool TryAddParameter(string key, AnimatorControllerParameterType type,
                                     out AnimatorControllerParameter animatorParam)
        {
            var parameters = animatorController.parameters;
            var index = ArrayUtility.FindIndex(parameters, p => p.name.Equals(key));

            if (index < 0)
            {
                animatorParam = new AnimatorControllerParameter
                {
                    name = animatorController.MakeUniqueParameterName(key),
                    type = type
                };

                animatorController.AddParameter(animatorParam);
                return true;
            }

            animatorParam = parameters[index];
            return false;
        }

        private bool TryAddEmoteState(string key, int idx, out AnimatorState animatorState)
        {
            var stateMachine = animatorController.layers[0].stateMachine;
            var emoteStateMachine = GetOrCreateStateMachine(stateMachine, EMOTE_KEY, Vector3.up * 100f);
            var states = emoteStateMachine.states;
            var index = ArrayUtility.FindIndex(states, s => s.state.name.Equals(key));

            if (index < 0)
            {
                animatorState = emoteStateMachine.AddState(key, new Vector3(300f, idx * 50f, 0f));

                if (stateMachineBehaviorMap[EMOTE_KEY].TryGetValue(key, out var behaviors))
                {
                    animatorState.behaviours = behaviors;

                    foreach (var behavior in behaviors)
                    {
                        var stateMachineBehavior = animatorState.AddStateMachineBehaviour(behavior.GetType());
                        ComponentDeepCopy(stateMachineBehavior, behavior);
                    }
                }

                return true;
            }

            animatorState = states[index].state;
            states[index].position = new Vector3(300f, idx * 50f, 0f);
            return false;
        }

        private bool TryAddInteractableState(string key, int idx, out AnimatorState animatorState)
        {
            var stateMachine = animatorController.layers[0].stateMachine;
            var emoteStateMachine = GetOrCreateStateMachine(stateMachine, INTERACTABLE_KEY, Vector3.down * 100f);
            var states = emoteStateMachine.states;
            var index = ArrayUtility.FindIndex(states, s => s.state.name.Equals(key));

            if (index < 0)
            {
                animatorState = emoteStateMachine.AddState(key, new Vector3(300f, idx * 50f, 0f));

                if (stateMachineBehaviorMap[INTERACTABLE_KEY].TryGetValue(key, out var behaviors))
                {
                    foreach (var behavior in behaviors)
                    {
                        var stateMachineBehavior = animatorState.AddStateMachineBehaviour(behavior.GetType());
                        ComponentDeepCopy(stateMachineBehavior, behavior);
                    }
                }

                return true;
            }

            animatorState = states[index].state;
            states[index].position = new Vector3(300f, idx * 50f, 0f);
            return false;
        }

        private bool TryConnectToMovementState(AnimatorState state, string parameterKey,
                                               out AnimatorStateTransition animTransition)
        {
            var stateMachine = animatorController.layers[0].stateMachine;
            var movementState = GetOrCreateStateMachine(stateMachine, "Movement", Vector3.zero).defaultState;

            var index = ArrayUtility.FindIndex(state.transitions,
                    s => s.destinationState != null
                         && s.destinationState.nameHash.Equals(movementState.nameHash)
                         && s.destinationState.GetHashCode() == movementState.GetHashCode());

            if (index < 0)
            {
                animTransition = state.AddTransition(movementState);
                AddTransitionCondition(animTransition, AnimatorConditionMode.IfNot, parameterKey, outDuration);
                return true;
            }

            animTransition = state.transitions[index];
            AddTransitionCondition(animTransition, AnimatorConditionMode.IfNot, parameterKey, outDuration);
            return false;
        }

        private bool TryConnectFromMovementState(AnimatorState state, string parameterKey,
                                                 out AnimatorStateTransition animTransition)
        {
            var stateMachine = animatorController.layers[0].stateMachine;
            var movementState = GetOrCreateStateMachine(stateMachine, "Movement", Vector3.zero).defaultState;

            var index = ArrayUtility.FindIndex(movementState.transitions,
                    s => s.destinationState != null
                         && s.destinationState.nameHash.Equals(state.nameHash)
                         && s.destinationState.GetHashCode() == state.GetHashCode());

            if (index < 0)
            {
                animTransition = movementState.AddTransition(state);
                AddTransitionCondition(animTransition, AnimatorConditionMode.If, parameterKey, inDuration);
                return true;
            }

            animTransition = movementState.transitions[index];
            AddTransitionCondition(animTransition, AnimatorConditionMode.If, parameterKey, inDuration);
            return false;
        }

        private void ResetEmoteParameters()
        {
            var paramIdx = 0;
            var paramLen = animatorController.parameters.Length;
            while (paramLen > animParamFilter.Length)
            {
                var parameter = animatorController.parameters[paramIdx];

                if (!Array.Exists(animParamFilter, a => a.Equals(parameter.name)))
                {
                    animatorController.RemoveParameter(paramIdx);
                    paramLen = animatorController.parameters.Length;
                    continue;
                }

                paramIdx++;
            }
        }

        private void ResetAnimationStateMachine()
        {
            ClearAnimationStateMachine(EMOTE_KEY, Vector3.up * 100f);
            ClearAnimationStateMachine(INTERACTABLE_KEY, Vector3.down * 100f);
        }

        private void ClearAnimationStateMachine(string key, Vector3 position)
        {
            var stateMachine = animatorController.layers[0].stateMachine;

            var animStateMachine = GetOrCreateStateMachine(stateMachine, key, position);
            stateMachineBehaviorMap[key].Clear();

            while (animStateMachine.states.Length > 0)
            {
                var animState = animStateMachine.states[0].state;

                if (animState.behaviours != null && animState.behaviours.Length > 0)
                {
                    stateMachineBehaviorMap[EMOTE_KEY].Add(animState.name, animState.behaviours);
                }

                animStateMachine.RemoveState(animState);
            }
        }

        private void AddTransitionCondition(AnimatorStateTransition animTransition, AnimatorConditionMode animMode,
                                            string paramKey, float duration = 0.0f)
        {
            animTransition.conditions = Array.Empty<AnimatorCondition>();
            animTransition.AddCondition(animMode, 0f, paramKey);
            animTransition.duration = duration;
            animTransition.hasExitTime = false;
            animTransition.exitTime = 1f;
            animTransition.hasFixedDuration = true;
        }

        private AnimatorStateMachine GetOrCreateStateMachine(AnimatorStateMachine animStateMachine,
                                                             string stateMachineName, Vector3 position)
        {
            var stateMachine = Array.Find(animStateMachine.stateMachines,
                    a => a.stateMachine.name.Equals(stateMachineName)).stateMachine;

            if (stateMachine == null)
            {
                stateMachine = animStateMachine.AddStateMachine(stateMachineName, position);
            }

            return stateMachine;
        }

        /// <summary>
        ///     Reference: https://stackoverflow.com/a/62553770
        /// </summary>
        public static T ComponentDeepCopy<T>(T comp, T other) where T : ScriptableObject
        {
            var type = comp.GetType();
            var othersType = other.GetType();

            if (type != othersType)
            {
                Debug.LogError(
                        $"The type \"{type.AssemblyQualifiedName}\" of \"{comp}\" does not match the type \"{othersType.AssemblyQualifiedName}\" of \"{other}\"!");
                return null;
            }

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                    BindingFlags.Default;
            var pinfos = type.GetProperties(flags);

            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {
                        /*
                         * In case of NotImplementedException being thrown.
                         * For some reason specifying that exception didn't seem to catch it,
                         * so I didn't catch anything specific.
                         */
                    }
                }
            }

            var finfos = type.GetFields(flags);

            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }

            return comp;
        }

        private T CreateAnimationData<T>(string path, string fileName) where T : AnimationDataAsset
        {
            var animData = ScriptableObject.CreateInstance<T>();
            var dupFiles = Directory.GetFiles(path, $"{fileName}*.asset");
            fileName = !dupFiles.Any() ? fileName : $"{fileName} {dupFiles.Count()}";

            var filePath = Path.Combine(path, fileName);
            filePath = Path.ChangeExtension(filePath, ".asset");

            ProjectWindowUtil.CreateAsset(animData, filePath);
            AssetDatabase.SaveAssetIfDirty(animData);
            AssetDatabase.Refresh();

            SetAnimationsDirty();
            characterAnimations.Add(animData);
            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();

            Selection.activeObject = animData;
            EditorGUIUtility.PingObject(animData);

            return animData;
        }

        private void SetAnimationsDirty()
        {
            characterAnimations.RemoveAll(a => a == null);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
        }
#endif
    }
}
