using System;
using System.Collections.Generic;

namespace MapTime.Handlers
{
    public static class LocationHandler
    {
        public static List<Location> SavedLocations { get; set; } = new List<Location>();

        public static void AddLocation(float latitude, float longitude)
        {
            SavedLocations.Add(new Location(latitude, longitude));
        }

        public static void AddLocation(Location loc)
        {
            SavedLocations.Add(loc);
        }
    }

    public class Location
    {
        public string Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public Location(float latitude = 0, float longitude = 0, string name = "")
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Name = name;
        }
    }
}
