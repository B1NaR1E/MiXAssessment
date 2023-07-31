namespace MiXAssessment.Models;
internal class Position
{
    public Position(int positionId, double latitude, double longitude)
    {
        PositionId = positionId;
        Coordinates = new Coordinate(latitude, longitude);
    }

    public int PositionId { get; private set; }
    public Coordinate Coordinates { get; private set; }
}