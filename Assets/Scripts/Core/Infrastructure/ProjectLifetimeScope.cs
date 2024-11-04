using VContainer;
using VContainer.Unity;

namespace Anomalus.Infrastructure
{
    public sealed class ProjectLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SceneService>(Lifetime.Singleton);
        }
    }
}
