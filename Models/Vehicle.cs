namespace MiXAssessment.Models;

internal class Vehicle
{
    public Vehicle(int vehicleId, string vehicleRegistration, float latitude, float longitude, ulong recordedTimeUTC)
    {
        VehicleId = vehicleId;
        VehicleRegistration = vehicleRegistration;
        Latitude = latitude;
        Longitude = longitude;
        RecordedTimeUTC = recordedTimeUTC;
    }

    public int VehicleId { get; set; }
    public string VehicleRegistration { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public ulong RecordedTimeUTC { get; set; }
}

