using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Ruleset.SuperHexagon.GameOver;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class Wall : ObjectPoolingBase
    {
        [SerializeField] WallRenderer _wallRenderer; public WallRenderer wallRenderer => _wallRenderer;

        public Field field { get; private set; }
        public Player player { get; private set; }

        public int index { get; private set; }
        public SuperHexagonNoteFile note { get; private set; }

        public void Refresh(Field field, Player player, int index, SuperHexagonNoteFile note)
        {
            this.field = field;
            this.player = player;

            this.index = index;
            this.note = note;
        }

        bool missLock = false;
        bool holdLock = false;
        public void CrashVerdict()
        {
            float sidesAngle = (float)(360d / field.sides);
            float playerWallRotation = (float)((index * sidesAngle) + (player.rotation - ((int)player.index * sidesAngle)));

            WallVector2 wallVector2 = wallRenderer.wallVector2;
            if (!missLock)
            {
                PlayerVector2 playerVector2 = RenderUtility.GetPlayerColliderPos(player.player.distance, playerWallRotation, player.player.width);
                if (LineIntersection(wallVector2.leftBottom, wallVector2.rightBottom, playerVector2.bottom * -10000, playerVector2.top, out _))
                {
                    if (index == (int)player.index)
                        field.judgementManager.Miss(RhythmManager.currentBeatScreen);

                    missLock = true;
                }
            }

            if (missLock && !holdLock)
            {
                if (index == (int)player.index)
                {
                    if (player.index.Repeat(1) >= 0.5f)
                        player.rotation = (((int)player.index + 1) * sidesAngle) + 0.0001f;
                    else
                        player.rotation = ((int)player.index * sidesAngle) - 0.0001f;
                }

                PlayerVector2 playerVector2 = RenderUtility.GetPlayerColliderPos(player.player.distance, playerWallRotation, player.player.width);
                if (LineIntersection(wallVector2.leftTop, wallVector2.rightTop, playerVector2.bottom * -10000, playerVector2.top, out _))
                    holdLock = true;
            }
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            wallRenderer.distance = 0;
            wallRenderer.width = 1;
            wallRenderer.color = Color.white;
            wallRenderer.sides = 6;
            wallRenderer.min = 1;
            wallRenderer.index = 0;

            return true;
        }

        static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
        {
            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num, offset;
            float x1lo, x1hi, y1lo, y1hi;

            Ax = p2.x - p1.x;
            Bx = p3.x - p4.x;

            // X bound box test/
            if (Ax < 0)
            {
                x1lo = p2.x;
                x1hi = p1.x;
            }
            else
            {
                x1hi = p2.x;
                x1lo = p1.x;
            }

            if (Bx > 0)
            {
                if (x1hi < p4.x || p3.x < x1lo)
                {
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else if (x1hi < p3.x || p4.x < x1lo)
            {
                intersection = Vector2.zero;
                return false;
            }

            Ay = p2.y - p1.y;
            By = p3.y - p4.y;

            // Y bound box test//
            if (Ay < 0)
            {
                y1lo = p2.y;
                y1hi = p1.y;
            }
            else
            {
                y1hi = p2.y;
                y1lo = p1.y;
            }

            if (By > 0)
            {
                if (y1hi < p4.y || p3.y < y1lo)
                {
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else if (y1hi < p3.y || p4.y < y1lo)
            {
                intersection = Vector2.zero;
                return false;
            }

            Cx = p1.x - p3.x;
            Cy = p1.y - p3.y;
            d = By * Cx - Bx * Cy;  // alpha numerator//
            f = Ay * Bx - Ax * By;  // both denominator//

            // alpha tests//
            if (f > 0)
            {
                if (d < 0 || d > f)
                {
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else if (d > 0 || d < f)
            {
                intersection = Vector2.zero;
                return false;
            }

            e = Ax * Cy - Ay * Cx;  // beta numerator//

            // beta tests //
            if (f > 0)
            {
                if (e < 0 || e > f)
                {
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else if (e > 0 || e < f)
            {
                intersection = Vector2.zero;
                return false;
            }

            // check if they are parallel
            if (f == 0)
            {
                intersection = Vector2.zero;
                return false;
            }

            // compute intersection coordinates //
            num = d * Ax; // numerator //
            offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;   // round direction //
            intersection.x = p1.x + (num + offset) / f;

            num = d * Ay;
            offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;
            intersection.y = p1.y + (num + offset) / f;

            return true;

            static bool same_sign(float a, float b) => ((a * b) >= 0f);
        }
    }
}
