using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class World : MonoBehaviour
    {
        public int seed;
        public int width;
        public float height;
        public int length;
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public float fallof;
        public float centerRadius;
        public float fade;
        public bool autoUpdate;

        public Biome[] biomes;

        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public void GenerateMap()
        {
            float[,] heightMap = Noise.GenerateHeightMap(
                width,
                length,
                fallof,
                centerRadius,
                fade,
                height,
                scale,
                octaves,
                persistance,
                lacunarity,
                seed);

            MeshData meshData = new MeshData(width, height, length, biomes);
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Generate triangle associated with each point
                    meshData.AddVertex(x, heightMap[x,z], z);

                    if ( (z < length - 1) && (x < width - 1))
                    {
                        meshData.CreateTriangles(x, z);
                    }
                }
            }
            meshData.BuildColors();
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
            if (length < 1)
            {
                length = 1;
            }
            if (length > 128)
            {
                length = 128;
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
            foreach(var biome in biomes){
                if (biome.minHeight < 0) biome.minHeight = 0;
                else if (biome.minHeight > 1) biome.minHeight = 1;

                if (biome.maxHeight < 0) biome.maxHeight = 0;
                else if (biome.maxHeight > 1) biome.maxHeight = 1;

                if (biome.steepness < 0) biome.steepness = 0;
                else if (biome.steepness > 1) biome.steepness = 1;
            }
        }
    }
}