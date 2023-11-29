using UnityEngine;
using UnityEditor;

namespace HexTools.UI.Components.Editor
{
    [CustomEditor(typeof(Container))]
    public class ContainerEditor : UnityEditor.Editor
    {
        private readonly string[] options = { "Auto", "Clamped", "Constant", "Fit Content" };
        private int widthSelect = -1;
        private int heightSelect = -1;
        private Container container;
        [SerializeField] private SerializedProperty width;
        [SerializeField] private SerializedProperty minWidth;
        [SerializeField] private SerializedProperty maxWidth;
        [SerializeField] private SerializedProperty height;
        [SerializeField] private SerializedProperty minHeight;
        [SerializeField] private SerializedProperty maxHeight;

        public override void OnInspectorGUI()
        {
            widthSelect = EditorGUILayout.Popup("Set Width", widthSelect, options);
            EditorGUI.indentLevel++;
            if (widthSelect == 1)
            {
                serializedObject.Update();
                width.floatValue = -1;
                DrawHorizontalPadding();
                EditorGUILayout.PropertyField(minWidth);
                EditorGUILayout.PropertyField(maxWidth);
                if (minWidth.floatValue < 0)
                    minWidth.floatValue = 0;
                if (maxWidth.floatValue <= 0)
                    maxWidth.floatValue = 1.0f;
                if (maxWidth.floatValue < minWidth.floatValue)
                    maxWidth.floatValue = minWidth.floatValue + 1.0f;
                serializedObject.ApplyModifiedProperties();
            }
            else if (widthSelect == 2)
            {
                serializedObject.Update();
                float currWidth = EditorGUILayout.FloatField("Width", width.floatValue);
                if (currWidth <= 0)
                    currWidth = maxWidth.floatValue;
                width.floatValue = currWidth;
                if (currWidth > maxWidth.floatValue)
                    maxWidth.floatValue = currWidth;
                serializedObject.ApplyModifiedProperties();
            }
            else if (widthSelect == 3)
            {
                serializedObject.Update();
                width.floatValue = -2;
                serializedObject.ApplyModifiedProperties();
            }
            else if (widthSelect == 0)
            {
                serializedObject.Update();
                width.floatValue = 0;
                DrawHorizontalPadding();
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.indentLevel--;
            heightSelect = EditorGUILayout.Popup("Set Height", heightSelect, options);
            EditorGUI.indentLevel++;
            if (heightSelect == 1)
            {
                serializedObject.Update();
                height.floatValue = -1;
                DrawVerticalPadding();
                EditorGUILayout.PropertyField(minHeight);
                EditorGUILayout.PropertyField(maxHeight);
                if (minHeight.floatValue < 0)
                    minHeight.floatValue = 0;
                if (maxHeight.floatValue <= 0)
                    maxHeight.floatValue = 1.0f;
                if (maxHeight.floatValue < minHeight.floatValue)
                    maxHeight.floatValue = minHeight.floatValue + 1.0f;
                serializedObject.ApplyModifiedProperties();
            }
            else if (heightSelect == 2)
            {
                serializedObject.Update();
                float currHeight = EditorGUILayout.FloatField("Heigth", height.floatValue);
                if (currHeight <= 0)
                    currHeight = maxHeight.floatValue;
                height.floatValue = currHeight;
                if (currHeight > maxHeight.floatValue)
                    maxHeight.floatValue = currHeight;
                serializedObject.ApplyModifiedProperties();
            }
            else if (heightSelect == 3)
            {
                serializedObject.Update();
                height.floatValue = -2;
                serializedObject.ApplyModifiedProperties();
            }
            else if (heightSelect == 0)
            {
                serializedObject.Update();
                height.floatValue = 0;
                DrawVerticalPadding();
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.indentLevel--;
        }

        private void OnEnable()
        {
            container = target as Container;
            container.SetLayoutHorizontal();

            width = serializedObject.FindProperty("width");
            minWidth = serializedObject.FindProperty("minWidth");
            maxWidth = serializedObject.FindProperty("maxWidth");
            height = serializedObject.FindProperty("height");
            minHeight = serializedObject.FindProperty("minHeight");
            maxHeight = serializedObject.FindProperty("maxHeight");

            widthSelect = ValueToSelect((int)width.floatValue);
            heightSelect = ValueToSelect((int)height.floatValue);

            if (width.floatValue == -3.0f)
            {
                RectTransform rectTransform = container.transform as RectTransform;
                width.floatValue = rectTransform.rect.width;
                serializedObject.ApplyModifiedProperties();
            }
            if (height.floatValue == -3.0f)
            {
                RectTransform rectTransform = container.transform as RectTransform;
                height.floatValue = rectTransform.rect.height;
                serializedObject.ApplyModifiedProperties();
            }
        }
        private int ValueToSelect(int value)
        {
            if (value == 0)
                return 0;
            else
            if (value == -1)
                return 1;
            else
            if (value == -2)
                return 3;
            else
                return 2;
        }
        private void DrawHorizontalPadding()
        {
            bool foldout = SessionState.GetBool("container-horizontal-offset-foldout", false);
            SessionState.SetBool("container-horizontal-offset-foldout", EditorGUILayout.Foldout(foldout, "Padding"));
            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                container.Padding.left = EditorGUILayout.IntField("Left", container.Padding.left);
                container.Padding.right = EditorGUILayout.IntField("Right", container.Padding.right);
                if (EditorGUI.EndChangeCheck())
                {
                    container.SetLayoutHorizontal();
                    EditorUtility.SetDirty(container);
                }
                EditorGUI.indentLevel--;
            }
        }
        private void DrawVerticalPadding()
        {
            bool foldout = SessionState.GetBool("container-vertical-offset-foldout", false);
            SessionState.SetBool("container-vertical-offset-foldout", EditorGUILayout.Foldout(foldout, "Padding"));
            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                container.Padding.bottom = EditorGUILayout.IntField("Bottom", container.Padding.bottom);
                container.Padding.top = EditorGUILayout.IntField("Top", container.Padding.top);
                if (EditorGUI.EndChangeCheck())
                {
                    container.SetLayoutVertical();
                    EditorUtility.SetDirty(container);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}