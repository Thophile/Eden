using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    [System.Serializable]
    public class MapGenerator
    {
        [Header("General")]
        public int width;
        public int seed = 0;
        [Header("Perlin Noise Settings")]
        public float noiseRatio = 0;
        public float noiseScale = 1;
        [Header("Fallof Settings")]
        public float centerRadius;
        public float fallof;
        [Header("Cellular Noise Settings")]
        public int cellSize;
        public int heightStep;
        [Header("Blurr Settings")]
        public int blurrReach = 1;

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
                    noiseMap[x, z] = (1-noiseRatio)*GetClosestNodeHeight(x,z,nodes) + noiseRatio*GetPerlinNoise(x, z);
                }
            }
            

            return BlurEffect(noiseMap);
        }
        public float[,] BlurEffect(float[,] noiseMap)
        {
            float[,] blurredNoiseMap = new float[width, width];
            List<float> neighboors = new List<float>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < width; z++)
                {
                    neighboors.Clear();
                    for (int i = -blurrReach; i <= blurrReach; i++)
                    {
                        for (int j = -blurrReach; j <= blurrReach; j++)
                        {
                            if(x + i > 0 && x + i < width && z + j > 0 && z + j < width)
                            {
                                neighboors.Add(noiseMap[x + i, z + j]);
                            }
                        }
                    }
                    

                    blurredNoiseMap[x,z] = GetAverage(neighboors);
                }
            }

            return blurredNoiseMap;
        }

        public float GetAverage(List<float> values)
        {
            float sum = 0;
            values.ForEach(value =>
            {
                sum += value;
            });
            return sum/values.Count;
        }

        public float GetPerlinNoise(int x, int z)
        {
            return Mathf.PerlinNoise((float)x / width * noiseScale, (float)z / width * noiseScale) * GetFallof(x,z);
        }

        public float GetFallof(int x, int z)
        {
            Vector2 dist = (new Vector2(x, z) - new Vector2(width / 2, width / 2));
            if (dist.magnitude < (centerRadius * width / 2f)) return 1;
            return Mathf.Clamp(
                1 - fallof - ((dist.magnitude - (centerRadius * width / 2f)) / (centerRadius * width / 2f)),
                0f,
                1f);
            
            /*return 1 - Mathf.Clamp(
                (fallof * (dist.magnitude - width / centerRadius)) / width * fade,
                0f,
                1f);*/
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