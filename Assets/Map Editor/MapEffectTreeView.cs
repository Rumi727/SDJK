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

                object value = propertyInfo.GetValue(targetObject);
                if (!propertyInfo.DeclaringType.IsClass && !propertyInfo.PropertyType.IsClass)
                    continue;

                if (propertyType == typeof(Type))
                    continue;
                else if (IsDefaultType(propertyType))
                    AddChild(item, propertyInfo, targetObject, ref id);
                else if (typeof(IBeatValuePairList).IsAssignableFrom(propertyType) && IsDefaultType(((IBeatValuePairList)value).defaultValue.GetType()))
                    AddChild(item, propertyInfo, targetObject, ref id);
                else if (typeof(ITypeList).IsAssignableFrom(propertyType))
                    ListReflectionRecursion(AddChild(item, propertyInfo, targetObject, ref id), (ITypeList)propertyInfo.GetValue(targetObject), ref id);
                else if (typeof(ICollection).IsAssignableFrom(propertyType))
                    continue;
                else
                    ReflectionRecursion(propertyType, AddChild(item, propertyInfo, targetObject, ref id), propertyInfo.GetValue(targetObject), ref id);

                void ListReflectionRecursion(TreeViewItem item, ITypeList list, ref int id)
                {
                    InfiniteLoopDetector.Run();

                    if (typeof(IBeatValuePairList).IsAssignableFrom(list.GetType()))
                    {
                        IBeatValuePairList pair = (IBeatValuePairList)list;
                        if (!pair.defaultValue.GetType().IsClass)
                            return;
                    }
                    else if (IsDefaultType(list.listType))
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
                        else
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

                static bool IsDefaultType(Type type)
                {
                    if (type == typeof(bool)
                        || type == typeof(byte)
                        || type == typeof(sbyte)
                        || type == typeof(short)
                        || type == typeof(ushort)
                        || type == typeof(int)
                        || type == typeof(uint)
                        || type == typeof(long)
                        || type == typeof(ulong)
                        || type == typeof(float)
                        || type == typeof(double)
                        || type == typeof(decimal)
                        || type == typeof(nint)
                        || type == typeof(nuint)
                        || type == typeof(char)
                        || type == typeof(string)
                        || type == typeof(BigInteger)
                        || type == typeof(BigDecimal)
                        || type == typeof(JVector2)
                        || type == typeof(JVector3)
                        || type == typeof(JVector4)
                        || type == typeof(JRect)
                        || type == typeof(JColor)
                        || type == typeof(JColor32)
                        || type == typeof(AnimationCurve))
                        return true;

                    return false;
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
        public bool readonlyField;
    }
}
