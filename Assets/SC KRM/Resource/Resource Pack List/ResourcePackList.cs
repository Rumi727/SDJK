using Newtonsoft.Json;
using SCKRM.Object;
using SCKRM.UI.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SCKRM.Resource.UI
{
    [WikiDescription("리소스팩 리스트를 표시하기 위한 클래스 입니다")]
    [AddComponentMenu("SC KRM/Resource/Resource Pack List/Resource Pack List")]
    public sealed class ResourcePackList : MonoBehaviour
    {
        public static bool isResourcePackListChanged = false;


        [SerializeField] VerticalLayout _selectedResourcePacksContentLayout; public VerticalLayout selectedResourcePacksContentLayout => _selectedResourcePacksContentLayout;
        [SerializeField] VerticalLayout _availableResourcePacksContentLayout; public VerticalLayout availableResourcePacksContentLayout => _availableResourcePacksContentLayout;

        [SerializeField] RectTransform _selectedResourcePacks; public RectTransform selectedResourcePacks => _selectedResourcePacks;
        [SerializeField] RectTransform _availableResourcePacks; public RectTransform availableResourcePacks => _availableResourcePacks;

        [SerializeField] RectTransform _selectedResourcePacksContent; public RectTransform selectedResourcePacksContent => _selectedResourcePacksContent;
        [SerializeField] RectTransform _availableResourcePacksContent; public RectTransform availableResourcePacksContent => _availableResourcePacksContent;

        void OnDisable() => ChildRemove();

        void OnApplicationFocus(bool focus)
        {
            if (InitialLoadManager.isInitialLoadEnd)
                Refresh();
        }

        public void ChildRemove()
        {
            ResourcePack[] resourcePacks = GetComponentsInChildren<ResourcePack>(true);
            for (int i = 0; i < resourcePacks.Length; i++)
                resourcePacks[i].Remove();

            createdResourcePacks.Clear();
        }

        List<ResourcePack> createdResourcePacks = new List<ResourcePack>();
        public void Refresh()
        {
            ChildRemove();

            ResourcePackLoad(ResourceManager.SaveData.resourcePacks.ToArray(), _selectedResourcePacksContent, false);
            ResourcePackLoad(Directory.GetDirectories(Kernel.resourcePackPath), _availableResourcePacksContent, true);

            selectedResourcePacksContentLayout.LayoutRefresh();
            availableResourcePacksContentLayout.LayoutRefresh();
            selectedResourcePacksContentLayout.SizeUpdate(false);
            availableResourcePacksContentLayout.SizeUpdate(false);

            void ResourcePackLoad(string[] resourcePackPaths, Transform transform, bool available)
            {
                for (int i = 0; i < resourcePackPaths.Length; i++)
                {
                    string resourcePackPath = resourcePackPaths[i].Replace("\\", "/");
                    if (available && ResourceManager.SaveData.resourcePacks.Contains(resourcePackPath))
                        continue;

                    string jsonPath = PathTool.Combine(resourcePackPath, "pack.json");
                    if (File.Exists(jsonPath))
                    {
                        try
                        {
                            ResourcePackJson resourcePackJson = JsonConvert.DeserializeObject<ResourcePackJson>(File.ReadAllText(jsonPath));
                            if (resourcePackJson != null)
                            {
                                Texture2D texture = ResourceManager.GetTexture(PathTool.Combine(resourcePackPath, "pack"), false, new TextureMetaData() { filterMode = FilterMode.Bilinear });
                                Sprite sprite = ResourceManager.GetSprite(texture);
                                ResourcePack resourcePack = (ResourcePack)ObjectPoolingSystem.ObjectCreate("resource_pack_list.resource_pack", transform).monoBehaviour;
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                                resourcePack.resourcePackList = this;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

                                createdResourcePacks.Add(resourcePack);

                                resourcePack.nameText.text = resourcePackJson.name.ConstEnvironmentVariable();
                                resourcePack.descriptionText.text = resourcePackJson.description.ConstEnvironmentVariable();

                                resourcePack.resourcePackPath = resourcePackPath;

                                resourcePack.selected = !available;

                                if (sprite != null)
                                {
                                    resourcePack.icon.gameObject.SetActive(true);
                                    resourcePack.icon.sprite = sprite;

                                    resourcePack.childSizeFitter.min = 70;
                                    resourcePack.verticalLayout.padding.left = 70;
                                }
                                else
                                {
                                    resourcePack.childSizeFitter.min = 40;
                                    resourcePack.verticalLayout.padding.left = 10;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }

        public static void HideEvent()
        {
            if (isResourcePackListChanged)
            {
                Kernel.AllRefresh().Forget();
                isResourcePackListChanged = false;
            }
        }

        class ResourcePackJson
        {
            public string name = "";
            public string description = "";
        }
    }
}
