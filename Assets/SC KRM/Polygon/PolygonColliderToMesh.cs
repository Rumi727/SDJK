using UnityEngine;

namespace SCKRM.Polygon
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(PolygonCollider2D))]
    [AddComponentMenu("SC KRM/Polygon/Polygon Collider To Mesh")]
    public sealed class PolygonColliderToMesh : MonoBehaviour
    {
        PolygonCollider2D _polygonCollider;
        public PolygonCollider2D polygonCollider => _polygonCollider = this.GetComponentFieldSave(_polygonCollider);

        MeshFilter _meshFilter;
        public MeshFilter meshFilter => _meshFilter = this.GetComponentFieldSave(_meshFilter);



        void Update()
        {
            if (meshFilter.sharedMesh == null)
                Initialize();

            polygonCollider.PathToMesh(meshFilter.sharedMesh);
        }



        public void Initialize()
        {
            // This creates a unique mesh per instance. If you re-use shapes
            // frequently, then you may want to look into sharing them in a pool.
            Mesh mesh = new Mesh();
            mesh.MarkDynamic();

            meshFilter.sharedMesh = mesh;
        }
    }
}