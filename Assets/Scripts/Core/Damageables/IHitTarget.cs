using Anomalus.Projectiles;
using UnityEngine;

namespace Anomalus.Damageables
{
    public interface IHitTarget
    {
        bool CanBeHit(CreatureFraction fraction);
        void OnBulletHit(Bullet bullet);
        void OnPoisonHit(float damage);
        void OnAreaDamageHit(float damage, Vector2 center);
    }
}
