using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    [System.Serializable]
    public class MapGenerator
    {
        [Header("General")]
        public int width;
        public int seed = 0;
        [Header("Fallof Settings")]
        public float fallof;
        public float centerRadius;
        public float fade;
        [Header("Noise Settings")]
        public int cellSize;
        public int heightStep;

        public float[,] GenerateHeightMap()
        {
            if (seed == 0)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
            }
            Random.InitState(seed);

            float[,] noiseMap = new float[width, width];
            Vector3[,] nodes = GenerateNodes(width, cellSize);

            for (int z = 0; z < width; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseMap[x, z] = GetClosestNodeHeight(
                        x,
                        z,
                        nodes)
                        / (2*cellSize);
                }
            }
            
            return noiseMap;
        }

        public float GetFallof(int x, int z)
        {
            Vector2 dist = (new Vector2(x, z) - new Vector2(width / 2, width / 2));
            return 1 - Mathf.Clamp(
                (fallof * (dist.magnitude - width / centerRadius)) / width * fade,
                0f,
                1f);
        }

        public Vector3[,] GenerateNodes(int width, int cellSize)
        {
            Vector3[,] nodes = new Vector3[width/cellSize, width/cellSize];
            for (int z = 0; z < width/cellSize; z++)
            {
                for (int x = 0; x < width/cellSize; x++)
                {

                    nodes[x,z] = new Vector3(Random.Range(x * (cellSize), (x + 1) * (cellSize)),
                        Random.Range(1, heightStep) / (float)heightStep,
                        Random.Range(z * (cellSize), (z + 1) * (cellSize)));

                    nodes[x, z].y *= GetFallof((int)nodes[x, z].x, (int)nodes[x, z].z);
                }
            }
            return nodes;
        }

        public float GetClosestNodeHeight(float x, float z, Vector3[,] nodes)
        {
            float minDist = float.MaxValue;
            float value = 0;

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    float dist = (new Vector3(x, z) - new Vector3(nodes[i, j].x, nodes[i, j].z)).magnitude;
                    if (dist <= minDist)
                    {
                        minDist = dist;
                        value = nodes[i, j].y;
                    }
                }
            }
            return value;
        }
    }
}