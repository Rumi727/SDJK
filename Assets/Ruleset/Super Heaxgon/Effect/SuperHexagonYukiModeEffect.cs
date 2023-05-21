using SCKRM;
using SDJK.Effect;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public class SuperHexagonYukiModeEffect : YukiModeEffect
    {
        [SerializeField, FieldNotNull] GLRenderInvoker _glRenderInvoker; public GLRenderInvoker glRenderInvoker => _glRenderInvoker;

        [SerializeField, FieldNotNull] Field _field; public Field field => _field;
        [SerializeField, FieldNotNull] RegularPolygonRenderer _regularPolygonRenderer; public RegularPolygonRenderer regularPolygonRenderer => _regularPolygonRenderer;
    }
}
