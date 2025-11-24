using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Presets;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using static Santelmo.Rinsurv.Editor.SpriteGenerator;

namespace Santelmo.Rinsurv.Editor
{
    public class SpriteGenerator : OdinEditorWindow
    {
        #region Private Properties
        private SpriteGeneratorSourcesScriptableObject spriteGeneratorSourcesScriptableObject;
        private CameraAnglesScriptableObject cameraAnglesScriptableObject;
        private GameObject cameraPivot;
        private RecorderController recorderController;
        private Camera mainCamera = new Camera();
        private GameObject sourceObjectParent;
        private GameObject currentActiveObject;
        private AnimationClip currentAnimationClip;

        private List<GameObject> sourceObjects = new List<GameObject>();
        private List<AnimationClip> animationClips = new List<AnimationClip>();
        private List<Object> selectedSpriteSheets = new List<Object>();
        private List<Texture> multiSlicedSprites = new List<Texture>();

        private string SpriteGeneratorSourcesScriptableObjectPath = "Assets/_Source/Content/ScriptableObjects/ArtTools/SpriteGeneratorSources.asset";

        private int cameraAngleIndex = 0;
        private int currentActiveObjectIndex = 0;
        private int currentAnimationClipIndex = 0;

        private bool startRecording;
        private bool isRecording;
        private bool isReady;
        private bool isPlaying;
        private bool isStep;
        private bool isPaused;

        private float cameraRotation = 0;
        private float animationTime;
        public enum AssetGenerationType { spriteSheet, animationClip }
        public enum Resolution { _256, _512, _1024, _2048, _4096 }
        public enum FrameRate { _15, _30, _60 }
        public enum CameraShot { single, multiple }
        public enum PreviewCameraAngle { _0, _45, _90, _135, _180 }
        public enum SourceType { assign, selection, folder }
        #endregion

        #region Active Object
        [HorizontalGroup("SpriteGenerator"), BoxGroup("SpriteGenerator/Options", centerLabel: true)]
        [HorizontalGroup("SpriteGenerator/Options/Active Object/Name", MarginLeft = 100, MarginRight = 100), HideLabel,
            BoxGroup("SpriteGenerator/Options/Active Object", centerLabel: true), PropertyOrder(-1), 
            DisplayAsString(Alignment = TextAlignment.Center), GUIColor("Yellow")]
        public string currentActiveObjectName = "";

