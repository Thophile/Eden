using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class ScreenShooter : MonoBehaviour
    {
        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                Shot();
            }
        }

        public void Shot()
        {
            var basePath = Application.persistentDataPath + "/Screenshots/";
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            ScreenCapture.CaptureScreenshot(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".png", 4);
        }
    }
}