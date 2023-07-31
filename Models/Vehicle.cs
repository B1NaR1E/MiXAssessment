
namespace MiXAssessment.Models;

internal class Vehicle
{
    public Vehicle(int vehicleId, double latitude, double longitude)
    {
        VehicleId = vehicleId;
        Latitude = latitude;
        Longitude = longitude;
    }

    public int VehicleId { get; set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }
}

