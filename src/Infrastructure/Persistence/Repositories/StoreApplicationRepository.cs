using Application.Abstractions;
using Application.DTOs.Stores;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class StoreApplicationRepository : Repository<StoreApplication>, IStoreApplicationRepository
{
    public StoreApplicationRepository(IDbContext dbContext, ILogger<StoreApplicationRepository> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) 
        : base(dbContext, logger, tableNameResolver, columnNameResolver)
    {
    }

    public async Task<IEnumerable<StoreApplication>> GetByStatusAsync(string status)
    {
        const string sql = "SELECT * FROM store_applications WHERE status = @Status ORDER BY created_at DESC";
        return await QueryAsync<StoreApplication>(sql, new { Status = status });
    }

    public async Task<IEnumerable<StoreApplication>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        const string sql = "SELECT * FROM store_applications WHERE created_at BETWEEN @FromDate AND @ToDate ORDER BY created_at DESC";
        return await QueryAsync<StoreApplication>(sql, new { FromDate = from, ToDate = to });
    }

    public async Task<IEnumerable<StoreApplication>> SearchAsync(StoreApplicationSearchRequest request)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(request.Status))
        {
            conditions.Add("status = @Status");
            parameters["Status"] = request.Status;
        }

        if (!string.IsNullOrEmpty(request.BusinessType))
        {
            conditions.Add("business_type = @BusinessType");
            parameters["BusinessType"] = request.BusinessType;
        }

        if (!string.IsNullOrEmpty(request.PrimaryCategory))
        {
            conditions.Add("primary_category = @PrimaryCategory");
            parameters["PrimaryCategory"] = request.PrimaryCategory;
        }

        if (!string.IsNullOrEmpty(request.City))
        {
            conditions.Add("city = @City");
            parameters["City"] = request.City;
        }

        if (request.FromDate.HasValue)
        {
            conditions.Add("created_at >= @FromDate");
            parameters["FromDate"] = request.FromDate.Value;
        }

        if (request.ToDate.HasValue)
        {
            conditions.Add("created_at <= @ToDate");
            parameters["ToDate"] = request.ToDate.Value;
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
        var sql = $"SELECT * FROM store_applications {whereClause} ORDER BY created_at DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters["Offset"] = (request.Page - 1) * request.PageSize;
        parameters["PageSize"] = request.PageSize;

        return await QueryAsync<StoreApplication>(sql, parameters);
    }
}
