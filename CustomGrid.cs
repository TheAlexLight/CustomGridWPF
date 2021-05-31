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
        //public static readonly DependencyProperty MarginProperty;
        static CustomGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomGrid), new FrameworkPropertyMetadata(typeof(CustomGrid)));
            //Panel.MarginProperty.OverrideMetadata(typeof(CustomGrid), new PropertyMetadata(new Thickness(0));

            ElementsWidthProperty = DependencyProperty.Register(nameof(ElementsWidth), typeof(double), typeof(CustomGrid), new PropertyMetadata(346.0));
            ElementsHeightProperty = DependencyProperty.Register(nameof(ElementsHeight), typeof(double), typeof(CustomGrid), new PropertyMetadata(260.0));
            MyMarginProperty = DependencyProperty.Register(nameof(MyMargin), typeof(Thickness), typeof(CustomGrid), new PropertyMetadata(new Thickness(0)));
        }

        public CustomGrid()
        {
        }

        private double lastChildWidth;
        private double lastChildHeight;
        private double tempLastHeight;

        BorderChecker checker = new();

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
            Window parentWindow = Application.Current.MainWindow;

            if (checker.IsLeftLimit(ElementsWidth, availableSize.Width))
            {
                parentWindow.SetValue(MinWidthProperty, ElementsWidth + MyMargin.Left + MyMargin.Right);
            }

            if (checker.IsTopLimit(ElementsHeight, availableSize.Height))
            {
                parentWindow.SetValue(MinHeightProperty, ElementsHeight + MyMargin.Top + MyMargin.Bottom);
            }

            foreach (UIElement child in InternalChildren)
            {
                child.SetValue(WidthProperty, ElementsWidth);
                child.SetValue(HeightProperty,ElementsHeight);
                
                child.Measure(availableSize);
            }

            return base.ArrangeOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            lastChildWidth = 0;
            lastChildHeight = 0;

            int countChildsInRow = (int)(finalSize.Width / (ElementsWidth + MyMargin.Left + MyMargin.Right));
            double additionalMargin = (finalSize.Width - (ElementsWidth + MyMargin.Left + MyMargin.Right) * countChildsInRow) / 2 / countChildsInRow;
            bool isFirst = true;

            

            foreach (UIElement child in InternalChildren)
            {
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
                tempLastHeight = child.DesiredSize.Height + ((Thickness)child.GetValue(MyMarginProperty)).Top; 
            }

            lastChildWidth = 0;
            lastChildHeight = 0;

            return finalSize; // Returns the final Arranged size
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
