using SCKRM;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    [ExecuteAlways]
    public sealed class GLRenderInvoker : MonoBehaviour
    {
        [SerializeField] Camera _targetCamera; public Camera targetCamera => _targetCamera;

        [SerializeField] BackgroundColorRenderer _backgroundRenderer; public BackgroundColorRenderer backgroundRenderer => _backgroundRenderer;
        [SerializeField] List<PolygonRendererBase> _polygonRendererBases = new List<PolygonRendererBase>(); public List<PolygonRendererBase> polygonRendererBases => _polygonRendererBases;
        [SerializeField] List<PolygonRendererBase> _yukiModeRendererBases = new List<PolygonRendererBase>(); public List<PolygonRendererBase> yukiModeRendererBases => _yukiModeRendererBases;
        [SerializeField] List<PolygonRendererBase> _wallRenderers = new List<PolygonRendererBase>(); public List<PolygonRendererBase> wallRenderers => _wallRenderers;

        void OnRenderObject()
        {
#if UNITY_EDITOR
            if (Camera.current != targetCamera && !UnityEditor.SceneView.currentDrawingSceneView)
                return;
#else
            if (Camera.current != targetCamera)
                return;
#endif
            GL.PushMatrix();

            if (Kernel.coloredMaterial.SetPass(0))
            {
                if (backgroundRenderer != null)
                    backgroundRenderer.Render();

                for (int i = 0; i < yukiModeRendererBases.Count; i++)
                {
                    PolygonRendererBase polygonRendererBase = yukiModeRendererBases[i];
                    if (polygonRendererBase != null)
                        polygonRendererBase.Render();
                }

                for (int i = 0; i < polygonRendererBases.Count; i++)
                {
                    PolygonRendererBase polygonRendererBase = polygonRendererBases[i];
                    if (polygonRendererBase != null)
                        polygonRendererBase.Render();
                }

                for (int i = 0; i < wallRenderers.Count; i++)
                {
                    PolygonRendererBase polygonRendererBase = wallRenderers[i];
                    if (polygonRendererBase != null)
                    {
                        if (polygonRendererBase.distance + polygonRendererBase.width >= 0 && polygonRendererBase.distance <= 50)
                            polygonRendererBase.Render();
                    }
                }
            }

            GL.PopMatrix();
        }
    }
}
