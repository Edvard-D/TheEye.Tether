namespace TheEyeTether.Types
{
    public struct SnapshotType
    {
        private string _name;


        public string Name { get { return _name; } }


        public SnapshotType(string name)
        {
            _name = name;
        }
    }
}
