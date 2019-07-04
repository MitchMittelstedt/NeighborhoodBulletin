using System;
using Domain;

namespace Infrastructure
{
    public interface IGoogleMaps
    {
        Location GetLocationByPostalCode(int zipCode);
    }
}
