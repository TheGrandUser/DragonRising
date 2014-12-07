namespace DraconicEngine.Media
{
    public struct Size
    {
        double width, height;

        public double Width { get { return width; } }
        public double Height { get { return height; } }

        public static Size Empty { get { return new Size(); } }

        public Size(double width, double height)
        {
            this.width = width;
            this.height = height;
        }
    }
}