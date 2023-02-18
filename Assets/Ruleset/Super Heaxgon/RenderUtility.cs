using SCKRM;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    public static class RenderUtility
    {
        public static void PlayerGLRender(this PlayerRenderer renderer)
        {
            Material renderMaterial = Kernel.coloredMaterial;
            GL.PushMatrix();

            if (renderMaterial.SetPass(0))
            {
                GL.MultMatrix(renderer.transform.localToWorldMatrix);

                GL.Begin(GL.TRIANGLES);
                GL.Color(renderer.color);

                GL.Vertex(renderer.GetPlayerRenderPos(new Vector2(-0.5f, -0.5f) * renderer.width));
                GL.Vertex(renderer.GetPlayerRenderPos(new Vector2(0.5f, -0.5f) * renderer.width));
                GL.Vertex(renderer.GetPlayerRenderPos(new Vector2(0, 0.5f) * renderer.width));

                GL.End();
            }

            GL.PopMatrix();
        }

        public static PlayerVector2 GetPlayerColliderPos(this PlayerRenderer renderer)
        {
            Vector2 top = renderer.GetPlayerRenderPos(new Vector2(0, -0.5f) * renderer.width);
            Vector2 bottom = renderer.GetPlayerRenderPos(new Vector2(0, 0.5f) * renderer.width);

            return new PlayerVector2(top, bottom);
        }

        static Vector2 GetPlayerRenderPos(this PlayerRenderer renderer, Vector2 pos)
        {
            float degRotation = renderer.rotation * MathUtility.deg2Rad;
            float sin = degRotation.Sin();
            float cos = degRotation.Cos();

            float x = (pos.y * sin) - (pos.x * cos);
            float y = (pos.x * sin) + (pos.y * cos);

            x += renderer.distance * sin;
            y += renderer.distance * cos;

            return new Vector2(x, y);
        }

        public static void RegularPolygonGLRender(this RegularPolygonRenderer renderer)
        {
            for (int i = 0; i < renderer.sides.CeilToInt(); i++)
                WallGLRender(renderer.transform, renderer.color, GetWallVector2(i, renderer.distance, renderer.width, renderer.sides, renderer.min));
        }

        public static void WallGLRender(this WallRenderer renderer) => WallGLRender(renderer.transform, renderer.color, GetWallVector2(renderer));

        public static void WallGLRender(this Transform transform, Color color, WallVector2 wallVector2)
        {
            Material renderMaterial = Kernel.coloredMaterial;
            GL.PushMatrix();

            if (renderMaterial.SetPass(0))
            {
                GL.MultMatrix(transform.localToWorldMatrix);
                
                GL.Begin(GL.QUADS);
                GL.Color(color);

                GL.Vertex(wallVector2.leftBottom);
                GL.Vertex(wallVector2.rightBottom);
                GL.Vertex(wallVector2.rightTop);
                GL.Vertex(wallVector2.leftTop);

                GL.End();
            }

            GL.PopMatrix();
        }

        public static WallVector2 GetWallVector2(this WallRenderer renderer) => GetWallVector2(renderer.index, renderer.distance, renderer.width, renderer.sides, renderer.min);

        public static WallVector2 GetWallVector2(int index, float distance, float width, float sides, float min)
        {
            float distanceClamp = distance.Clamp(min);
            float distanceClampWidth = (distance + width.Clamp(0)).Clamp(min);

            WallVector2 wallVector = new WallVector2();
            wallVector.leftBottom = GetWallGLRenderPos(sides, index, distanceClamp);
            wallVector.rightBottom = GetWallGLRenderPos(sides, index + 1, distanceClamp);
            wallVector.rightTop = GetWallGLRenderPos(sides, index + 1, distanceClampWidth);
            wallVector.leftTop = GetWallGLRenderPos(sides, index, distanceClampWidth);

            return wallVector;
        }

        static Vector2 GetWallGLRenderPos(float sides, int index, float dis)
        {
            const float tau = MathUtility.pi * 2;

            index = index.Clamp(0);

            float radian = (index - (index - sides).Clamp(0)) / sides * tau;
            float x = Mathf.Sin(radian) * dis;
            float y = Mathf.Cos(radian) * dis;

            return new Vector2(x, y);
        }
    }
}