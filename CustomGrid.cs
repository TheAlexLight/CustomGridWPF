using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfCustomGridLibrary
{
    #region Instruction of using
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomGridLibrary"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomGridLibrary;assembly=WpfCustomGridLibrary"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
#endregion

    public class CustomGrid : Panel//, IScrollInfo
    {
        static CustomGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomGrid), new FrameworkPropertyMetadata(typeof(CustomGrid)));

            ElementsWidthProperty = DependencyProperty.Register(nameof(ElementsWidth), typeof(double), typeof(CustomGrid), new PropertyMetadata(360.0));
            ElementsHeightProperty = DependencyProperty.Register(nameof(ElementsHeight), typeof(double), typeof(CustomGrid), new PropertyMetadata(270.0));
            MyMarginProperty = DependencyProperty.Register(nameof(MyMargin), typeof(Thickness), typeof(CustomGrid), new PropertyMetadata(new Thickness(0)));
        }


        public CustomGrid()
        {
        }

        private double lastChildWidth;
        private double lastChildHeight;
        private double tempLastHeight;

        BorderChecker checker = new();
        Dictionary<int, int> childrenRatioDict = new();

        public static readonly DependencyProperty ElementsWidthProperty;
        public static readonly DependencyProperty ElementsHeightProperty;
        public static readonly DependencyProperty MyMarginProperty;

        public double ElementsWidth
        {
            get => (double)GetValue(ElementsWidthProperty);
            set => SetValue(ElementsWidthProperty, value);
        }

        public double ElementsHeight
        {
            get => (double)GetValue(ElementsHeightProperty);
            set => SetValue(ElementsHeightProperty, value);
        }

        public Thickness MyMargin {
            get => (Thickness)GetValue(MyMarginProperty);
            set => SetValue(MyMarginProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            bool needToRecalculateRatio = true;

            if (childrenRatioDict.Count != 0)
            {
                needToRecalculateRatio = false;
            }

            Window parentWindow = Application.Current.MainWindow;

            if (checker.IsLeftLimit(ElementsWidth + MyMargin.Left + MyMargin.Right, parentWindow.Width))
            {
                parentWindow.SetValue(MinWidthProperty, ElementsWidth + MyMargin.Left + MyMargin.Right);
            }

            if (checker.IsTopLimit(ElementsHeight + MyMargin.Top + MyMargin.Bottom + SystemParameters.WindowCaptionHeight, parentWindow.Height))
            {
                parentWindow.SetValue(MinHeightProperty, ElementsHeight + MyMargin.Top + SystemParameters.WindowCaptionHeight + MyMargin.Bottom);
            }

            foreach (UIElement child in InternalChildren)
            {
                if (needToRecalculateRatio)
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double childRatio;

                    if (child.DesiredSize.Width - MyMargin.Left - MyMargin.Right > child.DesiredSize.Height - MyMargin.Top - MyMargin.Bottom)
                    {
                        childRatio = CountRatio(child, true);
                    }
                    else
                    {
                        childRatio = CountRatio(child, false);
                    }

                    if (!childrenRatioDict.ContainsKey(InternalChildren.IndexOf(child)))
                    {
                        childrenRatioDict.Add(InternalChildren.IndexOf(child), (int)childRatio);
                    }

                    //switch (childRatio)
                    //{
                    //    case >=2:
                    //        listofRatio[0].Add(InternalChildren.IndexOf(child));  
                    //            break;
                    //    default:
                    //        break;
                    //}
                }
                availableSize = new Size(parentWindow.Width,parentWindow.Height);

                child.Measure(availableSize);
            }

            if (needToRecalculateRatio)
            {
                for (int i = 3; i >= 2; i--)
                {
                    int countOneType = childrenRatioDict.Where(item => item.Value == i).Count();

                    if (countOneType % i != 0)
                    {
                        int movedElements = 0;
                        while ((countOneType - movedElements) % i != 0)
                        {
                            var foundedChild = childrenRatioDict.First(children => children.Value == i);
                            childrenRatioDict[foundedChild.Key] = foundedChild.Value - 1;
                            InternalChildren[foundedChild.Key].SetValue(HeightProperty, ElementsHeight / (i - 1));
                            InternalChildren[foundedChild.Key].Measure(availableSize);
                            movedElements++;
                        }
                    }
                }
            }
            return base.ArrangeOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            lastChildWidth = 0;
            lastChildHeight = 0;

            int countChildsInRow = (int)(finalSize.Width / (ElementsWidth + MyMargin.Left + MyMargin.Right));
            int elementsInRow = CountPossibleAmountInRow();
            if (elementsInRow < countChildsInRow)
            {
                countChildsInRow = elementsInRow;
            }
            double additionalMargin = (finalSize.Width - (ElementsWidth + MyMargin.Left + MyMargin.Right) * countChildsInRow) / 2 / countChildsInRow;
            bool isFirst = true;

            double maxHeight;

            foreach (var item in childrenRatioDict.Where(ch => ch.Value == 1))
            {
                var child = InternalChildren[item.Key];
                //foreach (UIElement child in InternalChildren)
                //{
                if (checker.IsRightLimit(lastChildWidth + child.DesiredSize.Width, finalSize.Width))
                {
                    lastChildWidth = 0;
                    lastChildHeight += tempLastHeight;
                    isFirst = true;
                }

                if (!isFirst)
                {
                    lastChildWidth += additionalMargin;
                }

                isFirst = false;

                child.Arrange(new Rect(new Point(lastChildWidth, lastChildHeight), child.DesiredSize));

                lastChildWidth += child.DesiredSize.Width;
                tempLastHeight = child.DesiredSize.Height;// + MyMargin.Bottom/*((Thickness)child.GetValue(MyMarginProperty)).Bottom*/;
                //}

            }

            maxHeight = tempLastHeight;
            tempLastHeight = lastChildHeight;


            for (int i = 2; i < 4; i++)
            {
                int count = 0;

                foreach (var item in childrenRatioDict.Where(ch => ch.Value == i))
                {
                    var child = InternalChildren[item.Key];
                    //foreach (UIElement child in InternalChildren)
                    //{
                    if (checker.IsRightLimit(lastChildWidth + child.DesiredSize.Width, finalSize.Width) && (count == i || count == 0))
                    {
                        lastChildWidth = 0;
                        lastChildHeight = maxHeight;
                        tempLastHeight = lastChildHeight;
                        isFirst = true;
                    }

                    if (!isFirst && count == 0)
                    {
                        lastChildWidth += additionalMargin;
                    }

                    isFirst = false;

                    if (count != 0)
                    {
                        //child.SetValue(MarginProperty, new Thickness(MyMargin.Left, 0, MyMargin.Right, 0));
                        child.Arrange(new Rect(new Point(lastChildWidth, tempLastHeight - MyMargin.Top * count - MyMargin.Bottom  * count), child.DesiredSize));
                    }
                    else
                    {
                        //child.SetValue(MarginProperty, new Thickness(MyMargin.Left, 0, MyMargin.Right, 0));
                        child.Arrange(new Rect(new Point(lastChildWidth, tempLastHeight), child.DesiredSize));
                    }

                   // child.SetValue(MarginProperty, MyMargin);
                    tempLastHeight += child.DesiredSize.Height;



                    count++;
                    //}

                    if (count == i)
                    {
                        maxHeight = tempLastHeight;
                        tempLastHeight = lastChildHeight;
                        lastChildWidth += child.DesiredSize.Width;
                        count = 0;

                        //if (!isFirst)
                        //{
                        //    lastChildWidth += additionalMargin;
                        //}
                    }
                }
            }
            lastChildWidth = 0;
            lastChildHeight = 0;

            return finalSize; // Returns the final Arranged size
        }

        private int CountPossibleAmountInRow()
        {
            int amount = 0;

            for (int i = 1; i < 4; i++)
            {
                amount += childrenRatioDict.Where(ch => ch.Value == i).Count() / i;
            }

            return amount;
        }

        public double CountRatio(UIElement child, bool isWidthBigger)
        {
            double childRatio;
            double setWidth = ElementsWidth;
            double setHeight = ElementsHeight;

            if (isWidthBigger)
            {
                childRatio = (child.DesiredSize.Width - MyMargin.Left - MyMargin.Right) / (child.DesiredSize.Height - MyMargin.Top - MyMargin.Bottom);
                setHeight = ElementsHeight / (int)childRatio;
            }
            else
            {
                childRatio = -(child.DesiredSize.Height - MyMargin.Top - MyMargin.Bottom) / (child.DesiredSize.Width - MyMargin.Left - MyMargin.Right);
                setWidth = ElementsWidth / Math.Abs((int)childRatio);
            }
            
            child.SetValue(WidthProperty, setWidth);
            child.SetValue(HeightProperty, setHeight);

            if (childRatio > 3)
            {
                childRatio = 3;
            }

            return childRatio;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded != null)
            {
                visualAdded.SetValue(MarginProperty, MyMargin);
            }
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}
