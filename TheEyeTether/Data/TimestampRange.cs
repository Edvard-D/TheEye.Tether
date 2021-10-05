namespace TheEyeTether.Data
{
    public record TimestampRange
    {
        public double End;
        public double Start;


        public TimestampRange(double start, double end) => (Start, End) = (start, end);
    }
}
