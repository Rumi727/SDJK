using SCKRM;
using SCKRM.Sound;
using SCKRM.UI;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class RankUI : SuperHexagonUIBase
    {
        [SerializeField, NotNull] SuperHexagonManager manager;
        [SerializeField, NotNull] TMP_Text text;
        [SerializeField] float startX = -130;

        RankMetaData? lastRankMetaData;
        void Update()
        {
            RankMetaData rank = manager.ruleset.GetRank(judgementManager.rankProgress);

            text.text = rank.name;
            text.color = rank.color;

            parentRectTransform.anchoredPosition = parentRectTransform.anchoredPosition.MoveTowards(new Vector2(startX, parentRectTransform.anchoredPosition.y), 8 * Kernel.fpsDeltaTime);

            if (lastRankMetaData != null && lastRankMetaData != rank)
            {
                parentRectTransform.anchoredPosition = new Vector2(startX + 64, parentRectTransform.anchoredPosition.y);

                switch (rank.name)
                {
                    case "line":
                        SoundManager.PlaySound("ruleset.super_hexagon.line", "sdjk");
                        break;
                    case "triangle":
                        SoundManager.PlaySound("ruleset.super_hexagon.triangle", "sdjk");
                        break;
                    case "square":
                        SoundManager.PlaySound("ruleset.super_hexagon.square", "sdjk");
                        break;
                    case "pentagon":
                        SoundManager.PlaySound("ruleset.super_hexagon.pentagon", "sdjk");
                        break;
                    case "hexagon":
                        SoundManager.PlaySound("ruleset.super_hexagon.", "sdjk");
                        break;
                }
            }

            lastRankMetaData = rank;
        }
    }
}
