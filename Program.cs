using MiXAssessment;
using MiXAssessment.Extentions;
using MiXAssessment.Models;
using System.Diagnostics;
using System.IO;

var stopWatch = new Stopwatch();
var locations = InstantiateLocations();
var tree = new KdBush(2);
long totalTime  = 0;

//try
{
    //Console.WriteLine("Loading vehicles into K Dimentional Tree.\n");

    //stopWatch.Start();

    //LoadDataIntoKdTree();

    //stopWatch.Stop();
    //totalTime += stopWatch.ElapsedMilliseconds;
    //Console.WriteLine($"Data loaded. Time Elapsed: {stopWatch.ElapsedMilliseconds} ms.\n");

    Console.WriteLine("Loading vehicles into K Dimentional Tree.\n");

    await LoadDataIntoKdTree();

    //Console.WriteLine("Starting Nearest Neighbors algorithm.\n");

    //stopWatch.Restart();

    //foreach (var location in locations)
    //{
    //    var vehicle = tree.NearestNeighbors(new double[] { location.Coordinates.Longitude, location.Coordinates.Latitude }, 1).FirstOrDefault();
    //    Console.WriteLine($"The closest vehicle to position: {location.PositionId} is vehicleId: {vehicle!.Item2}. With Distance: {vehicle!.Item1} m");
    //}

    //stopWatch.Stop();

    //totalTime += stopWatch.ElapsedMilliseconds;

    //Console.WriteLine();

    //Console.WriteLine($"Algorithm completed. Time Elapsed: {stopWatch.ElapsedMilliseconds} ms.\n");

    //Console.WriteLine($"Total Time(s): {totalTime / 1000} s.\nTotal Time(ms): {totalTime} ms.");

}
//catch (Exception ex)
//{
//    Console.WriteLine($"An unexpected error has occurred. Message: {ex.Message}");
//}

#region Helper Methods

// This method takes on average 10 seconds to read all the data and build the K Dimentional Tree. Need to find a way to speed this method up.
async Task LoadDataIntoKdTree()
{
    int count = 0;
    var result = new List<Vehicle>();
    Stopwatch sWatch = new Stopwatch();

    if (!File.Exists("VehiclePositions.dat"))
    {
        Console.WriteLine($"The data file was not found.");
    }

    var dataBytes = File.ReadAllBytes("VehiclePositions.dat");

    var dataL = dataBytes.Length / 2000001;
    var bufferSize = 200 * dataL;

    using (var stream = new MemoryStream(dataBytes))
    {
        sWatch.Start();

        using (System.IO.BinaryReader reader = new System.IO.BinaryReader(stream))
        {
            do
            {
                var id = reader.ReadInt32();
                var reg = reader.ReadAscii();
                var lat = reader.ReadSingle();
                var lon = reader.ReadSingle();
                var time = reader.ReadUInt64();
                ++count;
            }
            while (reader.BaseStream.Position < reader.BaseStream.Length);

        }

        sWatch.Stop();
        Console.WriteLine($"Data loaded. Time Elapsed: {sWatch.ElapsedMilliseconds} ms. Total Rec: {count}\n");
        count = 0;

    }

    using (var stream = new MemoryStream(dataBytes))
    {
        sWatch.Restart();
        using (BufferedBinaryReader reader = new BufferedBinaryReader(stream, bufferSize))
        {
            do
            {
                while (await reader.FillBufferAsync())
                {
                    for (int i = 0; i < 199; i++)
                    {
                        var id = reader.ReadInt32();
                        var reg = reader.ReadAscii();
                        var lat = reader.ReadSingle();
                        var lon = reader.ReadSingle();
                        var time = reader.ReadUInt64();
                        Console.WriteLine($"String byte length = {reg}");
                    }
                    ++count;
                }
            }
            while (stream.Position < stream.Length);
        }

        sWatch.Stop();
        Console.WriteLine($"Data loaded. Time Elapsed: {sWatch.ElapsedMilliseconds} ms. Total Rec: {count}\n"); 
    }

    //return count;
}

//int LoadData()
//{
//    int count = 0;
//    if (!File.Exists("VehiclePositions.dat"))
//    {
//        Console.WriteLine($"The data file was not found.");
//    }

//    var dataBytes = File.ReadAllBytes("VehiclePositions.dat");

//    var reader = new BufferedBinaryReader(dataBytes);

//    do
//    {
//        var id = reader.ReadInt32();
//        var reg = reader.ReadAscii();
//        var lat = reader.ReadSingle();
//        var lon = reader.ReadSingle();
//        var time = reader.ReadUInt64();
//        ++count;
//    }
//    while (reader.Position < dataBytes.Length);

//    return count;
//}

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

