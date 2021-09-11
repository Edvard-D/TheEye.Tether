using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public struct Snapshot
    {
        private List<DataPoint> _dataPoints;
        private string _tableName;
        private float _timestamp;

        
        public List<DataPoint> DataPoints { get { return _dataPoints; } }
        public Snapshot(string tableName, float timestamp)
        {
            _dataPoints = new List<DataPoint>();
            _tableName = tableName;
            _timestamp = timestamp;
        }

        public void AddDataPoint(DataPoint dataPoint)
        {
            _dataPoints.Add(dataPoint);
        }
    }
}
