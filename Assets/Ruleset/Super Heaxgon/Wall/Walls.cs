using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Ruleset.SuperHexagon.Renderer;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class Walls : MonoBehaviour
    {
        [SerializeField] GLRenderInvoker _glRenderInvoker; public GLRenderInvoker glRenderInvoker => _glRenderInvoker;

        [SerializeField] string _wallPrefab = "ruleset.super_hexagon.wall"; public string wallPrefab => _wallPrefab;
        [SerializeField] Field _field; public Field field => _field;

        [SerializeField] Player _player; public Player player => _player;

        public SuperHexagonMapFile map => field.map;

        List<Wall> walls = new List<Wall>();
        void Update()
        {
            if (!RhythmManager.isPlaying || map == null)
                return;

            double currentBeat = RhythmManager.currentBeatScreen;
            float sides = (float)field.sides;
            float zoom = (float)field.zoom;
            Color mainColor = field.mainColor;
            double globalNoteDistance = map.effect.globalNoteDistance.GetValue(currentBeat);

            for (int i = 0; i < walls.Count; i++)
            {
                Wall wall = walls[i];
                if (wall == null || wall.isRemoved)
                {
                    walls.RemoveAt(i);
                    i--;

                    continue;
                }

                double noteSpeed = globalNoteDistance * map.effect.globalNoteSpeed.GetValue(wall.note.beat);

                WallRenderer wallRenderer = wall.wallRenderer;
                float distance = (float)(zoom + ((wall.note.beat - currentBeat) * noteSpeed));
                float width;

                wallRenderer.distance = distance;
                if (wall.note.holdLength > 0)
                    width = (float)(wall.note.holdLength * noteSpeed);
                else
                    width = 1;

                wallRenderer.width = width;

                wallRenderer.sides = sides;
                wallRenderer.min = zoom;
                wallRenderer.color = mainColor;

                if (distance <= 50)
                    wall.CrashVerdict();

                if (distance + width < 0)
                {
                    wall.Remove();
                    walls.RemoveAt(i);
                    i--;

                    continue;
                }
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < map.notes.Count; i++)
            {
                List<SuperHexagonNoteFile> notes = map.notes[i];
                for (int j = 0; j < notes.Count; j++)
                {
                    SuperHexagonNoteFile note = notes[j];
                    Wall wall = (Wall)ObjectPoolingSystem.ObjectCreate(wallPrefab, transform).monoBehaviour;
                    wall.Refresh(field, player, i, note);
                    walls.Add(wall);

                    wall.wallRenderer.index = i;
                    glRenderInvoker.wallRenderers.Add(wall.wallRenderer);
                }
            }
        }
    }
}
