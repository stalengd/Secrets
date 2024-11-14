using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Infrastructure.Configurators
{
    public sealed class SimpleMonoConfigurator : MonoConfigurator
    {
        [SerializeField] private Object[] _objectsToRegister;

        public override void Configure(IContainerBuilder builder)
        {
            foreach (var obj in _objectsToRegister)
            {
                builder.RegisterComponent(obj).AsSelf().AsImplementedInterfaces();
            }
        }
    }
}
