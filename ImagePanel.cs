using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ImagePanelLibrary.Helpers;
using WpfCustomGridLibrary.Helpers;

namespace ImagePanelLibrary
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

   public class ImagePanel : Panel//, IScrollInfo
    {
        static ImagePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagePanel), new FrameworkPropertyMetadata(typeof(ImagePanel)));

            ItemsWidthProperty = DependencyProperty.Register(nameof(ItemsWidth), typeof(double), typeof(ImagePanel), new PropertyMetadata(360.0));
            ItemsHeightProperty = DependencyProperty.Register(nameof(ItemsHeight), typeof(double), typeof(ImagePanel), new PropertyMetadata(270.0));
            ItemsMarginProperty = DependencyProperty.Register(nameof(ItemsMargin), typeof(Thickness), typeof(ImagePanel), new PropertyMetadata(new Thickness(0)));
        }


        public ImagePanel()
        {
        }

        BorderChecker checker = new();
        RatioManager manager = new();
        Dictionary<int, int> childrenRatioDict = new();

        double lastChildWidth;
        double lastChildHeight;
        double tempLastHeight;

        public const int PARTS_IN_HORIZONTAL_BLOCK = 3;
        public const int PARTS_IN_VERTICAL_BLOCK = 4;

        public static readonly DependencyProperty ItemsWidthProperty;
        public static readonly DependencyProperty ItemsHeightProperty;
        public static readonly DependencyProperty ItemsMarginProperty;

        public double ItemsWidth
        {
            get => (double)base.GetValue(ItemsWidthProperty);
            set => SetValue(ItemsWidthProperty, value);
        }

        public double ItemsHeight
        {
            get => (double)GetValue(ItemsHeightProperty);
            set => SetValue(ItemsHeightProperty, value);
        }

        public Thickness ItemsMargin {
            get => (Thickness)GetValue(ItemsMarginProperty);
            set => SetValue(ItemsMarginProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            bool needToRecalculateRatio = true;

            if (childrenRatioDict.Count != 0)
            {
                needToRecalculateRatio = false;
            }

            Window parentWindow = Application.Current.MainWindow;

            checker.SetMinimunWidth(parentWindow, ItemsWidth, ItemsMargin, MinWidthProperty);
            checker.SetMinimunHeight(parentWindow, ItemsHeight, ItemsMargin, MinHeightProperty);

            manager.Initialize(ItemsWidth, ItemsHeight, ItemsMargin);

            foreach (UIElement child in InternalChildren)
            {
                if (needToRecalculateRatio)
                {
                  childrenRatioDict = manager.DefineElementsRatio(child, WidthProperty, HeightProperty, childrenRatioDict, InternalChildren);
                }

                availableSize = new Size(parentWindow.Width,parentWindow.Height);

                child.Measure(availableSize);
            }

            if (needToRecalculateRatio)
            {
                SearchAllPairs(availableSize);
            }

            return base.ArrangeOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            lastChildWidth = 0;
            lastChildWidth = 0;
            tempLastHeight = 0;

            int countChildsInRow = (int)(finalSize.Width / (ItemsWidth + ItemsMargin.Left + ItemsMargin.Right));
            int elementsInRow = CountPossibleAmountInRow();

            if (elementsInRow < countChildsInRow)
            {
                countChildsInRow = elementsInRow;
            }

            double additionalMargin = (finalSize.Width - (ItemsWidth + ItemsMargin.Left + ItemsMargin.Right) * countChildsInRow) / 2 / countChildsInRow;

            SetOneItem(finalSize, additionalMargin);
            SetVerticalItems(finalSize, additionalMargin);
            SetHorizontalItems(finalSize, additionalMargin);

            lastChildWidth = 0;
            lastChildHeight = 0;

            return finalSize; // Returns the final Arranged size
        }

        private double SetOneItem(Size finalSize, double additionalMargin)
        {
            foreach (var item in childrenRatioDict.Where(ch => ch.Value == 1))
            {
                var child = InternalChildren[item.Key];

                if (checker.IsRightLimit(lastChildWidth + child.DesiredSize.Width, finalSize.Width))
                {
                    lastChildWidth = 0;
                    lastChildHeight += ItemsHeight + ItemsMargin.Top + ItemsMargin.Bottom;
                }

                child.Arrange(new Rect(new Point(lastChildWidth, lastChildHeight), child.DesiredSize));

                lastChildWidth += child.DesiredSize.Width + additionalMargin;
            }

            return lastChildHeight;
        }

        private void SetVerticalItems(Size finalSize, double additionalMargin)
        {
            for (int i = 2; i <= ImagePanel.PARTS_IN_VERTICAL_BLOCK; i++)
            {
                int count = 0;

                foreach (var item in childrenRatioDict.Where(ch => ch.Value == i))
                {
                    var child = InternalChildren[item.Key];

                    if (checker.IsRightLimit(lastChildWidth + child.DesiredSize.Width, finalSize.Width) && (count == i || count == 0))
                    {
                        lastChildWidth = 0;
                        lastChildHeight += ItemsHeight + ItemsMargin.Top + ItemsMargin.Bottom;
                        tempLastHeight = lastChildHeight;
                    }

                    if (count != 0)
                    {
                        child.Arrange(new Rect(new Point(lastChildWidth, tempLastHeight - ItemsMargin.Top * count - ItemsMargin.Bottom * count), child.DesiredSize));
                    }
                    else
                    {
                        child.Arrange(new Rect(new Point(lastChildWidth, tempLastHeight), child.DesiredSize));
                    }

                    tempLastHeight += child.DesiredSize.Height;

                    count++;

                    if (count == i)
                    {
                        tempLastHeight = lastChildHeight;
                        lastChildWidth += child.DesiredSize.Width + additionalMargin;
                        count = 0;
                    }
                }
            }
        }

        private void SetHorizontalItems(Size finalSize, double additionalMargin)
        {
            double tempLastWidth = lastChildWidth;

            for (int i = -2; i >= -PARTS_IN_HORIZONTAL_BLOCK; i--)
            {
                int count = 0;

                foreach (var item in childrenRatioDict.Where(ch => ch.Value == i))
                {
                    var child = InternalChildren[item.Key];

                    if (checker.IsRightLimit(lastChildWidth + (child.DesiredSize.Width - ItemsMargin.Left - ItemsMargin.Right) * (count - i) 
                            + ItemsMargin.Left + ItemsMargin.Right, finalSize.Width) && (count == i || count == 0))
                    {
                        lastChildWidth = 0;
                        lastChildHeight += ItemsHeight + ItemsMargin.Top + ItemsMargin.Bottom;
                        tempLastWidth = lastChildWidth;
                    }

                    if (count != 0)
                    {
                        child.Arrange(new Rect(new Point(tempLastWidth - ItemsMargin.Left * count - ItemsMargin.Right* count, lastChildHeight), child.DesiredSize));
                    }
                    else
                    {
                        child.Arrange(new Rect(new Point(tempLastWidth, lastChildHeight), child.DesiredSize));
                    }

                    tempLastWidth += child.DesiredSize.Width;

                    count++;

                    if (count == Math.Abs(i))
                    {
                        tempLastHeight = lastChildHeight;
                        lastChildWidth += (child.DesiredSize.Width - ItemsMargin.Left - ItemsMargin.Right) * count + ItemsMargin.Left + ItemsMargin.Right + additionalMargin;
                        count = 0;
                    }
                }
            }
        }

        private int CountPossibleAmountInRow()
        {
            int amount = 0;

            for (int i = 1; i <= ImagePanel.PARTS_IN_VERTICAL_BLOCK; i++)
            {
                amount += childrenRatioDict.Where(ch => ch.Value == i).Count() / i;
            }

            for (int i = -2; i >= -ImagePanel.PARTS_IN_HORIZONTAL_BLOCK; i--)
            {
                amount += childrenRatioDict.Where(ch => ch.Value == i).Count() / Math.Abs(i);
            }

            return amount;
        }

        private void SearchAllPairs(Size availableSize)
        {
            for (int i = ImagePanel.PARTS_IN_VERTICAL_BLOCK; i >= 2; i--)
            {
                int countOneType = childrenRatioDict.Where(item => item.Value == i).Count();

                if (countOneType % i != 0)
                {
                    int movedElements = 0;
                    while ((countOneType - movedElements) % i != 0)
                    {
                        var foundedChild = childrenRatioDict.First(children => children.Value == i);
                        childrenRatioDict[foundedChild.Key] = foundedChild.Value - 1;
                        InternalChildren[foundedChild.Key].SetValue(HeightProperty, ItemsHeight / (i - 1));
                        InternalChildren[foundedChild.Key].Measure(availableSize);
                        movedElements++;
                    }
                }
            }

            for (int i = -ImagePanel.PARTS_IN_HORIZONTAL_BLOCK; i <= -2; i++)
            {
                int countOneType = childrenRatioDict.Where(item => item.Value == i).Count();

                if (countOneType % Math.Abs(i) != 0)
                {
                    int movedElements = 0;
                    while ((countOneType - movedElements) % Math.Abs(i) != 0)
                    {
                        var foundedChild = childrenRatioDict.First(children => children.Value == i);
                        childrenRatioDict[foundedChild.Key] = foundedChild.Value + 1;
                        if (childrenRatioDict[foundedChild.Key] == -1)
                        {
                            childrenRatioDict[foundedChild.Key] = 1;
                        }
                        InternalChildren[foundedChild.Key].SetValue(WidthProperty, ItemsWidth / Math.Abs((i + 1)));
                        InternalChildren[foundedChild.Key].Measure(availableSize);
                        movedElements++;
                    }
                }
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded != null)
            {
                visualAdded.SetValue(MarginProperty, ItemsMargin);
            }
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}
