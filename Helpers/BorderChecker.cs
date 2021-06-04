using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCustomGridLibrary.Helpers
{
    class BorderChecker
    {
        public bool IsRightLimit(double sizeWantToUse, double fullSize)
        {
            return sizeWantToUse > fullSize;
        }

        public bool IsLeftLimit(double sizeWantToUse, double fullSize)
        {
            return sizeWantToUse > fullSize;
        }

        public bool IsTopLimit(double sizeWantToUse, double fullSize)
        {
            return sizeWantToUse > fullSize;
        }

        public void SetMinimunWidth(Window parentWindow, double elementsWidth, Thickness elementsMargin, DependencyProperty minWidthProperty)
        {
            if (IsLeftLimit(elementsWidth + elementsMargin.Left + elementsMargin.Right, parentWindow.Width))
            {
                parentWindow.SetValue(minWidthProperty, elementsWidth + elementsMargin.Left + elementsMargin.Right);
            }
        }

        public void SetMinimunHeight(Window parentWindow, double elementsHeight, Thickness elementsMargin, DependencyProperty minHeightProperty)
        {
            if (IsTopLimit(elementsHeight + elementsMargin.Top + elementsMargin.Bottom + SystemParameters.WindowCaptionHeight, parentWindow.Height))
            {
                parentWindow.SetValue(minHeightProperty, elementsHeight + elementsMargin.Top + SystemParameters.WindowCaptionHeight + elementsMargin.Bottom);
            }
        }
    }
}
