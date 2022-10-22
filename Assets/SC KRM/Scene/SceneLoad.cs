using SCKRM.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM
{
    public sealed class SceneLoad : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] ProgressBar progressBar;

        float timer = 0;
        void Update()
        {
            if (timer >= 3)
            {
                if (SceneLoadManager.progress >= 0.9f)
                {
                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.01f * Kernel.fpsDeltaTime);

                    if (canvasGroup.alpha <= 0)
                        SceneLoadManager.allowSceneActivation = true;

                    progressBar.progress = 1;
                }
                else
                {
                    canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.01f * Kernel.fpsDeltaTime);
                    SceneLoadManager.allowSceneActivation = false;

                    progressBar.progress = SceneLoadManager.progress;
                }
            }
            else
            {
                timer += Kernel.deltaTime;
                SceneLoadManager.allowSceneActivation = true;

                progressBar.progress = SceneLoadManager.progress;
            }
        }
    }
}
