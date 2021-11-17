using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class World : MonoBehaviour
    {
        public int width;
        public int height;
        public int regionNb;
        public float noiseScale;
        public int cellSize;
        public int seed;

        public Renderer textureRenderer;

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(width, height, regionNb, noiseScale, seed);
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
                    colors[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y] / cellSize);
                }
            }
            texture.SetPixels(colors);
            texture.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(width, 1, height);
        }
    }
}