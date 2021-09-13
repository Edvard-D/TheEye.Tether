namespace TheEyeTether.Types
{
    public record TimestampRange
    {
        public float End;
        public float Start;


        public TimestampRange(float start, float end) => (Start, End) = (start, end);
    }
}
