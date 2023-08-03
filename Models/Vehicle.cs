
using System.Runtime.InteropServices;

namespace MiXAssessment.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
internal struct Vehicle
{
    public int VehicleId { get; set; }

    public string VehicleRegistration { get; set; }

    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public ulong RecordedTimeUTC { get; set; }
}

