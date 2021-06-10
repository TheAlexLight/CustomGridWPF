using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImagePanelWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<UIElement> itemsList;

        public MainWindow()
        {
            InitializeComponent();

            itemsList = new();

            FillItemsList();

            panelItemsControl.ItemsSource = itemsList;

            //customImagePanel.ItemsList = itemsList;

        }
        private void FillItemsList()
        {
            itemsList.Add(this.FindResource("imageQ") as Image);
            itemsList.Add(this.FindResource("imageQwe") as Image);
            itemsList.Add(new Button() { Width = 30, Height = 120, Background = new SolidColorBrush(Colors.Black) });
            itemsList.Add(new Button() { Width = 30, Height = 120, Background = new SolidColorBrush(Colors.Red) });
            itemsList.Add(new Button() { Width = 120, Height = 15, Background = new SolidColorBrush(Colors.Yellow) });
            itemsList.Add(new Button() { Width = 120, Height = 15, Background = new SolidColorBrush(Colors.Blue) });
            itemsList.Add(new Button() { Width = 120, Height = 15, Background = new SolidColorBrush(Colors.Cyan) });
            itemsList.Add(new Button() { Width = 30, Height = 120, Background = new SolidColorBrush(Colors.Orange) });
            itemsList.Add(new Button() { Width = 120, Height = 20, Background = new SolidColorBrush(Colors.LightGray) });
        }
    }
}
