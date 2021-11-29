using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class World : MonoBehaviour
    {
        public float height;
        public bool autoUpdate;
        public bool autoPreview;
        public MapGenerator mapGenerator;

        public Biome[] biomes;

        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public void GenerateMap()
        {
            float[,] heightMap = mapGenerator.GenerateHeightMap();

            MeshData meshData = new MeshData(mapGenerator.width, height, biomes);
            for (int z = 0; z < mapGenerator.width; z++)
            {
                for (int x = 0; x < mapGenerator.width; x++)
                {
                    // Generate triangle associated with each point
                    meshData.AddVertex(x, heightMap[x,z] * height, z);

                    if ( (z < mapGenerator.width - 1) && (x < mapGenerator.width - 1))
                    {
                        meshData.CreateTriangles(x, z);
                    }
                }
            }
            meshData.BuildColors();
            meshFilter.mesh = meshData.BuildMesh();
            meshRenderer.transform.localScale = new Vector3(10, 10, 10);
            meshRenderer.transform.position = new Vector3(- mapGenerator.width * 5, 0, - mapGenerator.width * 5);

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
            if (mapGenerator.width < 1)
            {
                mapGenerator.width = 1;
            }
            if (mapGenerator.width > 256)
            {
                mapGenerator.width = 256;
            }
            if (mapGenerator.width < 1)
            {
                mapGenerator.width = 1;
            }
            if (mapGenerator.width > 256)
            {
                mapGenerator.width = 256;
            }
            if (mapGenerator.cellSize < 1)
            {
                mapGenerator.cellSize = 1;
            }
            if (mapGenerator.cellSize > mapGenerator.width)
            {
                mapGenerator.cellSize = mapGenerator.width;
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

                if (biome.steepness < 0) biome.steepness = 0;
                else if (biome.steepness > 1) biome.steepness = 1;
            }
        }
    }
}