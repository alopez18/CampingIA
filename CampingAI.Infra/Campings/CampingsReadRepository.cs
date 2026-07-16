using Dapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CampingAI.Infra.Campings;

public class CampingsReadRepository : Domain.Repositories.ICampingsReadRepository
{
    #region Dependencies
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPINGS> _modelExtractor;
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_FACILITIES> _campingFacilitiesExtractor;
    readonly Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_CATEGORIES> _campingCategoriesExtractor;
    readonly Configuration.Factories.Interfaces.ISqlConnectionFactory _sqlConnectionFactory;
    readonly Mappers.CampingsMapper _campingsMapper;
    readonly ILogger<CampingsReadRepository> _logger;
    #endregion

    public CampingsReadRepository(Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPINGS> modelExtractor,
                                  Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_FACILITIES> campingFacilitiesExtractor,
                                  Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_CATEGORIES> campingCategoriesExtractor,
                                  Configuration.Factories.Interfaces.ISqlConnectionFactory sqlConnectionFactory,
                                  Mappers.CampingsMapper campingsMapper,
                                  ILogger<CampingsReadRepository> logger)
    {
        _modelExtractor = modelExtractor;
        _campingFacilitiesExtractor = campingFacilitiesExtractor;
        _campingCategoriesExtractor = campingCategoriesExtractor;
        _sqlConnectionFactory = sqlConnectionFactory;
        _campingsMapper = campingsMapper;
        _logger = logger;
    }

