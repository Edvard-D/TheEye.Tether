namespace TheEye.Tether.Data
{
	public record DataPointSetting
	{
		public string EndMarker;
		public int EndMarkerPosition;
		public int[] SubTypeCategoryPositions;


		public DataPointSetting(string endMarker, int endMarkerPosition, int[] subTypeCategoryPositions) =>
				(EndMarker, EndMarkerPosition, SubTypeCategoryPositions) =
						(endMarker, endMarkerPosition, subTypeCategoryPositions);

		public DataPointSetting() =>
				(EndMarker, EndMarkerPosition, SubTypeCategoryPositions) = (string.Empty, -1, new int[0]);
	}
}
