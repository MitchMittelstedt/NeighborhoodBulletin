using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Domain;
using unirest;

namespace Infrastructure
{
    public class GoogleMaps : IGoogleMaps
    {
        public Location GetLocationByPostalCode(int zipCode)
        {

            //string url = $"{zipCode}";
            //var response = Unirest.get(url)
            //    .header()
            Location location = new Location();
            return location;
        }
    }
}
