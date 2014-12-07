using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public struct Matrix
    {
        static readonly Matrix identity = new Matrix(1, 0, 0, 1, 0, 0);

        double m11, m12, m21, m22;
        double offsetX;
        double offsetY;

        public Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public double M11 { get { return m11; } }
        public double M12 { get { return m12; } }
        public double M21 { get { return m21; } }
        public double M22 { get { return m22; } }
        public double OffsetX { get { return offsetX; } }
        public double OffsetY { get { return offsetY; } }

        public bool IsIdentity { get { return m11 == 1 && m22 == 1 && m12 == 0 && m21 == 0 && offsetX == 0 && offsetY == 0; } }

        public static Matrix Identity { get { return identity; } }
    }

    public abstract class Transform
    {
        public static Transform Identity { get { return new MatrixTransform() { Matrix = Matrix.Identity }; } }
    }

    public sealed class MatrixTransform : Transform
    {
        public MatrixTransform()
        {
        }

        public Matrix Matrix { get; set; }
    }

    public sealed class RotateTransform : Transform
    {
        public RotateTransform()
        {
        }

        public RotateTransform(double angle)
        {
            this.Angle = angle;
        }

        public RotateTransform(double angle, double centerX, double centerY)
        {
            this.Angle = angle;
            this.CenterX = centerX;
            this.CenterY = centerY;
        }

        public double Angle { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
    }

    public sealed class TranslateTransform : Transform
    {
        public TranslateTransform()
        {
        }

        public TranslateTransform(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
}
