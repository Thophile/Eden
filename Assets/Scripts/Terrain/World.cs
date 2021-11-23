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

            float[,] noiseMap = Noise.GenerateNoiseMap(
                width, 
                height, 
                scale, 
                octaves, 
                persistance, 
                lacunarity, 
                seed);
            float[,] fallofMap = Noise.GenerateFallofMap(width, height, fallof, centerRadius, fade);

            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();
            List<Color> colors = new List<Color>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float mapHeight = (Mathf.InverseLerp(
                            Noise.minNoiseHeight,
                            Noise.maxNoiseHeight,
                            noiseMap[x, y]) - fallofMap[x, y]) * yScale;

                    vertices.Add(new Vector3(y, mapHeight, x));
                    uvs.Add(new Vector2(x / (float)width, y / (float)height));

                    int index = y * height + x;
                    if (x < width - 1 && y < height - 1)
                    {

                        indices.Add(index);
                        indices.Add(index + width + 1);
                        indices.Add(index + width);

                        indices.Add(index + width + 1);
                        indices.Add(index);
                        indices.Add(index + 1);
                    }
                }
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * height + x;
                    float mapHeight = vertices[index].y;
                    Color color = Color.white;
                    foreach (var biome in biomes)
                    {

                        if (mapHeight > (biome.height * yScale))
                        {
                            color = biome.color;
                            
                            break;
                        }

                    }
                    colors.Add(color);
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
            mesh.SetUVs(0, uvs);

            mesh.RecalculateNormals();

            BuildMesh(mesh);
            BuildTexture(colors);
        }

        public void LoadMap()
        {

        }

        public void BuildMesh(Mesh mesh)
        {
            meshFilter.mesh = mesh;
        }

        public void BuildTexture(List<Color> colors)
        {
            Texture2D texture = new Texture2D(width, height)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels(colors.ToArray());
            texture.Apply();
            meshRenderer.sharedMaterial.mainTexture = texture;
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