namespace TheEyeTether.Types
{
    public struct DataPoint
    {
        private string _subTypeName;
        private float _timestamp;
        private string _typeName;


        public string SubTypeName { get { return _subTypeName; } }
        public float Timestamp { get { return _timestamp; } }
        public string TypeName { get { return _typeName; } }


        public DataPoint(string type, string name, float timestamp)
        {
            _subTypeName = name;
            _timestamp = timestamp;
            _typeName = type;
        }

        public override bool Equals(object obj)
        {            
            return obj is DataPoint && Equals((DataPoint)obj);
        }
        public bool Equals(DataPoint dataPoint)
        {
            return dataPoint._subTypeName == _subTypeName
                    && dataPoint._timestamp == _timestamp
                    && dataPoint._typeName == _typeName;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(_subTypeName, _timestamp, _typeName);
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
