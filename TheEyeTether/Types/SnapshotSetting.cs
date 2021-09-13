namespace TheEyeTether.Types
{
    public struct SnapshotSetting
    {
        private string _name;
        private string[] _dataPointTypeNames;


        public string Name { get { return _name; } }
        public string[] DataPointTypeNames { get { return _dataPointTypeNames; } }


        public SnapshotSetting(
                string name,
                string[] dataPointTypeNames)
        {
            _name = name;
            _dataPointTypeNames = dataPointTypeNames;
        }
    }
}
