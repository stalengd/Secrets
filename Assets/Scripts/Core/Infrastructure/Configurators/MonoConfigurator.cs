using UnityEngine;
using VContainer;

namespace Anomalus.Infrastructure.Configurators
{
    public abstract class MonoConfigurator : MonoBehaviour
    {
        public abstract void Configure(IContainerBuilder builder);
    }
}
