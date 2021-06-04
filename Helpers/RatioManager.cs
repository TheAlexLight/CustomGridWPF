//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;

//namespace WpfCustomGridLibrary.Helpers
//{
//    class RatioManager
//    {
//        public int AA(double b)
//        {
//            return 0;
//        }

//        private double _elementsWidth;
//        private double _elementsHeight;
//        private Thickness _elementsMargin;

//        public RatioManager(double elementsWidth, double elementsHeight, Thickness elementsMargin)
//        {
//            _elementsWidth = elementsWidth;
//            _elementsHeight = elementsHeight;
//            _elementsMargin = elementsMargin;

           
//        }

//        public double CountRatio(ref DependencyProperty widthProperty, ref DependencyProperty heightProperty, UIElement child, bool isWidthBigger)
//        {
//            double childRatio;
//            double setWidth = _elementsWidth;
//            double setHeight = _elementsHeight;

//            if (isWidthBigger)
//            {
//                childRatio = (child.DesiredSize.Width - _elementsMargin.Left - _elementsMargin.Right) 
//                        / (child.DesiredSize.Height - _elementsMargin.Top - _elementsMargin.Bottom);
//                setHeight = _elementsHeight / (int)childRatio;
//            }
//            else
//            {
//                childRatio = -(child.DesiredSize.Height - _elementsMargin.Top - _elementsMargin.Bottom) 
//                        / (child.DesiredSize.Width - _elementsMargin.Left - _elementsMargin.Right);
//                setWidth = _elementsWidth / Math.Abs((int)childRatio);
//            }

//            child.SetValue(widthProperty, setWidth);
//            child.SetValue(heightProperty, setHeight);

//            if (childRatio > 3)
//            {
//                childRatio = 3;
//            }

//            return childRatio;
//        }

//        private double ChooseRatio(UIElement child,DependencyProperty widthProperty, DependencyProperty heightProperty)
//        {
//            double childRatio;

//            if (child.DesiredSize.Width - _elementsMargin.Left - _elementsMargin.Right > child.DesiredSize.Height - _elementsMargin.Top - _elementsMargin.Bottom)
//            {
//                childRatio = CountRatio(ref widthProperty,ref heightProperty, child, true);
//            }
//            else
//            {
//                childRatio = CountRatio(ref widthProperty,ref heightProperty, child, false);
//            }

//            return childRatio;
//        }

//        private void DefineElementsRatio(UIElement child, DependencyProperty widthProperty, DependencyProperty heightProperty, 
//                Dictionary<int,int> childrenRatioDict, UIElementCollection internalChildren)
//        {
//            child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
//            double childRatio;

//            childRatio = ChooseRatio(child, widthProperty, heightProperty);

//            if (!childrenRatioDict.ContainsKey(internalChildren.IndexOf(child)))
//            {
//                childrenRatioDict.Add(internalChildren.IndexOf(child), (int)childRatio);
//            }
//        }
//    }
//}
