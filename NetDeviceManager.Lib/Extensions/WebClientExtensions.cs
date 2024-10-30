using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Lib.Extensions;

//
public static class WebClientExtensions
{
    private static ILogger? _logger;

    public static void Initialize(ILogger? logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Method <c>DownloadObjectAsJson<T>(this WebClient webClient, Uri uri)</c> extension method for obsoled webClient for downloading object from json rest api.
    /// </summary>
    public static T DownloadObjectAsJson<T>(this WebClient webClient, Uri uri)
    {
        T output;
        try
        {
            var response = webClient.DownloadString(uri);
            output = JsonSerializer.Deserialize<T>(response) ?? Activator.CreateInstance<T>();
            
            if (_logger != null)
                _logger.LogInformation($"Successfully downloaded object as json from {uri}");
            else
                Debug.WriteLine($"Successfully downloaded object as json from {uri}");
        }
        catch (Exception e)
        {
            output = Activator.CreateInstance<T>();
            
            if (_logger != null)
                _logger.LogInformation($"Error while downloading object as json from {uri} - Message: {e.Message}");
            else
                Debug.WriteLine($"Error while downloading object as json from {uri} - Message: {e.Message}");
        }

        return output;
    }
}