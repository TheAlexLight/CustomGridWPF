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
        //public static readonly DependencyProperty MarginProperty;
        static CustomGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomGrid), new FrameworkPropertyMetadata(typeof(CustomGrid)));
             // Panel.MarginProperty.OverrideMetadata(typeof(CustomGrid), new PropertyMetadata(new Thickness(0));

            //MarginProperty.AddOwner(CustomGrid);
            //MarginProperty.
            //MarginProperty = DependencyProperty.Register(nameof(Margin),
            //   typeof(Thickness), typeof(CustomGrid));
        }

          public Thickness MyMargin {
            get => (Thickness)GetValue(MyMarginProperty);
            set
            {
                foreach (UIElement child in InternalChildren)
                {
                    child.SetValue(MyMarginProperty, value);
                }

                SetValue(MyMarginProperty, value);
            } 
        }

        public CustomGrid()
        {
            oneRowChilds = new List<UIElement>();
        }     

        public double ElementsWidth
        {
            get => (double)GetValue(ElementsWidthProperty);
            set => SetValue(ElementsWidthProperty, value);
        }

        public static readonly DependencyProperty ElementsWidthProperty =
            DependencyProperty.Register(nameof(ElementsWidth), typeof(double), typeof(CustomGrid), new PropertyMetadata(346.0));

        public static  readonly DependencyProperty MyMarginProperty =
          DependencyProperty.Register(nameof(MyMargin), typeof(Thickness), typeof(CustomGrid), new PropertyMetadata(new Thickness(0)));

        public double ElementsHeight
        {
            get => (double)GetValue(ElementsHeightProperty);
            set => SetValue(ElementsHeightProperty, value);
        }

        public static readonly DependencyProperty ElementsHeightProperty =
            DependencyProperty.Register(nameof(ElementsHeight), typeof(double), typeof(CustomGrid), new PropertyMetadata(260.0));

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

        private double lastWidth;
        private double lastHeight;
        private double tempLastHeight;
        private List<UIElement> oneRowChilds;


        protected override Size MeasureOverride(Size availableSize)
        {
            Size panelDesiredSize = new Size();




            // In our example, we just have one child.
            // Report that our panel requires just the size of its only child.
            foreach (UIElement child in InternalChildren)
            {   
                oneRowChilds.Add(child);

                 child.SetValue(WidthProperty, ElementsWidth);
                 child.SetValue(HeightProperty,ElementsHeight);

                

                Size tempAvailableSize = availableSize;

                #region
                //if (tempAvailableSize.Width - lastWidth <=0)
                //{
                //    tempAvailableSize.Width = 0;
                //}
                //else
                //{
                //    tempAvailableSize.Width -= lastWidth;
                //}
                #endregion

                tempAvailableSize.Width -= lastWidth;

                child.Measure(availableSize);



                //if (tempAvailableSize.Width - (double)child.DesiredSize.Width <= 0 )
                //{
                //    //if (tempAvailableSize.Width - (double)child.GetValue(WidthProperty) * 0.7 > 0 && oneRowChilds.Count != 1)
                //    //{
                //    //    foreach (var rowChild in oneRowChilds)
                //    //    {
                //    //        rowChild.SetValue(WidthProperty, availableSize.Width / oneRowChilds.Count);
                //    //        rowChild.Measure(availableSize);
                //    //    }

                //    //    tempAvailableSize = availableSize;
                //    //    lastWidth = 0;
                //    //    lastHeight += (double)oneRowChilds.First().GetValue(HeightProperty); //TODO: find max height

                //    //    oneRowChilds.Clear();
                //    //}
                //    //else
                //    //{
                //    //    lastHeight += (double)oneRowChilds.First().GetValue(HeightProperty); //TODO: find max height
                //    //    oneRowChilds.Remove(oneRowChilds.Last());

                //    //    foreach (var rowChild in oneRowChilds)
                //    //    {
                //    //        rowChild.SetValue(WidthProperty, (double)(availableSize.Width / oneRowChilds.Count));
                //    //        rowChild.Measure(availableSize);
                //    //    }

                //    //    oneRowChilds.Clear();
                //    //    oneRowChilds.Add(child);

                //    //    tempAvailableSize = availableSize;
                //    //    lastWidth = 0;
                //    //}
                //}
                //else
                //{
                //    lastWidth += child.DesiredSize.Width;
                //}

                child.Measure(availableSize);
                

                panelDesiredSize = child.DesiredSize;
            }

            return base.ArrangeOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            lastWidth = 0;
            lastHeight = 0;
            var a = SystemParameters.PrimaryScreenWidth;

            int countChildsInRow = (int)(finalSize.Width / ElementsWidth);
            double additionalMargin = (finalSize.Width - ElementsWidth * countChildsInRow)  / countChildsInRow;
            int count = 0;
            foreach (UIElement child in InternalChildren)
            {
                //double x = 50;
                //double y = 50;

                //child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
                //child.Arrange(new Rect(child.DesiredSize));
                //child.Arrange(new Rect(new Point(0,0), new Size(200,200)/*child.DesiredSize*/));
                if (lastWidth + child.DesiredSize.Width > finalSize.Width)
                {
                    lastWidth = 0;
                    lastHeight += tempLastHeight;
                    count = 0;
                }

                Thickness currentMargin = (Thickness)child.GetValue(MyMarginProperty);
                // child.SetValue(MarginProperty, new Thickness(currentMargin.Left , currentMargin.Top, currentMargin.Right + additionalMargin, currentMargin.Bottom));
               

                if (count != 0)
                {
                    lastWidth += additionalMargin;

                }
                count++;

                child.Arrange(new Rect(new Point(lastWidth, lastHeight), child.DesiredSize));

                currentMargin = (Thickness)child.GetValue(MyMarginProperty);
                //child.SetValue(MarginProperty, new Thickness(currentMargin.Left , currentMargin.Top, currentMargin.Right - additionalMargin, currentMargin.Bottom));

                lastWidth += child.DesiredSize.Width;
                tempLastHeight = child.DesiredSize.Height + ((Thickness)child.GetValue(MyMarginProperty)).Top;
            }

            lastWidth = 0;
            lastHeight = 0;

            return finalSize; // Returns the final Arranged size
        }


        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {


            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}
