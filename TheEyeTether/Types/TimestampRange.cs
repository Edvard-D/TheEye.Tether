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


        public override bool Equals(object obj)
        {            
            return obj is TimestampRange && Equals((TimestampRange)obj);
        }
        public bool Equals(TimestampRange timestampRange)
        {
            return timestampRange._end == _end
                    && timestampRange._start == _start;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(_end, _start);
        }

        public static bool operator ==(TimestampRange timestampRange1, TimestampRange timestampRange2)
        {
            return timestampRange1.Equals(timestampRange2);
        }

        public static bool operator !=(TimestampRange timestampRange1, TimestampRange timestampRange2)
        {
            return !timestampRange1.Equals(timestampRange2);
        }

        public bool IsTimestampInRange(float timestamp)
        {
            return timestamp >= _start && timestamp < _end;
        }
    }
}
