using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public static class Noise
    {
        public static float[,] GenerateHeightMap(
            int width, 
            int height,
            float fallof,
            float centerRadius,
            float fade,
            float yScale,
            float scale, 
            int octaves, 
            float persistance, 
            float lacunarity,
            int seed = 0)
        {
            if (seed == 0)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
            }
            Random.InitState(seed);

            float[,] noiseMap = new float[width, height];
            int nodeNb = width + height;
            Vector2[] nodes;
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
                            nodes)
                            / width;
                        if(i == octaves - 1)
                        {
                            Vector2 dist = (new Vector2(x, y) - new Vector2(width / 2, height / 2));
                            noiseMap[x, y] *= yScale * (1 - Mathf.Clamp(
                                (fallof * (dist.magnitude - width / centerRadius)) / width * fade,
                                0f,
                                1f));
                        }
                    }
                }
                amplitude *= persistance;
                frequency *= lacunarity;
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