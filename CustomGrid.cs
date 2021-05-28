using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

    public class CustomGrid : Panel
    {
        static CustomGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomGrid), new FrameworkPropertyMetadata(typeof(CustomGrid)));
        }

        public double RequestedWidth
        {
            get { return (double)GetValue(RequestedWidthProperty); }
            set { SetValue(RequestedWidthProperty, value); }
        }

        public static readonly DependencyProperty RequestedWidthProperty =
            DependencyProperty.Register(nameof(RequestedWidth), typeof(double), typeof(CustomGrid), new PropertyMetadata(double.NaN));

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    if (!double.IsNaN(RequestedWidth))
        //    {
        //        double requestedWidth = RequestedWidth;
        //        double panelWidth = constraint.Width;

        //        if (panelWidth < requestedWidth)
        //        {
        //            requestedWidth = panelWidth;
        //        }

        //        foreach (UIElement child in InternalChildren)
        //        {
        //            Thickness margin = (Thickness)child.GetValue(MarginProperty);
        //            double width = requestedWidth - margin.Left - margin.Right;

        //            //if (width < 0)
        //            //{
        //            //    width = 0;
        //            //}
        //            child.Measure(constraint);
        //           // child.SetValue(WidthProperty, width);
        //        }
        //    }

        //    return base.MeasureOverride(constraint);
        //}

        //Override the default Measure method of Panel
        protected override Size MeasureOverride(Size availableSize)
        {
            Size panelDesiredSize = new Size();

            // In our example, we just have one child.
            // Report that our panel requires just the size of its only child.
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
                panelDesiredSize = child.DesiredSize;
            }

            return panelDesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                //double x = 50;
                //double y = 50;

                //child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
                child.Arrange(new Rect(child.DesiredSize));
            }
            return finalSize; // Returns the final Arranged size
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {


            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}
