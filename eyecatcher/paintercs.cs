using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace eyecatcher
{
    //the painter knows what the canvas is, and how to manipulate it
    //the canvas does not determine how the design is created, only the actions that manipulate the canvas
    //  or, in short, the painter holds the brush, but is told where to draw the lines
    class paintercs
    {
        public Canvas PaintersCanvas;
        Point[] cornerPoints;

        //set the painter's canvas and it's four corners
        public paintercs(Canvas paintersCanvas)
        {

            PaintersCanvas = paintersCanvas;
            cornerPoints = new Point[4];
            cornerPoints[0].X = 0;
            cornerPoints[0].Y = 0;

            cornerPoints[1].X = PaintersCanvas.Width;
            cornerPoints[1].Y = 0;

            cornerPoints[2].X = PaintersCanvas.Width;
            cornerPoints[2].Y = PaintersCanvas.Height;

            cornerPoints[3].X = 0;
            cornerPoints[3].Y = PaintersCanvas.Height;
        }

        //using the painter's canvas draw a line from one point to another, specify the brush (color)
        public void DrawLine(Point From, Point To, Brush brushColor, double Thickness = 1)
        {
            if (isInBounds(From) && isInBounds(To))
            {
                var newLine = new Line();
                newLine.Stroke = brushColor;
                newLine.X1 = From.X;
                newLine.Y1 = From.Y;
                newLine.X2 = To.X;
                newLine.Y2 = To.Y;

                newLine.StrokeThickness = Thickness;

                PaintersCanvas.Children.Add(newLine);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

        }

        public void DrawLine(linedata line)
        {
            DrawLine(line.Start, line.End, Brushes.Black);
        }

        //apply the canvasdata object onto the canvas element
        public void DrawCanvas(canvasdata canvas)
        {
            foreach(linedata canvasline in canvas.CanvasLines)
            {
                DrawLine(canvasline);
            }
        }

        //my trig needed some refreshing for this
        //find a second point, at an angle and distance from the first
        public Point PointToPoint(Point From, double Length, double angle)
        {
            var cosangle = Math.Cos(angle * (Math.PI / 180));
            var sinangle = Math.Sin(angle * (Math.PI / 180));
            var cosdist = Length * cosangle;
            var sindist = Length * sinangle;
            return new Point(From.X + cosdist, From.Y + sindist);
        }

        //given a point and an angle, what is the longest line we can draw before we hit the edge of our canvas?
        public double LengthToBounds(Point From, double angle)
        {
            var linesToCollide = getLinesToCollide(); //we detect intersections with 4 lines, each an edge of the canvas
            var collideLine = new Line();
            collideLine.X1 = From.X;
            collideLine.Y1 = From.Y;
            var endPoint = PointToPoint(From, PaintersCanvas.Width + 2, angle); //make a sample line that is guarenteed to intersect with one of the canvas edges
            collideLine.X2 = endPoint.X;
            collideLine.Y2 = endPoint.Y;

            Point collidePoint = new Point();

            foreach(Line l in linesToCollide) //for each canvas edge
            {
                //at what point does it intersect?
                Point intersection = LineIntersectionPoint2(new Point(l.X1, l.Y1),
                                                            new Point(l.X2, l.Y2),
                                                            new Point(collideLine.X1, collideLine.Y1),
                                                            new Point(collideLine.X2, collideLine.Y2));
                if(!double.IsNaN(intersection.X)) //save the coordinate if we find the line we intersect with
                {
                    collidePoint = intersection;
                }
                
            }
            return PointDistance(From, collidePoint); //find the distance between our point and the point of intersection
        }

        public List<int> getValidAngles(Point startPoint, double distance, int snapToAngle)
        {
            var masterlist = new List<int>();
            int availableAngleCount = (360 % snapToAngle == 0) ? ((360/snapToAngle)) : (int)(Math.Floor((double)360/snapToAngle));
            for(int i = 0; i < availableAngleCount; i++)
            {
                masterlist.Add(snapToAngle * i);
            }
            var returnList = new List<int>();
            foreach(int possibleAngle in masterlist)
            {
                var endPoint = PointToPoint(startPoint, distance, possibleAngle);
                if (isInBounds(endPoint))
                {
                    returnList.Add(possibleAngle);
                }
            }
            return returnList;
        }

        //distance between any two points
        //always positive: angle determines direction, not positive or negative distance
        public double PointDistance(Point a, Point b)
        {
            double deltaA = a.X - b.X;
            double deltaB = a.Y - b.Y;
            double distance = Math.Sqrt(deltaA * deltaA + deltaB * deltaB);
            return distance;
        }

        //turn 4 points of a canvas into 4 lines to check collision with
        private List<Line> getLinesToCollide()
        {
            List<Line> linesToCollide = new List<Line>();
            for(int i = 0; i < 4; i++)
            {
                var thisLine = new Line();
                thisLine.X1 = cornerPoints[i].X;
                thisLine.Y1 = cornerPoints[i].Y;
                if (i == 3)
                {
                    thisLine.X2 = cornerPoints[0].X;
                    thisLine.Y2 = cornerPoints[0].Y;
                }
                else
                {
                    thisLine.X2 = cornerPoints[i + 1].X;
                    thisLine.Y2 = cornerPoints[i + 1].Y;
                }
                linesToCollide.Add(thisLine);
            }
            return linesToCollide;
        }

        //would a line escape the bounds of the canvas?
        private bool isInBounds(linedata line)
        {
            return (isInBounds(line.Start) && isInBounds(line.End));
        }

        //would a point escape the bounds of the canvas?
        private bool isInBounds(Point point)
        {
            return !(point.X > PaintersCanvas.Width - 1 || point.X < 0 || point.Y > PaintersCanvas.Height - 1 || point.Y < 0);
        }

        //source comes mostly from this page:
        //  http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/
        // my first attempt was a failure, my math is far too rusty and the math isn't what i'd like to spend the most time on
        //the web page is in floats, i just did this in doubles so i could use the Point class
        //the web page also takes into account for lines and not just line segments, well all i'm dealing with are finite line segments
        //  so i've cut it a bit short and stop at finding the segment intersections
        Point LineIntersectionPoint2(Point p1, Point p2, Point p3, Point p4)
        {
            var dx12 = p2.X - p1.X;
            var dy12 = p2.Y - p1.Y;
            var dx34 = p4.X - p3.X;
            var dy34 = p4.Y - p3.Y;

            var denominator = (dy12 * dx34 - dx12 * dy34);

            var t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;

            if (double.IsInfinity(t1))
            {
                return new Point(double.NaN, double.NaN);
            }

            var t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            var intersection = new Point(p1.X + dx12 * t1,
                                         p1.Y + dy12 * t1);

            var segment_intersection = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));


            if (segment_intersection)
            {
                return intersection;
            } else //the lines would intersect, but not the segments, return blank point
            {
                return new Point(double.NaN, double.NaN);
            }


        }
    }
}
