using System.Threading.Tasks;

namespace MyProject.Services;

/// <summary>
/// 工单号生成器接口
/// 格式：PO + yyyyMMdd + 4位流水号，如 PO202607220001
/// </summary>
public interface IOrderNumberGenerator
{
    /// <summary>
    /// 生成下一个工单号
    /// </summary>
    Task<string> GenerateAsync();
}
