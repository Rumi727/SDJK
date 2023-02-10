using SCKRM.UI.StatusBar;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCKRM.UI
{
    [ExecuteAlways]
    [AddComponentMenu("SC KRM/UI/Canvas Setting")]
    public sealed class CanvasSetting : UIBase
    {
        /// <summary>
        /// 이 변수를 활성화 하면 에디터에서 씬 가시성이 항상 활성화 됩니다.
        /// 이 프로퍼티는 런타임에 영향을 미치지 않습니다.
        /// </summary>
        public bool alwaysVisible { get => _alwaysVisible; set => _alwaysVisible = value; }
        [SerializeField] bool _alwaysVisible = false;
        [SerializeField] bool _customSetting = false; public bool customSetting { get => _customSetting; set => _customSetting = value; }
        [SerializeField] bool _customGuiSize = false; public bool customGuiSize { get => _customGuiSize; set => _customGuiSize = value; }
        [SerializeField] float _guiSize = 1; public float guiSize { get => _guiSize; set => _guiSize = value; }
        [SerializeField] bool _worldRenderMode = false; public bool worldRenderMode { get => _worldRenderMode; set => _worldRenderMode = value; }
        [SerializeField] float _planeDistance = 14; public float planeDistance { get => _planeDistance; set => _planeDistance = value; }
        [SerializeField] bool _forceSafeScreenEnable = false; public bool forceSafeScreenEnable { get => _forceSafeScreenEnable; set => _forceSafeScreenEnable = value; }
        [SerializeField] Vector2 _safeScreenMultiple = Vector2.one; public Vector2 safeScreenMultiple { get => _safeScreenMultiple; set => _safeScreenMultiple = value; }



        [SerializeField, HideInInspector, FormerlySerializedAs("safeScreen")] RectTransform _safeScreen; public RectTransform safeScreen => _safeScreen;

        DrivenRectTransformTracker tracker;

        protected override void OnEnable() => Canvas.preWillRenderCanvases += Refresh;
        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();

            Canvas.preWillRenderCanvases -= Refresh;
        }

        void Refresh()
        {
            if (canvas == null)
                return;

            if (!customGuiSize)
            {
                if (Kernel.isPlaying)
                    canvas.scaleFactor = UIManager.currentGuiSize * guiSize;
                else
                    canvas.scaleFactor = guiSize;
            }

            if (!customSetting)
            {
                if (!worldRenderMode)
                {
                    if (!Kernel.isPlaying)
                        tracker.Clear();

                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || forceSafeScreenEnable)
                        SafeScreenSetting();
                    else
                        SafeScreenDestroy();
                }
                else
                {
                    SafeScreenDestroy();
                    WorldRenderCamera();
                }
            }
        }

        void SafeScreenSetting()
        {
            if (safeScreen == null)
            {
                if (Kernel.isPlaying)
                {
                    if (Kernel.emptyRectTransform == null)
                        return;

                    _safeScreen = Instantiate(Kernel.emptyRectTransform, transform.parent);
                }
                else
                    _safeScreen = new GameObject().AddComponent<RectTransform>();

                safeScreen.name = "Safe Screen";
            }

            if (!Kernel.isPlaying)
            {
                tracker.Clear();
                tracker.Add(this, safeScreen, DrivenTransformProperties.All);
            }

#if UNITY_EDITOR
            {
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(safeScreen.gameObject);
                if (prefabAssetType == PrefabAssetType.NotAPrefab)
                {
                    if (safeScreen.parent != transform)
                        safeScreen.SetParent(transform);
                }
            }
#else
            if (safeScreen.parent != transform)
                safeScreen.SetParent(transform);
#endif

            safeScreen.anchorMin = Vector2.zero;
            safeScreen.anchorMax = Vector2.one;

            if (Kernel.isPlaying)
            {
                float guiSize = 1 / this.guiSize;
                if (customGuiSize)
                    guiSize = UIManager.currentGuiSize / canvas.scaleFactor;

                safeScreen.offsetMin = StatusBarManager.cropedRect.min * guiSize * safeScreenMultiple;
                safeScreen.offsetMax = StatusBarManager.cropedRect.max * guiSize * safeScreenMultiple;
            }
            else
            {
                safeScreen.offsetMin = Vector2.zero;
                safeScreen.offsetMax = Vector2.one;
            }

            safeScreen.pivot = Vector2.zero;

            safeScreen.localEulerAngles = Vector3.zero;
            safeScreen.localScale = Vector3.one;

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childtransform = transform.GetChild(i);
                if (childtransform != safeScreen)
                {
#if UNITY_EDITOR
                    {
                        PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(childtransform.gameObject);
                        if (prefabAssetType == PrefabAssetType.NotAPrefab)
                            childtransform.SetParent(safeScreen);
                    }
#else
                    childtransform.SetParent(safeScreen);
#endif
                    i--;
                    childCount--;
                }
            }
        }

        void SafeScreenDestroy()
        {
            if (safeScreen == null)
                return;

            int childCount = safeScreen.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childtransform = safeScreen.GetChild(i);
                if (childtransform != safeScreen)
                {
#if UNITY_EDITOR
                    {
                        PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(childtransform.gameObject);
                        if (prefabAssetType == PrefabAssetType.NotAPrefab)
                            childtransform.SetParent(transform);
                    }
#else
                    childtransform.SetParent(transform);
#endif
                    i--;
                    childCount--;
                }
            }

#if UNITY_EDITOR
            {
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(safeScreen.gameObject);
                if (prefabAssetType == PrefabAssetType.NotAPrefab)
                    DestroyImmediate(safeScreen.gameObject);
            }
#else
            DestroyImmediate(safeScreen.gameObject);
#endif
        }

        void WorldRenderCamera()
        {
            if (!Kernel.isPlaying)
            {
                tracker.Clear();
                tracker.Add(this, rectTransform, DrivenTransformProperties.All);
            }


            UnityEngine.Camera camera = canvas.worldCamera;
            if (camera == null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                return;
            }
            else
                canvas.renderMode = RenderMode.WorldSpace;



            transform.rotation = camera.transform.rotation;
            transform.position = camera.transform.position + (transform.forward * planeDistance);


            float guiSize;
            if (customGuiSize)
                guiSize = canvas.scaleFactor;
            else
                guiSize = UIManager.currentGuiSize * this.guiSize;

            float width = camera.pixelWidth * (1 / guiSize);
            float height = camera.pixelHeight * (1 / guiSize);

            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.pivot = Vector2.one * 0.5f;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;



            float screenX, screenY;

            if (camera.orthographic)
            {
                screenY = camera.orthographicSize * 2;
                screenX = screenY / height * width;
            }
            else
            {
                screenY = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f * planeDistance;
                screenX = screenY / height * width;
            }

            transform.localScale = new Vector3(screenX / width, screenY / height, screenX / width);
        }
    }
}