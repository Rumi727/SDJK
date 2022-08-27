using SCKRM.UI;
using SCKRM.UI.Layout;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCKRM.Resource.UI
{
    [WikiDescription("리소스팩 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Resource/Resource Pack List/Resource Pack")]
    public sealed class ResourcePack : UIObjectPooling, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ResourcePackList resourcePackList { get; [Obsolete("It is managed by the ResourcePackList class. Please do not touch it.")] internal set; }

        public string resourcePackPath { get; set; } = "";
        public bool selected = false;

        [SerializeField, HideInInspector] VerticalLayout _verticalLayout; public VerticalLayout verticalLayout => _verticalLayout = this.GetComponentFieldSave(_verticalLayout);
        [SerializeField, HideInInspector] ChildSizeFitter _childSizeFitter; public ChildSizeFitter childSizeFitter => _childSizeFitter = this.GetComponentFieldSave(_childSizeFitter);



        [SerializeField] Image _icon; public Image icon => _icon;
        [SerializeField] TMP_Text _nameText; public TMP_Text nameText => _nameText;

        [SerializeField] TMP_Text _descriptionText; public TMP_Text descriptionText => _descriptionText;

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            if (icon.gameObject.activeSelf)
            {
                icon.gameObject.SetActive(false);
                Destroy(icon.sprite);
                icon.sprite = null;
            }

            nameText.text = "";
            descriptionText.text = "";
            childSizeFitter.min = 40;
            verticalLayout.padding.left = 10;

            return true;
        }

        Vector2 posOffset = Vector2.zero;
        RectTransform[] selectedChildRectTransforms;
        List<float> selectedChildYPosList = new List<float>();
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            //기본 리소스 팩 일 경우 무시
            if (Kernel.streamingAssetsPath == resourcePackPath)
                return;

            posOffset = (eventData.position / UIManager.currentGuiSize) - rectTransform.anchoredPosition;

            transform.SetParent(transform.parent.parent);

            selectedChildRectTransforms = new RectTransform[resourcePackList.selectedResourcePacksContent.childCount];
            for (int i = 0; i < selectedChildRectTransforms.Length; i++)
                selectedChildRectTransforms[i] = (RectTransform)resourcePackList.selectedResourcePacksContent.GetChild(i);

            if (selected)
                resourcePackList.selectedResourcePacks.SetAsLastSibling();
            else
                resourcePackList.availableResourcePacks.SetAsLastSibling();
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            //기본 리소스 팩 일 경우 무시
            if (Kernel.streamingAssetsPath == resourcePackPath)
                return;

            rectTransform.anchoredPosition = (eventData.position / UIManager.currentGuiSize) - posOffset;

            selectedChildYPosList.Clear();
            for (int i = 0; i < selectedChildRectTransforms.Length; i++)
                selectedChildYPosList.Add(selectedChildRectTransforms[i].anchoredPosition.y);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            //기본 리소스 팩 일 경우 무시
            if (Kernel.streamingAssetsPath == resourcePackPath)
                return;

            int index = 0;
            if (selected)
                index = ResourceManager.SaveData.resourcePacks.IndexOf(resourcePackPath);

            //드래그 하는 오브젝트가 선택 할 수 있는 오브젝트일 경우
            if (!selected)
            {
                //오브젝트를 왼쪽으로 끌었을때
                if (rectTransform.anchoredPosition.x <= -115)
                {
                    transform.SetParent(resourcePackList.selectedResourcePacksContent);
                    selected = true;

                    index = selectedChildYPosList.CloseValueIndex(rectTransform.anchoredPosition.y);
                    ResourceManager.SaveData.resourcePacks.Insert(index, resourcePackPath);

                    ResourcePackList.isResourcePackListChanged = true;
                }
                else
                    transform.SetParent(resourcePackList.availableResourcePacksContent);

            }
            //드래그 하는 오브젝트가 선택 된 오브젝트일 경우
            else if (selected)
            {
                //오브젝트를 오른쪽으로 끌었을때
                if (rectTransform.anchoredPosition.x >= 110)
                {
                    transform.SetParent(resourcePackList.availableResourcePacksContent);
                    selected = false;

                    ResourceManager.SaveData.resourcePacks.Remove(resourcePackPath);
                    ResourcePackList.isResourcePackListChanged = true;
                }
                else
                {
                    transform.SetParent(resourcePackList.selectedResourcePacksContent);

                    int oldIndex = index;
                    index = selectedChildYPosList.CloseValueIndex(rectTransform.anchoredPosition.y);
                    ResourceManager.SaveData.resourcePacks.Move(oldIndex, index);

                    if (oldIndex != index)
                        ResourcePackList.isResourcePackListChanged = true;
                }
            }

            transform.SetSiblingIndex(index);
        }
    }
}
