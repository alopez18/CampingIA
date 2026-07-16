using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using CampingAI.AI.DTOs;
using CampingAI.AI.Prompts;
using CampingAI.AI.Providers;
using CampingAI.Application.Abstractions;
using CampingAI.Application.Queries.Camping.SearchCampings;
using CampingAI.Application.Queries.Category.GetCategories;

namespace CampingAI.AI.Search;
/// <summary>
/// Asistente de búsqueda inteligente: convierte una consulta en lenguaje natural
/// en filtros estructurados (validados contra el catálogo real) y ejecuta la búsqueda.
/// </summary>
public class CampingSearchAssistant {

    #region Dependencias
    readonly IAIProvider _aiProvider;
    readonly IMediator _mediator;
    readonly Domain.Repositories.IFacilitiesReadRepository _facilitiesReadRepository;
    readonly ILogger<CampingSearchAssistant> _logger;
    #endregion

    static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public CampingSearchAssistant(IAIProvider aiProvider,
                                  IMediator mediator,
                                  Domain.Repositories.IFacilitiesReadRepository facilitiesReadRepository,
                                  ILogger<CampingSearchAssistant> logger) {
        _aiProvider = aiProvider;
        _mediator = mediator;
        _facilitiesReadRepository = facilitiesReadRepository;
        _logger = logger;
    }

    public async Task<SearchCampingsResult> SearchAsync(string query, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) {
        var filters = await ExtractFiltersAsync(query, cancellationToken);

        var searchQuery = new SearchCampingsQuery(
            Name: filters.Name,
            ProvinciaId: null,
            ProvinciaCode: null,
            CategoryIds: filters.CategoryIds.Count > 0 ? filters.CategoryIds : null,
            MinPrice: filters.MinPrice,
            MaxPrice: filters.MaxPrice,
            FacilityIds: filters.FacilityIds.Count > 0 ? filters.FacilityIds : null,
            MinLat: null,
            MaxLat: null,
            MinLng: null,
            MaxLng: null,
            Page: page,
            PageSize: pageSize);

        return await _mediator.SendQueryAsync<SearchCampingsQuery, SearchCampingsResult>(searchQuery);
    }

    public async Task<AiSearchFilters> ExtractFiltersAsync(string query, CancellationToken cancellationToken = default) {
        var categories = (await _mediator.SendQueryAsync<GetCategoriesQuery, GetCategoriesResult>(new GetCategoriesQuery())).Items.ToList();
        var facilities = (await _facilitiesReadRepository.GetAllAsync()).ToList();

        var categoriesCatalog = BuildCatalog(categories.Select(c => (c.Id, c.Name.ToString())));
        var facilitiesCatalog = BuildCatalog(facilities.Select(f => (f.Id, f.Name.ToString())));

        var userPrompt = PromptTemplates.BuildSearchUser(query, categoriesCatalog, facilitiesCatalog);
        var json = await _aiProvider.GenerateJsonAsync(PromptTemplates.SearchSystem, userPrompt, cancellationToken);

        var filters = Deserialize(json);
        ValidateAgainstCatalog(filters, categories.Select(c => c.Id).ToHashSet(), facilities.Select(f => f.Id).ToHashSet());
        return filters;
    }

    AiSearchFilters Deserialize(string json) {
        try {
            return JsonSerializer.Deserialize<AiSearchFilters>(json, JsonOptions) ?? new AiSearchFilters();
        }
        catch (JsonException ex) {
            _logger.LogError(ex, "La IA devolvió un JSON de filtros inválido: {Json}", json);
            throw new InvalidOperationException("El asistente de IA devolvió una respuesta no válida.", ex);
        }
    }

    void ValidateAgainstCatalog(AiSearchFilters filters, HashSet<Guid> validCategoryIds, HashSet<Guid> validFacilityIds) {
        var removedCategories = filters.CategoryIds.RemoveAll(id => !validCategoryIds.Contains(id));
        var removedFacilities = filters.FacilityIds.RemoveAll(id => !validFacilityIds.Contains(id));

        if (removedCategories > 0 || removedFacilities > 0)
            _logger.LogWarning("Se descartaron {Categories} categorías y {Facilities} servicios no presentes en el catálogo.", removedCategories, removedFacilities);
    }

    static string BuildCatalog(IEnumerable<(Guid Id, string Name)> items) {
        var sb = new StringBuilder();
        foreach (var (id, name) in items)
            sb.AppendLine($"{id} = {name}");
        return sb.ToString();
    }
}
