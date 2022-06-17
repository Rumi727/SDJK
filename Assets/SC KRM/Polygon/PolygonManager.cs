using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Polygon
{
    public static class PolygonManager
    {
        //code author: https://gamedev.stackexchange.com/a/146460
        public static void ToFilledMesh(this Vector2[] positions, Mesh mesh)
        {
            if (positions == null)
                throw new ArgumentNullException(nameof(positions));
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            List<int> triangles = new List<int>();

            // Triangulate the loop of points around the collider's perimeter.
            LoopToTriangles();

            // Populate our mesh with the resulting geometry.
            Vector3[] vertices = new Vector3[positions.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = positions[i];

            // We want to make sure we never assign fewer verts than we're indexing.
            if (vertices.Length <= mesh.vertexCount)
            {
                mesh.triangles = triangles.ToArray();
                mesh.vertices = vertices;
                mesh.uv = positions;
            }
            else
            {
                mesh.vertices = vertices;
                mesh.uv = positions;
                mesh.triangles = triangles.ToArray();
            }

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            void LoopToTriangles()
            {
                // This uses a naive O(n^3) ear clipping approach for simplicity.
                // Higher-performance triangulation methods exist if you need to
                // do this at runtime or with high-vertex-count polygons, or
                // polygons with holes & self-intersections.
                triangles.Clear();

                // Mode switch for clockwise/counterclockwise paths.
                int winding = ComputeWinding(positions);

                List<Vector2> ring = new List<Vector2>(positions);
                List<int> indices = new List<int>(ring.Count);
                for (int i = 0; i < ring.Count; i++)
                    indices.Add(i);

                while (indices.Count > 3)
                {
                    int tip;
                    for (tip = 0; tip < indices.Count; tip++)
                        if (IsEar(ring, tip, winding))
                            break;

                    int count = indices.Count;
                    int cw = (tip + count + winding) % count;
                    int ccw = (tip + count - winding) % count;
                    triangles.Add(indices[cw]);
                    triangles.Add(indices[ccw]);
                    triangles.Add(indices[tip]);
                    ring.RemoveAt(tip);
                    indices.RemoveAt(tip);
                }

                if (winding < 0)
                {
                    triangles.Add(indices[2]);
                    triangles.Add(indices[1]);
                    triangles.Add(indices[0]);
                }
                else
                    triangles.AddRange(indices);
            }

            // Returns -1 for counter-clockwise, +1 for clockwise.
            int ComputeWinding(Vector2[] ring)
            {
                float windingSum = 0;
                Vector2 previous = ring[ring.Length - 1];
                for (int i = 0; i < ring.Length; i++)
                {
                    Vector2 next = ring[i];
                    windingSum += (next.x - previous.x) * (next.y + previous.y);
                    previous = next;
                }

                return windingSum > 0f ? 1 : -1;
            }

            // Checks if a given point forms an "ear" of the polygon.
            // (A convex protrusion with no other vertices inside it)
            bool IsEar(List<Vector2> ring, int tip, int winding)
            {
                int count = ring.Count;
                int cw = (tip + count + winding) % count;
                int ccw = (tip + count - winding) % count;
                Vector2 a = ring[cw];
                Vector2 b = ring[tip];
                Vector2 c = ring[ccw];

                Vector2 ab = b - a;
                Vector2 bc = c - b;
                Vector2 ca = a - c;

                // Early-out for concave vertices.
                if (DotPerp(ab, bc) < 0f)
                    return false;

                float abThresh = DotPerp(ab, a);
                float bcThresh = DotPerp(bc, b);
                float caThresh = DotPerp(ca, c);

                for (int i = (ccw + 1) % count; i != cw; i = (i + 1) % count)
                {
                    Vector2 test = ring[i];
                    if (DotPerp(ab, test) > abThresh && DotPerp(bc, test) > bcThresh && DotPerp(ca, test) > caThresh)
                        return false;
                }

                return true;
            }

            // Dot product of the perpendicular of vector a against vector b.
            float DotPerp(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;
        }
        public static void ToFilledMesh(this Vector3[] positions, Mesh mesh)
        {
            Vector2[] positions2D = new Vector2[positions.Length];
            for (int i = 0; i < positions.Length; i++)
                positions2D[i] = positions[i];

            ToFilledMesh(positions2D, mesh);
        }
        public static void PositionToFilledMesh(this LineRenderer lineRenderer, Mesh mesh)
        {
            if (lineRenderer == null)
                throw new ArgumentNullException(nameof(lineRenderer));
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            Vector2[] positions = new Vector2[lineRenderer.positionCount];
            for (int i = 0; i < positions.Length; i++)
                positions[i] = lineRenderer.GetPosition(i);

            positions.ToFilledMesh(mesh);
        }
        public static void PathToMesh(this PolygonCollider2D collider, Mesh mesh)
        {
            if (collider == null)
                throw new ArgumentNullException(nameof(collider));
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            // For simplicity, we'll only handle colliders made of a single path.
            // This method can be extended to handle multi-part colliders and
            // colliders with holes, but triangulating these gets more complex.
            collider.GetPath(0).ToFilledMesh(mesh);
        }

        public static void DrawRegularPolygon(this LineRenderer lineRenderer, float sides, float radius, float width)
        {
            if (lineRenderer == null)
                throw new ArgumentNullException(nameof(lineRenderer));


            sides = sides.Clamp(1);
            radius = radius.Clamp(0);
            width = width.Clamp(0);


            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            lineRenderer.loop = false;



            const float tau = Mathf.PI * 2;
            int sidesInt = sides.CeilToInt();

            lineRenderer.positionCount = sidesInt;
            for (int i = 0; i < sidesInt; i++)
            {
                float radian = i / sides * tau;
                float x = radian.Sin() * radius;
                float y = radian.Cos() * radius;

                lineRenderer.SetPosition(i, new Vector3(x, y));
            }



            const int extraSteps = 2;
            lineRenderer.positionCount += extraSteps;

            int posCount = lineRenderer.positionCount;
            for (int i = 0; i < extraSteps; i++)
                lineRenderer.SetPosition(posCount - extraSteps + i, lineRenderer.GetPosition(i));
        }

        public static void PositionToPolygonCollider(this LineRenderer lineRenderer, PolygonCollider2D polygonCollider)
        {
            if (lineRenderer == null)
                throw new ArgumentNullException(nameof(lineRenderer));
            if (polygonCollider == null)
                throw new ArgumentNullException(nameof(polygonCollider));

            Vector2[] line = new Vector2[lineRenderer.positionCount];
            for (int i = 0; i < line.Length; i++)
                line[i] = lineRenderer.GetPosition(i);

            polygonCollider.pathCount = 1;
            polygonCollider.SetPath(0, line);
        }
        public static void PathToLineRenderer(this PolygonCollider2D polygonCollider, LineRenderer lineRenderer)
        {
            if (polygonCollider == null)
                throw new ArgumentNullException(nameof(polygonCollider));
            if (lineRenderer == null)
                throw new ArgumentNullException(nameof(lineRenderer));

            Vector2[] paths = polygonCollider.GetPath(0);
            for (int i = 0; i < paths.Length; i++)
                lineRenderer.SetPosition(i, paths[i]);
        }
    }
}