using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public static class Noise
    {

        public static float[,] GenerateNoiseMap(int width, int height, int regionNb, float scale, int seed = 0)
        {

            if (scale <= 0)
            {
                scale = float.Epsilon;
            }
            if (seed == 0)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
            }
            Random.InitState(seed);

            float[,] noiseMap = new float[width, height];
            Vector2[] nodes = GenerateNodes(width, height, regionNb);
            float maxDist = 0f;


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dist = GetDistanceToClosestNode(x / scale, y / scale, nodes);
                    if (dist > maxDist)
                    {
                        maxDist = dist;
                    }
                    noiseMap[x, y] = dist;
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