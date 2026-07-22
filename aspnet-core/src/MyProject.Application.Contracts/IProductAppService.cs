using System;
using MyProject.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MyProject;

/// <summary>
/// 产品主数据应用服务接口，继承 ABP 标准 ICrudAppService
/// </summary>
public interface IProductAppService : ICrudAppService<ProductDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductDto>
{
}
