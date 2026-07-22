using System;
using System.Threading.Tasks;
using MyProject.Dtos;
using MyProject.Entities;
using MyProject.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MyProject;

/// <summary>
/// 工艺路线主数据应用服务，继承 ABP 标准 CrudAppService
/// 重写 CreateAsync/UpdateAsync 以处理工序子集合
/// </summary>
public class ProcessRouteAppService :
    CrudAppService<ProcessRoute, ProcessRouteDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProcessRouteDto>,
    IProcessRouteAppService
{
    public ProcessRouteAppService(IRepository<ProcessRoute, Guid> repository)
        : base(repository)
    {
        GetPolicyName = MyProjectPermissions.ProcessRoute.Default;
        GetListPolicyName = MyProjectPermissions.ProcessRoute.Default;
        CreatePolicyName = MyProjectPermissions.ProcessRoute.Create;
        UpdatePolicyName = MyProjectPermissions.ProcessRoute.Update;
        DeletePolicyName = MyProjectPermissions.ProcessRoute.Delete;
    }

    /// <summary>
    /// 重写创建：处理工序子集合
    /// </summary>
    public override async Task<ProcessRouteDto> CreateAsync(CreateUpdateProcessRouteDto input)
    {
        await CheckCreatePolicyAsync();

        var entity = new ProcessRoute(
            id: GuidGenerator.Create(),
            productCode: input.ProductCode,
            productionOrganizationCode: input.ProductionOrganizationCode,
            routeNo: input.RouteNo,
            routeName: input.RouteName,
            version: input.Version,
            effectiveDate: input.EffectiveDate,
            expiryDate: input.ExpiryDate);

        foreach (var op in input.Operations)
        {
            entity.Operations.Add(new ProcessRouteOperation(
                id: GuidGenerator.Create(),
                processRouteId: entity.Id,
                sequenceNumber: op.SequenceNumber,
                operationCode: op.OperationCode,
                operationName: op.OperationName));
        }

        await Repository.InsertAsync(entity);
        return ObjectMapper.Map<ProcessRoute, ProcessRouteDto>(entity);
    }

    /// <summary>
    /// 重写更新：处理工序子集合
    /// </summary>
    public override async Task<ProcessRouteDto> UpdateAsync(Guid id, CreateUpdateProcessRouteDto input)
    {
        await CheckUpdatePolicyAsync();

        var entity = await Repository.GetAsync(id);

        // 更新主表字段（标准 DDD 应通过实体方法，此处暂时重新构造）
        var updated = new ProcessRoute(
            id: entity.Id,
            productCode: input.ProductCode,
            productionOrganizationCode: input.ProductionOrganizationCode,
            routeNo: input.RouteNo,
            routeName: input.RouteName,
            version: input.Version,
            effectiveDate: input.EffectiveDate,
            expiryDate: input.ExpiryDate);

        updated.Operations.Clear();
        foreach (var op in input.Operations)
        {
            updated.Operations.Add(new ProcessRouteOperation(
                id: GuidGenerator.Create(),
                processRouteId: updated.Id,
                sequenceNumber: op.SequenceNumber,
                operationCode: op.OperationCode,
                operationName: op.OperationName));
        }

        await Repository.UpdateAsync(updated);
        return ObjectMapper.Map<ProcessRoute, ProcessRouteDto>(updated);
    }
}
