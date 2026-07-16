using System.Reflection;
using System.Text.Json;
using CampingAI.DataImporter.Models.Geo;
using CampingAI.DataImporter.Services.Interfaces;

namespace CampingAI.DataImporter.Services;

// Resuelve la provincia de un punto (lat/lon) mediante point-in-polygon offline
// contra los contornos provinciales embebidos (Resources/spain-provinces.geojson).
// El GeoJSON expone el código INE (cod_prov) que se traduce al PRV_Code interno.
public class ProvinceGeoResolver : IProvinceGeoResolver {
    #region Dependencies
    readonly ILogger<ProvinceGeoResolver> _logger;
    #endregion

    const string ResourceName = "CampingAI.DataImporter.Resources.spain-provinces.geojson";

    // Mapa código INE (2 dígitos) → PRV_Code interno (ver 3.CreateLocationTables.sql).
    static readonly IReadOnlyDictionary<string, string> IneToProvinceCode = new Dictionary<string, string> {
        ["01"] = "ALA",  ["02"] = "ALB",  ["03"] = "ALC",  ["04"] = "ALM",
        ["05"] = "AVI",  ["06"] = "BAD",  ["07"] = "BAL",  ["08"] = "BCN",
        ["09"] = "BUR",  ["10"] = "CAC",  ["11"] = "CAD",  ["12"] = "CST",
        ["13"] = "CIU",  ["14"] = "COR",  ["15"] = "COR2", ["16"] = "CUE",
        ["17"] = "GIR",  ["18"] = "GRA",  ["19"] = "GUA",  ["20"] = "GUI",
        ["21"] = "HUE",  ["22"] = "HUS",  ["23"] = "JAE",  ["24"] = "LEO",
        ["25"] = "LLE",  ["26"] = "RIO",  ["27"] = "LUG",  ["28"] = "MAD",
        ["29"] = "MAL",  ["30"] = "MUR",  ["31"] = "NAV",  ["32"] = "OUR",
        ["33"] = "AST",  ["34"] = "PAL",  ["35"] = "LPA",  ["36"] = "PON",
        ["37"] = "SAL",  ["38"] = "TFE",  ["39"] = "CAN",  ["40"] = "SEG",
        ["41"] = "SEV",  ["42"] = "SOR",  ["43"] = "TAR",  ["44"] = "TER",
        ["45"] = "TOL",  ["46"] = "VLC",  ["47"] = "VAL",  ["48"] = "VIZ",
        ["49"] = "ZAM",  ["50"] = "ZAR",  ["51"] = "CEU",  ["52"] = "MEL",
    };

    readonly List<ProvinceGeometry> _provinces;

    public ProvinceGeoResolver(ILogger<ProvinceGeoResolver> logger) {
        _logger = logger;
        _provinces = LoadProvinces();
        _logger.LogInformation("ProvinceGeoResolver cargado con {Count} geometrías provinciales.", _provinces.Count);
    }

    public string? ResolveProvinceCode(double latitude, double longitude) {
        // GeoJSON usa el orden [longitud, latitud]; aquí x = lon, y = lat.
        foreach (var province in _provinces) {
            if (longitude < province.MinX || longitude > province.MaxX ||
                latitude < province.MinY || latitude > province.MaxY)
                continue;

            foreach (var polygon in province.Polygons) {
                if (polygon.Contains(longitude, latitude))
                    return province.Code;
            }
        }

        return null;
    }

    // ── Carga del GeoJSON embebido ──────────────────────────────────────────

    private List<ProvinceGeometry> LoadProvinces() {
        var result = new List<ProvinceGeometry>();

        try {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)
                ?? throw new InvalidOperationException($"No se encontró el recurso embebido '{ResourceName}'.");

            var collection = JsonSerializer.Deserialize<GeoJsonFeatureCollection>(stream)
                ?? new GeoJsonFeatureCollection();

            foreach (var feature in collection.Features) {
                var ine = feature.Properties.CodProv?.Trim();
                if (string.IsNullOrEmpty(ine) || !IneToProvinceCode.TryGetValue(ine, out var code))
                    continue;

                var polygons = ParseGeometry(feature.Geometry);
                if (polygons.Count == 0)
                    continue;

                result.Add(new ProvinceGeometry(code, polygons));
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al cargar los contornos provinciales desde el GeoJSON embebido.");
        }

        return result;
    }

    private static List<Polygon> ParseGeometry(GeoJsonGeometry geometry) {
        var polygons = new List<Polygon>();

        switch (geometry.Type) {
            case "Polygon":
                // coordinates: [ ring, hole, ... ]
                polygons.Add(ParsePolygon(geometry.Coordinates));
                break;

            case "MultiPolygon":
                // coordinates: [ polygon, polygon, ... ]
                foreach (var polygonElement in geometry.Coordinates.EnumerateArray())
                    polygons.Add(ParsePolygon(polygonElement));
                break;
        }

        return polygons.Where(p => p.Outer.Points.Length > 0).ToList();
    }

    private static Polygon ParsePolygon(JsonElement polygonElement) {
        Ring? outer = null;
        var holes = new List<Ring>();

        var ringIndex = 0;
        foreach (var ringElement in polygonElement.EnumerateArray()) {
            var ring = ParseRing(ringElement);
            if (ringIndex == 0)
                outer = ring;
            else
                holes.Add(ring);
            ringIndex++;
        }

        return new Polygon(outer ?? new Ring(Array.Empty<(double, double)>()), holes);
    }

    private static Ring ParseRing(JsonElement ringElement) {
        var points = new List<(double X, double Y)>();
        foreach (var position in ringElement.EnumerateArray()) {
            var lon = position[0].GetDouble();
            var lat = position[1].GetDouble();
            points.Add((lon, lat));
        }
        return new Ring(points.ToArray());
    }

    // ── Estructuras geométricas ─────────────────────────────────────────────

    private sealed class ProvinceGeometry {
        public string Code { get; }
        public List<Polygon> Polygons { get; }
        public double MinX { get; }
        public double MinY { get; }
        public double MaxX { get; }
        public double MaxY { get; }

        public ProvinceGeometry(string code, List<Polygon> polygons) {
            Code = code;
            Polygons = polygons;

            MinX = MinY = double.MaxValue;
            MaxX = MaxY = double.MinValue;
            foreach (var polygon in polygons) {
                foreach (var (x, y) in polygon.Outer.Points) {
                    if (x < MinX) MinX = x;
                    if (x > MaxX) MaxX = x;
                    if (y < MinY) MinY = y;
                    if (y > MaxY) MaxY = y;
                }
            }
        }
    }

    private sealed class Polygon {
        public Ring Outer { get; }
        public List<Ring> Holes { get; }

        public Polygon(Ring outer, List<Ring> holes) {
            Outer = outer;
            Holes = holes;
        }

        public bool Contains(double x, double y) {
            if (!Outer.Contains(x, y))
                return false;

            foreach (var hole in Holes) {
                if (hole.Contains(x, y))
                    return false;
            }

            return true;
        }
    }

    private sealed class Ring {
        public (double X, double Y)[] Points { get; }

        public Ring((double X, double Y)[] points) {
            Points = points;
        }

        // Algoritmo ray casting (par-impar).
        public bool Contains(double x, double y) {
            var inside = false;
            var n = Points.Length;
            for (int i = 0, j = n - 1; i < n; j = i++) {
                var (xi, yi) = Points[i];
                var (xj, yj) = Points[j];

                var intersects = ((yi > y) != (yj > y)) &&
                                 (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
                if (intersects)
                    inside = !inside;
            }
            return inside;
        }
    }
}
