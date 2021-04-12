using AdvancedCoding2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopFGApp.Model
{
    public static class ModelUtil
    {

        /*given a circle and point checking if the point is in the circle.
 * using distance between 2 points - Math.Sqrt((x1-x2)^2+(y1-y2)^2)
 * if the distance bigger than radius so point not in circle, return false. else, return true */
        static bool pointIsInsideCircle(Circle c, Point p)
        {
            Point circleCenter = c.center;
            float x1MinusX2 = (float)Math.Pow((circleCenter.x - p.x), 2);
            float y1MinusY2 = (float)Math.Pow((circleCenter.y - p.y), 2);
            float pointFromCenter = (float)Math.Sqrt((x1MinusX2 + y1MinusY2));
            return c.radius >= pointFromCenter;
        }

        /* given a circle and vector of points checking that all points are inside or on the boundary of circle*/
        static bool isValidCircle(Circle c, List<Point> points)
        {
            //iterate all points in vector checking it is inside circle
            foreach (Point p in points)
            {
                if (!pointIsInsideCircle(c, p))
                {
                    return false;
                }
            }
            return true;
        }

        /* return min circle given 2 points - using the following equation -
         * center (x,y) -
         * x= (x1+x2)/2
         * y= (y1+y2)/2
         * r = Math.Sqrt((x1-x2)^2+(y1-y2)^2)/2
         */
        static Circle twoPointsCircle(Point a, Point b)
        {
            float x = (a.x + b.x) / 2;
            float y = (a.y + b.y) / 2;
            float radius = (float) Math.Sqrt((Math.Pow((a.x - b.x), 2) + Math.Pow((a.y - b.y), 2))) / 2;
            return new Circle(new Point(x, y), radius);
        }

        /* return min circle given 3 points - using the following equation -
         * center (x,y) -
         * x= ((x1^2+y1^2)(y2-y3)+(x2^2+y2^2)(y3-y1)+(x3^2+y3^2)(y1-y2))/(2(x1(y2-y3)-y1(x2-x3)+x2y3-x3y2)
         * y= ((x1^2+y1^2)(x2-x3)+(x2^2+y2^2)(x1-x3)+(x3^2+y3^2)(x2-x1))/(2(x1(y2-y3)-y1(x2-x3)+x2y3-x3y2)
         * r = Math.Sqrt((x-x1)^2+(y-y1)^2)
         */
        static Circle threePointsCircle(Point a, Point b, Point c)
        {
            List<Point> points = new List<Point>();
            points.Add(a);
            points.Add(b);
            points.Add(c);

            //for all pairs trying to make circle without third point, if succeed return this circle,
            //else return circle with 3 points
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Circle circ = twoPointsCircle(points[i], points[j]);
                    if (isValidCircle(circ, points))
                    {
                        return circ;
                    }
                }
            }
            //calculating 3 points circle
            float denominator = 2 * (a.x * (b.y - c.y) - a.y * (b.x - c.x) + b.x * c.y - c.x * b.y);
            float xNumerator = (float) (Math.Pow(a.x, 2) + Math.Pow(a.y, 2)) * (b.y - c.y) + (float)(Math.Pow(b.x, 2) + Math.Pow(b.y, 2)) * (c.y - a.y)
                               + (float)(Math.Pow(c.x, 2) + Math.Pow(c.y, 2)) * (a.y - b.y);
            float yNumerator = (float) (Math.Pow(a.x, 2) + Math.Pow(a.y, 2)) * (c.x - b.x) + (float)(Math.Pow(b.x, 2) + Math.Pow(b.y, 2)) * (a.x - c.x)
                               + (float)(Math.Pow(c.x, 2) + Math.Pow(c.y, 2)) * (b.x - a.x);
            float x = xNumerator / denominator;
            float y = yNumerator / denominator;
            float radius = (float)Math.Sqrt((Math.Pow((x - a.x), 2) + Math.Pow((y - a.y), 2)));
            return new Circle(new Point(x, y), radius);
        }


        /* return min circle given 1 point - using the following equation -
         * center (x,y) -
         * x= x1
         * y= y1
         * r = 0
         */
        static Circle onePointCircle(Point a)
        {
            return new Circle(a, 0);
        }

        /*given vector of points, and vector with points that sit on boundary of the min circle.
         * this function work recursively until getting min circle. */
        static Circle findMinWithBoundaryPoints(List<Point> points, List<Point> boundaryPoints, int pointsSize)
        {
            int randIdx = 0, size = pointsSize;
            //base case pointsSize == 0 || R.pointsSize ==3
            if (size == 0 || boundaryPoints.Count() == 3)
            {
                switch (boundaryPoints.Count())
                {
                    case 0:
                        return new Circle(new Point(0, 0), 0);
                    case 1:
                        return onePointCircle(boundaryPoints[0]);
                        break;
                    case 2:
                        return twoPointsCircle(boundaryPoints[0], boundaryPoints[1]);
                    case 3:
                        return threePointsCircle(boundaryPoints[0], boundaryPoints[1], boundaryPoints[2]);
                        break;
                    default:
                        break;

                }
            }
            //getting random Point from array
            if (size > 0)
            {
                Random r = new Random();
                randIdx = r.Next(size);
                Point randPoint = new Point(points[randIdx].x, points[randIdx].y);
                //sending the random point to the end of the vector
                // so in next call it we be out of boundary of the vector size
                swap(points[randIdx], points[size - 1]);

                //calculating min circle minCircle
                Circle minCircle = findMinWithBoundaryPoints(points, boundaryPoints, size - 1);
                //checking if the random point in the circle - if it is return the circle
                if (pointIsInsideCircle(minCircle, randPoint))
                {
                    return minCircle;
                }
                else
                { // else, point not in the circle, so it must be on the boundary
                  //insert point to boundary points vector
                    boundaryPoints.Add(randPoint);
                    //calling function with the new boundary point
                    return findMinWithBoundaryPoints(points, boundaryPoints, size - 1);
                }
            } else
            {
                return new Circle(new Point(0,0) ,0);
            }
            
        }

        private static void swap(Point point1, Point point2)
        {
            Point tmp = point1;
            point1 = point2;
            point2 = point1;
        }

        /*using welzl's algorithm returning minimum circle given array of points.
         * we will pick a random point p, remove it from circle and recursively find a min circle d,
         * if the point p is in that circle, return the circle we found.
         * else, p must be on min circle boundary, adding p to R array of points that is on the circle boundary.
         * base case - if Points array is empty, R size is 3 recursion stops.
         * if the base case is reached - if R size is 1, return circle with radius 0 and the point is in R.
         * R size 2 return min circle with 2 points, radius is half distance and also center is between 2 points.
         * R size 3 return min circle using equation to find circle using 3 points.
         */
        static public Circle findMinCircle(List<Point> points, int size)
        {
            List<Point> vectorPoints = new List<Point>(), boundaryPoints = new List<Point>();
            //base cases - size == 0 || 1 || 2 || 3
            switch (size)
            {
                case 0:
                    return new Circle( new Point(0, 0), 0);
                    break;
                case 1:
                    return onePointCircle(points[0]);
                    break;
                case 2:
                    return twoPointsCircle(points[0], points[1]);
                case 3:
                    return threePointsCircle(points[0], points[1], points[2]);
                    break;
                default:
                    break;
            }
            //inserting the array points into vector
            for (int i = 0; i < size; i++)
            {
                vectorPoints.Add(points[i]);
            }
            return findMinWithBoundaryPoints(vectorPoints, boundaryPoints, size);
        }
    }
}
