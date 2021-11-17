using Assets.Scripts.Terrain;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CustomEditor (typeof (World))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            World world = (World)target;
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                world.GenerateMap();
            }
        }
    }
}