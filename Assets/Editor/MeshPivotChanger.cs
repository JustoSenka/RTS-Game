using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MeshPivotChangerEditor : EditorWindow
{
	private Vector3 pivotPos;

	[MenuItem("Window/Mesh Pivot Changer")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(MeshPivotChangerEditor));
	}

	void OnGUI()
	{
		BeginHorizGroup(() =>
		{
			pivotPos = EditorGUILayout.Vector3Field("Exact pivot: ", pivotPos);
			if (GUILayout.Button("Set"))
			{
				SetPivots(new Vector3(1, 1, 1));
			}
		});

		if (GUILayout.Button("Set only X"))
		{
			SetPivots(new Vector3(1, 0, 0));
		}

		if (GUILayout.Button("Set only Y"))
		{
			SetPivots(new Vector3(0, 1, 0));
		}

		if (GUILayout.Button("Set only Z"))
		{
			SetPivots(new Vector3(0, 0, 1));
		}
	}

	private void SetPivots(Vector3 mask)
	{
		var meshes = Selection.GetFiltered<MeshFilter>(SelectionMode.DeepAssets | SelectionMode.Editable).Select((mf) => mf.mesh);

		foreach (var mesh in meshes)
		{
			//mesh.
		}
	}

	private void BeginHorizGroup(Action ac)
	{
		EditorGUILayout.BeginHorizontal();
		ac.Invoke();
		EditorGUILayout.BeginHorizontal();
	}
}
