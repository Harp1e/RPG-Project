using UnityEngine;
using UnityEditor;

namespace RPG.Stats
{
    [CustomEditor (typeof (Progression))]
    public class ProgressionEditor : Editor
    {
        //Progression source;
        SerializedProperty s_characterClasses;

        bool showLevels = false;

        void OnEnable ()
        {
            // source = (Progression)target;
            s_characterClasses = serializedObject.FindProperty ("characterClasses");
        }

        public override void OnInspectorGUI ()
        {
            //base.OnInspectorGUI ();
            serializedObject.Update ();

            if (s_characterClasses.arraySize == 0)
            {
                if (GUILayout.Button("Add Character Class"))
                {
                    s_characterClasses.InsertArrayElementAtIndex (0);
                    s_characterClasses.GetArrayElementAtIndex (0).FindPropertyRelative ("stats").InsertArrayElementAtIndex (0);
                }
            }

            GUILayout.BeginVertical ();
            
            for (int i = 0; i < s_characterClasses.arraySize; i++)
            {
                GUILayout.BeginVertical ("box");
                
                GUILayout.BeginHorizontal ();
                
                EditorGUILayout.PropertyField (s_characterClasses.GetArrayElementAtIndex (i).FindPropertyRelative ("characterClass"));
                if (GUILayout.Button ("-", GUILayout.Width (20f)))
                {
                    s_characterClasses.DeleteArrayElementAtIndex (i);
                    continue;
                }
                if (i == s_characterClasses.arraySize - 1)
                {
                    if (GUILayout.Button ("+", GUILayout.Width (20f)))
                    {
                        s_characterClasses.InsertArrayElementAtIndex (s_characterClasses.arraySize);
                    }
                }
                GUILayout.EndHorizontal ();

                SerializedProperty s_stats = s_characterClasses.GetArrayElementAtIndex (i).FindPropertyRelative ("stats");

                if (s_stats == null)
                {
                    s_stats.InsertArrayElementAtIndex (0);
                }

                EditorGUI.indentLevel++;
                GUILayout.BeginVertical ();
                
                for (int j = 0; j < s_stats.arraySize; j++)
                {
                    GUILayout.BeginHorizontal ();

                    EditorGUILayout.PropertyField (s_stats.GetArrayElementAtIndex (j).FindPropertyRelative ("stat"));

                    if (GUILayout.Button ("-", GUILayout.Width (20f)))
                    {
                        s_stats.DeleteArrayElementAtIndex (i);
                        continue;
                    }
                    if (j == s_stats.arraySize - 1)
                    {
                        if (GUILayout.Button ("+", GUILayout.Width (20f)))
                        {
                            s_stats.InsertArrayElementAtIndex (s_stats.arraySize);
                        }
                    }
                    GUILayout.Space (20f);
                    GUILayout.EndHorizontal ();

                    SerializedProperty s_levels = 
                        s_stats.GetArrayElementAtIndex (j).FindPropertyRelative ("levels");

                    if (s_levels == null)
                    {
                        s_levels.InsertArrayElementAtIndex (0);
                    }
                    DrawArray (s_levels);
                }                

                GUILayout.EndVertical ();

                GUILayout.EndVertical ();
                EditorGUI.indentLevel--;

            }
            GUILayout.EndVertical ();

            serializedObject.ApplyModifiedProperties ();
        }

        void DrawArray (SerializedProperty array)
        {
            EditorGUI.indentLevel++;
            GUILayout.BeginVertical ();
            if (array.arraySize == 0)
            {
                if (GUILayout.Button ("Add Level Value"))
                {
                    array.InsertArrayElementAtIndex (0);
                }
            }
            for (int i = 0; i < array.arraySize; i++)
            {
                GUILayout.BeginHorizontal ();
                EditorGUILayout.LabelField ("Level " + (i + 1) + ":", GUILayout.Width(80f));
                EditorGUILayout.PropertyField (array.GetArrayElementAtIndex (i), GUIContent.none);
                if (GUILayout.Button ("-", GUILayout.Width (20f)))
                {
                    array.DeleteArrayElementAtIndex (i);
                }
                if (i == array.arraySize - 1)
                {
                    if (GUILayout.Button ("+", GUILayout.Width (20f)))
                    {
                        array.InsertArrayElementAtIndex (array.arraySize);
                    }
                }
                GUILayout.Space (40f);

                GUILayout.EndHorizontal ();
            }
            GUILayout.EndVertical ();
            EditorGUI.indentLevel--;
        }
    }
}
