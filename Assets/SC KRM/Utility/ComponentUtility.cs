using UnityEngine;

namespace SCKRM
{
    public static class ComponentUtility
    {
        public static T GetComponentFieldSave<T>(this Component component, T fieldToSave, GetComponentMode mode = GetComponentMode.addIfNull) where T : Component
        {
            if (fieldToSave == null || fieldToSave.gameObject != component.gameObject)
            {
                fieldToSave = component.GetComponent<T>();
                if (fieldToSave == null)
                {
                    if (mode == GetComponentMode.addIfNull)
                        return component.gameObject.AddComponent<T>();
                    else if (mode == GetComponentMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }

        public static T[] GetComponentsFieldSave<T>(this Component component, T[] fieldToSave, GetComponentsMode mode = GetComponentsMode.addZeroLengthIfNull)
        {
            if (fieldToSave == null)
            {
                fieldToSave = component.GetComponents<T>();
                if (fieldToSave == null)
                {
                    if (mode == GetComponentsMode.addZeroLengthIfNull)
                        return new T[0];
                    else if (mode == GetComponentsMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }



        public static T GetComponentInParentFieldSave<T>(this Component component, T fieldToSave, bool includeInactive = false, GetComponentInMode mode = GetComponentInMode.none) where T : Component
        {
            if (fieldToSave == null || fieldToSave.gameObject != component.gameObject)
            {
                fieldToSave = component.GetComponentInParent<T>(includeInactive);

                if (fieldToSave == null && mode == GetComponentInMode.destroyIfNull)
                {
                    UnityEngine.Object.DestroyImmediate(component);
                    return null;
                }
            }

            return fieldToSave;
        }

        public static T[] GetComponentsInParentFieldSave<T>(this Component component, T[] fieldToSave, bool includeInactive = false, GetComponentsMode mode = GetComponentsMode.addZeroLengthIfNull)
        {
            if (fieldToSave == null)
            {
                fieldToSave = component.GetComponentsInParent<T>(includeInactive);
                if (fieldToSave == null)
                {
                    if (mode == GetComponentsMode.addZeroLengthIfNull)
                        return new T[0];
                    else if (mode == GetComponentsMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }



        public static T GetComponentInChildrenFieldSave<T>(this Component component, T fieldToSave, bool includeInactive = false, GetComponentInMode mode = GetComponentInMode.none) where T : Component
        {
            if (fieldToSave == null || fieldToSave.gameObject != component.gameObject)
            {
                fieldToSave = component.GetComponentInChildren<T>(includeInactive);
                if (fieldToSave == null && mode == GetComponentInMode.destroyIfNull)
                {
                    UnityEngine.Object.DestroyImmediate(component);
                    return null;
                }
            }

            return fieldToSave;
        }

        public static T[] GetComponentsInChildrenFieldSave<T>(this Component component, T[] fieldToSave, bool includeInactive = false, GetComponentsMode mode = GetComponentsMode.addZeroLengthIfNull)
        {
            if (fieldToSave == null)
            {
                fieldToSave = component.GetComponentsInChildren<T>(includeInactive);
                if (fieldToSave == null)
                {
                    if (mode == GetComponentsMode.addZeroLengthIfNull)
                        return new T[0];
                    else if (mode == GetComponentsMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }

        public enum GetComponentMode
        {
            none,
            addIfNull,
            destroyIfNull
        }

        public enum GetComponentInMode
        {
            none,
            destroyIfNull
        }

        public enum GetComponentsMode
        {
            none,
            addZeroLengthIfNull,
            destroyIfNull
        }
    }
}
