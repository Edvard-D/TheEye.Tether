namespace TheEyeTether.Types
{
    public struct TimestampRange
    {
        private float _end;
        private float _start;

        
        public float End { get { return _end; } }
        public float Start { get { return _start; } }

        
        public TimestampRange(float start, float end)
        {
            _end = end;
            _start = start;
        }
    }
}
