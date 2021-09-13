namespace TheEyeTether.Types
{
    public record DataPointSetting
    {
        public string EndMarker;
        public int EndMarkerPosition;


        public DataPointSetting(string endMarker, int endMarkerPosition) =>
                (EndMarker, EndMarkerPosition) = (endMarker, endMarkerPosition);

        public DataPointSetting() => (EndMarker, EndMarkerPosition) = (null, -1);
    }
}
