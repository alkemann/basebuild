using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AutomaticVerticalSize))]
public class UpdateButtonPanel : Editor
{

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (GUILayout.Button ("Refresh menu")) {
			((AutomaticVerticalSize)target).adjustSize ();
		}
	}
}
