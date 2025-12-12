using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NFramework;
using UnityEngine;

public class GeoManager : SingletonMono<GeoManager>
{
    public GeolocationData data;
    public async Task CheckUserLocationAsync()
    {
        string ipApiUrl = "http://ip-api.com/json";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                string jsonResponse = await client.GetStringAsync(ipApiUrl);
                data = JsonUtility.FromJson<GeolocationData>(jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("Failed to retrieve geolocation data: " + e.Message);
            }
        }
    }

    public bool IsUserInEU()
    {
        // Check if the user's country is in the EU
        string[] euCountryCodes = { "PL", "PT", "RO", "SK", "SI", "ES", "SE", "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR", "DE", "GR", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL", "GB" };
        return Array.Exists(euCountryCodes, countryCode => countryCode == data.countryCode);
    }

    [Serializable]
    public class GeolocationData
    {
        public string countryCode;
    }

}
