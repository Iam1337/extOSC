/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Core;

namespace extOSC.Editor.Drawers
{
	public class OSCPacketDrawer
	{
		#region Static Private Vars

		private static readonly GUIContent _addressContent = new GUIContent("Address:");

		private static readonly GUIContent _bundleContent = new GUIContent("Bundle:");

		private static readonly GUIContent _bundleEmptyContent = new GUIContent("Bundle is empty");

		private static readonly GUIContent _beginArrayContent = new GUIContent("Begin Array");

		private static readonly GUIContent _endArrayContent = new GUIContent("End Array");

		#endregion

		#region Private Methods

		#endregion

		#region Public 

		public void DrawLayout(IOSCPacket packet)
		{
			if (packet.IsBundle())
			{
				DrawBundle((OSCBundle) packet);
			}
			else
			{
				DrawMessage((OSCMessage) packet);
			}
		}

		#endregion

		#region Private Methods

		private void DrawBundle(OSCBundle bundle)
		{
			if (bundle != null)
			{
				if (bundle.Packets.Count > 0)
				{
					foreach (var bundlePacket in bundle.Packets)
					{
						EditorGUILayout.LabelField(_bundleContent, EditorStyles.boldLabel);
						using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
						{
							DrawLayout(bundlePacket);
						}
					}
				}
				else
				{
					using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
					{
						EditorGUILayout.LabelField(_bundleEmptyContent, OSCEditorStyles.CenterLabel);
					}
				}
			}
		}

		private void DrawMessage(OSCMessage message)
		{
			if (message != null)
			{
				EditorGUILayout.LabelField(_addressContent, EditorStyles.boldLabel);
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					EditorGUILayout.SelectableLabel(message.Address, GUILayout.Height(EditorGUIUtility.singleLineHeight));
				}

				if (message.Values.Count > 0)
				{
					EditorGUILayout.LabelField($"Values ({OSCEditorUtils.GetValueTags(message)}):", EditorStyles.boldLabel);
					using (new GUILayout.VerticalScope())
					{
						foreach (var value in message.Values)
						{
							DrawValue(value);
						}
					}
				}
			}
		}

		private void DrawArray(OSCValue value)
		{
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					EditorGUILayout.LabelField(_beginArrayContent, OSCEditorStyles.CenterBoldLabel);
				}

				foreach (var arrayValues in value.ArrayValue)
				{
					DrawValue(arrayValues);
				}

				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					EditorGUILayout.LabelField(_endArrayContent, OSCEditorStyles.CenterBoldLabel);
				}
			}
		}

		private void DrawValue(OSCValue value)
		{
			if (value.Type == OSCValueType.Array)
			{
				DrawArray(value);
				return;
			}

			var firstColumn = 40f;
			var secondColumn = 60f;

			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					EditorGUILayout.LabelField($"Tag: {value.Tag}", OSCEditorStyles.CenterLabel, GUILayout.Width(firstColumn));
				}

				using (new GUILayout.HorizontalScope())
				{
					if (value.Type == OSCValueType.Blob ||
						value.Type == OSCValueType.Impulse ||
						value.Type == OSCValueType.Null)
					{
						using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
						{
							EditorGUILayout.LabelField(value.Type.ToString(), OSCEditorStyles.CenterLabel);
						}
					}
					else
					{
						using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
						{
							EditorGUILayout.LabelField(value.Type + ":", GUILayout.Width(secondColumn));
						}

						using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
						{
							switch (value.Type)
							{
								case OSCValueType.Color:
									EditorGUILayout.ColorField(value.ColorValue);
									break;
								case OSCValueType.True:
								case OSCValueType.False:
									EditorGUILayout.Toggle(value.BoolValue);
									break;
								default:
									EditorGUILayout.SelectableLabel(value.Value.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
									break;
							}
						}
					}
				}
			}
		}

		#endregion
	}
}