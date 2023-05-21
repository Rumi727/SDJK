using SCKRM;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class RankUI : SuperHexagonUIBase
    {
        [SerializeField, FieldNotNull] SuperHexagonManager manager;
        [SerializeField, FieldNotNull] TMP_Text text;
        [SerializeField] float startX = -130;

        RankMetaData? lastRankMetaData;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            RankMetaData rank = manager.ruleset.GetRank(judgementManager.rankProgress);

            text.text = rank.name;
            text.color = rank.color;

            parentRectTransform.anchoredPosition = parentRectTransform.anchoredPosition.MoveTowards(new Vector2(startX, parentRectTransform.anchoredPosition.y), 8 * Kernel.fpsDeltaTime);

            if (lastRankMetaData != null && lastRankMetaData != rank)
            {
                parentRectTransform.anchoredPosition = new Vector2(startX + 64, parentRectTransform.anchoredPosition.y);

                switch (rank.name)
                {
                    case "LINE":
                        SoundManager.PlaySound("ruleset.super_hexagon.line", "sdjk");
                        break;
                    case "TRIANGLE":
                        SoundManager.PlaySound("ruleset.super_hexagon.triangle", "sdjk");
                        break;
                    case "SQUARE":
                        SoundManager.PlaySound("ruleset.super_hexagon.square", "sdjk");
                        break;
                    case "PENTAGON":
                        SoundManager.PlaySound("ruleset.super_hexagon.pentagon", "sdjk");
                        break;
                    case "HEXAGON":
                        SoundManager.PlaySound("ruleset.super_hexagon.hexagon", "sdjk");
                        break;
                }

                SoundManager.PlaySound("ruleset.super_hexagon.rankup", "sdjk");
            }

            lastRankMetaData = rank;
        }
    }
}
