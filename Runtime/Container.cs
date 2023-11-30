using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HexTools.UI.Components
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class Container : UIBehaviour, ILayoutSelfController, ILayoutController
    {
        private class Offset
        {
            private float left;
            private float right;
            private float top;
            private float bottom;

            public Offset()
            {

            }
            public Offset(float left, float right, float top, float bottom)
            {
                this.left = left;
                this.right = right;
                this.top = top;
                this.bottom = bottom;
            }

            public float Left { get => left; set => left = value; }
            public float Right { get => right; set => right = value; }
            public float Top { get => top; set => top = value; }
            public float Bottom { get => bottom; set => bottom = value; }
        }

        [SerializeField] private float width = -3;
        [SerializeField] private float minWidth;
        [SerializeField] private float maxWidth;
        [SerializeField] private float height = -3;
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;
        [SerializeField] private RectOffset padding;
        private readonly Offset offsetBackup = new Offset();
        [System.NonSerialized] private RectTransform m_Rect;
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        public float Width { get => width; set => width = value; }
        public float MinWidth { get => minWidth; set => minWidth = value; }
        public float MaxWidth { get => maxWidth; set => maxWidth = value; }
        public float Height { get => height; set => height = value; }
        public float MinHeight { get => minHeight; set => minHeight = value; }
        public float MaxHeight { get => maxHeight; set => maxHeight = value; }
        public RectOffset Padding { get => padding; set => padding = value; }

        public void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            CalculateRect();
        }
        public void SetLayoutVertical()
        {
            CalculateRect();
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Rect = transform as RectTransform;
            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);
            SetDirty();
        }
        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }
        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif

        private void CalculateRect()
        {
            if (rectTransform.hasChanged && padding != null)
            {
                Vector2 pivot = rectTransform.pivot;
                offsetBackup.Left = rectTransform.offsetMin.x;
                offsetBackup.Right = rectTransform.offsetMax.x;
                offsetBackup.Bottom = rectTransform.offsetMin.y;
                offsetBackup.Top = rectTransform.offsetMax.y;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                Rect rect = rectTransform.rect;
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors);
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
                if (width == -1)
                {
                    offsetBackup.Left = padding.left;
                    offsetBackup.Right = -padding.right;
                    rectTransform.offsetMin = new Vector2(offsetBackup.Left, offsetBackup.Bottom);
                    rectTransform.offsetMax = new Vector2(offsetBackup.Right, offsetBackup.Top);
                    rect = rectTransform.rect;
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX);
                    if (rect.width > maxWidth)
                    {
                        float offset = rect.width - maxWidth;
                        offsetBackup.Left += offset / 2;
                        offsetBackup.Right -= offset / 2;
                    }
                    else
                    if (rect.width < minWidth)
                    {
                        float offset = minWidth - rect.width;
                        offsetBackup.Left -= offset / 2;
                        offsetBackup.Right += offset / 2;
                    }
                }
                else
                if (width == 0)
                {
                    offsetBackup.Left = padding.left;
                    offsetBackup.Right = -padding.right;
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX);
                }
                else if (width == -2)
                {
                    rectTransform.SetSizeWithCurrentAnchors(0, LayoutUtility.GetPreferredSize(m_Rect, 0));
                    offsetBackup.Left = rectTransform.offsetMin.x;
                    offsetBackup.Right = rectTransform.offsetMax.x;
                }
                else
                if (width > 0)
                {
                    rectTransform.SetSizeWithCurrentAnchors(0, width);
                    offsetBackup.Left = rectTransform.offsetMin.x;
                    offsetBackup.Right = rectTransform.offsetMax.x;
                }

                if (height == -1)
                {
                    offsetBackup.Bottom = padding.bottom;
                    offsetBackup.Top = -padding.top;
                    rectTransform.offsetMin = new Vector2(offsetBackup.Left, offsetBackup.Bottom);
                    rectTransform.offsetMax = new Vector2(offsetBackup.Right, offsetBackup.Top);
                    rect = rectTransform.rect;
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionY);
                    if (rect.height > maxHeight)
                    {
                        float offset = rect.height - maxHeight;
                        offsetBackup.Bottom += offset / 2;
                        offsetBackup.Top -= offset / 2;
                    }
                    else
                    if (rect.height < minHeight)
                    {
                        float offset = minHeight - rect.height;
                        offsetBackup.Bottom -= offset / 2;
                        offsetBackup.Top += offset / 2;
                    }
                }
                else if (height == 0)
                {
                    offsetBackup.Bottom = padding.bottom;
                    offsetBackup.Top = -padding.top;
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionY);
                }
                else
                if (height == -2)
                {
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, LayoutUtility.GetPreferredSize(m_Rect, 1));
                    offsetBackup.Bottom = rectTransform.offsetMin.y;
                    offsetBackup.Top = rectTransform.offsetMax.y;
                }
                if (height > 0)
                {
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, height);
                    offsetBackup.Bottom = rectTransform.offsetMin.y;
                    offsetBackup.Top = rectTransform.offsetMax.y;
                }

                rectTransform.offsetMin = new Vector2(offsetBackup.Left, offsetBackup.Bottom);
                rectTransform.offsetMax = new Vector2(offsetBackup.Right, offsetBackup.Top);
                rectTransform.pivot = pivot;
            }
        }
    }
}
