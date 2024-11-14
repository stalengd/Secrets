using Anomalus.Infrastructure.Configurators;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Infrastructure
{
    public sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private MonoConfigurator[] _monoConfigurators;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SceneService>(Lifetime.Singleton);

            foreach (var configurator in _monoConfigurators)
            {
                configurator.Configure(builder);
            }
        }
    }
}
