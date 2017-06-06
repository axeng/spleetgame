#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Script
{
	public class SystemEditor
#if (UNITY_EDITOR)
	: EditorWindow
#endif
	{
		private const float WINDOW_MIN_WIDTH = 800.0f;
		private const float WINDOW_MIN_HEIGHT = 640.0f;

		private LanguageManager m_manager;
		private int m_index;

#if (UNITY_EDITOR)
		[MenuItem("Window/System Editor")]
		public static void GetWindow()
		{
			SystemEditor window = EditorWindow.GetWindow<SystemEditor>("System", true);
			window.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HEIGHT);
		}

		private void OnEnable()
		{
			m_manager = LanguageManager.Load();
			m_index = 0;
		}

		private void OnDisable()
		{
			m_manager.Save();
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			if (m_manager.translations.Count > 0)
			{
				EditorGUILayout.LabelField(string.Format("Translation {0} Out Of {1}", m_index + 1, m_manager.translations.Count));
				m_manager.translations[m_index].key = EditorGUILayout.TextField("Key:", m_manager.translations[m_index].key);
				m_manager.translations[m_index].english = EditorGUILayout.TextField("English:", m_manager.translations[m_index].english);
				m_manager.translations[m_index].french = EditorGUILayout.TextField("French:", m_manager.translations[m_index].french);

			}
			else
			{
				EditorGUILayout.LabelField("No translation found ...");
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			if (GUILayout.Button("New Entry"))
			{
				EditorGUI.FocusTextInControl(null);

				m_manager.Add(new Translation());
				m_index = m_manager.translations.Count - 1;
			}
			if (GUILayout.Button("Delete Entry") && m_manager.translations.Count > 0)
			{
				EditorGUI.FocusTextInControl(null);

				m_manager.translations.RemoveAt(m_index);
				m_index = Mathf.Clamp(m_index, 0, m_manager.translations.Count - 1);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			if (GUILayout.Button("Previous") && m_manager.translations.Count > 0)
			{
				EditorGUI.FocusTextInControl(null);

				m_index--;
				if (m_index < 0)
					m_index = 0;
			}
			if (GUILayout.Button("Next") && m_manager.translations.Count > 0)
			{
				EditorGUI.FocusTextInControl(null);

				m_index++;
				if (m_index >= m_manager.translations.Count)
					m_index = m_manager.translations.Count - 1;
			}
			EditorGUILayout.EndHorizontal();
		}
#endif
	}
}