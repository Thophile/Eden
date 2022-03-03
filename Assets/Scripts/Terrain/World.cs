using Assets.Scripts.Utils;
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
        public int zonesCount;
        public int instantiationTries = 10;
        public Zone[] zones;

        [Header("Generate")]
        public Material meshMaterial;
        public bool autoUpdate;
        public MapGenerator mapGenerator;
        public GroundType[] groundTypes;

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
                    MeshData meshData = new MeshData(chunkSize, height, groundTypes);

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

            List<(Zone, int)> weigthList = new List<(Zone, int)>();
            int sum = 0;

            foreach (Zone zone in zones)
            {
                sum += zone.probability;
                weigthList.Add((zone, sum));
            }

            for (int i = 0; i < zonesCount; i++)
            {
                Zone zone = RandomUtils.WeightedRandom(weigthList, sum);
                if(zone != default(Zone)) SpawnAsset(zone, parent);
            }
        }

        public void SpawnAsset(Zone zone, GameObject parent)
        {
            for (int i = 0; i < instantiationTries; i++)
            {
                RaycastHit zoneHit;
                int size = mapGenerator.width - (2 * mapGenerator.width / chunkSize);
                float x = UnityEngine.Random.Range(-size / 2, size / 2);
                float z = UnityEngine.Random.Range(-size / 2, size / 2);

                if (Physics.Raycast(new Vector3(x, 30, z), -transform.up, out zoneHit))
                {
                    if (zoneHit.collider.gameObject.layer == 3)
                    {
                        //For each valid zone
                        RaycastHit assetHit;
                        for (int j = 0; j < zone.density; j++)
                        {
                            Vector3 origin = zoneHit.point + Vector3.up * 10 + UnityEngine.Random.insideUnitSphere * zone.radius;
                            if (Physics.Raycast(origin, -transform.up, out assetHit))
                            {
                                if (assetHit.collider.gameObject.layer == 3 && zone.assets.Length > 0)
                                {
                                    List<(Asset, int)> weigthList = new List<(Asset, int)>();
                                    int sum = 0;

                                    foreach (Asset asset in zone.assets)
                                    {
                                        sum += asset.probability;
                                        weigthList.Add((asset, sum));
                                    }
                                    Asset selectedAsset = RandomUtils.WeightedRandom(weigthList, sum);
                                    if(selectedAsset != default(Asset))
                                    {
                                        var obj = Instantiate(
                                            selectedAsset.prefab,
                                            assetHit.point,
                                            Quaternion.LookRotation(Vector3.ProjectOnPlane(UnityEngine.Random.insideUnitSphere, Vector3.up), Vector3.up));
                                        obj.transform.parent = parent.transform;
                                    }
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
            foreach (var groundType in groundTypes){
                if (groundType.minHeight < 0) groundType.minHeight = 0;
                else if (groundType.minHeight > 1) groundType.minHeight = 1;

                if (groundType.maxHeight < 0) groundType.maxHeight = 0;
                else if (groundType.maxHeight > 1) groundType.maxHeight = 1;

                if (groundType.maxSteepness < 0) groundType.maxSteepness = 0;
                else if (groundType.maxSteepness > 1) groundType.maxSteepness = 1;
            }
        }
    }
}