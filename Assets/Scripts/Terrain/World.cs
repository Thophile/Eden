using System.Collections;
using System.Collections.Generic;
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
        public float fallof;
        public float centerRadius;
        public float fade;
        public float yScale;
        public bool autoUpdate;

        public Biome[] biomes;

        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public void GenerateMap()
        {
            float[,] heightMap = Noise.GenerateHeightMap(
                width,
                height,
                fallof,
                centerRadius,
                fade,
                yScale,
                scale,
                octaves,
                persistance,
                lacunarity,
                seed);

            MeshData meshData = new MeshData(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Generate triangle associated with each point
                    meshData.AddVertex(x, heightMap[x,y], y);

                    if ( (y < height - 1) && (x < width - 1))
                    {
                        meshData.CreateTriangles(x, y);
                    }
                }
            }
            meshFilter.mesh = meshData.BuildMesh();
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
                        noiseMap[x, y]
                    );
                }
            }
            texture.SetPixels(colors);
            texture.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(width, 1, height);
        }

        private void OnValidate()
        {
            if(width < 1)
            {
                width = 1;
            }
            if (width > 128)
            {
                width = 128;
            }
            if (height < 1)
            {
                height = 1;
            }
            if (height > 128)
            {
                height = 128;
            }
            if (scale < 1f)
            {
                scale = 1f;
            }
            if (scale > 5f)
            {
                scale = 5f;
            }
            if (octaves < 1)
            {
                octaves = 1;
            }
            if (octaves > 4)
            {
                octaves = 4;
            }
            if (persistance > 1)
            {
                persistance = 1;
            }
            if (persistance < 0)
            {
                persistance = 0;
            }
            if (lacunarity < 1)
            {
                lacunarity = 1;
            }
            if (lacunarity > 3)
            {
                lacunarity = 3;
            }
            if (fallof < 0)
            {
                fallof = 0;
            }
            if (fallof > 1)
            {
                fallof = 1;
            }
        }
    }
}