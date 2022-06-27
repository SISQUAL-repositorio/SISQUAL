using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Services.Maps;


namespace LocationRetrieve
{
    public class Class1
    {
    }
    private async void reverseGeocodeButton_Click(object sender, RoutedEventArgs e)
    {
        // The location to reverse geocode.
        BasicGeoposition location = new BasicGeoposition();
        location.Latitude = 47.643;
        location.Longitude = -122.131;
        Geopoint pointToReverseGeocode = new Geopoint(location);

        // Reverse geocode the specified geographic location.
        MapLocationFinderResult result =
              await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

        // If the query returns results, display the name of the town
        // contained in the address of the first result.
        if (result.Status == MapLocationFinderStatus.Success)
        {
            tbOutputText.Text = "town = " +
                  result.Locations[0].Address.Town;
        }
    }
}

