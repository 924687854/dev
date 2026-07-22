using System;
using System.Threading.Tasks;
using MyProject.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MyProject;

/// <summary>
/// 生产工单应用服务接口
/// 手工和ERP创建共用同一 AppService，通过不同方法区分入口
/// </summary>
public interface IProductionOrderAppService : IApplicationService
{
    /// <summary>手工创建生产工单</summary>
    Task<ProductionOrderDto> CreateAsync(CreateProductionOrderDto input);

    /// <summary>ERP接口创建生产工单</summary>
    Task<ProductionOrderDto> CreateFromErpAsync(CreateProductionOrderFromErpDto input);

    /// <summary>获取生产工单详情</summary>
    Task<ProductionOrderDto> GetAsync(Guid id);

    /// <summary>获取生产工单分页列表</summary>
    Task<PagedResultDto<ProductionOrderDto>> GetListAsync(GetProductionOrderListDto input);
}
