using SCKRM.Rhythm;
using TMPro;
using UnityEngine;

namespace SCKRM.UI
{
    [ExecuteAlways]
    [AddComponentMenu("SC KRM/UI/Color/Input Field Caret Color", 0)]
    public sealed class InputFieldCaretColor : UI
    {
        [SerializeField, NotNull] TMP_InputField _inputField; public TMP_InputField inputField => _inputField;

        /*protected override void OnEnable()
        {
            if (!Kernel.isPlaying)
                return;

            inputField.caretColor = defaultCaretColor;
        }*/

        protected override void Awake()
        {
            if (!Kernel.isPlaying)
                return;

            if (inputField == null)
                return;

            defaultCaretBlinkRate = inputField.caretBlinkRate;
            defaultCaretColor = inputField.caretColor;
        }

        float defaultCaretBlinkRate = 0;
        Color defaultCaretColor = Color.white;
        void Update()
        {
            if (inputField == null)
                return;

            inputField.customCaretColor = true;

            if (!Kernel.isPlaying)
                return;

            if (RhythmManager.isPlaying)
            {
                inputField.caretBlinkRate = 0;
                inputField.caretColor = defaultCaretColor.Lerp(new Color(defaultCaretColor.r, defaultCaretColor.g, defaultCaretColor.b, 0.3f), (float)RhythmManager.currentBeatScreen1Beat);
            }
            else
            {
                inputField.caretBlinkRate = defaultCaretBlinkRate;
                inputField.caretColor = inputField.caretColor.MoveTowards(defaultCaretColor, 0.025f * Kernel.fpsUnscaledSmoothDeltaTime);
            }
        }
    }
}
