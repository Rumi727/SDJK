using K4.Threading;
using SCKRM.Resource;
using SCKRM.Threads;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Renderer
{
    [WikiDescription("선택 가능한 오브젝트 렌더러")]
    [AddComponentMenu("SC KRM/Renderer/UI/Selectable")]
    [RequireComponent(typeof(RequireComponent))]
    public sealed class CustomSelectableRenderer : CustomImageRenderer
    {
        [SerializeField, HideInInspector] Selectable _selectable; public Selectable selectable => _selectable = this.GetComponentFieldSave(_selectable);



        [SerializeField] SpriteStateData _highlightedSprite;
        [SerializeField] SpriteStateData _pressedSprite;
        [SerializeField] SpriteStateData _selectedSprite;
        [SerializeField] SpriteStateData _disabledSprite;

        public SpriteStateData highlightedSprite { get => _highlightedSprite; set => _highlightedSprite = value; }
        public SpriteStateData pressedSprite { get => _pressedSprite; set => _pressedSprite = value; }
        public SpriteStateData selectedSprite { get => _selectedSprite; set => _selectedSprite = value; }
        public SpriteStateData disabledSprite { get => _disabledSprite; set => _disabledSprite = value; }

        [WikiDescription("새로고침")]
        public override void Refresh()
        {
            base.Refresh();

            SpriteState spriteState = new SpriteState();
            spriteState.highlightedSprite = GetSprite(highlightedSprite.type, highlightedSprite.name, highlightedSprite.index, highlightedSprite.nameSpace, highlightedSprite.tag);
            spriteState.pressedSprite = GetSprite(pressedSprite.type, pressedSprite.name, pressedSprite.index, pressedSprite.nameSpace, pressedSprite.tag);
            spriteState.selectedSprite = GetSprite(selectedSprite.type, selectedSprite.name, selectedSprite.index, selectedSprite.nameSpace, selectedSprite.tag);
            spriteState.disabledSprite = GetSprite(disabledSprite.type, disabledSprite.name, disabledSprite.index, disabledSprite.nameSpace, disabledSprite.tag);

            if (ThreadManager.isMainThread)
                selectable.spriteState = spriteState;
            else
                K4UnityThreadDispatcher.Execute(() => selectable.spriteState = spriteState);
        }

        [System.Serializable]
        public class SpriteStateData
        {
            [SerializeField] string _nameSpace = "";
            public string nameSpace { get => _nameSpace; set => _nameSpace = value; }

            [SerializeField] string _type = "";
            public string type { get => _type; set => _type = value; }

            [SerializeField] string _name = "";
            public string name { get => _name; set => _name = value; }

            [SerializeField, Min(0)] int _index = 0;
            public int index { get => _index; set => _index = value; }

            [SerializeField] string _tag = ResourceManager.spriteDefaultTag;
            public string tag { get => _tag; set => _tag = value; }
        }
    }
}