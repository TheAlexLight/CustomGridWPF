using ImagePanelLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfCustomGridLibrary.Helpers
{
    public class RatioManager
    {
        private double _itemsWidth;
        private double _itemsHeight;
        private Thickness _itemsMargin;

        public RatioManager()
        {
        }

        public void Initialize(double itemsWidth, double itemsHeight, Thickness itemsMargin)
        {
            _itemsWidth = itemsWidth;
            _itemsHeight = itemsHeight;
            _itemsMargin = itemsMargin;
        }

        public Dictionary<int,int> DefineElementsRatio(UIElement child, DependencyProperty widthProperty, DependencyProperty heightProperty,
                Dictionary<int, int> childrenRatioDict, UIElementCollection internalChildren)
        {
            child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double childRatio;

            childRatio = ChooseRatio(child, widthProperty, heightProperty);

            if (!childrenRatioDict.ContainsKey(internalChildren.IndexOf(child)))
            {
                childrenRatioDict.Add(internalChildren.IndexOf(child), (int)childRatio);
            }

            return childrenRatioDict;
        }

        private double ChooseRatio(UIElement child, DependencyProperty widthProperty, DependencyProperty heightProperty)
        {
            double childRatio;

            if (child.DesiredSize.Width - _itemsMargin.Left - _itemsMargin.Right > child.DesiredSize.Height - _itemsMargin.Top - _itemsMargin.Bottom)
            {
                childRatio = CountRatio(widthProperty, heightProperty, child, true);
            }
            else
            {
                childRatio = CountRatio(widthProperty, heightProperty, child, false);
            }

            return childRatio;
        }

        public double CountRatio(DependencyProperty widthProperty,DependencyProperty heightProperty, UIElement child, bool isWidthBigger)
        {
            double childRatio;
            double setWidth = _itemsWidth;
            double setHeight = _itemsHeight;

            if (isWidthBigger)
            {
                childRatio = (child.DesiredSize.Width - _itemsMargin.Left - _itemsMargin.Right) / (child.DesiredSize.Height - _itemsMargin.Top - _itemsMargin.Bottom);
                if (childRatio > ImagePanel.PARTS_IN_VERTICAL_BLOCK)
                {
                    childRatio = ImagePanel.PARTS_IN_VERTICAL_BLOCK;
                }
                if (double.IsNaN(childRatio))
                {
                    childRatio = 1;
                }
                setHeight = _itemsHeight / (int)childRatio;


            }
            else
            {
                childRatio = -(child.DesiredSize.Height - _itemsMargin.Top - _itemsMargin.Bottom) / (child.DesiredSize.Width - _itemsMargin.Left - _itemsMargin.Right);
                if (childRatio < -ImagePanel.PARTS_IN_HORIZONTAL_BLOCK)
                {
                    childRatio = -ImagePanel.PARTS_IN_HORIZONTAL_BLOCK;
                }
                if (double.IsNaN(childRatio))
                {
                    childRatio = 1;
                }
                setWidth = _itemsWidth / Math.Abs((int)childRatio);
            }

            child.SetValue(widthProperty, setWidth);
            child.SetValue(heightProperty, setHeight);

            return childRatio;
        }
    }
}
