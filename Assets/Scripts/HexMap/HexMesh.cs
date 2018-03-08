using maho;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maho
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

    public static class ListPool<T>
    {
        static Stack<List<T>> stack = new Stack<List<T>>();

        public static List<T> Get()
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            return new List<T>();
    }
        public static void Add(List<T> list)
        {
            list.Clear();
            stack.Push(list);
        }
    }

 
public class HexMesh : MonoBehaviour {
       
       Mesh hexMesh;
       [NonSerialized] List<Vector3> vertices;
       [NonSerialized] List<int> triangles;
       MeshCollider meshcoollider;
       [NonSerialized] List<Color> colors;
    void Awake()
    {   GetComponent<MeshFilter>().mesh= hexMesh = new Mesh();
            meshcoollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name= "Hex Mesh";
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
    }
        
        public void Clear()
        {
            hexMesh.Clear();
            vertices = ListPool<Vector3>.Get();
            colors = ListPool<Color>.Get();
            triangles = ListPool<int>.Get();
        }

        public void Apply()
        {
            hexMesh.SetVertices(vertices);
            ListPool<Vector3>.Add(vertices);
            hexMesh.SetColors(colors);
            ListPool<Color>.Add(colors);
            hexMesh.SetTriangles(triangles, 0);
            ListPool<int>.Add(triangles);
            hexMesh.RecalculateNormals();
            meshcoollider.sharedMesh = hexMesh;
        }
        

        public void Triangulate (HexCell[] cells)
    {

            hexMesh.Clear();

            vertices.Clear();

            colors.Clear();

            triangles.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }

            hexMesh.vertices = vertices.ToArray();

            hexMesh.colors = colors.ToArray();

            hexMesh.triangles = triangles.ToArray();

            hexMesh.RecalculateNormals();

        meshcoollider.sharedMesh = hexMesh;
        }

        void Triangulate(HexCell cell)
        {
            Vector3 center = cell.transform.localPosition;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(
                 center,
                 center + HexMetrics.corners[i],
                 center + HexMetrics.corners[i + 1]
                 );
                AddTriangleColor(cell.color);
            }
        }
        void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }
        void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
       
    }

}