    public async Task<Domain.Entities.Camping?> GetByIdAsync(Guid id)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)} = @Id ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} IS NULL");
        string query = sql.ToString();

        Models.CampingAI_DB.T_CAMPINGS? row;
        try
        {
            row = await dbConnection.QueryFirstOrDefaultAsync<Models.CampingAI_DB.T_CAMPINGS>(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting camping by id {Id}. Query: {Query}", id, query);
            throw;
        }

        if (row is null) return null;

        var camping = _campingsMapper.Map(row);
        var facilityIds = await GetFacilityIdsByCampingIdAsync(dbConnection, id);
        camping.SetFacilities(facilityIds);
        await LoadAdditionalCategoriesForCampingsAsync(dbConnection, [camping]);
        return camping;
    }

    public async Task<IEnumerable<Domain.Entities.Camping>> GetAllAsync()
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} IS NULL");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_CAMPINGS> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CAMPINGS>(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all campings. Query: {Query}", query);
            throw;
        }

        var campings = _campingsMapper.Map(rows).ToList();
        await LoadFacilitiesForCampingsAsync(dbConnection, campings);
        return campings;
    }

    public async Task<IEnumerable<Domain.Entities.Camping>> GetByCategoryAsync(Guid categoryId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE ({nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CategoryId)} = @CategoryId ");
        sql.AppendLine($"OR EXISTS (SELECT 1 FROM {_campingCategoriesExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"    WHERE {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)} = {_modelExtractor.GetTableNameForSql()}.{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)} ");
        sql.AppendLine($"    AND {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CategoryId)} = @CategoryId)) ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} IS NULL");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_CAMPINGS> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CAMPINGS>(query, new { CategoryId = categoryId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campings by category {CategoryId}. Query: {Query}", categoryId, query);
            throw;
        }

        var campings = _campingsMapper.Map(rows).ToList();
        await LoadFacilitiesForCampingsAsync(dbConnection, campings);
        return campings;
    }

    public async Task<IEnumerable<Domain.Entities.Camping>> GetByOwnerAsync(Guid ownerId)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        sql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_OwnerId)} = @OwnerId ");
        sql.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} IS NULL");
        string query = sql.ToString();

        IEnumerable<Models.CampingAI_DB.T_CAMPINGS> rows;
        try
        {
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CAMPINGS>(query, new { OwnerId = ownerId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campings by owner {OwnerId}. Query: {Query}", ownerId, query);
            throw;
        }

        var campings = _campingsMapper.Map(rows).ToList();
        await LoadFacilitiesForCampingsAsync(dbConnection, campings);
        return campings;
    }

    public async Task<(IEnumerable<Domain.Entities.Camping> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var countSql = new StringBuilder();
        countSql.AppendLine($"SELECT COUNT(*) ");
        countSql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        countSql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} IS NULL");

        var dataSql = new StringBuilder();
        dataSql.AppendLine($"SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        dataSql.AppendLine($"FROM {_modelExtractor.GetTableNameForSql()} ");
        dataSql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} IS NULL ");
        dataSql.AppendLine($"ORDER BY {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CreatedOn)} DESC ");
        dataSql.AppendLine($"OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

        int totalCount;
        IEnumerable<Models.CampingAI_DB.T_CAMPINGS> rows;
        try
        {
            totalCount = await dbConnection.ExecuteScalarAsync<int>(countSql.ToString());
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CAMPINGS>(
                dataSql.ToString(),
                new { Offset = (page - 1) * pageSize, PageSize = pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged campings (page={Page}, pageSize={PageSize}).", page, pageSize);
            throw;
        }

        var campings = _campingsMapper.Map(rows).ToList();
        await LoadFacilitiesForCampingsAsync(dbConnection, campings);
        return (campings, totalCount);
    }

    private async Task<IEnumerable<Guid>> GetFacilityIdsByCampingIdAsync(System.Data.IDbConnection dbConnection, Guid campingId)
    {
        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_FacilityId)} ");
        sql.AppendLine($"FROM {_campingFacilitiesExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)} = @CampingId");
        string query = sql.ToString();

        var facilityIdStrings = await dbConnection.QueryAsync<Guid>(query, new { CampingId = campingId });
        return facilityIdStrings;
    }

    private async Task LoadFacilitiesForCampingsAsync(System.Data.IDbConnection dbConnection, List<Domain.Entities.Camping> campings)
    {
        if (campings.Count == 0) return;

        var ids = campings.Select(c => c.Id).ToList();
        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)}, ");
        sql.AppendLine($"       {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_FacilityId)} ");
        sql.AppendLine($"FROM {_campingFacilitiesExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)} IN @Ids");
        string query = sql.ToString();

        var links = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CAMPING_FACILITIES>(query, new { Ids = ids });
        var grouped = links.GroupBy(l => l.CFE_CampingId);

        foreach (var group in grouped)
        {
            var camping = campings.FirstOrDefault(c => c.Id == group.Key);
            camping?.SetFacilities(group.Select(l => l.CFE_FacilityId));
        }

        await LoadAdditionalCategoriesForCampingsAsync(dbConnection, campings);
    }

    private async Task LoadAdditionalCategoriesForCampingsAsync(System.Data.IDbConnection dbConnection, List<Domain.Entities.Camping> campings)
    {
        if (campings.Count == 0) return;

        var ids = campings.Select(c => c.Id).ToList();
        var sql = new StringBuilder();
        sql.AppendLine($"SELECT {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)}, ");
        sql.AppendLine($"       {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CategoryId)} ");
        sql.AppendLine($"FROM {_campingCategoriesExtractor.GetTableNameForSql()} ");
        sql.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)} IN @Ids");
        string query = sql.ToString();

        var links = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CAMPING_CATEGORIES>(query, new { Ids = ids });
        var grouped = links.GroupBy(l => l.CCA_CampingId);

        foreach (var group in grouped)
        {
            var camping = campings.FirstOrDefault(c => c.Id == group.Key);
            camping?.SetAdditionalCategories(group.Select(l => l.CCA_CategoryId));
        }
    }

    public async Task<(IEnumerable<Domain.Entities.Camping> Items, int TotalCount)> SearchAsync(Domain.Repositories.CampingSearchFilters filters)
    {
        using var dbConnection = _sqlConnectionFactory.CreateConnection();

        var parameters = new Dapper.DynamicParameters();
        var where = new StringBuilder();
        where.AppendLine($"WHERE {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_DeletedOn)} IS NULL");

        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Name)} LIKE @Name");
            parameters.Add("Name", $"%{filters.Name}%");
        }

        if (filters.ProvinciaId.HasValue)
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_ProvinciaId)} = @ProvinciaId");
            parameters.Add("ProvinciaId", filters.ProvinciaId.Value);
        }

        var categoryIds = filters.CategoryIds?.ToList();
        if (categoryIds is { Count: > 0 })
        {
            var orConditions = new List<string>();
            for (int i = 0; i < categoryIds.Count; i++)
            {
                var paramName = $"CategoryId{i}";
                orConditions.Add(
                    $"{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_CategoryId)} = @{paramName} " +
                    $"OR EXISTS (SELECT 1 FROM {_campingCategoriesExtractor.GetTableNameForSql()} " +
                    $"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CampingId)} = {_modelExtractor.GetTableNameForSql()}.{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)} " +
                    $"AND {nameof(Models.CampingAI_DB.T_CAMPING_CATEGORIES.CCA_CategoryId)} = @{paramName})");
                parameters.Add(paramName, categoryIds[i]);
            }
            where.AppendLine($"AND ({string.Join(" OR ", orConditions)})");
        }

        if (filters.MinPrice.HasValue)
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_PricePerNight)} >= @MinPrice");
            parameters.Add("MinPrice", filters.MinPrice.Value);
        }

        if (filters.MaxPrice.HasValue)
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_PricePerNight)} <= @MaxPrice");
            parameters.Add("MaxPrice", filters.MaxPrice.Value);
        }

        if (filters.MinLat.HasValue)
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Latitude)} >= @MinLat");
            parameters.Add("MinLat", filters.MinLat.Value);
        }

        if (filters.MaxLat.HasValue)
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Latitude)} <= @MaxLat");
            parameters.Add("MaxLat", filters.MaxLat.Value);
        }

        if (filters.MinLng.HasValue)
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Longitude)} >= @MinLng");
            parameters.Add("MinLng", filters.MinLng.Value);
        }

        if (filters.MaxLng.HasValue)
        {
            where.AppendLine($"AND {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Longitude)} <= @MaxLng");
            parameters.Add("MaxLng", filters.MaxLng.Value);
        }

        var facilityIds = filters.FacilityIds?.ToList();
        if (facilityIds is { Count: > 0 })
        {
            for (int i = 0; i < facilityIds.Count; i++)
            {
                var paramName = $"FacilityId{i}";
                where.AppendLine(
                    $"AND EXISTS (SELECT 1 FROM {_campingFacilitiesExtractor.GetTableNameForSql()} " +
                    $"WHERE {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_CampingId)} = {_modelExtractor.GetTableNameForSql()}.{nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_IdCamping)} " +
                    $"AND {nameof(Models.CampingAI_DB.T_CAMPING_FACILITIES.CFE_FacilityId)} = @{paramName})");
                parameters.Add(paramName, facilityIds[i]);
            }
        }

        int offset = (filters.Page - 1) * filters.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", filters.PageSize);

        var countSql = new StringBuilder();
        countSql.AppendLine($" SELECT COUNT(*) FROM {_modelExtractor.GetTableNameForSql()} ");
        countSql.Append(where);

        var dataSql = new StringBuilder();
        dataSql.AppendLine($" SELECT {_modelExtractor.GetFieldNamesForSql()} ");
        dataSql.AppendLine($" FROM {_modelExtractor.GetTableNameForSql()} ");
        dataSql.Append(where);
        dataSql.AppendLine($" ORDER BY {nameof(Models.CampingAI_DB.T_CAMPINGS.CMP_Name)} ");
        dataSql.AppendLine(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY ");

        int totalCount;
        IEnumerable<Models.CampingAI_DB.T_CAMPINGS> rows;
        try
        {
            totalCount = await dbConnection.ExecuteScalarAsync<int>(countSql.ToString(), parameters);
            rows = await dbConnection.QueryAsync<Models.CampingAI_DB.T_CAMPINGS>(dataSql.ToString(), parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching campings with filters {@Filters}.", filters);
            throw;
        }

        var campings = _campingsMapper.Map(rows).ToList();
        await LoadFacilitiesForCampingsAsync(dbConnection, campings);
        return (campings, totalCount);
    }
}
