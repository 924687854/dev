using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MesSystem.Data;

/* This is used if database provider does't define
 * IMesSystemDbSchemaMigrator implementation.
 */
public class NullMesSystemDbSchemaMigrator : IMesSystemDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
