//https://github.com/miguel12345/EditorGUISplitView/blob/master/Assets/EditorGUISplitView/Scripts/Editor/EditorGUISplitView.cs
using UnityEngine;
using UnityEditor;
using SCKRM;

public class EditorGUISplitView
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    Direction splitDirection;
    public float splitNormalizedPosition;
    bool resize;
    public Vector2 scrollPosition;
    public Rect availableRect;
    EditorWindow editorWindow;


    public EditorGUISplitView(Direction splitDirection, EditorWindow editorWindow, float initialValue = 0.3f)
    {
        splitNormalizedPosition = initialValue;
        this.splitDirection = splitDirection;
        this.editorWindow = editorWindow;
    }

    public void BeginSplitView()
    {
        Rect tempRect;

        if (splitDirection == Direction.Horizontal)
            tempRect = EditorGUILayout.BeginHorizontal();
        else
            tempRect = EditorGUILayout.BeginVertical();

        if (tempRect.width > 0.0f)
        {
            availableRect = tempRect;
        }
        if (splitDirection == Direction.Horizontal)
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(availableRect.width * splitNormalizedPosition));
        else
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * splitNormalizedPosition));
    }

    public void Split()
    {
        GUILayout.EndScrollView();
        ResizeSplitFirstView();
    }

    public void EndSplitView()
    {
        if (splitDirection == Direction.Horizontal)
            EditorGUILayout.EndHorizontal();
        else
            EditorGUILayout.EndVertical();
    }

    private void ResizeSplitFirstView()
    {
        Rect resizeHandleDrawRect;
        Rect resizeHandleRect;

        if (splitDirection == Direction.Horizontal)
        {
            resizeHandleRect = new Rect(availableRect.width * splitNormalizedPosition - 2.5f, availableRect.y, 5f, availableRect.height);
            resizeHandleDrawRect = new Rect(availableRect.width * splitNormalizedPosition - 1f, availableRect.y, 2f, availableRect.height);
        }
        else
        {
            resizeHandleRect = new Rect(availableRect.x, availableRect.height * splitNormalizedPosition - 2.5f, availableRect.width, 5f);
            resizeHandleDrawRect = new Rect(availableRect.x, availableRect.height * splitNormalizedPosition - 1f, availableRect.width, 2f);
        }

        EditorGUI.DrawRect(resizeHandleDrawRect, new Color(0.4980392f, 0.4980392f, 0.4980392f));
        
        if (splitDirection == Direction.Horizontal)
            EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
        else
            EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

        if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
            resize = true;

        if (resize)
        {
            if (splitDirection == Direction.Horizontal)
                splitNormalizedPosition = (Event.current.mousePosition.x.Clamp(0) / availableRect.width).Clamp(0.2f, 0.8f);
            else
                splitNormalizedPosition = (Event.current.mousePosition.y.Clamp(0) / availableRect.height).Clamp(0.2f, 0.8f);

            editorWindow.Repaint();
        }

        if (Event.current.type == EventType.MouseUp)
            resize = false;
    }
}

