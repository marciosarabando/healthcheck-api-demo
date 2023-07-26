using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text;
using System.Reflection;

namespace HealthCheckDemo.API.HealthCheck;

public static class HealthCheck
{
    private static string? _informationalVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
    private static DateTime? _dateAssembly = File.GetLastWriteTime(Assembly.GetEntryAssembly()?.Location ?? "");
    private static DateTime? _startTime = System.Diagnostics.Process.GetCurrentProcess().StartTime;
    private static string? _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";

    public static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            
            jsonWriter.WriteString("Status", healthReport.Status.ToString());
            jsonWriter.WriteString("Version", _informationalVersion);
            jsonWriter.WriteString("CurrentDateTime", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffff}");
            jsonWriter.WriteString("BuildDateTime", $"{_dateAssembly:yyyy-MM-dd HH:mm:ss.fffff}");
            jsonWriter.WriteString("StartDateTime", $"{_startTime:yyyy-MM-dd HH:mm:ss.fffff}");
            jsonWriter.WriteString("TimeZone Name", $"{TimeZoneInfo.Local.DisplayName}");
            jsonWriter.WriteString("TimeZone Daylight", $"{TimeZoneInfo.Local.DaylightName}");
            jsonWriter.WriteString("EnvironmentName", _environmentName);

            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}
