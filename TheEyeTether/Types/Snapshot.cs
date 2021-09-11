namespace TheEyeTether.Types
{
    public struct Snapshot
    {
        private string _tableName;
        private float _timestamp;


        public Snapshot(string tableName, float timestamp)
        {
            _tableName = tableName;
            _timestamp = timestamp;
        }
    }
}
