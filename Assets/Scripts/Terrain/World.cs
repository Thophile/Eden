using System;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class World : MonoBehaviour
    {
        [Header("General")]
        public float height;
        public int chunkSize;

        [Header("Assets")]
        public int assetsCount;
        public int instantiationTries = 10;
        public LayerMask waterLayer;
        public GameObject[] terrainAssets;

        [Header("Generate")]
        public Material meshMaterial;
        public LayerMask groundLayer;
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
            var assetObject = GameObject.Find("Assets");
            while (assetObject.transform.childCount > 0)
            {
                DestroyImmediate(assetObject.transform.GetChild(0).gameObject);
            }

            for (int i = 0; i < assetsCount; i++)
            {
                var rand = Vector3.ProjectOnPlane(UnityEngine.Random.insideUnitSphere, Vector3.up);
                var prefab = terrainAssets[UnityEngine.Random.Range(0, terrainAssets.Length)];
                var pos = PickPosition(prefab);
                if (pos.Equals(Vector3.zero))
                {
                    var obj = Instantiate(prefab, pos, Quaternion.LookRotation(rand, Vector3.up));
                    obj.transform.parent = assetObject.transform;
                }
            }
        }
        private Vector3 PickPosition(GameObject prefab)
        {
            for (int i = 0; i < instantiationTries; i++)
            {
                RaycastHit hit;
                int size = mapGenerator.width - (2 * mapGenerator.width / chunkSize);
                float x = UnityEngine.Random.Range(-size / 2, +size / 2);
                float z = UnityEngine.Random.Range(-size / 2, +size / 2);
                Debug.DrawLine(new Vector3(x, 30, z), new Vector3(x, 30, z) - transform.up * 100);
                if (Physics.Raycast(new Vector3(x, 30, z), -transform.up, out hit, float.MaxValue, 3))
                {
                    return hit.point;
                }
            }
            return Vector3.zero;
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