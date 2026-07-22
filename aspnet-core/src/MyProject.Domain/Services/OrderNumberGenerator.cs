using System;
using System.Linq;
using System.Threading.Tasks;
using MyProject.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.DependencyInjection;

namespace MyProject.Services;

/// <summary>
/// 工单号生成器实现
/// 格式：PO + yyyyMMdd + 4位流水号，如 PO202607220001
/// 基于当日数据库中最大工单号递增，保证单实例内唯一
/// </summary>
public class OrderNumberGenerator : IOrderNumberGenerator, ITransientDependency
{
    private readonly IRepository<ProductionOrder, Guid> _orderRepository;

    public OrderNumberGenerator(IRepository<ProductionOrder, Guid> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<string> GenerateAsync()
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        var prefix = $"PO{today}";

        // 查询当日最大工单号
        var maxOrderNo = await _orderRepository
            .GetQueryableAsync()
            .ContinueWith(task =>
            {
                var queryable = task.Result;
                return queryable
                    .Where(x => x.OrderNo.StartsWith(prefix))
                    .OrderByDescending(x => x.OrderNo)
                    .Select(x => x.OrderNo)
                    .FirstOrDefault();
            });

        int nextSequence = 1;
        if (maxOrderNo != null && maxOrderNo.Length == prefix.Length + 4)
        {
            // 解析流水号部分并递增
            var sequencePart = maxOrderNo.Substring(prefix.Length, 4);
            if (int.TryParse(sequencePart, out var currentSequence))
            {
                nextSequence = currentSequence + 1;
            }
        }

        return $"{prefix}{nextSequence:D4}";
    }
}
