using ExtendedNumerics;
using SCKRM;
using SCKRM.Cursor;
using SCKRM.Editor;
using SCKRM.Json;
using SCKRM.Rhythm;
using SDJK.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SDJK.MapEditor
{
    public sealed class MapEffectEditorTimelineWindow
    {
        public MapEditorWindow editor => effectEditor.editor;
        public MapEffectEditorWindow effectEditor;
        public EditorGUISplitView splitView => effectEditor.splitView;

        public MapFile mapFile;

        public MapEffectEditorTimelineWindow(MapEffectEditorWindow effectEditor)
        {
            this.effectEditor = effectEditor;
            mapFile = editor.mapFile;
        }

        public struct PropertyInfoTypeListPair
        {
            public PropertyInfo info;
            public IBeatValuePairList list;

            public PropertyInfoTypeListPair(PropertyInfo info, IBeatValuePairList list)
            {
                this.info = info;
                this.list = list;
            }
        }

        public bool rawGUI = false;
        public float zoom = 10;
        public float minScroll = 0;
        public float maxScroll = 10;
        public float dragStartMinScroll = 0;
        public float dragStartMaxScroll = 10;
        public Vector2 verticalScroll = Vector2.zero;
        public Dictionary<int, Dictionary<int, double>> selectedItem = new();
        public Vector2 dragStartPos = Vector2.zero;
        public bool isPressing = false;
        public bool isDrag = false;
        public bool isBackgroundDrag = false;
        public bool isItemDrag = false;
        public bool lastIsDrag = false;
        public double itemDragStartBeat = 0;
        public bool isMouse2 = false;
        public bool isScrollDrag = false;
        public bool isBottomInput = false;
        public MapEffectTreeViewItem[] lastItems;
        public List<IBeatValuePair> clipboards = new List<IBeatValuePair>();
        public double createBeatOffset = 0;
        public bool initValueShow = false;
        public bool OnGUI(MapEffectTreeViewItem[] items)
        {
            List<PropertyInfoTypeListPair> lists = new List<PropertyInfoTypeListPair>();
            (List<(PropertyInfoTypeListPair listPair, IBeatValuePair pair, int pairIndex)> pairLists, Rect rect) overlays = (new(), Rect.zero);

            bool IgnoreRectCheck(Vector2 pos) => !overlays.rect.Contains(pos);

            if (items == null)
                return false;

            if (lastItems != items)
            {
                SelectedListClear();
                lastItems = items;
            }

            for (int i = 0; i < items.Length; i++)
            {
                MapEffectTreeViewItem item = items[i];
                if (typeof(IBeatValuePairList).IsAssignableFrom(item.propertyInfo.PropertyType))
                {
                    IBeatValuePairList list = (IBeatValuePairList)item.propertyInfo.GetValue(item.targetObject);
                    lists.Add(new PropertyInfoTypeListPair(item.propertyInfo, list));
                }
            }

            if (lists.Count <= 0 && items.Length <= 1)
                return false;

            Rect backgroundRect;

            double minBeat = 0;
            double maxBeat = 0;
            {
                rawGUI = EditorGUILayout.Toggle("Raw GUI", rawGUI);
                if (!rawGUI)
                {
                    createBeatOffset = EditorGUILayout.DoubleField("생성 비트 오프셋", createBeatOffset);
                    initValueShow = EditorGUILayout.Toggle("초기 값 표시", initValueShow);

                    if (GUILayout.Button("클립보드 비우기", GUILayout.ExpandWidth(false)))
                        clipboards.Clear();
                }

                EditorGUILayout.Space(0.1f);
                CustomInspectorEditor.DrawLine(2, 0);
                EditorGUILayout.Space(0);

                if (rawGUI)
                    return false;

                backgroundRect = splitView.availableRect;
                backgroundRect.x = 3;
                backgroundRect.y = 2;

                backgroundRect.width *= 1 - splitView.splitNormalizedPosition;
                backgroundRect.width -= 5;
                backgroundRect.height -= 4;

                #region Init Value
                if (initValueShow)
                    GUILayout.Label("초기 값", EditorStyles.boldLabel);

                for (int i = 0; i < lists.Count; i++)
                {
                    PropertyInfoTypeListPair listPair = lists[i];
                    bool initValueExists = false;

                    for (int j = 0; j < listPair.list.Count; j++)
                    {
                        IBeatValuePair pair = (IBeatValuePair)listPair.list[j];

                        if (pair.beat < sbyte.MinValue)
                        {
                            if (!initValueExists)
                            {
                                if (initValueShow)
                                {
                                    GUILayout.Label(listPair.info.Name, EditorStyles.boldLabel);

                                    effectEditor.DrawField(listPair.info.Name, listPair.list.listType, pair, out object outPair, false, 0, false, false);
                                    if (i < lists.Count - 1)
                                        CustomInspectorEditor.DrawLine(2, 0);

                                    listPair.list[j] = outPair;
                                }

                                initValueExists = true;
                            }

                            continue;
                        }

                        minBeat = minBeat.Min(pair.beat);
                        maxBeat = maxBeat.Max(pair.beat);
                    }

                    if (initValueShow)
                    {
                        if (!initValueExists)
                        {
                            if (GUILayout.Button("초기값 생성", GUILayout.ExpandWidth(false)))
                            {
                                IBeatValuePair pair = (IBeatValuePair)Activator.CreateInstance(listPair.list.listType);
                                pair.beat = double.MinValue;

                                listPair.list.Insert(0);
                            }
                        }
                        else if (GUILayout.Button("초기값 삭제", GUILayout.ExpandWidth(false)))
                            listPair.list.RemoveAt(0);
                    }

                    EditorGUILayout.Space(0);
                }
                #endregion
            }

            if (initValueShow)
                CustomInspectorEditor.DrawLine(2, 0);

            verticalScroll = EditorGUILayout.BeginScrollView(verticalScroll);

            backgroundRect = EditorGUILayout.BeginVertical();
            backgroundRect.height += 2;

            float width = backgroundRect.width / minScroll.Distance(maxScroll);
            float xOffset = width * minScroll;

            int topHeight = 21;

            #region Overlay Rect Check
            {
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                for (int i = 0; i < lists.Count; i++)
                {
                    PropertyInfoTypeListPair listPair = lists[i];
                    for (int j = 0; j < listPair.list.Count; j++)
                    {
                        if (selectedItem.ContainsKey(i) && selectedItem[i].ContainsKey(j))
                        {
                            IBeatValuePair pair = (IBeatValuePair)listPair.list[j];

                            float x = BeatToPos((float)pair.beat);
                            minX = minX.Min(x);
                            maxX = maxX.Max(x);
                            maxY = maxY.Max((60 * i) + backgroundRect.y + topHeight + 2 + 35);

                            overlays.pairLists.Add((listPair, pair, j));
                        }
                    }
                }

                overlays.rect = new Rect(minX.Lerp(maxX - 300, 0.5f).Clamp(0, backgroundRect.width - 300), maxY, 300, 200);
            }
            #endregion

            bool ignoreInput = !backgroundRect.Contains(Event.current.mousePosition) || !IgnoreRectCheck(Event.current.mousePosition) || isBottomInput;
            bool isDown = !ignoreInput && Event.current.type == EventType.MouseDown;
            if (isDown)
            {
                dragStartMinScroll = minScroll;
                dragStartMaxScroll = maxScroll;

                dragStartPos = Event.current.mousePosition;
                itemDragStartBeat = PosToBeat(dragStartPos.x);

                isPressing = true;
            }

            bool isDragStart = false;
            if (Event.current.type != EventType.Layout)
            {
                isDrag = (isPressing && Vector2.Distance(dragStartPos, Event.current.mousePosition) >= 10) || isDrag;
                if (lastIsDrag != isDrag)
                {
                    if (isDrag)
                        isDragStart = true;

                    lastIsDrag = isDrag;
                }
            }
            
            bool isUp = (!ignoreInput || isDrag) && Event.current.type == EventType.MouseUp;
            bool isDeleteDown = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete;
            bool isCtrlCDown = Event.current.type == EventType.KeyDown && Event.current.control && Event.current.keyCode == KeyCode.C;
            bool isCtrlVDown = Event.current.type == EventType.KeyDown && Event.current.control && Event.current.keyCode == KeyCode.V;

            if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
                isMouse2 = true;
            if (Event.current.type == EventType.MouseUp && Event.current.button == 2)
                isMouse2 = false;

            #region Beat
            EditorGUILayout.Space(-2);
            {
                {
                    Rect rect = new Rect(backgroundRect.x + BeatToPos(RhythmManager.currentBeatScreen) - 2, backgroundRect.y, 2, topHeight);
                    rect.height = backgroundRect.height;

                    EditorGUI.DrawRect(rect, Color.gray);
                }

                float division = 55 / width;

                if (width > 0)
                    division = division.NextPowerOf(2);
                else
                    division = 1;

                if (division > 0)
                {
                    for (float i = sbyte.MinValue; BeatToPos(i) < backgroundRect.width; i += division)
                    {
                        if (i < minScroll - 1 - division)
                            continue;

                        Rect rect = new Rect(backgroundRect.x + BeatToPos(i), backgroundRect.y, 1, topHeight);
                        Rect lineRect = rect;
                        lineRect.height = backgroundRect.height;

                        Rect textRect = rect;
                        textRect.x += 5;
                        textRect.width = (width * division).Floor() - 2;

                        GUI.Label(textRect, i.ToString());

                        EditorGUI.DrawRect(lineRect, Color.gray);
                        InfiniteLoopDetector.Run();
                    }
                }

                for (int i = topHeight; i < backgroundRect.height; i += 60)
                {
                    Rect rect = new Rect(backgroundRect.x - 2, backgroundRect.y + i, backgroundRect.width + 4, 1);
                    EditorGUI.DrawRect(rect, Color.gray);
                }

                CustomInspectorEditor.DrawLine(2, 0);
            }
            #endregion

            Rect backgroundDragRect = Rect.zero;
            if (isBackgroundDrag)
                backgroundDragRect = new Rect(dragStartPos, Event.current.mousePosition - dragStartPos);

            bool selectCancel = false;
            if (!Event.current.control && isUp && !isDrag && selectedItem.Count > 0)
            {
                SelectedListClear();
                selectCancel = true;
            }

            bool hoverLock = false;
            #region Items
            for (int i = 0; i < lists.Count; i++)
            {
                PropertyInfoTypeListPair listPair = lists[i];
                double lastBeat = double.MinValue;
                for (int j = 0; j < listPair.list.Count; j++)
                {
                    IBeatValuePair pair = (IBeatValuePair)listPair.list[j];
                    if (pair.beat < minBeat - 1 || pair.beat > maxBeat)
                        continue;

                    Rect itemBackgroundRect = new Rect(BeatToPos((float)pair.beat), backgroundRect.y + 60 * i + topHeight + 2, 8, 27);
                    if (lastBeat == pair.beat)
                        itemBackgroundRect.y += itemBackgroundRect.height + 2;

                    lastBeat = pair.beat;

                    bool isHover = (itemBackgroundRect.Contains(dragStartPos) && !hoverLock) || (itemBackgroundRect.Overlaps(backgroundDragRect, true) && isBackgroundDrag);
                    bool isSelected = false;

                    if (isHover)
                    {
                        hoverLock = true;

                        bool drag = !isBackgroundDrag && !isScrollDrag && isDrag;
                        if (drag)
                            isItemDrag = true;

                        if (isItemDrag || isUp)
                        {
                            if (selectedItem.TryGetValue(i, out Dictionary<int, double> value))
                            {
                                if (value.ContainsKey(j))
                                {
                                    if (!isItemDrag)
                                        value.Remove(j);
                                    else
                                        value[j] = pair.beat;
                                }
                                else
                                {
                                    if (isItemDrag)
                                    {
                                        SelectedListClear();
                                        selectedItem[i] = new() { { j, pair.beat } };
                                    }
                                    else
                                        value.Add(j, pair.beat);
                                }
                            }
                            else
                                selectedItem[i] = new() { { j, pair.beat } };
                        }
                    }

                    {
                        if (selectedItem.TryGetValue(i, out Dictionary<int, double> value))
                            isSelected = value.ContainsKey(j);

                        if (isSelected && isItemDrag && Event.current.isMouse)
                        {
                            double orgBeat = selectedItem[i][j];
                            double dragStartBeat = PosToBeat(dragStartPos.x);
                            double dragBeat = PosToBeat(Event.current.mousePosition.x);

                            float division = 55 / width;
                            if (width > 0)
                                division = division.NextPowerOf(2);
                            else
                                division = 1;

                            division *= 0.0625f;

                            double finalBeat = orgBeat - dragStartBeat + dragBeat;
                            finalBeat -= finalBeat.Repeat(division);
                            finalBeat += createBeatOffset;
                            finalBeat = finalBeat.Clamp(sbyte.MinValue);

                            pair.beat = finalBeat;
                            listPair.list[j] = pair;
                        }
                    }

                    if (Event.current.type == EventType.Repaint)
                        GUI.skin.button.Draw(itemBackgroundRect, new GUIContent(), isHover, isSelected, isSelected, isSelected);
                }
            }
            #endregion

            #region After treatment
            if (overlays.pairLists.Count > 0)
            {
                (PropertyInfoTypeListPair listPair, IBeatValuePair pair, int pairIndex) pairListFirst = overlays.pairLists[0];

                //Delete
                if (isDeleteDown)
                {
                    int removeIndex = 0;
                    for (int i = 0; i < overlays.pairLists.Count; i++)
                    {
                        (PropertyInfoTypeListPair listPair, IBeatValuePair pair, int pairIndex) pairList = overlays.pairLists[i];
                        pairList.listPair.list.RemoveAt(pairList.pairIndex - removeIndex);
                        removeIndex++;

                        SelectedListClear();
                    }
                }
                else if (isCtrlCDown) //Copy
                {
                    clipboards.Clear();

                    foreach (var selectedItemsList in selectedItem)
                    {
                        double minItemBeat = 0;
                        List<KeyValuePair<int, double>> ordedSelectedItemList = selectedItemsList.Value.OrderBy(x => x.Value).ToList();

                        for (int i = 0; i < ordedSelectedItemList.Count; i++)
                        {
                            KeyValuePair<int, double> selectedItem = ordedSelectedItemList[i];
                            if (i <= 0)
                                minItemBeat = selectedItem.Value;

                            Type type = lists[selectedItemsList.Key].list.listType;
                            IBeatValuePair pair = (IBeatValuePair)lists[selectedItemsList.Key].list[selectedItem.Key];

                            if (pair is IBeatValuePairAni pairAni)
                            {
                                IBeatValuePairAni createdPairAni = (IBeatValuePairAni)Activator.CreateInstance(type);

                                createdPairAni.beat = selectedItem.Value - minItemBeat;
                                createdPairAni.value = pairAni.value;
                                createdPairAni.disturbance = pairAni.disturbance;
                                createdPairAni.length = pairAni.length;
                                createdPairAni.easingFunction = pairAni.easingFunction;

                                clipboards.Add(createdPairAni);
                            }
                            else
                            {
                                IBeatValuePair createdPair = (IBeatValuePair)Activator.CreateInstance(type);

                                createdPair.beat = selectedItem.Value - minItemBeat;
                                createdPair.value = pair.value;
                                createdPair.disturbance = pair.disturbance;

                                clipboards.Add(createdPair);
                            }
                        }
                    }

                    clipboards = clipboards.OrderBy(x => x.beat).ToList();
                }
                else //Overlay
                {
                    GUI.BeginGroup(overlays.rect);
                    GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(300, 150)), GUI.skin.button);

                    #region Mixed Check
                    bool beatMixed = false;
                    bool valueMixed = false;
                    bool disturbanceMixed = false;
                    bool lengthMixed = false;
                    bool easingFunctionMixed = false;
                    for (int i = 0; i < overlays.pairLists.Count; i++)
                    {
                        IBeatValuePair pair = overlays.pairLists[i].pair;

                        if (pair.beat != pairListFirst.pair.beat)
                            beatMixed = true;
                        if (pair.value != null || pairListFirst.pair.value != null)
                        {
                            if ((pair.value == null && pairListFirst.pair.value != null) || (pair.value != null && pairListFirst.pair.value == null))
                                valueMixed = true;
                            else if (!pair.value.Equals(pairListFirst.pair.value))
                                valueMixed = true;
                        }
                        if (pair.disturbance != pairListFirst.pair.disturbance)
                            disturbanceMixed = true;
                        if (pair is IBeatValuePairAni pairAni)
                        {
                            IBeatValuePairAni tempPairListFirst = (IBeatValuePairAni)pairListFirst.pair;
                            if (pairAni.length != tempPairListFirst.length)
                                lengthMixed = true;
                            if (pairAni.easingFunction != tempPairListFirst.easingFunction)
                                easingFunctionMixed = true;
                        }
                    }
                    #endregion

                    (bool isMapChanged, _) = effectEditor.DrawBeatValuePair(pairListFirst.listPair.list.listType, pairListFirst.pair, out object outValue, false, 0, false, false, beatMixed, out bool beatChange, valueMixed, out bool valueChange, disturbanceMixed, out bool disturbanceChange, lengthMixed, out bool lengthChange, easingFunctionMixed, out bool easingFunctionChange);

                    #region Map Change
                    if (isMapChanged)
                    {
                        IBeatValuePair outPair = (IBeatValuePair)outValue;
                        IBeatValuePairAni outPairAni = outPair as IBeatValuePairAni;

                        for (int i = 0; i < overlays.pairLists.Count; i++)
                        {
                            (PropertyInfoTypeListPair listPair, IBeatValuePair pair, int pairIndex) overlaysPairList = overlays.pairLists[i];
                            if (outPairAni != null)
                            {
                                IBeatValuePairAni overlaysPairAni = (IBeatValuePairAni)overlaysPairList.pair;
                                IBeatValuePairAni createdPairAni = (IBeatValuePairAni)Activator.CreateInstance(overlaysPairList.listPair.list.listType);

                                createdPairAni.beat = overlaysPairAni.beat;
                                createdPairAni.value = overlaysPairAni.value;
                                createdPairAni.disturbance = overlaysPairAni.disturbance;
                                createdPairAni.length = overlaysPairAni.length;
                                createdPairAni.easingFunction = overlaysPairAni.easingFunction;

                                if (beatChange)
                                    createdPairAni.beat = outPairAni.beat.Clamp(sbyte.MinValue);
                                if (valueChange)
                                    createdPairAni.value = outPairAni.value;
                                if (disturbanceChange)
                                    createdPairAni.disturbance = outPairAni.disturbance;
                                if (lengthChange)
                                    createdPairAni.length = outPairAni.length;
                                if (easingFunctionChange)
                                    createdPairAni.easingFunction = outPairAni.easingFunction;

                                overlaysPairList.listPair.list[overlaysPairList.pairIndex] = createdPairAni;
                            }
                            else
                            {
                                IBeatValuePair overlaysPair = overlaysPairList.pair;
                                IBeatValuePair createdPair = (IBeatValuePair)Activator.CreateInstance(overlaysPairList.listPair.list.listType);

                                createdPair.beat = overlaysPair.beat;
                                createdPair.value = overlaysPair.value;
                                createdPair.disturbance = overlaysPair.disturbance;

                                if (beatChange)
                                    createdPair.beat = outPair.beat.Clamp(sbyte.MinValue);
                                if (valueChange)
                                    createdPair.value = outPair.value;
                                if (disturbanceChange)
                                    createdPair.disturbance = outPair.disturbance;

                                overlaysPairList.listPair.list[overlaysPairList.pairIndex] = createdPair;
                            }
                        }
                    }
                    #endregion

                    GUILayout.EndArea();
                    GUI.EndGroup();
                }
            }

            if (isCtrlVDown) //Paste
            {
                float division = 55 / width;
                if (width > 0)
                    division = division.NextPowerOf(2);
                else
                    division = 1;

                division *= 0.0625f;

                double finalBeat = PosToBeat(Event.current.mousePosition.x);
                finalBeat = (finalBeat - finalBeat.Repeat(division));
                finalBeat += createBeatOffset;
                finalBeat = finalBeat.Clamp(sbyte.MinValue);

                int listIndex = PosToListIndex(Event.current.mousePosition.y).Clamp(0, lists.Count - 1);
                PropertyInfoTypeListPair listPair = lists[listIndex];
                for (int i = 0; i < clipboards.Count; i++)
                {
                    IBeatValuePair pair = clipboards[i];

                    if (pair is IBeatValuePairAni pairAni && typeof(IBeatValuePairAni).IsAssignableFrom(listPair.list.listType))
                    {
                        IBeatValuePairAni createdPairAni = (IBeatValuePairAni)Activator.CreateInstance(listPair.list.listType);
                        createdPairAni.beat = pairAni.beat + finalBeat;

                        if (pair.type.IsAssignableFrom(createdPairAni.type))
                            createdPairAni.value = pairAni.value;
                        else
                            createdPairAni.value = listPair.list.defaultValue;

                        createdPairAni.disturbance = pairAni.disturbance;
                        createdPairAni.length = pairAni.length;
                        createdPairAni.easingFunction = pairAni.easingFunction;

                        Add(createdPairAni);
                    }
                    else
                    {
                        IBeatValuePair createdPair = (IBeatValuePair)Activator.CreateInstance(listPair.list.listType);
                        createdPair.beat = pair.beat + finalBeat;

                        if (pair.type.IsAssignableFrom(createdPair.type))
                            createdPair.value = pair.value;
                        else
                            createdPair.value = listPair.list.defaultValue;

                        createdPair.disturbance = pair.disturbance;

                        Add(createdPair);
                    }

                    void Add(IBeatValuePair createdPair)
                    {
                        for (int i = 0; i < listPair.list.Count + 1; i++)
                        {
                            if (i < listPair.list.Count)
                            {
                                IBeatValuePair pair = (IBeatValuePair)listPair.list[i];
                                if (pair.beat >= createdPair.beat)
                                {
                                    listPair.list.Insert(i, createdPair);

                                    selectedItem.TryAdd(listIndex, new Dictionary<int, double>());
                                    selectedItem[listIndex].TryAdd(i, createdPair.beat);

                                    break;
                                }
                            }
                            else
                            {
                                listPair.list.Add(createdPair);

                                selectedItem.TryAdd(listIndex, new Dictionary<int, double>());
                                selectedItem[listIndex].TryAdd(i, createdPair.beat);

                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            if (!isItemDrag && !isBackgroundDrag && isMouse2 && isDrag)
                isScrollDrag = true;

            if (!isItemDrag && !isScrollDrag && isDrag)
            {
                isBackgroundDrag = true;
                if (!Event.current.control && isDragStart)
                    SelectedListClear();
            }

            if (isBackgroundDrag)
                EditorGUI.DrawRect(backgroundDragRect, new Color(0, 0, 1, 0.25f));

            //Create
            if (Event.current.isMouse && !selectCancel && selectedItem.Count <= 0 && isUp && !isDrag)
            {
                int i = PosToListIndex(Event.current.mousePosition.y).Clamp(0, lists.Count - 1);
                PropertyInfoTypeListPair listPair = lists[i];

                float division = 55 / width;
                if (width > 0)
                    division = division.NextPowerOf(2);
                else
                    division = 1;

                division *= 0.0625f;

                double finalBeat = PosToBeat(Event.current.mousePosition.x);
                finalBeat = (finalBeat - finalBeat.Repeat(division));
                finalBeat += createBeatOffset;
                finalBeat = finalBeat.Clamp(sbyte.MinValue);

                for (int j = 0; j < listPair.list.Count + 1; j++)
                {
                    if (j < listPair.list.Count)
                    {
                        IBeatValuePair tempPair = (IBeatValuePair)listPair.list[j];
                        if (tempPair.beat >= finalBeat)
                        {
                            listPair.list.Insert(j);

                            IBeatValuePair pair = (IBeatValuePair)listPair.list[j];
                            pair.beat = finalBeat;

                            listPair.list[j] = pair;
                            selectedItem.Add(i, new Dictionary<int, double>() { { j, pair.beat } });

                            break;
                        }
                    }
                    else
                    {
                        listPair.list.Add();

                        IBeatValuePair pair = (IBeatValuePair)listPair.list[j];
                        pair.beat = finalBeat;

                        listPair.list[j] = pair;
                        selectedItem.Add(i, new Dictionary<int, double>() { { j, pair.beat } });

                        break;
                    }
                }
            }

            if (isUp)
            {
                isDrag = false;

                isItemDrag = false;
                isBackgroundDrag = false;
                isScrollDrag = false;

                isPressing = false;
            }

            if (isDrag && Event.current.isMouse)
            {
                Vector2 pos = Event.current.mousePosition;

                if (!isBackgroundDrag)
                {
                    if (pos.x < 2)
                    {
                        Vector2 tempPos = new Vector2(backgroundRect.width - 2, pos.y);
                        dragStartPos += tempPos - pos;

                        tempPos = GUIUtility.GUIToScreenPoint(tempPos);
                        CursorManager.SetCursorPosition((int)tempPos.x, (int)tempPos.y);
                    }

                    if (pos.x > backgroundRect.width - 2)
                    {
                        Vector2 tempPos = new Vector2(2, pos.y);
                        dragStartPos += tempPos - pos;

                        tempPos = GUIUtility.GUIToScreenPoint(tempPos);
                        CursorManager.SetCursorPosition((int)tempPos.x, (int)tempPos.y);
                    }

                    if (pos.y < 2)
                    {
                        Vector2 tempPos = new Vector2(pos.x, backgroundRect.height - 2);
                        dragStartPos += tempPos - pos;

                        tempPos = GUIUtility.GUIToScreenPoint(tempPos);
                        CursorManager.SetCursorPosition((int)tempPos.x, (int)tempPos.y);
                    }

                    if (pos.y > backgroundRect.height - 2)
                    {
                        Vector2 tempPos = new Vector2(pos.x, 2);
                        dragStartPos += tempPos - pos;

                        tempPos = GUIUtility.GUIToScreenPoint(tempPos);
                        CursorManager.SetCursorPosition((int)tempPos.x, (int)tempPos.y);
                    }
                }
            }


            GUILayout.Space(60 * lists.Count + 22);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            CustomInspectorEditor.DrawLine(2, 1);
            GUILayout.Space(1);

            #region Scroll
            {
                Rect rect = EditorGUILayout.BeginVertical();
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    isBottomInput = true;
                if (Event.current.type == EventType.MouseUp)
                    isBottomInput = false;

                if (width > 0 && isScrollDrag)
                {
                    minScroll = (dragStartPos.x - Event.current.mousePosition.x) / width + dragStartMinScroll;
                    maxScroll = (dragStartPos.x - Event.current.mousePosition.x) / width + dragStartMaxScroll;
                }

                if (Event.current.isScrollWheel)
                {
                    minScroll -= Event.current.delta.y / width * 6;
                    maxScroll += Event.current.delta.y / width * 6;
                }

                GUILayout.BeginHorizontal();

                minScroll = EditorGUILayout.FloatField(minScroll, GUILayout.Width(50));
                EditorGUILayout.MinMaxSlider(ref minScroll, ref maxScroll, (float)minBeat - 16, (float)maxBeat + 16);
                maxScroll = EditorGUILayout.FloatField(maxScroll, GUILayout.Width(50));

                GUILayout.EndHorizontal();

                minScroll = minScroll.Clamp(sbyte.MinValue);
                maxScroll = maxScroll.Clamp(minScroll + 0.04f);

                GUILayout.EndVertical();
            }
            #endregion

            if (isDown || isDrag || isUp || Event.current.isScrollWheel)
                editor.Repaint();

            double PosToBeat(float xPos) => (xPos / width) + minScroll;
            int PosToListIndex(float yPos) => ((yPos - topHeight) / 60).FloorToInt();
            float BeatToPos(double beat) => (float)((beat * width) - xOffset);

            void SelectedListClear()
            {
                selectedItem.Clear();
                for (int i = 0; i < lists.Count; i++)
                    lists[i].list.Sort();
            }

            return true;
        }
    }
}
