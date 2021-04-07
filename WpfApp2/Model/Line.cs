namespace DesktopFGApp.Model
{
    public class Line
    {
        public float a, b;
        public Line()
        {
            this.a = 0;
            this.b = 0;
        }
        public Line(float a, float b)
        {
            this.a = a;
            this.b = b;
        }
        public float f(float x)
        {
            return this.a * x + this.b;
        }
    }
}
