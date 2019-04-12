using UnityEditor;
using UnityEngine;

using System.Net;

namespace extOSC.Editor.Drawers
{
	[CustomPropertyDrawer(typeof(OSCHostAttribute))]
	public class OSCHostDrawer : PropertyDrawer
	{
		#region Static Private Vars

		private static IPAddress _tempAddress;

		#endregion

		#region Public Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var defaultColor = GUI.color;

			// REMOTE HOST
			GUI.color = IPAddress.TryParse(property.stringValue, out _tempAddress) ? defaultColor : Color.red;
			EditorGUI.PropertyField(position, property, label);
			GUI.color = Color.white;
		}

		#endregion

		#region Private Methods

		#endregion
	}
}