using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CampingAI.WebApi.Converters;
public class DateTimeConverter : JsonConverter<DateTime> {
    private const string Format = "yyyy-MM-ddTHH:mm:ss'Z'";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        // Las fechas se manejan siempre en UTC: si el cliente no envía zona, se asume UTC.
        return DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) {
        // Los valores persistidos son UTC (Kind puede venir Unspecified desde Dapper).
        var utc = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
        writer.WriteStringValue(utc.ToString(Format, CultureInfo.InvariantCulture));
    }
}
