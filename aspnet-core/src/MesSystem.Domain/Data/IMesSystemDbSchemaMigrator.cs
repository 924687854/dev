using System.Threading.Tasks;

namespace MesSystem.Data;

public interface IMesSystemDbSchemaMigrator
{
    Task MigrateAsync();
}
