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

            public Offset() { }

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
        [SerializeField] private bool isClampedHorizontally;
        [SerializeField] private float height = -3;
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;
        [SerializeField] private bool isClampedVertically;
        [SerializeField] private RectOffset margine;
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
        public RectOffset Margine { get => margine; set => margine = value; }

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
            if (margine == null)
                margine = new RectOffset(0, 0, 0, 0);
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
            if (rectTransform.hasChanged && margine != null)
            {
                Vector2 pivot = rectTransform.pivot;
                offsetBackup.Left = rectTransform.offsetMin.x;
                offsetBackup.Right = rectTransform.offsetMax.x;
                offsetBackup.Bottom = rectTransform.offsetMin.y;
                offsetBackup.Top = rectTransform.offsetMax.y;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors);
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);

                if (width == -1)
                {
                    float preferredWidth = LayoutUtility.GetPreferredSize(m_Rect, 0);
                    if (isClampedHorizontally && preferredWidth > maxWidth)
                        rectTransform.SetSizeWithCurrentAnchors(0, maxWidth);
                    else if (isClampedHorizontally && preferredWidth < minWidth)
                        rectTransform.SetSizeWithCurrentAnchors(0, minWidth);
                    else
                        rectTransform.SetSizeWithCurrentAnchors(0, preferredWidth);
                    offsetBackup.Left = rectTransform.offsetMin.x;
                    offsetBackup.Right = rectTransform.offsetMax.x;
                }
                else
                if (width == 0)
                {
                    offsetBackup.Left = margine.left;
                    offsetBackup.Right = -margine.right;
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX);

                    if (isClampedHorizontally)
                    {
                        rectTransform.offsetMin = new Vector2(offsetBackup.Left, offsetBackup.Bottom);
                        rectTransform.offsetMax = new Vector2(offsetBackup.Right, offsetBackup.Top);
                        float currentWidth = rectTransform.rect.width;

                        if (currentWidth > maxWidth)
                        {
                            float offset = currentWidth - maxWidth;
                            offsetBackup.Left += offset / 2;
                            offsetBackup.Right -= offset / 2;
                        }
                        else
                    if (currentWidth < minWidth)
                        {
                            float offset = minWidth - currentWidth;
                            offsetBackup.Left -= offset / 2;
                            offsetBackup.Right += offset / 2;
                        }
                    }
                }
                else
                {
                    rectTransform.SetSizeWithCurrentAnchors(0, width);
                    offsetBackup.Left = rectTransform.offsetMin.x;
                    offsetBackup.Right = rectTransform.offsetMax.x;
                }

                if (height == -1)
                {
                    float preferredHeight = LayoutUtility.GetPreferredSize(m_Rect, 1);
                    if (isClampedVertically && preferredHeight > maxHeight)
                        rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, maxHeight);
                    else if (isClampedVertically && preferredHeight < minHeight)
                        rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, minHeight);
                    else
                        rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, preferredHeight);
                    offsetBackup.Bottom = rectTransform.offsetMin.y;
                    offsetBackup.Top = rectTransform.offsetMax.y;
                }
                else
                if (height == 0)
                {
                    offsetBackup.Bottom = margine.bottom;
                    offsetBackup.Top = -margine.top;
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionY);

                    if (isClampedVertically)
                    {
                        rectTransform.offsetMin = new Vector2(offsetBackup.Left, offsetBackup.Bottom);
                        rectTransform.offsetMax = new Vector2(offsetBackup.Right, offsetBackup.Top);
                        float currentHeight = rectTransform.rect.height;

                        if (currentHeight > maxHeight)
                        {
                            float offset = currentHeight - maxHeight;
                            offsetBackup.Bottom += offset / 2;
                            offsetBackup.Top -= offset / 2;
                        }
                        else
                    if (currentHeight < minHeight)
                        {
                            float offset = minHeight - currentHeight;
                            offsetBackup.Bottom -= offset / 2;
                            offsetBackup.Top += offset / 2;
                        }
                    }
                }
                else
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
