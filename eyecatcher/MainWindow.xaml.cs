using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eyecatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private canvasdata CurrentMasterCanvas = new canvasdata();
        public MainWindow()
        {
            InitializeComponent();

            anglesbox.SelectedIndex = 0;
            genbox.SelectedIndex = 0;
            List<string> presetangles = new List<string>()
            {
                "90"
            };
            foreach(string angle in presetangles)
            {
                anglesbox.Items.Add(angle);
            }
            foreach(string type in new string[] {"Random","Grid-Erase"})
            {
                genbox.Items.Add(type);
            }

        }

        private void hundred_line()
        {
            displayCanvas.Children.Clear();
            // rip our snap angle from the UI
            var myAngle = int.Parse(anglesbox.SelectedItem?.ToString() ?? "90");
            // create a painter, give it our canvas element to draw on
            var newpainter = new paintercs(displayCanvas);
            //  create a custom canvas object to hold our design in a more managable way
            CurrentMasterCanvas = new canvasdata();
            //  create a designer, partner it with our painter, allow it access to the canvas data
            var newdesigner = new designercs(newpainter, CurrentMasterCanvas);
            //  create 100 lines
            for (int x = 0; x < 100; x++)
            {
                // tell the designer to make a random line with our given snap angle
                var thisLine = newdesigner.MakeRandomLine(myAngle);
                //  add the line to our custom canvas object
                CurrentMasterCanvas.addLine(thisLine);
            }
            //  tell our painter to draw our canvas
            newpainter.DrawCanvas(CurrentMasterCanvas);
            newpainter = null;
        }

        private void hundred_line_v2()
        {
            displayCanvas.Children.Clear();
            // rip our snap angle from the UI
            var myAngle = int.Parse(anglesbox.SelectedItem?.ToString() ?? "90");
            // create a painter, give it our canvas element to draw on
            var newpainter = new paintercs(displayCanvas);
            //  create a custom canvas object to hold our design in a more managable way
            CurrentMasterCanvas = new canvasdata();
            //  create a designer, partner it with our painter, allow it access to the canvas data
            var newdesigner = new designercs(newpainter, CurrentMasterCanvas);
            //  create 100 lines
            for (int x = 0; x < 6000; x++)
            {
                // tell the designer to make a random line with our given snap angle
                var thisLine = newdesigner.MakeRandomWallTestLine2(myAngle, 5);

                if (!thisLine.Distance.Equals(0))
                {
                    CurrentMasterCanvas.addLine(thisLine);
                }
                else
                {
                    break;
                }
                //  add the line to our custom canvas object
                
            }
            //  tell our painter to draw our canvas
            newpainter.DrawCanvas(CurrentMasterCanvas);
            newpainter = null;
        }

        private void hundred_line_v3()
        {
            displayCanvas.Children.Clear();
            // rip our snap angle from the UI
            var myAngle = int.Parse(anglesbox.SelectedItem?.ToString() ?? "90");
            // create a painter, give it our canvas element to draw on
            var newpainter = new paintercs(displayCanvas);
            //  create a custom canvas object to hold our design in a more managable way
            CurrentMasterCanvas = new canvasdata();
            //  create a designer, partner it with our painter, allow it access to the canvas data
            var newdesigner = new designercs(newpainter, CurrentMasterCanvas);

            var alllines = newdesigner.getGridablePoints(wallsize:8);
            var allpoints = newdesigner.drawAllGridPoints(alllines);
            allpoints.Shuffle();

            // take the top tenth (or however many)
            var takenpoints = allpoints.Take(Convert.ToInt32(allpoints.Count() / 10)).ToList();

            //see if we have any lines layer over each other
            //.Count is recalculated each time through the loop so we can't run go out of bounds if we don't
            //  access any modification of x
            for (int x = 0; x < takenpoints.Count(); x++)
            {
                var flippedPoint = new linedata(takenpoints[x]); //duplicate the point
                flippedPoint.Flip();  //flip it
                if (takenpoints.Contains(flippedPoint)) //if this is a duplicate
                {
                    takenpoints.Remove(flippedPoint);// remove its flipped duplicate
                }
            }

            foreach (linedata line in takenpoints)
            {
                CurrentMasterCanvas.addLine(line);
            }

            //  tell our painter to draw our canvas
            newpainter.DrawCanvas(CurrentMasterCanvas);
            newpainter = null;
        }

        private void hl2_step()
        {
            displayCanvas.Children.Clear();
            var myAngle = int.Parse(anglesbox.SelectedItem?.ToString() ?? "90");
            var newpainter = new paintercs(displayCanvas);
            var newdesigner = new designercs(newpainter, CurrentMasterCanvas);
            var thisLine = newdesigner.MakeRandomWallTestLine2(myAngle, 30);
            if (!thisLine.Angle.Equals(null))
            {
                CurrentMasterCanvas.addLine(thisLine);
            }
            newpainter.DrawCanvas(CurrentMasterCanvas);
            newpainter = null;
        }

        private void capturebutton_Click(object sender, RoutedEventArgs e)
        {
            var RTB = new RenderTargetBitmap((int)displayCanvas.RenderSize.Width,
                                             (int)displayCanvas.RenderSize.Height, 96d, 96d, PixelFormats.Default);
            RTB.Render(displayCanvas);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(RTB));

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Canvas";
            dlg.DefaultExt = ".png";
            dlg.Filter = "Png Files|*.png";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                var filename = dlg.FileName;
                var memstream = new System.IO.MemoryStream();
                encoder.Save(memstream);
                memstream.Close();
                System.IO.File.WriteAllBytes(filename, memstream.ToArray());

            }
            
        }

        private void generatebutton_Click(object sender, RoutedEventArgs e)
        {
            generatebutton.IsEnabled = false;
            switch (genbox.SelectedItem.ToString())
            {
                case "Random":
                    hundred_line();
                    break;

                case "Grid-Erase":
                    hundred_line_v3();
                    break;

                default:
                    MessageBox.Show("Error");
                    break;
            }
            generatebutton.IsEnabled = true;
        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            artvandelay.export(CurrentMasterCanvas);
        }

        private void import_Click(object sender, RoutedEventArgs e)
        {
            CurrentMasterCanvas = artvandelay.import();
            displayCanvas.Children.Clear();
            var newpainter = new paintercs(displayCanvas);
            newpainter.DrawCanvas(CurrentMasterCanvas);
            newpainter = null;
        }

        private void canvasSizeDialog_Click(object sender, RoutedEventArgs e)
        {
            var canvasSizeDialog = new CanvasSizeDialogWindow();
            canvasSizeDialog.ShowDialog();
        }

        private void canvasSize_Change(object sender, SizeChangedEventArgs e)
        {
            canvasBorder.Height = displayCanvas.Height;
            canvasBorder.Width = displayCanvas.Width;
            Height = canvasBorder.Height + 135.545;
            Width = canvasBorder.Width + 59.091;
        }
    }
}
