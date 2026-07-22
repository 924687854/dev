using System;
using System.Linq;
using System.Threading.Tasks;
using MyProject.Dtos;
using MyProject.Entities;
using MyProject.Permissions;
using MyProject.Services;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;

namespace MyProject;

/// <summary>
/// 生产工单应用服务
/// 手工和ERP创建共用同一 AppService，通过不同方法区分入口
/// </summary>
public class ProductionOrderAppService : MyProjectAppService, IProductionOrderAppService
{
    private readonly IRepository<ProductionOrder, Guid> _orderRepository;
    private readonly ProductionOrderManager _orderManager;

    public ProductionOrderAppService(
        IRepository<ProductionOrder, Guid> orderRepository,
        ProductionOrderManager orderManager)
    {
        _orderRepository = orderRepository;
        _orderManager = orderManager;
    }

    /// <summary>手工创建生产工单</summary>
    [Authorize(MyProjectPermissions.ProductionOrder.Create)]
    public async Task<ProductionOrderDto> CreateAsync(CreateProductionOrderDto input)
    {
        var order = await _orderManager.CreateAsync(
            productCode: input.ProductCode,
            plannedQuantity: input.PlannedQuantity,
            unit: input.Unit,
            productionOrganization: input.ProductionOrganization,
            plannedStartDate: input.PlannedStartDate,
            plannedEndDate: input.PlannedEndDate);

        await _orderRepository.InsertAsync(order);
        return ObjectMapper.Map<ProductionOrder, ProductionOrderDto>(order);
    }

    /// <summary>ERP接口创建生产工单</summary>
    [Authorize(MyProjectPermissions.ProductionOrder.Create)]
    public async Task<ProductionOrderDto> CreateFromErpAsync(CreateProductionOrderFromErpDto input)
    {
        var order = await _orderManager.CreateAsync(
            productCode: input.ProductCode,
            plannedQuantity: input.PlannedQuantity,
            unit: input.Unit,
            productionOrganization: input.ProductionOrganization,
            plannedStartDate: input.PlannedStartDate,
            plannedEndDate: input.PlannedEndDate,
            erpSourceId: input.ErpSourceId,
            erpSourceNo: input.ErpSourceNo);

        await _orderRepository.InsertAsync(order);
        return ObjectMapper.Map<ProductionOrder, ProductionOrderDto>(order);
    }

    /// <summary>获取生产工单详情</summary>
    [Authorize(MyProjectPermissions.ProductionOrder.Read)]
    public async Task<ProductionOrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return ObjectMapper.Map<ProductionOrder, ProductionOrderDto>(order);
    }

    /// <summary>获取生产工单分页列表</summary>
    [Authorize(MyProjectPermissions.ProductionOrder.Read)]
    public async Task<PagedResultDto<ProductionOrderDto>> GetListAsync(GetProductionOrderListDto input)
    {
        var queryable = await _orderRepository.GetQueryableAsync();

        var query = queryable
            .WhereIf(!input.OrderNo.IsNullOrWhiteSpace(), x => x.OrderNo.Contains(input.OrderNo!))
            .WhereIf(!input.ProductCode.IsNullOrWhiteSpace(), x => x.ProductCode.Contains(input.ProductCode!))
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.CreationTime)
                 .PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<ProductionOrderDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.IList<ProductionOrder>, System.Collections.Generic.List<ProductionOrderDto>>(items));
    }
}
