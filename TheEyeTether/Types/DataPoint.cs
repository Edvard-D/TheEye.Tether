namespace TheEyeTether.Types
{
    public struct DataPoint
    {
        private string _name;
        private float _timestamp;
        private string _type;


        public string Name { get { return _name; } }
        public float Timestamp { get { return _timestamp; } }
        public string Type { get { return _type; } }


        public DataPoint(string type, string name, float timestamp)
        {
            _name = name;
            _timestamp = timestamp;
            _type = type;
        }

        public override bool Equals(object obj)
        {            
            return obj is DataPoint && Equals((DataPoint)obj);
        }
        public bool Equals(DataPoint dataPoint)
        {
            return dataPoint._name == _name
                    && dataPoint._timestamp == _timestamp
                    && dataPoint._type == _type;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(_name, _timestamp, _type);
        }

        public static bool operator ==(DataPoint dataPoint1, DataPoint dataPoint2)
        {
            return dataPoint1.Equals(dataPoint2);
        }

        public static bool operator !=(DataPoint dataPoint1, DataPoint dataPoint2)
        {
            return !dataPoint1.Equals(dataPoint2);
        }
    }
}
