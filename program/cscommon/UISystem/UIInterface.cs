using System;
using System.Collections.Generic;

namespace UISystem
{
    public enum Visibility
    {
        Visible,
        Hidden,
        Collapsed,
    }

    namespace UI
    {
        public enum Orientation
        {
            Horizontal,
            Vertical,
        }

        public enum HorizontalAlignment
        {
            Left,
            Center,
            Right,
            Stretch,
        }

        public enum VerticalAlignment
        {
            Top,
            Center,
            Bottom,
            Stretch,
        }

        public enum TextWrapping
        {
            WrapWithOverflow,
            NoWrap,
            Wrap,
        }

        public enum TextTrimming
        {
            None,
            CharacterEllipsis,
            WordEllipsis,
        }

        public enum TextAlignment
        {
            Left,
            Right,
            Center,
            Justify,
        }

        public enum ContentAlignment
        {
            BottomCenter,    //内容在垂直方向上底边对齐，在水平方向上居中对齐
            BottomLeft,      //内容在垂直方向上底边对齐，在水平方向上左边对齐
            BottomRight,     //内容在垂直方向上底边对齐，在水平方向上右边对齐
            MiddleCenter,    //内容在垂直方向上中间对齐，在水平方向上居中对齐
            MiddleLeft,      //内容在垂直方向上中间对齐，在水平方向上左边对齐
            MiddleRight,     //内容在垂直方向上中间对齐，在水平方向上右边对齐
            TopCenter,       //内容在垂直方向上顶部对齐，在水平方向上居中对齐
            TopLeft,         //内容在垂直方向上顶部对齐，在水平方向上左边对齐
            TopRight,        //内容在垂直方向上顶部对齐，在水平方向上右边对齐
        }
    }

    public interface UIInterface
    {
        UIInterface Parent { get; set; }
        Visibility Visibility { get; set; }

        UIInterface GetRoot(Type rootType = null);
        List<UIInterface> GetAllChildControls(bool bExceptTemplateControl = true);
        void UpdateUI();
        int IndexOfChild(UIInterface win);
        void MoveChild(int fromIndex, int toIndex);
        int GetChildWinCount();

        void Load(CSUtility.Support.XmlNode pXml);

    }
}
