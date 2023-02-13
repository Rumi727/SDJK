using UnityEngine;

namespace SCKRM.UI
{
    public sealed class BackEventInvoke : MonoBehaviour
    {
        public static void BackEventInvokeMethod(bool selectedGameObjectIgnore) => UIManager.BackEventInvoke(selectedGameObjectIgnore);
    }
}
