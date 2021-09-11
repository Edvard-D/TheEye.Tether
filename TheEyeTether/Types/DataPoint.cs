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
    }
}
