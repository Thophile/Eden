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
        public List<Color32> colors = new List<Color32>();
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
            }

        }

        public void CreateTriangles(int x, int y)
        {           
            if (IsEven(x,y))
            {
                triangles.Add(IndexAtPoint(x, y));
                triangles.Add(IndexAtPoint(x + 1,y));
                triangles.Add(IndexAtPoint(x, y + 1));

                triangles.Add(IndexAtPoint(x + 1, y));
                triangles.Add(IndexAtPoint(x + 1, y + 1));
                triangles.Add(IndexAtPoint(x, y + 1));
            } else
            {
                triangles.Add(IndexAtPoint(x, y));
                triangles.Add(IndexAtPoint(x + 1, y + 1));
                triangles.Add(IndexAtPoint(x, y + 1));

                triangles.Add(IndexAtPoint(x + 1, y));
                triangles.Add(IndexAtPoint(x + 1, y + 1));
                triangles.Add(IndexAtPoint(x, y));
            }
        }
        public int IndexAtPoint(int x, int y)
        {
            int index = GetIndex(x, y) + points[x, y];
            points[x, y]++;
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
        public int GetIndex(int x, int y)
        {

            int rowOffset = 0;
            int colOffset = 0;
            if(y != 0)
            {
                for (int i = 0; i < y; i++)
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
            }

            if(x != 0)
            {
                for (int j = 0; j < x; j++)
                {
                    colOffset += GetPointAtCoordinate(j, y);
                }
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

        public Mesh BuildMesh()
        {
           
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors32 = colors.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}