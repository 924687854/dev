using System;
using MyProject.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MyProject;

/// <summary>
/// 工艺路线主数据应用服务接口，继承 ABP 标准 ICrudAppService
/// </summary>
public interface IProcessRouteAppService : ICrudAppService<ProcessRouteDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProcessRouteDto>
{
}
