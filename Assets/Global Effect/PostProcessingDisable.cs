using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SDJK
{
    public sealed class PostProcessingDisable : MonoBehaviour
    {
        [SerializeField] PostProcessLayer postProcessLayer;

        void Awake()
        {
#if UNITY_2019_3_OR_NEWER
            if (SystemInfo.usesLoadStoreActions)
#else
            if (Application.isMobilePlatform)
#endif
                postProcessLayer.enabled = false;
        }
    }
}
