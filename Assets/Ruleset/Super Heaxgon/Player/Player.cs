using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
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
                _rotation = value;
                Refresh();
            }
        }
        double _rotation = 0;

        public double index
        {
            get => _index;
            set
            {
                _rotation = index * (360f / field.sides);
                Refresh();
            }
        }
        double _index = 0;

        [SerializeField] RegularPolygonRenderer _playerBackground; public RegularPolygonRenderer playerBackground => _playerBackground;
        [SerializeField] RegularPolygonRenderer _playerBorder; public RegularPolygonRenderer playerBorder => _playerBorder;
        [SerializeField] PlayerRenderer _player; public PlayerRenderer player => _player;

        void Refresh()
        {
            _index = rotation / (360f / field.sides);
            player.rotation = (float)rotation;
        }

        void Update()
        {
            if (!RhythmManager.isPlaying || map == null)
                return;

            playerBackground.sides = (float)field.sides;
            playerBorder.sides = (float)field.sides;

            playerBackground.width = (float)(field.zoom - 0.15);
            playerBorder.distance = (float)(field.zoom - 0.15);
            player.distance = (float)(field.zoom + 0.5);

            double speed = map.playerSpeed.GetValue(RhythmManager.currentBeatScreen) * Kernel.fpsSmoothDeltaTime;
            if (InputManager.GetKey("ruleset.super_hexagon.left", InputType.Alway))
                rotation -= speed;
            if (InputManager.GetKey("ruleset.super_hexagon.right", InputType.Alway))
                rotation += speed;
        }
    }
}
