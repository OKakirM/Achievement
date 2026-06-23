using System.Collections.Generic;
using System.Linq;

namespace AchievementAPI.Dtos;

public class PagedResponseDto<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}