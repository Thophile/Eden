using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class MeshData
    {
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] triangles;
        public Color[] colors;

        public int width;
        public int height;

        int triangleIndex;

        public MeshData(int width, int height)
        {
            int lenght = (width - 1) * (height - 1) * 6;
            this.width = width;
            this.height = height;
            vertices = new Vector3[width*height];
            uvs = new Vector2[width * height];
            triangles = new int[lenght];
            colors = new Color[lenght];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh BuildMesh()
        {
            int lenght = triangles.Length;
            Vector3[] verticesModified = new Vector3[lenght];
            Vector2[] uvsModified = new Vector2[lenght];
            int[] trianglesModified = new int[lenght];

            Color currentColor = new Color();

            for (int i = 0; i < lenght; i++)
            {
                verticesModified[i] = vertices[triangles[i]];
                uvsModified[i] = uvs[triangles[i]];
                trianglesModified[i] = i;
                
                if (i == 0 || i == 1 || i == 2 /*|| i == 3 || i == 4*/)
                {
                    currentColor = new Color(
                        1f,
                        1f,
                        0f,
                        1.0f
                    );
                }
                else
                {
                    currentColor = new Color(
                        0f,
                        0f,
                        0f,
                        1.0f
                    );
                }
                colors[triangles[i]] = currentColor;

            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
        public Texture2D BuildTexture()
        {
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    }
}