        [PropertySpace(SpaceAfter = 5)]
        [Button(SdfIconType.CaretLeftFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
            HorizontalGroup("SpriteGenerator/Options/Active Object/ObjectSwitcher", MarginLeft = 150), PropertyOrder(-1), HideLabel]
        public void PreviousObject()
        {
            animationClips.Clear();
            currentAnimationClipIndex = 0;
            if (currentActiveObjectIndex == 0)
                currentActiveObjectIndex = sourceObjects.Count() - 1;
            else
                currentActiveObjectIndex--;

            UpdateAnimationClips();
        }

        [PropertySpace(SpaceAfter = 5)]
        [Button(SdfIconType.CaretRightFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
            HorizontalGroup("SpriteGenerator/Options/Active Object/ObjectSwitcher", MarginRight = 150), PropertyOrder(-1), HideLabel]
        public void NextObject()
        {
            animationClips.Clear();
            currentAnimationClipIndex = 0;
            if (currentActiveObjectIndex == sourceObjects.Count() - 1)
                currentActiveObjectIndex = 0;
            else
                currentActiveObjectIndex++;

            UpdateAnimationClips();
        }
        #endregion

        #region AnimationButtons
        [Button(SdfIconType.CaretLeftFill, ButtonHeight = 15, Stretch = false), BoxGroup("SpriteGenerator/Options/Active Object/Animation", centerLabel: true), HideLabel,
            HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/Clip", MarginLeft = 150)]
        [PropertySpace(SpaceAfter = 5)]
        public void PreviousClip()
        {
            if (currentAnimationClipIndex == 0)
                currentAnimationClipIndex = animationClips.Count() - 2;
            else
                currentAnimationClipIndex--;
        }

        [HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/Name", Width = -50), HideLabel, DisplayAsString(Alignment = TextAlignment.Center), GUIColor("Yellow")]
        public string currentActiveClipName;

        [PropertySpace(SpaceAfter = 5)]
        [Button(SdfIconType.CaretRightFill, ButtonHeight = 15, Stretch = false), HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/Clip", MarginRight = 150), 
            HideLabel]
        public void NextClip()
        {
            if (currentAnimationClipIndex == animationClips.Count() - 2)
                currentAnimationClipIndex = 0;
            else
                currentAnimationClipIndex++;
        }
        [Button(SdfIconType.SkipStartFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
            HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/State", MarginLeft = 125), HideLabel]
        public void PreviousFrame()
        {
            isStep = true;
            isPaused = false;
            AnimationMode.StartAnimationMode();

            if (animationTime < 0)
                animationTime = currentAnimationClip.length;

            animationTime -= Time.deltaTime;
        }

        [Button(SdfIconType.PlayFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
            HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/State", Order = 1), HideLabel, ShowIf("@ this.isPlaying == false")]
        public void PlayAnimation()
        {
            isPlaying = true;
            isPaused = false;
            isStep = false;
            AnimationMode.StartAnimationMode();
        }
        
        [Button(SdfIconType.StopFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
           HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/State", Order = 1), HideLabel, ShowIf("@ this.isPlaying == true")]
        public void StopAnimation()
        {
            isPlaying = false;
            isPaused = false;
            isStep = false;
            AnimationMode.StopAnimationMode();
        }

        [Button(SdfIconType.PauseFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
            HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/State", Order = 1), HideLabel,
            ShowIf("@ this.isPaused == false &&  this.isPlaying == true && this.isStep == false")]
        public void PauseAnimation()
        {
            isPaused = true;
            isStep = false;
            AnimationMode.StartAnimationMode();
        }

        [Button(SdfIconType.PlayFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
           HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/State", Order = 1), HideLabel, ShowIf("@ this.isPaused == true && this.isStep == false")]
        public void ReplayAnimation()
        {
            isPaused = false;
            isStep = false;
            AnimationMode.StartAnimationMode();
        }

        [Button(SdfIconType.SkipEndFill, IconAlignment.LeftOfText, ButtonHeight = 15, Stretch = false),
            HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/State", MarginRight = 125, Order = 1), HideLabel]
        public void NextFrame()
        {
            isStep = true;
            isPaused = false;
            AnimationMode.StartAnimationMode();

            if (animationTime > currentAnimationClip.length)
                animationTime = 0;

            animationTime += Time.deltaTime;
        }

        [HorizontalGroup("SpriteGenerator/Options/Active Object/Animation/AnimationSpeed", MarginLeft = 110, MarginRight = 110, Order = 2), LabelWidth(110),
            PropertySpace(SpaceBefore = 5)]
        public float animationSpeed = 0.5f;
        #endregion

        #region SpriteGenerationParameters
        [BoxGroup("SpriteGenerator/Options/Asset Generation Type", centerLabel: true), HideLabel, EnumToggleButtons]
        public AssetGenerationType assetGenerationType;

        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/Sources"), LabelText("Output Folder"), LabelWidth(80), Required]
        public DefaultAsset outputFolder;
        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/SpriteName", Width = 150), LabelWidth(120),
        ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet")]
        public bool customSpriteName;
        
        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/SpriteName"), HideLabel,
        ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet && this.customSpriteName == true"), Required]
        public string spriteName = "";

        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/Resolution"), LabelText("Resolution"), LabelWidth(80), EnumToggleButtons,
                        ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet")]
        public Resolution resolutionType = Resolution._4096;
        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/FrameRate"), LabelText("Frame Rate"), LabelWidth(80), EnumToggleButtons]
        public FrameRate frameRate = FrameRate._30;

        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/FrameTime", Width = 100), LabelWidth(80),
        ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet")]
        public bool singleFrame;
        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/FrameTime", Width = 125), LabelWidth(80),
                ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet")]
        public int startFrame = 0;
        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/FrameTime", Width = 125), LabelWidth(80),
        ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet && this.singleFrame == false")]
        public int endFrame = 10;

        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/CameraShot"), LabelText("Camera Shot"), LabelWidth(80), EnumToggleButtons,
                        ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet")]
        public CameraShot cameraShot;
        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/SourceType"), LabelText("Source Type"), LabelWidth(80), EnumToggleButtons,
                ShowIf("@ this.assetGenerationType == AssetGenerationType.animationClip")]
        public SourceType sourceType;

        [BoxGroup("SpriteGenerator/Options/Asset Generation Type/Sprite Sheet", centerLabel: true),
                ShowIf("@ this.assetGenerationType == AssetGenerationType.animationClip && this.sourceType == SourceType.assign"),
                PreviewField(80, Alignment = ObjectFieldAlignment.Center), HideLabel, Required]
        public Texture spriteSheetAsset;
        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/Folder"), LabelText("Sprite Sheet Folder"), LabelWidth(130),
                 ShowIf("@ this.assetGenerationType == AssetGenerationType.animationClip && this.sourceType == SourceType.folder"), Required]
        public DefaultAsset spriteSheetFolder;

        [HorizontalGroup("SpriteGenerator/Options/Asset Generation Type/CameraAngle"), LabelText("Camera Angle"), LabelWidth(80), EnumToggleButtons,
        ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet && this.cameraShot == CameraShot.multiple")]
        public List<CameraAngle> cameraAngles = new List<CameraAngle>();

        [BoxGroup("SpriteGenerator/Preview", centerLabel: true), PreviewField(380, Alignment = ObjectFieldAlignment.Center), HideLabel, ReadOnly]
        public RenderTexture renderTexture;
        [BoxGroup("SpriteGenerator/Preview/Settings", centerLabel: true), LabelText("Angle"), LabelWidth(45), EnumToggleButtons]
        public PreviewCameraAngle cameraAngle;
        [HorizontalGroup("SpriteGenerator/Preview/Settings/Position&Size", Width = 80, Gap =10), LabelText("Size"), LabelWidth(45)]
        public float cameraSize = 30;
        [HorizontalGroup("SpriteGenerator/Preview/Settings/Position&Size", Width = 250), LabelText("Position"), LabelWidth(60)]
        public Vector2 cameraPosition = new Vector2();
        #endregion

        #region Main Buttons
        [PropertySpace(SpaceBefore = 10)]
        [Button(SdfIconType.Images, IconAlignment.LeftOfText, ButtonHeight = 40, Name = "Generate Sprites Sheet"), GUIColor("green"),
            EnableIf("@ this.isReady == true"),ShowIf("@ this.assetGenerationType == AssetGenerationType.spriteSheet"), HorizontalGroup("SpriteGenerator/Options/Button")]
        public void InitializeSpriteGeneration()
        {
            if (cameraShot == CameraShot.multiple)
                cameraRotation = int.Parse(cameraAnglesScriptableObject.cameraAngles[cameraAngleIndex].angle.ToString().Remove(0,1));

            startRecording = true;
            EditorApplication.EnterPlaymode();
        }

        [PropertySpace(SpaceBefore = 10)]
        [Button(SdfIconType.CollectionPlayFill, IconAlignment.LeftOfText, ButtonHeight = 40, Name = "Generate Animation Clips"), GUIColor("green"),
          EnableIf("@ this.isReady == true"), ShowIf("@ this.assetGenerationType == AssetGenerationType.animationClip"), HorizontalGroup("SpriteGenerator/Options/Button")]
        public void InitializeClipGeneration()
        {
            switch (sourceType)
            {
                case SourceType.assign:
                    GenerateAnimationClip(spriteSheetAsset);
                    break;
                case SourceType.selection:
                    foreach (Texture sprite in selectedSpriteSheets)
                    {
                        GenerateAnimationClip(sprite);
                    }
                    break;
                case SourceType.folder:
                    foreach (Texture sprite in multiSlicedSprites)
                    {
                        GenerateAnimationClip(sprite);
                    }
                    multiSlicedSprites.Clear();
                    break;
            }
        }
        #endregion

        #region Initialize Window
        private static Vector2 windowSize = new Vector2(800, 520);
        [MenuItem("Santelmo/Art Tools/Sprite Generator")]
        private static void ShowWindow()
        {  
            EditorWindow spriteGeneratorWindow = GetWindow<SpriteGenerator>("Sprite Generator");

            spriteGeneratorWindow.position = new Rect(0, 0, windowSize.x, windowSize.y);
            spriteGeneratorWindow.minSize = new Vector2(windowSize.x, windowSize.y);
            spriteGeneratorWindow.maxSize = new Vector2(windowSize.x, windowSize.y);
        }

        private void Awake()
        {
            recorderController = new RecorderController(ScriptableObject.CreateInstance<RecorderControllerSettings>());
            spriteGeneratorSourcesScriptableObject = AssetDatabase.LoadAssetAtPath<SpriteGeneratorSourcesScriptableObject>(SpriteGeneratorSourcesScriptableObjectPath);

            renderTexture = spriteGeneratorSourcesScriptableObject.GetRenderTexture();
            cameraAnglesScriptableObject = spriteGeneratorSourcesScriptableObject.GetCameraAnglesScriptableObject();

            sourceObjectParent = GameObject.Find("Animated Objects");

            if (!GameObject.Find("Camera Pivot"))
            {
                GameObject instanced = Instantiate(spriteGeneratorSourcesScriptableObject.GetCameraPrefab());
                instanced.name = "Camera Pivot";
                cameraPivot = instanced;
            }
            else
                cameraPivot = GameObject.Find("Camera Pivot");

            UpdateCamera();
        }
        #endregion

        #region Unity Functions
        private void Update()
        {
            this.Repaint();
            IsReadyCheck();
            UpdateCamera();
            UpdateRenderTexture(renderTexture);
            UpdateActiveObject();
            UpdateRecorder();
        }

        private void OnSelectionChange()
        {
            selectedSpriteSheets.Clear();
            this.Repaint();
        }
        #endregion

        #region Editor Functions
        List<AnimatorState> ExpandStatesInLayer(AnimatorStateMachine sm, List<AnimatorState> collector = null)
        {
            if (collector == null)
                collector = new List<AnimatorState>();

            foreach (var state in sm.states)
            {
                collector.Add(state.state);

                foreach (var subSm in sm.stateMachines)
                    ExpandStatesInLayer(subSm.stateMachine, collector);
            }
            return collector;
        }
        private int StringToInt(string st)
        {
            return int.Parse(st.Remove(0, 1));
        }
        private void UpdateActiveObject()
        {

            if (sourceObjectParent != null)
            {
                animationSpeed = Mathf.Clamp(animationSpeed, 0, 1);

                foreach (Transform sourceObject in sourceObjectParent.transform)
                {
                    int index = sourceObject.GetSiblingIndex();

                    if (!sourceObjects.Contains(sourceObject.gameObject))
                        sourceObjects.Add(sourceObject.gameObject);

                    if (currentActiveObjectIndex == index)
                    {
                        sourceObject.gameObject.SetActive(true);
                        currentActiveObject = sourceObject.gameObject;
                    }
                    else
                        sourceObject.gameObject.SetActive(false);

                    currentActiveObjectName = currentActiveObject.name;
                }

                UpdateAnimationClips();
            }
        }
        private void UpdateAnimationClips()
        {
            Animator animator = currentActiveObject.GetComponent<Animator>();
            AnimatorController anmatorController = (AnimatorController)animator.runtimeAnimatorController;
            animationClips = animator.runtimeAnimatorController.animationClips.ToList();

            foreach (AnimationClip animationClip in animationClips)
            {
                if (currentAnimationClipIndex == animationClips.IndexOf(animationClip))
                    currentAnimationClip = animationClip;
            }

            AnimatorControllerLayer[] layers = anmatorController.layers;
            AnimatorControllerLayer workingLayer = layers[0];

            List<AnimatorState> stateList = ExpandStatesInLayer(workingLayer.stateMachine);

            foreach (var state in stateList)
            {
                if (state.name == "Active Clip")
                {
                    if (state.motion != currentAnimationClip)
                        state.motion = currentAnimationClip;
                }
            }

            if (currentActiveObject != null)
            {
                currentActiveClipName = currentAnimationClip.name;

                if (!isPaused && !isStep)
                {
                    animationTime += Time.deltaTime * animationSpeed;

                    if (animationTime > currentAnimationClip.length)
                        animationTime = 0;
                }

                if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
                {
                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(currentActiveObject, currentAnimationClip, animationTime);
                    AnimationMode.EndSampling();

                    SceneView.RepaintAll();
                }
            }
        }
        private void UpdateRecorder()
        {
            if (startRecording)
            {
                if (!EditorApplication.isPlaying)
                {
                    if (isRecording)
                        isRecording = false;

                    if (cameraShot == CameraShot.multiple)
                    {
                        cameraAngle = cameraAnglesScriptableObject.cameraAngles[cameraAngleIndex].angle;
                        cameraRotation = StringToInt(cameraAnglesScriptableObject.cameraAngles[cameraAngleIndex].angle.ToString());
                        cameraPivot.transform.rotation = Quaternion.Euler(0, cameraRotation, 0);
                        Debug.Log(cameraRotation);
                        EditorApplication.EnterPlaymode();
                    }
                }
                else
                {
                    if (isRecording)
                    {
                        if (!recorderController.IsRecording())
                        {
                            AssetDatabase.Refresh();
                            EditorApplication.ExitPlaymode();

                            if (cameraShot == CameraShot.single)
                                startRecording = false;
                            else
                            {
                                if (cameraAngleIndex == cameraAnglesScriptableObject.cameraAngles.Count)
                                {
                                    startRecording = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        StartRecorder();
                    }
                }
            }
            else
            {
                cameraRotation = StringToInt(cameraAngle.ToString()); ;
                cameraPivot.transform.rotation = Quaternion.Euler(0, cameraRotation, 0);
                cameraAngleIndex = 0;
            }
        }
        private void UpdateCamera()
        {
            if (cameraPivot == null)
            {
                GameObject instanced = Instantiate(spriteGeneratorSourcesScriptableObject.GetCameraPrefab());
                instanced.name = "Camera Pivot";
                cameraPivot = instanced;
            }

            mainCamera = Camera.main;

            if (cameraAngles.Count > 5)
                cameraAngles.RemoveAt(5);

            cameraAngles = cameraAnglesScriptableObject.cameraAngles;
            mainCamera.orthographicSize = cameraSize;
            cameraPivot.transform.position = cameraPosition;
        }
        private void UpdateRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture)
            {
                if (renderTexture.width != StringToInt(resolutionType.ToString()) || renderTexture.height != StringToInt(resolutionType.ToString()))
                {
                    renderTexture.Release();
                    renderTexture.width = StringToInt(resolutionType.ToString());
                    renderTexture.height = StringToInt(resolutionType.ToString());
                    renderTexture.Create();
                }
            }
        }
        private void IsReadyCheck()
        {
            if (assetGenerationType == AssetGenerationType.animationClip)
            {
                switch (sourceType)
                {
                    case SourceType.assign:

                        if (spriteSheetAsset != null && outputFolder != null)
                            isReady = true;
                        else
                            isReady = false;
                        break;
                    case SourceType.selection:
                        selectedSpriteSheets = Selection.GetFiltered(typeof(Texture), SelectionMode.Assets).ToList();
                        if (selectedSpriteSheets.Count > 0 && outputFolder != null)
                            isReady = true;
                        else
                            isReady = false;
                        break;
                    case SourceType.folder:
                        GetSpritesInFolder(AssetDatabase.GetAssetPath(spriteSheetFolder));

                        if (spriteSheetFolder != null && multiSlicedSprites.Count > 0 && outputFolder != null)
                            isReady = true;
                        else
                            isReady = false;
                        break;
                }
            }
            else
            {
                if (outputFolder != null)
                {
                    if (customSpriteName)
                    {
                        if (spriteName != string.Empty)
                            isReady = true;
                        else
                            isReady = false;
                    }
                    else
                        isReady = true;
                }
            }
        }    
        private void StartRecorder()
        {
            isRecording = true;
            cameraAngleIndex++;

            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            recorderController = new RecorderController(controllerSettings);

            var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            imageRecorder.name = "My Image Recorder";
            imageRecorder.Enabled = true;
            imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            imageRecorder.CaptureAlpha = false;

            string combinedName = "";
            string name = currentAnimationClip.name.Remove(0, 5);
            string newName = name.Replace("_01", "");

            if (!customSpriteName)
                combinedName = $"{newName}_{cameraRotation}";
            else
                combinedName = $"{spriteName}_{cameraRotation}";

            if (AssetDatabase.IsValidFolder($"{AssetDatabase.GetAssetPath(outputFolder)}/{combinedName}"))
                AssetDatabase.DeleteAsset($"{AssetDatabase.GetAssetPath(outputFolder)}/{combinedName}");

            string guid = AssetDatabase.CreateFolder(AssetDatabase.GetAssetPath(outputFolder), combinedName);

            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            imageRecorder.OutputFile = $"{newFolderPath}/{combinedName}_{DefaultWildcard.Frame}";

            imageRecorder.imageInputSettings = new RenderTextureInputSettings()
            {
                OutputWidth = renderTexture.width,
                OutputHeight = renderTexture.height,
                FlipFinalOutput = false,
                RenderTexture = renderTexture
            };

            controllerSettings.AddRecorderSettings(imageRecorder);
            controllerSettings.FrameRate = StringToInt(frameRate.ToString());

            if (singleFrame)
            {
                controllerSettings.SetRecordModeToSingleFrame(startFrame);
            }
            else
            {
                controllerSettings.SetRecordModeToFrameInterval(startFrame, endFrame);
            }

            recorderController.PrepareRecording();
            recorderController.StartRecording();
        }
        private void GetSpritesInFolder(string path)
        {
            string oldPath = "";
            if (oldPath != path)
            {
                multiSlicedSprites.Clear();
                oldPath = path;
            }

            List<Texture> spriteSheets = new List<Texture>();

            spriteSheets = AssetDatabase.FindAssets("t:Texture", new[] { path })
               .Select(AssetDatabase.GUIDToAssetPath)
               .Select(AssetDatabase.LoadAssetAtPath<Texture>).ToList();

            foreach (Texture sprite in spriteSheets)
            {
                List<Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(sprite)).OfType<Sprite>().ToList();
                if (sprites.Count > 1)
                {
                    if (!multiSlicedSprites.Contains(sprite))
                        multiSlicedSprites.Add(sprite);
                }
            }
        }
        private void GenerateAnimationClip(Object asset)
        {
            List<string> animations = new List<string>();
            List<SpritesContainer> spritesContainers = new List<SpritesContainer>();
            List<Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(asset)).OfType<Sprite>().ToList();

            spritesContainers.Clear();
            animations.Clear();

            foreach (Sprite sprite in sprites)
            {
                string spriteName = sprite.name.Remove(sprite.name.Length - 5, 5);

                string spriteAngle = spriteName.Remove(0, spriteName.LastIndexOf("_"));
                string filteredName = spriteName.Substring(0, spriteName.LastIndexOf("_"));
                string suffix = "";

                switch (spriteAngle)
                {
                    case "_45":
                        suffix = "-45";
                        break;
                    case "_90":
                        suffix = "-90";
                        break;
                    case "_135":
                        suffix = "-135";
                        break;
                }

                string flippedName = $"{filteredName}_{suffix}";

                if (!animations.Contains(flippedName))
                {
                    if (suffix != "")
                    {
                        SpritesContainer spritesContainer = new SpritesContainer();
                        spritesContainer.name = flippedName;
                        if (!spritesContainers.Contains(spritesContainer))
                            spritesContainers.Add(spritesContainer);

                        animations.Add(flippedName);
                    }
                }

                if (!animations.Contains(spriteName))
                {
                    SpritesContainer spritescontainer = new SpritesContainer();
                    spritescontainer.name = spriteName;
                    if (!spritesContainers.Contains(spritescontainer))
                        spritesContainers.Add(spritescontainer);

                    animations.Add(spriteName);
                }

                foreach (SpritesContainer spritesContainer in spritesContainers)
                {
                    if (spriteName == spritesContainer.name)
                        if (!spritesContainer.sprites.Contains(sprite))
                            spritesContainer.sprites.Add(sprite);

                    if (spritesContainer.name == $"{filteredName}_-45")
                    {
                        if (spriteName == $"{filteredName}_45")
                        {
                            if (!spritesContainer.sprites.Contains(sprite))
                                spritesContainer.sprites.Add(sprite);
                        }
                    }

                    if (spritesContainer.name == $"{filteredName}_-90")
                    {
                        if (spriteName == $"{filteredName}_90")
                        {
                            if (!spritesContainer.sprites.Contains(sprite))
                                spritesContainer.sprites.Add(sprite);
                        }
                    }

                    if (spritesContainer.name == $"{filteredName}_-135")
                    {
                        if (spriteName == $"{filteredName}_135")
                        {
                            if (!spritesContainer.sprites.Contains(sprite))
                                spritesContainer.sprites.Add(sprite);
                        }
                    }
                }
            }

            foreach (SpritesContainer spriteContainer in spritesContainers)
            {
                string spriteSheetDestinationPath = AssetDatabase.GetAssetPath(outputFolder);
                string animationClipFile = "";

                string spriteAngle = spriteContainer.name.Remove(0, spriteContainer.name.LastIndexOf("_"));
                string filteredName = spriteContainer.name.Substring(0, spriteContainer.name.LastIndexOf("_"));
                string suffix = "";
                string clipName = "";

                float flipValue = 0;

                switch (spriteAngle)
                {
                    case "_0":
                        flipValue = 0;
                        suffix = "forward";
                        break;
                    case "_45":
                        flipValue = 0;
                        suffix = "forwardLeft";
                        break;
                    case "_90":
                        flipValue = 0;
                        suffix = "left";
                        break;
                    case "_135":
                        flipValue = 0;
                        suffix = "backwardLeft";
                        break;
                    case "_180":
                        flipValue = 0;
                        suffix = "backward";
                        break;
                    case "_-45":
                        flipValue = 1;
                        suffix = "forwardRight";
                        break;
                    case "_-90":
                        flipValue = 1;
                        suffix = "right";
                        break;
                    case "_-135":
                        flipValue = 1;
                        suffix = "backwardRight";
                        break;
                }

                clipName = $"{filteredName}_{suffix}_01";

                animationClipFile = $"{spriteSheetDestinationPath}/{clipName}.anim";

                Sprite[] spritesSheets = spriteContainer.sprites.ToArray();
                AnimationClip animClip = new AnimationClip();

                animClip.frameRate = StringToInt(frameRate.ToString());

                AnimationClipSettings animClipSett = new AnimationClipSettings();
                animClipSett.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(animClip, animClipSett);

                EditorCurveBinding spriteBinding = new EditorCurveBinding();
                spriteBinding.type = typeof(SpriteRenderer);
                spriteBinding.path = "";
                spriteBinding.propertyName = "m_Sprite";
                float deltaFrameTime = 1 / animClip.frameRate;
                ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[spritesSheets.Length];
                for (int i = 0; i < (spritesSheets.Length); i++)
                {
                    spriteKeyFrames[i] = new ObjectReferenceKeyframe();
                    spriteKeyFrames[i].time = i * deltaFrameTime;
                    spriteKeyFrames[i].value = spriteContainer.sprites[i];
                }

                AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);
                AssetDatabase.CreateAsset(animClip, animationClipFile);

                AnimationCurve curve = new AnimationCurve(new Keyframe(0, flipValue));
                animClip.SetCurve("", typeof(SpriteRenderer), "m_FlipX", curve);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        #endregion
    }

    #region Camera Angle Class
    [System.Serializable]
    public class CameraAngle
    {
        [LabelWidth(60), EnumToggleButtons]
        public PreviewCameraAngle angle;
    }
    #endregion

    #region Sprite Animation Class
    public class SpritesContainer
    {
        public string name;
        public List<Sprite> sprites = new List<Sprite>();
    }
    #endregion
}


