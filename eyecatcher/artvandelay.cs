using System;
using System.Collections.Generic;
using System.Linq;
using eyecatcher;
using Microsoft.Win32;
using System.IO;

namespace eyecatcher
{
    //handles the importing/exporting
    //haha
    public static class artvandelay
    {
        public static canvasdata import()
        {
            canvasdata importcanvas = new canvasdata();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "Canvas Data";
            ofd.DefaultExt = ".ec";
            ofd.Filter = "eyecatcher files|*.ec";
            bool? result = ofd.ShowDialog();

            if (result == true)
            {
                StreamReader sr = new StreamReader(ofd.FileName);
                var readstream = sr.ReadToEnd();
                string[] csvArray = readstream.Split(new char[] { '\r', '\n' });
                foreach(string csvvalues in csvArray)
                {
                    if (!string.IsNullOrEmpty(csvvalues))
                    {
                        string[] values = csvvalues.Split(',');
                        List<double> doublevalues = values.ToList().Select(x => Convert.ToDouble(x)).ToList();
                        linedata newline = new linedata(doublevalues);
                        importcanvas.addLine(newline);
                    }
                }
            }
            return importcanvas;
        }

        public static void export(canvasdata canvas)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Canvas Data";
            sfd.DefaultExt = ".ec";
            sfd.Filter = "eyecatcher files|*.ec";
            bool? result = sfd.ShowDialog();

            if (result == true)
            {
                using (StreamWriter writer = new StreamWriter(sfd.OpenFile()))
                {
                    foreach(linedata line in canvas.CanvasLines)
                    {
                        writer.WriteLine(line.ToValuesCSV());
                    }
                }
            }
        }
    }
}
