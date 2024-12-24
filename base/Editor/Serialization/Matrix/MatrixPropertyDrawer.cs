using UnityEngine;
using UnityEditor;

namespace Nianyi.UnityToolkit
{
	/// <see href="https://gist.github.com/elaberge/36e43c1f459ee36cde64dc35bf54c312" />
	[CustomPropertyDrawer(typeof(Matrix4x4))]
	public class MatrixPropertyDrawer : PropertyDrawer
	{
		float CellHeight => EditorGUIUtility.singleLineHeight;
		float SlitHeight => EditorGUIUtility.standardVerticalSpacing;
		const int n = 4;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return CellHeight * n + SlitHeight * (n - 1);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			for(int column = 0; column < n; ++column)
			{
				for(int row = 0; row < n; ++row)
				{
					Vector2 cellPos = position.position;
					cellPos.x += position.width * column / n;
					cellPos.y += CellHeight * row + SlitHeight * row;

					string propertyName = $"e{column}{row}";
					const string axisNames = "xyzw";
					string labelText = $"{axisNames[row]}{axisNames[column]}";

					Rect area = new(cellPos, new Vector2(position.width / n, CellHeight));
					EditorGUI.LabelField(
						new Rect(area) { width = CellHeight * 2.0f, x = area.x - CellHeight * 0.5f },
						new GUIContent(labelText),
						new GUIStyle(GUI.skin.label)
						{
							border = new RectOffset(),
							margin = new RectOffset(),
							padding = new RectOffset(),
							alignment = TextAnchor.UpperLeft,
							contentOffset = Vector2.zero,
						}
					);
					EditorGUI.PropertyField(
						new Rect(area) { xMin = area.xMin + CellHeight * 1.0f },
						property.FindPropertyRelative(propertyName),
						GUIContent.none
					);
				}
			}

			EditorGUI.EndProperty();
		}
	}
}