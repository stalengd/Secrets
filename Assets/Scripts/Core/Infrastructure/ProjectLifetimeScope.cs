using VContainer;
using VContainer.Unity;

namespace Secrets.Infrastructure
{
    public sealed class ProjectLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SceneService>(Lifetime.Singleton);
        }
    }
}
