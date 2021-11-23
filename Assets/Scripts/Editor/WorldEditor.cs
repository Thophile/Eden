﻿using Assets.Scripts.Terrain;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CustomEditor (typeof (World))]
    public class WorldEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            World world = (World)target;
            if(DrawDefaultInspector())
            {
                if (world.autoUpdate)
                {
                    world.GenerateMap();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                world.GenerateMap();
            }
        }
    }
}