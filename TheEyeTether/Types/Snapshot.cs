using System.Collections.Generic;

namespace TheEyeTether.Types
{
    public struct Snapshot
    {
        private DataPoint _dataPoint;
        private List<string> _dataPoints;

        
        public DataPoint DataPoint { get { return _dataPoint; } }
        public List<string> DataPointsIds { get { return _dataPoints; } }


        public Snapshot(DataPoint dataPoint)
        {
            _dataPoint = dataPoint;
            _dataPoints = new List<string>();
        }

        public override bool Equals(object obj)
        {            
            return obj is Snapshot && Equals((Snapshot)obj);
        }
        public bool Equals(Snapshot snapshot)
        {
            return snapshot._dataPoint == _dataPoint
                    && snapshot._dataPoints == _dataPoints;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(_dataPoint, _dataPoints);
        }

        public static bool operator ==(Snapshot snapshot1, Snapshot snapshot2)
        {
            return snapshot1.Equals(snapshot2);
        }

        public static bool operator !=(Snapshot snapshot1, Snapshot snapshot2)
        {
            return !snapshot1.Equals(snapshot2);
        }
    }
}
