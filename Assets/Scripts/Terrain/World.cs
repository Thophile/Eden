using System;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class World : MonoBehaviour
    {
        public float height;
        public int chunkSize;
        public Material meshMaterial;
        public bool autoUpdate;
        public bool autoPreview;
        public MapGenerator mapGenerator;

        public Biome[] biomes;

        public Renderer textureRenderer;

        public void GenerateMap()
        {
            float[,] heightMap = mapGenerator.GenerateHeightMap();

            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            int chunkNb = Mathf.CeilToInt(mapGenerator.width / (float)chunkSize);
            for (int chunkZ = 0; chunkZ < chunkNb; chunkZ++)
            {
                for (int chunkX = 0; chunkX < chunkNb; chunkX++)
                {
                    int offsetZ = chunkZ * (chunkSize - 1);
                    int offsetX = chunkX * (chunkSize - 1);

                    GameObject chunk = new GameObject();
                    chunk.name = String.Concat("Chunk.", chunkX, ".", chunkZ);
                    chunk.transform.parent = this.transform;
                    chunk.transform.position = new Vector3(offsetZ, 0, offsetX);
                    MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
                    MeshData meshData = new MeshData(chunkSize, height, biomes);

                    for (int z = 0; z < chunkSize; z++)
                    {
                        for (int x = 0; x < chunkSize; x++)
                        {
                            // Generate triangle associated with each point
                            meshData.AddVertex(x, heightMap[offsetX + x, offsetZ + z] * height, z);

                            if ((z < (chunkSize - 1)) && (x < (chunkSize - 1)))
                            {
                                meshData.CreateTriangles(x, z);
                            }
                        }
                    }
                    meshData.BuildColors();
                    meshFilter.mesh = meshData.BuildMesh();
                    meshRenderer.sharedMaterial = meshMaterial;
                    //meshRenderer.transform.localScale = new Vector3(10, 10, 10);
                    //meshRenderer.transform.position = new Vector3(- width * 5, 0, - length * 5);
                }
            }


        }

        public void PreviewMap()
        {
            float[,] heightMap = mapGenerator.GenerateHeightMap();

            Texture2D texture = new Texture2D(mapGenerator.width, mapGenerator.width);

            Color[] colors = new Color[mapGenerator.width * mapGenerator.width];

            for (int y = 0; y < mapGenerator.width; y++)
            {
                for (int x = 0; x < mapGenerator.width; x++)
                {
                    colors[y * mapGenerator.width + x] = Color.Lerp(
                        Color.black, 
                        Color.white,
                        heightMap[x, y]
                    );
                }
            }
            texture.SetPixels(colors);
            texture.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(mapGenerator.width, 1, mapGenerator.width);
        }

        private void OnValidate()
        {
            
            if ((mapGenerator.width / (float)chunkSize) != 0)
            {
                float value = (mapGenerator.width / (float)chunkSize);
                int nearest = Mathf.RoundToInt(value);
                if (value > nearest)
                {
                    mapGenerator.width = (nearest + 1) * chunkSize;
                }
                else if (value < nearest)
                {
                    mapGenerator.width = (nearest - 1) * chunkSize;

                }
            }
            if (mapGenerator.width < 1)
            {
                mapGenerator.width = 1;
            }
            if (mapGenerator.width > 100*chunkSize)
            {
                mapGenerator.width = 100*chunkSize;
            }
            if (mapGenerator.cellSize < 1)
            {
                mapGenerator.cellSize = 1;
            }
            if (mapGenerator.cellSize > mapGenerator.width)
            {
                mapGenerator.cellSize = mapGenerator.width;
            }
            if (mapGenerator.fallof < 0)
            {
                mapGenerator.fallof = 0;
            }
            if (mapGenerator.fallof > 1)
            {
                mapGenerator.fallof = 1;
            }
            foreach (var biome in biomes){
                if (biome.minHeight < 0) biome.minHeight = 0;
                else if (biome.minHeight > 1) biome.minHeight = 1;

                if (biome.maxHeight < 0) biome.maxHeight = 0;
                else if (biome.maxHeight > 1) biome.maxHeight = 1;

                if (biome.maxSteepness < 0) biome.maxSteepness = 0;
                else if (biome.maxSteepness > 1) biome.maxSteepness = 1;
            }
        }
    }
}