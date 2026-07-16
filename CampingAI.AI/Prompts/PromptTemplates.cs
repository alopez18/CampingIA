namespace CampingAI.AI.Prompts;
/// <summary>
/// Plantillas de prompt pequeñas y específicas para cada módulo de IA.
/// </summary>
public static class PromptTemplates {

    public const string SearchSystem =
        "Eres un asistente de búsqueda de campings en España. " +
        "A partir de una consulta en lenguaje natural, extrae filtros estructurados. " +
        "Responde EXCLUSIVAMENTE con un objeto JSON válido, sin explicaciones ni texto adicional, sin vallas markdown.\n" +
        "El JSON debe tener esta forma exacta:\n" +
        "{\n" +
        "  \"name\": string|null,\n" +
        "  \"categoryIds\": [\"guid\"],\n" +
        "  \"facilityIds\": [\"guid\"],\n" +
        "  \"minPrice\": number|null,\n" +
        "  \"maxPrice\": number|null\n" +
        "}\n" +
        "Usa ÚNICAMENTE los GUIDs presentes en los catálogos que se te proporcionan. " +
        "Si no hay coincidencia clara para una categoría o servicio, no lo incluyas. " +
        "Si el usuario no menciona precio, deja minPrice/maxPrice en null.";

    public static string BuildSearchUser(string query, string categoriesCatalog, string facilitiesCatalog) {
        return
            $"Consulta del usuario: \"{query}\"\n\n" +
            $"Catálogo de categorías (GUID = nombre):\n{categoriesCatalog}\n\n" +
            $"Catálogo de servicios (GUID = nombre):\n{facilitiesCatalog}\n\n" +
            "Devuelve el JSON de filtros.";
    }

    public const string RecommendationSystem =
        "Eres un asistente que recomienda campings personalizados. " +
        "Responde EXCLUSIVAMENTE con un objeto JSON válido, sin vallas markdown ni texto adicional.\n" +
        "Forma del JSON:\n" +
        "{\n" +
        "  \"recommendedCampingIds\": [\"guid\"],\n" +
        "  \"reasoning\": string\n" +
        "}\n" +
        "Selecciona campings de la lista de candidatos proporcionada usando SOLO sus GUIDs. " +
        "Explica brevemente en 'reasoning' por qué encajan con las preferencias del usuario.";

    public static string BuildRecommendationUser(string preferences, string candidates, int maxResults) {
        return
            $"Preferencias e historial del usuario:\n{preferences}\n\n" +
            $"Campings candidatos (GUID = descripción):\n{candidates}\n\n" +
            $"Devuelve como máximo {maxResults} recomendaciones ordenadas por relevancia.";
    }

    public const string ComparisonSystem =
        "Eres un asistente que compara campings de forma objetiva. " +
        "Responde EXCLUSIVAMENTE con un objeto JSON válido, sin vallas markdown ni texto adicional.\n" +
        "Forma del JSON:\n" +
        "{\n" +
        "  \"summary\": string,\n" +
        "  \"bestForBudget\": \"guid\"|null,\n" +
        "  \"bestOverall\": \"guid\"|null\n" +
        "}\n" +
        "Basa la comparación únicamente en los datos proporcionados.";

    public static string BuildComparisonUser(string campingsData) {
        return
            $"Datos de los campings a comparar (GUID = detalles):\n{campingsData}\n\n" +
            "Devuelve la comparación en JSON.";
    }
}
