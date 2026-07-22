using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace MyProject.Dtos;

/// <summary>生产工单列表查询输入 DTO</summary>
public class GetProductionOrderListDto : PagedAndSortedResultRequestDto
{
    /// <summary>按工单号筛选</summary>
    public string OrderNo { get; set; }

    /// <summary>按产品编码筛选</summary>
    public string ProductCode { get; set; }

    /// <summary>按状态筛选</summary>
    public Enums.ProductionOrderStatus? Status { get; set; }
}
