using ExtendedNumerics;
using SCKRM.Json;
using SCKRM.Rhythm;
using SCKRM;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System;

namespace SDJK.MapEditor
{
    public sealed class MapEffectTreeView : TreeView
    {
        public MapEffectTreeView(TreeViewState treeViewState, MapFile mapFile) : base(treeViewState)
        {
            this.mapFile = mapFile;
            Reload();
        }

        public MapFile mapFile;
        public MapEffectTreeViewItem selectedItem;
        public MapEffectTreeViewItem[] selectedItems;

        protected override TreeViewItem BuildRoot()
        {
            MapEffectTreeViewItem root = new MapEffectTreeViewItem(0, -1);
            Type type = mapFile.GetType();

            {
                int id = 0;
                ReflectionRecursion(type, root, mapFile, ref id);
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        void ReflectionRecursion(Type type, TreeViewItem item, object targetObject, ref int id)
        {
            InfiniteLoopDetector.Run();

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                Type propertyType = propertyInfo.PropertyType;

                if (!propertyInfo.CanRead)
                    continue;

                if (typeof(IBeatValuePairAniList).IsAssignableFrom(propertyType))
                    AddChild(item, propertyInfo, targetObject, ref id);
                else if (typeof(IBeatValuePairList).IsAssignableFrom(propertyType))
                    AddChild(item, propertyInfo, targetObject, ref id);
                else
                {
                    if (propertyType == typeof(bool)
                        || propertyType == typeof(byte)
                        || propertyType == typeof(sbyte)
                        || propertyType == typeof(short)
                        || propertyType == typeof(ushort)
                        || propertyType == typeof(int)
                        || propertyType == typeof(uint)
                        || propertyType == typeof(long)
                        || propertyType == typeof(ulong)
                        || propertyType == typeof(float)
                        || propertyType == typeof(double)
                        || propertyType == typeof(decimal)
                        || propertyType == typeof(nint)
                        || propertyType == typeof(nuint)
                        || propertyType == typeof(char)
                        || propertyType == typeof(string)
                        || propertyType == typeof(BigInteger)
                        || propertyType == typeof(BigDecimal)
                        || propertyType == typeof(JVector2)
                        || propertyType == typeof(JVector3)
                        || propertyType == typeof(JVector4)
                        || propertyType == typeof(JRect)
                        || propertyType == typeof(JColor)
                        || propertyType == typeof(JColor32)
                        || propertyType == typeof(AnimationCurve))
                        AddChild(item, propertyInfo, targetObject, ref id);
                    else if (typeof(ITypeList).IsAssignableFrom(propertyType))
                        ListReflectionRecursion(AddChild(item, propertyInfo, targetObject, ref id), (ITypeList)propertyInfo.GetValue(targetObject), ref id);
                    else if (typeof(ICollection).IsAssignableFrom(propertyType))
                        continue;
                    else
                        ReflectionRecursion(propertyType, AddChild(item, propertyInfo, targetObject, ref id), propertyInfo.GetValue(targetObject), ref id);
                }

                void ListReflectionRecursion(TreeViewItem item, ITypeList list, ref int id)
                {
                    InfiniteLoopDetector.Run();

                    if (!list.listType.IsClass)
                        return;

                    for (int j = 0; j < list.Count; j++)
                    {
                        object value = list[j];
                        if (value == null)
                            continue;

                        if (typeof(ITypeList).IsAssignableFrom(list.listType))
                        {
                            ListReflectionRecursion(AddChild(item, propertyInfo, targetObject, ref id), (ITypeList)value, ref id);
                            continue;
                        }
                        else if (typeof(ICollection).IsAssignableFrom(list.listType))
                            continue;

                        ReflectionRecursion(list.listType, AddChild(item, propertyInfo, targetObject, ref id), value, ref id);
                    }
                }

                static MapEffectTreeViewItem AddChild(TreeViewItem item, PropertyInfo propertyInfo, object targetObject, ref int id)
                {
                    id++;

                    MapEffectTreeViewItem childItem = new MapEffectTreeViewItem(id, 0, propertyInfo.Name);

                    childItem.targetObject = targetObject;
                    childItem.propertyInfo = propertyInfo;

                    item.AddChild(childItem);
                    return childItem;
                }
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count == 1)
                selectedItem = (MapEffectTreeViewItem)FindItem(selectedIds[0], rootItem);
            else
                selectedItem = null;

            selectedItems = new MapEffectTreeViewItem[selectedIds.Count];
            for (int i = 0; i < selectedItems.Length; i++)
                selectedItems[i] = (MapEffectTreeViewItem)FindItem(selectedIds[i], rootItem);
        }
    }

    public class MapEffectTreeViewItem : TreeViewItem
    {
        public MapEffectTreeViewItem(int id) : base(id) { }
        public MapEffectTreeViewItem(int id, int depth) : base(id, depth) { }
        public MapEffectTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName) { }

        public object targetObject;
        public PropertyInfo propertyInfo;
    }
}
