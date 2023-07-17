namespace MiXAssessment.Models;
internal class Position
{
    public Position(int positionId, float latitude, float longitude)
    {
        PositionId = positionId;
        Latitude = latitude;
        Longitude = longitude;
    }

    public int PositionId { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
}