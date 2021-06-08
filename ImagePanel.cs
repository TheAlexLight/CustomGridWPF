using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ImagePanelLibrary.Enums;
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

            ItemsWidthProperty = DependencyProperty.Register(nameof(ItemsWidth), typeof(double), typeof(ImagePanel)
                    , new PropertyMetadata(360.0, ItemsWidthPropertyChanged, ItemsWidthCoerceValue));
            ItemsHeightProperty = DependencyProperty.Register(nameof(ItemsHeight), typeof(double), typeof(ImagePanel)
                    , new PropertyMetadata(270.0, ItemsHeightPropertyChanged, ItemsHeightCoerceValue));
            ItemsMarginProperty = DependencyProperty.Register(nameof(ItemsMargin), typeof(Thickness), typeof(ImagePanel)
                    , new PropertyMetadata(new Thickness(10), ItemsMarginPropertyChanged, ItemsMarginCoerceValue));
            ItemsListProperty = DependencyProperty.Register(nameof(ItemsList), typeof(IEnumerable<UIElement>), typeof(ImagePanel)
                    , new PropertyMetadata(null, ItemsListPropertyChanged));
            ItemsSortingProperty = DependencyProperty.Register(nameof(ItemsSorting), typeof(SortingType), typeof(ImagePanel)
                    , new PropertyMetadata(SortingType.OneHorizontal));

            ItemsWidthChangedEvent = EventManager.RegisterRoutedEvent(nameof(ItemsWidthChanged), RoutingStrategy.Bubble
                , typeof(RoutedPropertyChangedEventHandler<double>), typeof(ImagePanel));
            ItemsHeightChangedEvent = EventManager.RegisterRoutedEvent(nameof(ItemsHeightChanged), RoutingStrategy.Bubble
                , typeof(RoutedPropertyChangedEventHandler<double>), typeof(ImagePanel));
            ItemsMarginChangedEvent = EventManager.RegisterRoutedEvent(nameof(ItemsMarginChanged), RoutingStrategy.Bubble
                , typeof(RoutedPropertyChangedEventHandler<Thickness>), typeof(ImagePanel));
            ItemsListChangedEvent = EventManager.RegisterRoutedEvent(nameof(ItemsListChanged), RoutingStrategy.Bubble
                , typeof(RoutedPropertyChangedEventHandler<IEnumerable<UIElement>>), typeof(ImagePanel));
        }

        public ImagePanel()
        {
            //this.Loaded += new RoutedEventHandler(ImagePanel_Loaded);
        }

        //private void ImagePanel_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var parent = VisualTreeHelper.GetParent(this);

        //    ScrollViewer viewer = new ScrollViewer
        //    {
        //        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
        //        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        //    };

        //    viewer.Content = this;

        //    parent.Children.Add(viewer);
        //}




        BorderChecker checker = new();
        RatioManager manager = new();
        Dictionary<int, int> childrenRatioDict = new();

        double lastChildWidth;
        double lastChildHeight;
        double tempLastHeight;

        public const int PARTS_IN_HORIZONTAL_BLOCK = 3;
        public const int PARTS_IN_VERTICAL_BLOCK = 3;

        public static readonly DependencyProperty ItemsWidthProperty;
        public static readonly DependencyProperty ItemsHeightProperty;
        public static readonly DependencyProperty ItemsMarginProperty;
        public static readonly DependencyProperty ItemsListProperty;
        public static readonly DependencyProperty ItemsSortingProperty;

        public static readonly RoutedEvent ItemsWidthChangedEvent;
        public static readonly RoutedEvent ItemsHeightChangedEvent;
        public static readonly RoutedEvent ItemsMarginChangedEvent;
        public static readonly RoutedEvent ItemsListChangedEvent;

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

        public IEnumerable<UIElement> ItemsList
        {
            get => (IEnumerable<UIElement>)GetValue(ItemsListProperty);
            set => SetValue(ItemsListProperty, value);
        }

        public SortingType ItemsSorting
        {
            get => (SortingType)GetValue(ItemsSortingProperty);
            set => SetValue(ItemsSortingProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<double> ItemsWidthChanged
        {
            add => AddHandler(ItemsWidthChangedEvent, value);
            remove => RemoveHandler(ItemsWidthChangedEvent, value);
        }

        public event RoutedPropertyChangedEventHandler<double> ItemsHeightChanged
        {
            add => AddHandler(ItemsHeightChangedEvent, value);
            remove => RemoveHandler(ItemsHeightChangedEvent, value);
        }

        public event RoutedPropertyChangedEventHandler<Thickness> ItemsMarginChanged
        {
            add => AddHandler(ItemsMarginChangedEvent, value);
            remove => RemoveHandler(ItemsMarginChangedEvent, value);
        }

        public event RoutedPropertyChangedEventHandler<IEnumerable<UIElement>> ItemsListChanged
        {
            add => AddHandler(ItemsListChangedEvent, value);
            remove => RemoveHandler(ItemsListChangedEvent, value);
        }

        private static object ItemsWidthCoerceValue(DependencyObject d, object baseValue)
        {
            if (baseValue is double widthValue)
            {
                if (widthValue < 36.0)
                {
                    baseValue = 36.0;
                }
                else if (widthValue > 4800.0)
                {
                    baseValue = 4800.0;
                }
            }

            return baseValue;
        }

        private static void ItemsWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImagePanel)
            {
                ImagePanel panel = d as ImagePanel;

                if (panel.ItemsHeight != panel.ItemsWidth * 3.0 / 4.0)
                {
                    panel.ItemsHeight = panel.ItemsWidth * 3.0 / 4.0;
                }

                panel.RaiseEvent(new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue, ItemsWidthChangedEvent));
            }
        }

        private static object ItemsHeightCoerceValue(DependencyObject d, object baseValue)
        {
            if (baseValue is double heightValue)
            {
                if (heightValue < 27.0)
                {
                    baseValue = 27.0;
                }
                else if (heightValue > 3600.0)
                {
                    baseValue = 3600.0;
                }
            }

            return baseValue;
        }

        private static void ItemsHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImagePanel)
            {
                ImagePanel panel = d as ImagePanel;

                if (panel.ItemsWidth != panel.ItemsHeight * 4.0 / 3.0)
                {
                    panel.ItemsWidth = panel.ItemsHeight * 4.0 / 3.0;
                }

                panel.RaiseEvent(new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue, ItemsHeightChangedEvent));
            }
        }

        private static object ItemsMarginCoerceValue(DependencyObject d, object baseValue)
        {
            if (baseValue is Thickness marginValue)
            {
                Thickness minThickness = new(0);
                Thickness maxThickness = new(100);

                if (marginValue.Left < minThickness.Left)
                {
                    marginValue.Left = minThickness.Left;
                }
                if (marginValue.Right < minThickness.Right)
                {
                    marginValue.Right = minThickness.Right;
                }
                if (marginValue.Top < minThickness.Top)
                {
                    marginValue.Top = minThickness.Top;
                }
                if (marginValue.Bottom < minThickness.Bottom)
                {
                    marginValue.Bottom = minThickness.Bottom;
                }


                if (marginValue.Left > maxThickness.Left)
                {
                    marginValue.Left = maxThickness.Left;
                }
                if (marginValue.Right > maxThickness.Right)
                {
                    marginValue.Right = maxThickness.Right;
                }
                if (marginValue.Top > maxThickness.Top)
                {
                    marginValue.Top = maxThickness.Top;
                }
                if (marginValue.Bottom > maxThickness.Bottom)
                {
                    marginValue.Bottom = maxThickness.Bottom;
                }

                baseValue = marginValue;
            }

            return baseValue;
        }

        private static void ItemsMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImagePanel)
            {
                ImagePanel panel = d as ImagePanel;
                panel.RaiseEvent(new RoutedPropertyChangedEventArgs<Thickness>((Thickness)e.OldValue, (Thickness)e.NewValue, ItemsMarginChangedEvent));
            }
        }

        private static void ItemsListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImagePanel)
            {
                ImagePanel panel = d as ImagePanel;

                if (e.OldValue != null && e.OldValue is IEnumerable<UIElement> && e.NewValue is IEnumerable<UIElement>)
                {
                    var oldValue = e.OldValue as IEnumerable<UIElement>;
                    var newValue = e.NewValue as IEnumerable<UIElement>;

                    if (newValue.Count() > oldValue.Count())
                    {
                        var newElements = newValue.Except(oldValue);

                        foreach (var item in newElements)
                        {
                            panel.InternalChildren.Add(item);
                        }
                    }
                    else
                    {
                        var newElements = oldValue.Except(newValue);

                        foreach (var item in newElements)
                        {
                            panel.InternalChildren.Remove(item);
                        }
                    }
                }
                else
                {
                    var newValue = e.NewValue as IEnumerable<UIElement>;

                    foreach (var item in newValue)
                    {
                        panel.InternalChildren.Add(item);
                    }
                }

                panel.RaiseEvent(new RoutedPropertyChangedEventArgs<IEnumerable<UIElement>>((IEnumerable<UIElement>)e.OldValue
                        , (IEnumerable<UIElement>)e.NewValue, ItemsListChangedEvent));
            }
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

            switch (ItemsSorting)
            {
                case SortingType.OneHorizontal:
                    SetOneItem(finalSize, additionalMargin);
                    SetHorizontalItems(finalSize, additionalMargin);
                    SetVerticalItems(finalSize, additionalMargin);
                    break;
                case SortingType.ManyHorizontals:
                    SetHorizontalItems(finalSize, additionalMargin);
                    SetVerticalItems(finalSize, additionalMargin);
                    SetOneItem(finalSize, additionalMargin);
                    break;
                case SortingType.ManyVerticals:
                    SetVerticalItems(finalSize, additionalMargin);
                    SetHorizontalItems(finalSize, additionalMargin);
                    SetOneItem(finalSize, additionalMargin);
                    break;
                default:
                    break;
            }

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
            tempLastHeight = lastChildHeight;

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
                SwitchItems(availableSize, i, -1,HeightProperty, ItemsHeight);
            }

            for (int i = -ImagePanel.PARTS_IN_HORIZONTAL_BLOCK; i <= -2; i++)
            {
                SwitchItems(availableSize, i, 1,WidthProperty, ItemsWidth);
            }

            int countOneType = childrenRatioDict.Where(item => item.Value == -1).Count();

            for (int i = 0; i < countOneType; i++)
            {
                var foundedChild = childrenRatioDict.First(children => children.Value == -1);

                childrenRatioDict[foundedChild.Key] = 1;
            }
        }

        private void SwitchItems(Size availableSize, int i, int onePointToSide,DependencyProperty dependencyProp, double itemValue)
        {
            int countOneType = childrenRatioDict.Where(item => item.Value == i).Count();

            if (countOneType % Math.Abs(i) != 0)
            {
                int movedElements = 0;
                while ((countOneType - movedElements) % Math.Abs(i) != 0)
                {
                    var foundedChild = childrenRatioDict.First(children => children.Value == i);
                    childrenRatioDict[foundedChild.Key] = foundedChild.Value + onePointToSide;
                    if (childrenRatioDict[foundedChild.Key] == -1)
                    {
                        childrenRatioDict[foundedChild.Key] = 1;
                    }
                    InternalChildren[foundedChild.Key].SetValue( dependencyProp, itemValue / Math.Abs(i + onePointToSide));
                    InternalChildren[foundedChild.Key].Measure(availableSize);
                    movedElements++;
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
