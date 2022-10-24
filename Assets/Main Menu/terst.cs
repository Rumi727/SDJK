using SCKRM.Input;
using SCKRM.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu
{
    public class terst : MonoBehaviour
    {
        void Update()
        {
            if (InputManager.GetKey(KeyCode.L))
                MainMenu.MainMenuLoad().Forget();
        }
    }
}
