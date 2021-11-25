using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class MeshData
    {
        public int width;
        public int height;
        public List<Vector3> vertices = new List<Vector3>();
        public int[,] points;
        public List<Vector2> uv = new List<Vector2>();
        public Color32[] colors = new Color32[5046];
        public List<int> triangles = new List<int>();

        public MeshData(int width, int height)
        {
            this.width = width;
            this.height = height;
            points = new int[width, height];
        }

        public void AddVertex(float x, float y, float z)
        {
            int n = GetPointAtCoordinate((int)x, (int)z);

            for (int i = 0; i < n; i++)
            {
                vertices.Add(new Vector3(z, y, x));
                uv.Add(new Vector2(x / (float)width, y / (float)height));
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
            
            if (z < height - 1 && z > 0)
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
                if (i < height - 1 && i > 0)
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
            Color32 color = new Color32();
            for (int i = 0; i < triangles.Count; i++)
            {

                if ((i % 3) == 0)
                {
                    color = GetBiomeColor(i);
                }
                    colors[triangles[i]] = color;
            }
        }

        public Color32 GetBiomeColor(int index)
        {
            return new Color32(
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                255);
        }

        public Mesh BuildMesh()
        {
            Debug.Log(vertices.Count);
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