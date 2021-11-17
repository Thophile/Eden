using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public static class Noise
    {
        public static float maxNoiseHeight;
        public static float minNoiseHeight;

        public static float[,] GenerateNoiseMap(
            int width, 
            int height, 
            float scale, 
            int octaves, 
            float persistance, 
            float lacunarity, 
            int seed = 0)
        {

            if (scale < 1f)
            {
                scale = 1f;
            }
            if (seed == 0)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
            }
            Random.InitState(seed);

            float[,] noiseMap = new float[width, height];
            int nodeNb = width + height;
            Vector2[] nodes;
            maxNoiseHeight = float.MinValue;
            minNoiseHeight = float.MaxValue;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < octaves; i++)
            {
                nodes = GenerateNodes(width, height, (int)(nodeNb*frequency));
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        noiseMap[x, y] += amplitude * GetDistanceToClosestNode(
                            x / scale,
                            y / scale,
                            nodes);
                    }
                }
                amplitude *= persistance;
                frequency *= lacunarity;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noiseHeight = noiseMap[x, y];

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                }
            }
            return noiseMap;
        }

        public static Vector2[] GenerateNodes(int width, int height, int nodeNb)
        {
            Vector2[] nodes = new Vector2[nodeNb];

            for (int i = 0; i < nodeNb; i++)
            {
                nodes[i] = new Vector2(Random.Range(0, width), Random.Range(0, height));
            }
            return nodes;
        }

        public static float GetDistanceToClosestNode(float x, float y, Vector2[] nodes)
        {
            float minDist = float.MaxValue;

            foreach (Vector2 node in nodes)
            {
                float dist = (new Vector2(x, y) - node).sqrMagnitude;
                if(dist <= minDist)
                {
                    minDist = dist;
                }
            }
            return minDist;
        }

    }
}