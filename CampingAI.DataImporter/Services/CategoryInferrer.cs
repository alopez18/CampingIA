using System.Globalization;
using System.Text;

namespace CampingAI.DataImporter.Services;

/// <summary>
/// Infiere una lista ordenada de categorías para un camping a partir de su nombre,
/// ciudad, provincia y dirección.
/// El primer elemento es la categoría principal; el resto son categorías adicionales.
/// </summary>
internal static class CategoryInferrer {
    // Catálogo con GUIDs fijos (ver 12.SeedCategories.sql).
    // Orden de prioridad: las reglas más específicas van primero.
    internal static readonly (Guid Id, string[] Keywords)[] CategoryKeywords =
    [
        // Naturista — muy específico, máxima prioridad
        (new("B1000001-0000-0000-0000-000000000010"), ["naturist", "nudist", "naturista", "nudismo", "nudista"]),
        // Glamping / Lujo
        (new("B1000001-0000-0000-0000-000000000006"), ["glamping", "lujo", "luxury", "resort", "boutique"]),
        // Playa / Costero
        (new("B1000001-0000-0000-0000-000000000002"), ["playa", "beach", "costa ", "costero", "litoral", "cala", "marina", "maritim", "maritimo"]),
        // Lago / Río
        (new("B1000001-0000-0000-0000-000000000005"), ["lago", " lake", "pantano", "embalse", "ribera", "fluvial", " rio ", "riera"]),
        // Montaña
        (new("B1000001-0000-0000-0000-000000000003"), ["montanya", "montana", "sierra", " monte ", "pirineu", "pirineo", "cordillera", "alpin", "cumbre", "pico", "serr"]),
        // Aventura / Deportivo
        (new("B1000001-0000-0000-0000-000000000007"), ["aventura", "adventure", "deportiv", "sport", "rafting", "kayak", "senderismo", "escalada", "activ"]),
        // Rural / Naturaleza
        (new("B1000001-0000-0000-0000-000000000004"), ["rural", "naturaleza", "nature", "bosque", "forest", "campo", "finca", "agro", "masia", "granja"]),
        // Familiar
        (new("B1000001-0000-0000-0000-000000000001"), ["familiar", "family", "infantil", "familia"]),
        // Tranquilo / Relax
        (new("B1000001-0000-0000-0000-000000000008"), ["tranquil", "relax", "descans", "zen", "quiet", "paz", "repos"]),
        // Urbano / City
        (new("B1000001-0000-0000-0000-000000000009"), ["urban", "city", "ciudad", "centre", "centro"]),
    ];

    // GUIDs de todas las categorías para el fallback aleatorio.
    internal static readonly Guid[] AllCategoryIds = CategoryKeywords.Select(c => c.Id).ToArray();

    private static readonly Guid BeachCategoryId = new("B1000001-0000-0000-0000-000000000002");

    // Provincias costeras españolas (código INE de 2 dígitos).
    private static readonly HashSet<string> CoastalProvinceCodes =
    [
        "04", // Almería
        "11", // Cádiz
        "21", // Huelva
        "29", // Málaga
        "30", // Murcia
        "03", // Alicante
        "12", // Castellón
        "46", // Valencia
        "43", // Tarragona
        "08", // Barcelona
        "17", // Girona
        "07", // Illes Balears
        "35", // Las Palmas
        "38", // Santa Cruz de Tenerife
        "15", // A Coruña
        "27", // Lugo
        "33", // Asturias
        "39", // Cantabria
        "48", // Bizkaia
        "20", // Gipuzkoa
    ];

    /// <summary>
    /// Devuelve todas las categorías que aplican al camping, ordenadas por prioridad.
    /// El primer elemento debe usarse como categoría principal (CMP_CategoryId).
    /// Los demás son categorías adicionales para T_CAMPING_CATEGORIES.
    /// Nunca devuelve una lista vacía: si no hay match, hace fallback determinista.
    /// </summary>
    internal static IReadOnlyList<Guid> InferAll(Guid campingId, string name, string? city, string? province, string? address) {
        var searchText = Normalize(string.Join(" ", new[] { name, city ?? string.Empty, province ?? string.Empty, address ?? string.Empty }));

        var matched = new List<Guid>();

        // 1) Coincidencia por palabras clave (respeta el orden de prioridad).
        foreach (var (id, keywords) in CategoryKeywords) {
            if (keywords.Any(k => searchText.Contains(Normalize(k), StringComparison.Ordinal)))
                matched.Add(id);
        }

        // 2) Señal secundaria: provincia costera → Playa / Costero.
        if (!matched.Contains(BeachCategoryId) && !string.IsNullOrWhiteSpace(province)) {
            if (CoastalProvinceCodes.Contains(province.Trim()))
                matched.Add(BeachCategoryId);
        }

        // 3) Fallback determinista si no se encontró ningún match.
        if (matched.Count == 0) {
            var seed = BitConverter.ToInt32(campingId.ToByteArray(), 0);
            var rnd  = new Random(seed);
            matched.Add(AllCategoryIds[rnd.Next(AllCategoryIds.Length)]);
        }

        return matched;
    }

    internal static string Normalize(string value) {
        var trimmed    = value.Trim().ToLowerInvariant();
        var decomposed = trimmed.Normalize(NormalizationForm.FormD);
        var sb         = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed) {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
