using UnityEngine;

namespace SCKRM.Polygon
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("SC KRM/Polygon/Line Renderer To Filled Mesh")]
    [WikiDescription("라인 렌더러를 채워진 메쉬로 변환하는 클래스 입니다")]
    public sealed class LineRendererToFilledMesh : MonoBehaviour
    {
        LineRenderer _lineRenderer;
        public LineRenderer lineRenderer => _lineRenderer = this.GetComponentFieldSave(_lineRenderer);

        MeshFilter _meshFilter;
        public MeshFilter meshFilter => _meshFilter = this.GetComponentFieldSave(_meshFilter);



        void Awake() => Initialize();

        void Update()
        {
            if (meshFilter.sharedMesh == null)
                Initialize();

            lineRenderer.PositionToFilledMesh(meshFilter.sharedMesh);
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