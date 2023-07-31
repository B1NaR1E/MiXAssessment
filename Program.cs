using MiXAssessment;
using MiXAssessment.Extentions;
using MiXAssessment.Models;
using System.Diagnostics;

var stopWatch = new Stopwatch();
var locations = InstantiateLocations();
var tree = new KdBush(2);

try
{
    Console.WriteLine("Loading vehicles into KD Tree.\n");

    stopWatch.Start();

    var test = LoadDataIntoKdTree();

    stopWatch.Stop();

    Console.WriteLine($"Data loaded. Total Time:{stopWatch.ElapsedMilliseconds}.\n");

    //if (vehicles.Count != 0)
    {
        Console.WriteLine("Starting Nearest Neighbors algorithm.\n");
        stopWatch.Restart();

        foreach (var location in locations)
        {
            var vehicle = tree.NearestNeighbors2(new double[] { location.Coordinates.Longitude, location.Coordinates.Latitude }, 1).FirstOrDefault();
            Console.WriteLine($"The closest vehicle to position: {location.PositionId} is vehicleId: {vehicle!.Item2}.");
        }

        stopWatch.Stop();

        Console.WriteLine();

        Console.WriteLine($"Algorithm completed. Total time elapsed: {stopWatch.ElapsedMilliseconds} ms.\n\n\n");

        Console.WriteLine("Starting brute force algorithm.\n");

        stopWatch.Restart();

        foreach (var location in locations)
        {
            var vehicle = test
                    .OrderBy(x =>
                    ((location.Coordinates.Latitude - x.Latitude) * (location.Coordinates.Latitude - x.Latitude)) +
                    ((location.Coordinates.Longitude - x.Longitude) * (location.Coordinates.Longitude - x.Longitude)))
                    .First();

            Console.WriteLine($"The closest vehicle to position: {location.PositionId} is vehicleId: {vehicle!.VehicleId}.");
        }

        stopWatch.Stop();

        Console.WriteLine();

        Console.WriteLine($"Algorithm completed. Total time elapsed: {stopWatch.ElapsedMilliseconds} ms.\n\n\n");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An unexpected error has occurred. Message: {ex.Message}");
}

#region Helper Methods

List<Vehicle> LoadDataIntoKdTree()
{
    List<double[]> data = new List<double[]>();
    List<int> nodes = new List<int>();
    List<Vehicle> result = new List<Vehicle>();

    if (!File.Exists("VehiclePositions.dat"))
    {
        Console.WriteLine($"The data file was not found.");
    }


    var dataBytes = File.ReadAllBytes("VehiclePositions.dat");

    using (var stream = new MemoryStream(dataBytes))
    {
        using (System.IO.BinaryReader reader = new System.IO.BinaryReader(stream))
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var id = reader.ReadInt32();
                var reg = reader.ReadAscii();
                var lat = reader.ReadSingle();
                var lon = reader.ReadSingle();
                var time = reader.ReadUInt64();

                data.Add(new double[] { lon, lat });
                nodes.Add(id);
                result.Add(new Vehicle(id, lon, lat));
                if (data.Count == 200000)
                { 

                    tree.Insert(data.ToArray(), nodes.ToArray()); 
                    data.Clear();
                    nodes.Clear();

                }
            }
        }
    }

    //tree.LoadData(data.Take(10).ToArray(), nodes.Take(10).ToArray());

    return result;
}

List<Position> InstantiateLocations()
{
    return new List<Position>
    {
        new Position(1, 34.544909f, -102.100843f),
        new Position(2, 32.345544f, -99.123124f),
        new Position(3, 33.234235f, -100.214124f),
        new Position(4, 35.195739f, -95.348899f),
        new Position(5, 31.895839f, -97.789573f),
        new Position(6, 32.895839f, -101.789573f),
        new Position(7, 34.115839f, -100.225732f),
        new Position(8, 32.335839f, -99.992232f),
        new Position(9, 33.535339f, -94.792232f),
        new Position(10, 32.234235f, -100.222222f)
    };
}

#endregion

