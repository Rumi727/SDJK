using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCKRM;

[Serializable]
internal class ToolbarTimeslider : BaseToolbarElement
{
	[SerializeField] float minTime = 1;
	[SerializeField] float maxTime = 120;

	public override string NameInList => "[Slider] Timescale";

	public override void Init()
	{

	}

	public ToolbarTimeslider(float minTime = 0.0f, float maxTime = 10.0f) : base(200)
	{
		this.minTime = minTime;
		this.maxTime = maxTime;
	}

	protected override void OnDrawInList(Rect position)
	{
		position.width = 70.0f;
		EditorGUI.LabelField(position, "Min Time");

		position.x += position.width + FieldSizeSpace;
		position.width = 50.0f;
		minTime = EditorGUI.FloatField(position, "", minTime);

		position.x += position.width + FieldSizeSpace;
		position.width = 70.0f;
		EditorGUI.LabelField(position, "Max Time");

		position.x += position.width + FieldSizeSpace;
		position.width = 50.0f;
		maxTime = EditorGUI.FloatField(position, "", maxTime);
	}

	protected override void OnDrawInToolbar()
	{
		GUILayout.Label("Game Speed", GUILayout.ExpandWidth(false));

		GUI.enabled = Kernel.isPlaying;
		Kernel.gameSpeed = EditorGUILayout.Slider("", Kernel.gameSpeed, minTime, maxTime, GUILayout.Width(WidthInToolbar - 30.0f));
		GUI.enabled = true;
	}
}