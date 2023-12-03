using UnityEngine;
using UnityEditor;

namespace HexTools.UI.Components.Editor
{
    [CustomEditor(typeof(Container))]
    public class ContainerEditor : UnityEditor.Editor
    {
        private readonly string[] options = { "Auto", "Constant", "Fit Content" };
        private int widthSelect = -1;
        private int heightSelect = -1;
        private Container container;
        [SerializeField] private SerializedProperty width;
        [SerializeField] private SerializedProperty minWidth;
        [SerializeField] private SerializedProperty maxWidth;
        [SerializeField] private SerializedProperty isClampedHorizontally;
        [SerializeField] private SerializedProperty height;
        [SerializeField] private SerializedProperty minHeight;
        [SerializeField] private SerializedProperty maxHeight;
        [SerializeField] private SerializedProperty isClampedVertically;

        public override void OnInspectorGUI()
        {
            widthSelect = EditorGUILayout.Popup("Set Width", widthSelect, options);
            EditorGUI.indentLevel++;

            if (widthSelect == 0)
            {
                serializedObject.Update();
                width.floatValue = 0;
                DrawHorizontalMargine();
                bool clamped = isClampedHorizontally.boolValue;
                clamped = isClampedHorizontally.boolValue = EditorGUILayout.Toggle("Clamped", clamped);
                if (clamped)
                {
                    EditorGUILayout.PropertyField(minWidth);
                    EditorGUILayout.PropertyField(maxWidth);
                    if (minWidth.floatValue < 0)
                        minWidth.floatValue = 0;
                    if (maxWidth.floatValue <= 0)
                        maxWidth.floatValue = 1.0f;
                    if (maxWidth.floatValue < minWidth.floatValue)
                        maxWidth.floatValue = minWidth.floatValue + 1.0f;
                }
                serializedObject.ApplyModifiedProperties();
            }
            else
            if (widthSelect == 1)
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
            else
            if (widthSelect == 2)
            {
                serializedObject.Update();
                width.floatValue = -1;
                bool clamped = isClampedHorizontally.boolValue;
                clamped = isClampedHorizontally.boolValue = EditorGUILayout.Toggle("Clamped", clamped);
                if (clamped)
                {
                    EditorGUILayout.PropertyField(minWidth);
                    EditorGUILayout.PropertyField(maxWidth);
                    if (minWidth.floatValue < 0)
                        minWidth.floatValue = 0;
                    if (maxWidth.floatValue <= 0)
                        maxWidth.floatValue = 1.0f;
                    if (maxWidth.floatValue < minWidth.floatValue)
                        maxWidth.floatValue = minWidth.floatValue + 1.0f;
                }
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.indentLevel--;

            heightSelect = EditorGUILayout.Popup("Set Height", heightSelect, options);
            EditorGUI.indentLevel++;

            if (heightSelect == 0)
            {
                serializedObject.Update();
                height.floatValue = 0;
                DrawVerticalMargine();
                bool clamped = isClampedVertically.boolValue;
                clamped = isClampedVertically.boolValue = EditorGUILayout.Toggle("Clamped", clamped);
                if (clamped)
                {
                    EditorGUILayout.PropertyField(minHeight);
                    EditorGUILayout.PropertyField(maxHeight);
                    if (minHeight.floatValue < 0)
                        minHeight.floatValue = 0;
                    if (maxHeight.floatValue <= 0)
                        maxHeight.floatValue = 1.0f;
                    if (maxHeight.floatValue < minHeight.floatValue)
                        maxHeight.floatValue = minHeight.floatValue + 1.0f;
                }
                serializedObject.ApplyModifiedProperties();
            }
            else
            if (heightSelect == 1)
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
            else
            if (heightSelect == 2)
            {
                serializedObject.Update();
                height.floatValue = -1;
                bool clamped = isClampedVertically.boolValue;
                clamped = isClampedVertically.boolValue = EditorGUILayout.Toggle("Clamped", clamped);
                if (clamped)
                {
                    EditorGUILayout.PropertyField(minHeight);
                    EditorGUILayout.PropertyField(maxHeight);
                    if (minHeight.floatValue < 0)
                        minHeight.floatValue = 0;
                    if (maxHeight.floatValue <= 0)
                        maxHeight.floatValue = 1.0f;
                    if (maxHeight.floatValue < minHeight.floatValue)
                        maxHeight.floatValue = minHeight.floatValue + 1.0f;
                }
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
            isClampedHorizontally = serializedObject.FindProperty("isClampedHorizontally");
            height = serializedObject.FindProperty("height");
            minHeight = serializedObject.FindProperty("minHeight");
            maxHeight = serializedObject.FindProperty("maxHeight");
            isClampedVertically = serializedObject.FindProperty("isClampedVertically");

            widthSelect = ValueToSelect((int)width.floatValue);
            heightSelect = ValueToSelect((int)height.floatValue);

            if (width.floatValue == -2.0f)
            {
                RectTransform rectTransform = container.transform as RectTransform;
                width.floatValue = rectTransform.rect.width;
                serializedObject.ApplyModifiedProperties();
            }
            if (height.floatValue == -2.0f)
            {
                RectTransform rectTransform = container.transform as RectTransform;
                height.floatValue = rectTransform.rect.height;
                serializedObject.ApplyModifiedProperties();
            }
        }
        private int ValueToSelect(int value)
        {
            if (value == -1)
                return 2;
            else
            if (value == 0)
                return 0;
            else
                return 1;
        }
        private void DrawHorizontalMargine()
        {
            bool foldout = SessionState.GetBool("container-horizontal-offset-foldout", false);
            SessionState.SetBool("container-horizontal-offset-foldout", EditorGUILayout.Foldout(foldout, "Margine"));
            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                container.Margine.left = EditorGUILayout.IntField("Left", container.Margine.left);
                container.Margine.right = EditorGUILayout.IntField("Right", container.Margine.right);
                if (EditorGUI.EndChangeCheck())
                {
                    container.SetLayoutHorizontal();
                    EditorUtility.SetDirty(container);
                }
                EditorGUI.indentLevel--;
            }
        }
        private void DrawVerticalMargine()
        {
            bool foldout = SessionState.GetBool("container-vertical-offset-foldout", false);
            SessionState.SetBool("container-vertical-offset-foldout", EditorGUILayout.Foldout(foldout, "Margine"));
            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                container.Margine.bottom = EditorGUILayout.IntField("Bottom", container.Margine.bottom);
                container.Margine.top = EditorGUILayout.IntField("Top", container.Margine.top);
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
