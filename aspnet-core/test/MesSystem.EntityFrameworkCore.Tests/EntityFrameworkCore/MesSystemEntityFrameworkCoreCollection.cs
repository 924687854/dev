using Xunit;

namespace MesSystem.EntityFrameworkCore;

[CollectionDefinition(MesSystemTestConsts.CollectionDefinitionName)]
public class MesSystemEntityFrameworkCoreCollection : ICollectionFixture<MesSystemEntityFrameworkCoreFixture>
{

}
