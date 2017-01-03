using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace eyecatcher
{
    public class canvasdata
    {
        private HashSet<linedata> _canvaslines = new HashSet<linedata>();
        private List<Point> _uniquepoints = new List<Point>();
        public HashSet<linedata> CanvasLines
        {
            get { return _canvaslines; }
        }
        public List<Point> UniquePoints
        {
            get { return _uniquepoints; }
        }

        public double VisualHeight;
        public double VisualWidth;

        public canvasdata()
        {
            _canvaslines = new HashSet<linedata>();
        }

        public canvasdata(HashSet<linedata> lines)
        {
            _canvaslines = lines;
            foreach (linedata line in lines)
            {
                if (!_uniquepoints.Contains(line.Start)) _uniquepoints.Add(line.Start);
                if (!_uniquepoints.Contains(line.End)) _uniquepoints.Add(line.End);
            }
        }

        public bool pointsContains(Point point)
        {
            return _uniquepoints.Contains(point);
        }

        public void addLine(linedata line)
        {
            line.truncate();
            if (!pointsContains(line.Start)) _uniquepoints.Add(line.Start);
            if (!pointsContains(line.End)) _uniquepoints.Add(line.End);
            _canvaslines.Add(line);
        }
    }
}
