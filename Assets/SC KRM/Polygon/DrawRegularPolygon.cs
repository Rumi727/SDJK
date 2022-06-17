using UnityEngine;

namespace SCKRM.Polygon
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("SC KRM/Polygon/Draw Regular Polygon")]
    public sealed class DrawRegularPolygon : MonoBehaviour
    {
        LineRenderer _lineRenderer;
        public LineRenderer lineRenderer => _lineRenderer = this.GetComponentFieldSave(_lineRenderer);



        [Min(1), SerializeField] float _sides = 5;
        [Min(0), SerializeField] float _radius = 3;
        [Min(0), SerializeField] float _width = 1;
        public float sides { get => _sides; set => _sides = value; }
        public float radius { get => _radius; set => _radius = value; }
        public float width { get => _width; set => _width = value; }



        [System.NonSerialized] float tempSides = 0;
        [System.NonSerialized] float tempRadius = 0;
        [System.NonSerialized] float tempWidth = 0;
        void Update()
        {
            if (tempSides != sides || tempRadius != radius || tempWidth != width)
                lineRenderer.DrawRegularPolygon(sides, radius, width);
        }
    }
}