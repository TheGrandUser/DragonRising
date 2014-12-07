using System;
using System.Numerics;

namespace DraconicEngine.Media
{
    public struct Rect
    {
        double x, y, width, height;

        public double X { get { return x; } }
        public double Y { get { return y; } }
        public double Width { get { return width; } }
        public double Height { get { return height; } }

        public Rect(double width, double height)
        {
            this.x = 0;
            this.y = 0;
            this.width = width;
            this.height = height;
        }

        public Rect(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rect(Size size)
        {
            this.x = 0;
            this.y = 0;
            this.width = size.Width;
            this.height = size.Height;
        }

        public Rect(Point location, Size size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }

        public Rect(Point point1, Point point2)
        {
            this.x = Math.Min(point1.X, point2.X);
            this.y = Math.Min(point1.Y, point2.Y);
            this.width = Math.Abs(point2.X - point1.X);
            this.height = Math.Abs(point2.Y - point1.Y);
        }

        public static Rect Empty { get { return new Rect(); } }
    }
}