using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Map;
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
            Color mainColorAlt = field.mainColorAlt;
            double globalNoteDistance = map.effect.globalNoteDistance.GetValue(currentBeat);

            for (int i = 0; i < walls.Count; i++)
            {
                Wall wall = walls[i];
                if (wall == null || wall.isRemoved)
                {
                    glRenderInvoker.wallRenderers.RemoveAt(i);
                    walls.RemoveAt(i);
                    i--;

                    continue;
                }

                SuperHexagonBarEffectFile barEffect = null;
                NoteConfigFile config = new NoteConfigFile();
                if (wall.index < map.effect.barEffect.Count)
                {
                    barEffect = map.effect.barEffect[wall.index];
                    config = barEffect.noteConfig.GetValue(wall.note.beat);
                }

                if (barEffect == null)
                    continue;

                double localNoteSpeed = barEffect.noteDistance.GetValue(currentBeat) * map.effect.globalNoteSpeed.GetValue(wall.note.beat) * config.noteSpeed.GetValue(wall.note.beat);
                double noteSpeed = globalNoteDistance * localNoteSpeed;

                WallRenderer wallRenderer = wall.wallRenderer;
                float distance = (float)(zoom + ((wall.note.beat - currentBeat) * noteSpeed));
                float width;

                wallRenderer.distance = distance + field.globalWallOffset;
                if (wall.note.holdLength > 0)
                    width = (float)(wall.note.holdLength * noteSpeed);
                else
                    width = 1;

                wallRenderer.width = width;

                wallRenderer.sides = sides;
                wallRenderer.min = zoom;

                if (field.isMainColorAltReversal)
                {
                    wallRenderer.color = mainColor;
                    wallRenderer.colorAlt = mainColorAlt;
                }
                else
                {
                    wallRenderer.color = mainColorAlt;
                    wallRenderer.colorAlt = mainColor;
                }

                if (distance <= 50 && !field.manager.gameOverManager.isGameOver)
                    wall.CrashVerdict();

                if (distance + width < 0 && field.manager.gameOverManager.isGameOver)
                {
                    wall.Remove();
                    glRenderInvoker.wallRenderers.RemoveAt(i);
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
                TypeList<SuperHexagonNoteFile> notes = map.notes[i];
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
