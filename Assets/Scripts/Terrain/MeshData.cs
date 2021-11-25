using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class MeshData
    {
        public int width;
        public float height;
        public int length;
        public Biome[] biomes;
        public List<Vector3> vertices = new List<Vector3>();
        public int[,] points;
        public List<Vector2> uv = new List<Vector2>();
        public Color32[] colors;
        public List<int> triangles = new List<int>();

        public MeshData(int width, float height, int length, Biome[] biomes)
        {
            this.width = width;
            this.height = height;
            this.length = length;
            this.biomes = biomes;
            points = new int[width, length];
        }

        public void AddVertex(float x, float y, float z)
        {
            int n = GetPointAtCoordinate((int)x, (int)z);

            for (int i = 0; i < n; i++)
            {
                vertices.Add(new Vector3(z, y, x));
                uv.Add(new Vector2(x / (float)width, y / (float)length));
            }

        }

        public void CreateTriangles(int x, int z)
        {           
            if (IsEven(x,z))
            {
                triangles.Add(IndexAtPoint(x, z));
                triangles.Add(IndexAtPoint(x + 1, z));
                triangles.Add(IndexAtPoint(x, z + 1));

                triangles.Add(IndexAtPoint(x + 1, z));
                triangles.Add(IndexAtPoint(x + 1, z + 1));
                triangles.Add(IndexAtPoint(x, z + 1));
            } else
            {
                triangles.Add(IndexAtPoint(x, z));
                triangles.Add(IndexAtPoint(x + 1, z + 1));
                triangles.Add(IndexAtPoint(x, z + 1));

                triangles.Add(IndexAtPoint(x + 1, z));
                triangles.Add(IndexAtPoint(x + 1, z + 1));
                triangles.Add(IndexAtPoint(x, z));
            }
        }
        public int IndexAtPoint(int x, int z)
        {
            int index = GetIndex(x, z) + points[x, z];
            points[x, z]++;
            return index;
        }
        public int GetPointAtCoordinate(int x, int z)
        {
            int n;
            if( IsEven(x,z) )
            {
                n = 1;
            }
            else
            {
                n = 2;
            }
            
            if (z < length - 1 && z > 0)
            {
                n *= 2;
            }

            if (x < width - 1 && x > 0)
            {
                n *= 2;
            }
            return n;
        }
        public int GetIndex(int x, int z)
        {

            int rowOffset = 0;
            int colOffset = 0;

            for (int i = 0; i < z; i++)
            {
                if (i < length - 1 && i > 0)
                {
                    rowOffset += 6 * (width - 1);

                }
                else
                {
                    rowOffset += 3 * (width - 1);
                }
            }

            for (int j = 0; j < x; j++)
            {
                colOffset += GetPointAtCoordinate(j, z);
            }

            return rowOffset + colOffset;
        }
        bool IsEven(int x, int z)
        {
            if ((width % 2) == 0)
            {
                return ((x + z) % 2 == 0);
            }
            else
            {
                return (x % 2 == 0);
            }
        }

        public void BuildColors()
        {
            colors = new Color32[triangles.Count];
            Color32 color = new Color32();

            for (int i = 0; i < triangles.Count; i++)
            {

                if ((i % 3) == 0)
                {
                    Vector3 p0 = vertices[triangles[i + 0]];
                    Vector3 p1 = vertices[triangles[i + 1]];
                    Vector3 p2 = vertices[triangles[i + 2]];

                    Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0).normalized;
                    float height = (p0.y + p1.y + p2.y) / 3;
                    color = GetBiomeColor(normal, height);

                }
                colors[triangles[i]] = color;
            }
        }

        public Color32 GetBiomeColor(Vector3 normal, float y)
        {
            float steepness = Vector3.ProjectOnPlane(normal, Vector3.up).magnitude;

            foreach (var biome in biomes)
            {
                if(steepness <= biome.steepness && y <= biome.maxHeight * this.height && y >= biome.minHeight * this.height)
                {
                    return (Color32)biome.color;
                }
            }
            return new Color32(255,255,255,255);
        }

        public Mesh BuildMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors32 = colors;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}