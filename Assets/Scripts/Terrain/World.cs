using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class World : MonoBehaviour
    {
        public int seed;
        public int width;
        public int height;
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public bool autoUpdate;

        public Renderer textureRenderer;

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(width, height, scale, octaves, persistance, lacunarity, seed);
            DrawNoiseMap(noiseMap);
        }

        public void LoadMap()
        {

        }

        public void DrawNoiseMap(float[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            Color[] colors = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[y * width + x] = Color.Lerp(
                        Color.black, 
                        Color.white, 
                        Mathf.InverseLerp(Noise.minNoiseHeight, Noise.maxNoiseHeight, noiseMap[x, y]));
                }
            }
            texture.SetPixels(colors);
            texture.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(width, 1, height);
        }
    }
}