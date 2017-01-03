using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace eyecatcher
{
    //the designer does not apply lines to the canvas
    //  but knows how to create the design we're looking for
    //the designer does not tell the painter to paint, but needs the painter's feedback
    //  in order to know where lines are allowed to be drawn
    class designercs
    {
        paintercs Partner;
        canvasdata Canvas;
        Random lineRandomizer;


    public designercs(paintercs painter, canvasdata canvas)
        {
            Partner = painter;
            Canvas = canvas;
            lineRandomizer = new Random();
        }

        public linedata MakeRandomLine(int snapToAngle = 45)
        {
            var thisLine = new linedata();
            // start with a random point within the canvas
            thisLine.Start = getRandomPoint();
            // get a random angle in snapToAngle degree increments
            thisLine.Angle = getRandomAngleWithSnap(snapToAngle);
            // how far along this angle must we go to reach the edge of the canvas?
            var lengthtobound = Partner.LengthToBounds(thisLine.Start, thisLine.Angle);
            // get a random distance between 0 and the distance to the edge of the canvas
            thisLine.Distance = lineRandomizer.NextDouble() * lengthtobound;
            // find the coordinates of the end point
            thisLine.End = Partner.PointToPoint(thisLine.Start, thisLine.Distance, thisLine.Angle);
            return thisLine;
        }

        public linedata MakeRandomWallTestLine(Point startPoint, int snapToAngle = 45, double wallsize = 30)
        {
            var thisLine = new linedata();
            thisLine.Start = startPoint;
            List<int> validAngles = Partner.getValidAngles(startPoint, wallsize, snapToAngle);
            thisLine.Angle = validAngles[lineRandomizer.Next(validAngles.Count())];
            thisLine.Distance = wallsize;
            thisLine.End = Partner.PointToPoint(thisLine.Start, thisLine.Distance, thisLine.Angle);
            return thisLine;
        }

        public linedata MakeRandomWallTestLine2(int snapToAngle = 45, double wallsize = 30)
        {
            linedata walltestline = new linedata();

            if(Canvas.CanvasLines.Count() == 0)
            {
                return MakeRandomWallTestLine(getRandomPoint(),snapToAngle,wallsize);
            }
            else
            {
                Canvas.UniquePoints.Shuffle();
                foreach(Point uniquepoint in Canvas.UniquePoints)
                {
                    List<int> validAngles = Partner.getValidAngles(uniquepoint, wallsize, snapToAngle);
                    validAngles.Shuffle();
                    foreach(int angle in validAngles)
                    {
                        var potentialEndPoint = Partner.PointToPoint(uniquepoint, wallsize, angle);
                        if (!pointExistsOnCanvas(potentialEndPoint)){
                            walltestline.Start = uniquepoint;
                            walltestline.End = potentialEndPoint;
                            walltestline.Distance = wallsize;
                            walltestline.Angle = angle;
                            return walltestline;
                        }
                    }
                }
                return new linedata();
            }
        }
        public Tuple<List<Point>,int,int> getGridablePoints(int snapToAngle = 45, double wallsize = 30)
        {
            List<Point> gridpoints = new List<Point>();
            int pointsWide = Convert.ToInt32(Math.Ceiling(Partner.PaintersCanvas.Width / wallsize));
            int pointsTall = Convert.ToInt32(Math.Ceiling(Partner.PaintersCanvas.Height / wallsize));
            for(int w = 0; w < pointsWide; w++)
            {
                for(int h = 0; h < pointsTall; h++)
                {
                    gridpoints.Add(new Point(w*wallsize, h*wallsize));
                }
            }
            return Tuple.Create(gridpoints, pointsWide, pointsTall);
        }

        public List<linedata> drawAllGridPoints(Tuple<List<Point>, int, int> griddata)
        {
            List<Point> gridpoints = griddata.Item1;
            int pointswide = griddata.Item2;
            int pointstall = griddata.Item3;
            List<linedata> linelist = new List<linedata>();
            for (int x = 0; x < gridpoints.Count(); x++)
            {
                List<Point> nextpoints = new List<Point>();
                if (gridpoints[x].X != gridpoints.Last().X)
                {
                    //we can draw to the right
                    nextpoints.Add(gridpoints[x + pointstall]);
                }
                if (gridpoints[x].Y != gridpoints.Last().Y)
                {
                    //we can draw above
                    nextpoints.Add(gridpoints[x + 1]);
                }
                if (gridpoints[x].X != 0)
                {
                    //we can draw to the left
                    nextpoints.Add(gridpoints[x - pointstall]);
                }
                if (gridpoints[x].Y != 0)
                {
                    //we can draw below
                    nextpoints.Add(gridpoints[x - 1]);
                }
                foreach(Point p in nextpoints)
                {
                    var thisline = new linedata();
                    thisline.Start = gridpoints[x];
                    thisline.End = p;
                    thisline.Distance = Partner.PointDistance(gridpoints[x], p);
                    thisline.Angle = getAngleBetweenTwoPoints(gridpoints[x], p);
                    linelist.Add(thisline);
                }
            }
            return linelist;
        }

        private bool pointExistsOnCanvas(Point point)
        {
            return Canvas.pointsContains(point);
        }

        private int getRandomAngleWithSnap(int snapAngle)
        {
            return lineRandomizer.Next(360 / snapAngle) * snapAngle;
        }

        //I've been out of math class for way too long
        // thank you stackoverflow:
        //  http://stackoverflow.com/questions/7586063/how-to-calculate-the-angle-between-a-line-and-the-horizontal-axis
        private double getAngleBetweenTwoPoints(Point A, Point B)
        {

            var deltaY = A.Y - B.Y;
            var deltaX = A.X - B.X;

            return Math.Atan2(deltaY, deltaX) * (180 / Math.PI);

        }

        public Point getRandomPoint()
        {
            return new Point(lineRandomizer.NextDouble() * Partner.PaintersCanvas.Width, 
                             lineRandomizer.NextDouble() * Partner.PaintersCanvas.Height);
        }
    }


}
