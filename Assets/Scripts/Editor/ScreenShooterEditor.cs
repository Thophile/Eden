using Assets.Scripts.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CustomEditor(typeof(ScreenShooter))]
    public class ScreenShooterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ScreenShooter screenShooter = (ScreenShooter)target;

            if (GUILayout.Button("Take screenshot"))
            {
                screenShooter.Shot();
            }
        }
    }
}
