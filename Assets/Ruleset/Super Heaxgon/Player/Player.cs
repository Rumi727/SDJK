using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SCKRM.UI;
using SDJK.Effect;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] Field _field; public Field field => _field;
        public SuperHexagonMapFile map => field.map;

        public double rotation
        {
            get => _rotation;
            set
            {
                _rotation = value.Repeat(360);
                _index = _rotation / (360f / field.sides);

                player.rotation = (float)rotation;
            }
        }
        double _rotation = 0;

        public double index
        {
            get => _index;
            set
            {
                _index = value.Repeat(field.sides);
                _rotation = _index * (360f / field.sides);

                player.rotation = (float)rotation;
            }
        }
        double _index = 0;

        [SerializeField] RegularPolygonRenderer _playerBackground; public RegularPolygonRenderer playerBackground => _playerBackground;
        [SerializeField] RegularPolygonRenderer _playerBorder; public RegularPolygonRenderer playerBorder => _playerBorder;
        [SerializeField] PlayerRenderer _player; public PlayerRenderer player => _player;

        void Update()
        {
            if (!RhythmManager.isPlaying || map == null)
                return;

            playerBackground.sides = (float)field.sides;
            playerBorder.sides = (float)field.sides;

            playerBackground.width = (float)(field.zoom - 0.15);
            playerBorder.distance = (float)(field.zoom - 0.15);
            player.distance = (float)(field.zoom + 0.5);

            if (!field.manager.gameOverManager.isGameOver && !field.manager.isPaused && Kernel.gameSpeed != 0)
            {
                double speed = map.playerSpeed.GetValue(RhythmManager.currentBeatScreen) * Kernel.fpsSmoothDeltaTime;
                if (InputManager.GetKey("ruleset.super_hexagon.left", InputType.Alway))
                    rotation -= speed;
                if (InputManager.GetKey("ruleset.super_hexagon.right", InputType.Alway))
                    rotation += speed;
            }
        }
    }
}
