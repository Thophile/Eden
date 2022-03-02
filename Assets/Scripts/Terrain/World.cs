using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class World : MonoBehaviour
    {
        [Header("General")]
        public float height;
        public int chunkSize;
        public LayerMask waterLayer;
        public LayerMask groundLayer;
        public LayerMask terrainLayers;

        [Header("Assets")]
        public int assetsCount;
        public int instantiationTries = 10;
        public Asset[] assets;

        [Header("Generate")]
        public Material meshMaterial;
        public bool autoUpdate;
        public MapGenerator mapGenerator;
        public Biome[] biomes;

        [Header("Preview")]
        public bool autoPreview;
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
                    chunk.transform.position = new Vector3(offsetZ - (mapGenerator.width / 2), 0, offsetX - (mapGenerator.width / 2));
                    chunk.layer = 3;
                    MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
                    MeshCollider meshCollider = chunk.AddComponent<MeshCollider>();
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
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
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

        public void PlaceAssets()
        {
            var parent = GameObject.Find("Assets");
            while (parent.transform.childCount > 0)
            {
                DestroyImmediate(parent.transform.GetChild(0).gameObject);
            }

            List<(int, int)> weigthList = new List<(int, int)>();
            int sum = 0;

            for (int i = 0; i < assets.Length; i++)
            {
                sum += assets[i].probability;
                weigthList.Add((i, sum));
            }

            for (int i = 0; i < assetsCount; i++)
            {
                float dice = UnityEngine.Random.Range(0, sum);
                foreach (var tuple in weigthList)
                {
                    if (tuple.Item2 >= dice)
                    {
                        SpawnAsset(assets[tuple.Item1], parent);
                        break;
                    }
                }
            }
        }

        public void SpawnAsset(Asset asset, GameObject parent)
        {
            for (int i = 0; i < instantiationTries; i++)
            {
                RaycastHit zone;
                int size = mapGenerator.width - (2 * mapGenerator.width / chunkSize);
                float x = UnityEngine.Random.Range(-size / 2, +size / 2);
                float z = UnityEngine.Random.Range(-size / 2, +size / 2);

                if (Physics.Raycast(new Vector3(x, 30, z), -transform.up, out zone))
                {
                    if (zone.collider.gameObject.layer == 3)
                    {
                        //For each valid zone
                        RaycastHit hit;
                        for (int j = 0; j < asset.density; j++)
                        {
                            Vector3 origin = zone.point + Vector3.up * 10 + UnityEngine.Random.insideUnitSphere * asset.radius;
                            if (Physics.Raycast(origin, -transform.up, out hit))
                            {
                                if (hit.collider.gameObject.layer == 3 && asset.prefabs.Length > 0)
                                {
                                    GameObject prefab = asset.prefabs[UnityEngine.Random.Range(0, asset.prefabs.Length)];
                                    var obj = Instantiate(
                                        prefab,
                                        hit.point,
                                        Quaternion.LookRotation(Vector3.ProjectOnPlane(UnityEngine.Random.insideUnitSphere, Vector3.up), Vector3.up));
                                    obj.transform.parent = parent.transform;
                                }
                            }
                        }
                        return;
                    }
                }
            }
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
            if (mapGenerator.centerRadius < 0)
            {
                mapGenerator.centerRadius = 0;
            }
            if (mapGenerator.centerRadius > 1)
            {
                mapGenerator.centerRadius = 1;
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