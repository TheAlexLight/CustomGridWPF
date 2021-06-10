using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImagePanelLibrary.Helpers
{
    class BorderChecker
    {
        public bool IsRightLimit(double sizeWantToUse, double fullSize)
        {
            return sizeWantToUse > fullSize;
        }

        public void SetMinimunWidth(Window parentWindow, double elementsWidth, Thickness elementsMargin, DependencyProperty minWidthProperty)
        {
                parentWindow.SetValue(minWidthProperty, elementsWidth + elementsMargin.Left + elementsMargin.Right);
        }

        public void SetMinimunHeight(Window parentWindow, double elementsHeight, Thickness elementsMargin, DependencyProperty minHeightProperty)
        {
                parentWindow.SetValue(minHeightProperty, elementsHeight + elementsMargin.Top + SystemParameters.WindowCaptionHeight + elementsMargin.Bottom);
        }
    }
}
