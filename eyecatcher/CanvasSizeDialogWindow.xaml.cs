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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace eyecatcher
{
    /// <summary>
    /// Interaction logic for CanvasSizeDialogWindow.xaml
    /// </summary>
    public partial class CanvasSizeDialogWindow : Window
    {
        MainWindow mainwindow = (MainWindow)Application.Current.MainWindow;
        public CanvasSizeDialogWindow()
        {
            InitializeComponent();
            heightBox.Text = mainwindow.displayCanvas.Height.ToString();
            widthBox.Text = mainwindow.displayCanvas.Width.ToString();
            applyBtn.IsEnabled = false;
        }

        private void canvasResize_Click(object sender, RoutedEventArgs e)
        {
            var maincanvas = mainwindow.displayCanvas;
            try
            {
                maincanvas.Height = Convert.ToDouble(heightBox.Text);
                maincanvas.Width = Convert.ToDouble(widthBox.Text);
                Close();
            } catch(Exception err)
            {
                var result = MessageBox.Show("Height and Width must be positive whole numbers.");
            }
            
        }

        private void dimensionText_Change(object sender, TextChangedEventArgs e)
        {
            applyBtn.IsEnabled = mainwindow.displayCanvas.Height.ToString() != heightBox.Text || mainwindow.displayCanvas.Width.ToString() != widthBox.Text;

        }

        private void dialogClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
