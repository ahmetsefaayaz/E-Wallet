namespace BuildingBlocks.Pagination;

public class PaginatedResult<T>(int pageIndex, int pageSize, long totalItemCount, IEnumerable<T>data)
{
    public IEnumerable<T> Data { get; } = data;
    public int PageIndex { get; init; } = pageIndex;
    public int PageSize { get; init; } = pageSize;
    public long TotalItemCount { get; init; } = totalItemCount;
}