using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomNavMeshPoint : MonoBehaviour
{
    private UnityEngine.AI.NavMeshTriangulation nav;
    private Mesh mesh;
    private float totalArea;

    public void Awake()
    {
        nav = UnityEngine.AI.NavMesh.CalculateTriangulation();
        mesh = new Mesh();
        mesh.vertices = nav.vertices;
        mesh.triangles = nav.indices;

        totalArea = 0.0f;
        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            totalArea += GetTriangleArea(i);
        }
    }

    /** Get a random triangle on the NavMesh
      * Steps:
      * 1. Get a random triangle on the mesh (weighted by it's area)
      * 2. Get a random point inside that triangle
      */ 
    public Vector3 GetRandomPointOnNavMesh()
    {
        int triangle = GetRandomTriangleOnNavMesh();
        return GetRandomPointOnTriangle(triangle);
    }

    /** Grabs a random triangle in the mesh, weighted by size so random point distribution is even
      *
      */
    private int GetRandomTriangleOnNavMesh()
    {
        float rnd = Random.Range(0, totalArea);
        int nTriangles = mesh.triangles.Length / 3;
        for (int i = 0; i < nTriangles; i++)
        {
            rnd -= GetTriangleArea(i);
            if (rnd <= 0)
                return i;
        }
        return 0;
    }

    /** Gets a random point on a triangle.
      * 
      * @var int idx THe triangle index in the NavMesh
      */
    private Vector3 GetRandomPointOnTriangle(int idx)
    {
        Vector3[] v = new Vector3[3];


        v[0] = mesh.vertices[mesh.triangles[3 * idx + 0]];
        v[1] = mesh.vertices[mesh.triangles[3 * idx + 1]];
        v[2] = mesh.vertices[mesh.triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        // Generate a random point in the trapezoid
        Vector3 result = v[0] + Random.Range(0f, 1f) * a + Random.Range(0f, 1f) * b;

        // Barycentric coordinates on triangles
        float alpha = ((v[1].z - v[2].z) * (result.x - v[2].x) + (v[2].x - v[1].x) * (result.z - v[2].z)) /
                ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float beta = ((v[2].z - v[0].z) * (result.x - v[2].x) + (v[0].x - v[2].x) * (result.z - v[2].z)) /
               ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float gamma = 1.0f - alpha - beta;

        // The selected point is outside of the triangle (wrong side of the trapezoid), project it inside through the center.
        if (alpha < 0 || beta < 0 || gamma < 0)
        {
            Vector3 center = v[0] + c / 2;
            center = center - result;
            result += 2 * center;
        }

        return result;
    }

    /** Helper function to calculate the area of a triangle.
      * Used as weights when selecting a random triangle so bigger triangles have a higher chance (hence yielding an even distribution of points on the entire mesh)
      *
      * @var int idx The index of the triangle to calculate the area of
      */
    private float GetTriangleArea(int idx)
    {
        Vector3[] v = new Vector3[3];


        v[0] = mesh.vertices[mesh.triangles[3 * idx + 0]];
        v[1] = mesh.vertices[mesh.triangles[3 * idx + 1]];
        v[2] = mesh.vertices[mesh.triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        float ma = a.magnitude;
        float mb = b.magnitude;
        float mc = c.magnitude;

        float area = 0f;

        float S = (ma + mb + mc) / 2;
        area = Mathf.Sqrt(S * (S - ma) * (S - mb) * (S - mc));

        return area;
    }
}