namespace WindowPlatformer;

public static class Easing
{
    public static class In
    {
        public static f32 Sqr(f32 t) => t*t;
        public static f32 Cube(f32 t) => t*t*t;
        public static f32 Rad(f32 t) => -f32.Cos(f32.Pi/2f * t) + 1f;
        public static f32 Back(f32 t) => 2.70158f * t*t*t - 1.70158f * t*t;
        public static f32 Circ(f32 t) => 1f - f32.Sqrt(1f - t*t);
        public static f32 Exp(f32 t) => Exp(t, f32.E, 2f);
        public static f32 Exp(f32 t, f32 b, f32 f) => f32.Pow(b, f * (t - 1f)) * t;
    }

    public static class Out
    {
        public static f32 Sqr(f32 t) => -t*t + 2f*t;
        public static f32 Cube(f32 t) => f32.Pow(t-1f, 3f) + 1f;
        public static f32 Back(f32 t) => 1f + 2.70158f * f32.Pow(t-1f, 3f) + 1.70158f * f32.Pow(t-1f, 2f);
        public static f32 Rad(f32 t) => f32.Sin(f32.Pi/2f * t);
        public static f32 Circ(f32 t) => f32.Sqrt(1f - f32.Pow(t-1f, 2f));
        public static f32 Exp(f32 t) => Exp(t, f32.E, 4f);
        public static f32 Exp(f32 t, f32 b, f32 f) => 1f - f32.Pow(b, -f * t);
    }

    public static class InOut
    {
        public static f32 Linear(f32 t) => t;
        public static f32 Rad(f32 t) => -0.5f * (f32.Cos(f32.Pi * t) - 1f);
        public static f32 Asin(f32 t) => f32.AsinPi(2f*t - 1f) + 0.5f;
        public static f32 SmoothStep(f32 t) => f32.Lerp(In.Sqr(t), Out.Sqr(t), t);
    }
}