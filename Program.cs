using MiXAssessment.Extentions;
using MiXAssessment.Models;
using System.Diagnostics;
using System.Text;

var stopWatch = new Stopwatch();
var locations = InstantiateLocations();

try
{
    Console.WriteLine("Loading vehicle locations into memory.\n");

    var vehicles = GetVehiclesFromFile();

    if (vehicles.Count != 0)
    {
        Console.WriteLine($"Vehicles loaded. Total:{vehicles.Count()}.\n");

        Console.WriteLine("Starting location algorithm.\n");

        stopWatch.Start();

        foreach (var location in locations)
        {
            var vehicle = vehicles
                .OrderBy(x =>
                ((location.Latitude - x.Latitude) * (location.Latitude - x.Latitude)) +
                ((location.Longitude - x.Longitude) * (location.Longitude - x.Longitude)))
                .First();

            Console.WriteLine($"The closest vehicle to position: {location.PositionId} is vehicleId: {vehicle.VehicleId}, Registration No: {vehicle.VehicleRegistration}.");
        }

        stopWatch.Stop();

        Console.WriteLine();

        Console.WriteLine($"Algorithm completed. Total time elapsed: {stopWatch.ElapsedMilliseconds} ms.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An unexpected error has occurred. Message: {ex.Message}");
} 

#region Helper Methods

static List<Vehicle> GetVehiclesFromFile()
{
    List<Vehicle> result = new List<Vehicle>();

    if(!File.Exists("VehiclePositions.dat"))
    {
        Console.WriteLine($"The data file was not found.");
        return result;
    }

    using (FileStream fileStream = File.Open("VehiclePositions.dat", FileMode.Open))
    {
        using (System.IO.BinaryReader reader = new System.IO.BinaryReader(fileStream, Encoding.UTF8))
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                result.Add(new Vehicle(
                    reader.ReadInt32(),
                    reader.ReadAscii(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadUInt64()));
            }
        }
    }

    return result;
}

static List<Location> InstantiateLocations()
{
    return new List<Location>
    {
        new Location(1, 34.544909f, -102.100843f),
        new Location(2, 32.345544f, -99.123124f),
        new Location(3, 33.234235f, -100.214124f),
        new Location(4, 35.195739f, -95.348899f),
        new Location(5, 31.895839f, -97.789573f),
        new Location(6, 32.895839f, -101.789573f),
        new Location(7, 34.115839f, -100.225732f),
        new Location(8, 32.335839f, -99.992232f),
        new Location(9, 33.535339f, -94.792232f),
        new Location(10, 32.234235f, -100.222222f)
    };
} 
#endregion

