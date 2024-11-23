using Anomalus.Damageables;
using Anomalus.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Anomalus.Projectiles
{
    public sealed class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private float _rotationSpeed = 0f;
        [SerializeField] private GameObject[] _destroyPrefabs;

        public CreatureFraction Fraction { get; set; }
        public Transform PinTarget { get; set; }

        public float TouchDamage => AreaDamageRange > 0 ? 0f : Damage;
        public float Damage { get; set; } = 1f;
        public float AreaDamageRange { get; set; } = 0f;
        public float Speed { get; set; } = 1f;
        public float Acceleration { get; set; } = 0f;
        public float AngularSpeed { get; set; } = 0f;
        public float Range { get; set; } = 1f;
        public bool GoesThrough { get; set; } = false;
        public float StunTime { get; set; } = 0f;
        public float PinTime { get; set; } = 0f;
        public bool DoNotDestroyWhilePinned { get; set; } = false;
        public float CenterRotationSpeed { get; set; } = 0f;
        public float FlyHeight { get; set; } = 0f;
        public Vector2 CenterOffset { get; set; }

        public UnityEvent<GameObject> OnHit { get; private set; } = new();

        private Vector2 _baseVelocity;
        private Vector2 _baseDirection;
        private ManualTimer _timer;
        private bool _isPinned;
        private Vector3 _pinTargetLastPosition;
        private float _distanceFromCenter;
        private float _startRotationPhase;

        private void Start()
        {
            _baseDirection = transform.right;
            _baseVelocity = _baseDirection * Speed;
            _rigidbody.linearVelocity = _baseVelocity;

            _timer = new(Speed > 0f ? Range / Speed : Range);

            if (PinTime != 0f && PinTarget != null)
            {
                _isPinned = true;
                _pinTargetLastPosition = PinTarget.position;

                if (DoNotDestroyWhilePinned)
                {
                    _timer.TimeToTrigger += 1f;
                }
            }

            _distanceFromCenter = CenterOffset.magnitude;
            if (_distanceFromCenter > 0f)
            {
                _startRotationPhase = Mathf.Atan2(CenterOffset.y, CenterOffset.x);
            }

            _rotationSpeed += AngularSpeed;
        }

        private void FixedUpdate()
        {
            transform.Rotate(0f, 0f, -_rotationSpeed * Time.fixedDeltaTime);

            if (_isPinned && PinTarget != null)
            {
                if (PinTarget != null)
                {
                    _rigidbody.position += (Vector2)(PinTarget.position - _pinTargetLastPosition);

                    _pinTargetLastPosition = PinTarget.position;

                    _isPinned = !(PinTime > 0f && _timer.CurrentTime > PinTime);
                    if (!_isPinned) Unpin();

                    if (DoNotDestroyWhilePinned)
                    {
                        _timer.TimeToTrigger += Time.fixedDeltaTime;
                    }
                }
                else
                {
                    Unpin();
                }
            }

            if (FlyHeight != 0f)
            {
                var flyVelocity = FlyMathFunc(_timer.Progress) * 2f * FlyHeight * Vector2.up;
                _rigidbody.linearVelocity = _baseVelocity + flyVelocity;
            }

            if (AngularSpeed != 0f)
            {
                var rotateAmount = -AngularSpeed * Time.fixedDeltaTime;
                _baseDirection = _baseDirection.Rotate(rotateAmount);
                _baseVelocity = _baseVelocity.Rotate(rotateAmount);
                _rigidbody.linearVelocity = _baseVelocity;
            }

            if (Acceleration != 0f)
            {
                _baseVelocity += Acceleration * Time.fixedDeltaTime * _baseDirection;
                _rigidbody.linearVelocity = _baseVelocity;
            }

            if (CenterRotationSpeed != 0f)
            {
                var radRotationSpeed = CenterRotationSpeed / 360f * 2f * Mathf.PI;
                var phase = _startRotationPhase + _timer.CurrentTime * radRotationSpeed;
                _rigidbody.linearVelocity =
                    _baseVelocity +
                    _distanceFromCenter * radRotationSpeed * new Vector2(-Mathf.Sin(phase), Mathf.Cos(phase));
            }

            if (_timer.FixedUpdate())
            {
                DefaultDestroy();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & _obstacleLayer.value) != 0)
            {
                DefaultDestroy();
                return;
            }

            var isAnyTargets = false;
            foreach (var hitTarget in collision.GetComponents<IHitTarget>())
            {
                if (hitTarget.CanBeHit(Fraction))
                {
                    hitTarget.OnBulletHit(this);
                    isAnyTargets = true;
                }
            }
            if (isAnyTargets)
            {
                OnHit.Invoke(collision.gameObject);
            }
        }

        public void Init(Parameters parameters, CreatureFraction fraction, Transform pinTarget, Vector2 centerOffset)
        {
            Fraction = fraction;
            PinTarget = pinTarget;
            CenterOffset = centerOffset;

            Damage = parameters.Damage;
            Speed = parameters.Speed;
            Acceleration = parameters.Acceleration;
            AngularSpeed = parameters.AngularSpeed;
            Range = parameters.Range;
            GoesThrough = parameters.GoesThrough;
            StunTime = parameters.StunTime;
            AreaDamageRange = parameters.AreaDamageRange;
            PinTime = parameters.PinToWeapon;
            DoNotDestroyWhilePinned = parameters.DoNotDestroyWhilePinned;
            CenterRotationSpeed = parameters.CenterRotationSpeed;
            FlyHeight = parameters.FlyHeight;
        }

        public void DefaultDestroy()
        {
            Destroy(gameObject);

            if (AreaDamageRange > 0f)
            {
                DoAreaDamage();
            }

            foreach (var prefab in _destroyPrefabs)
            {
                var go = Instantiate(prefab, transform.position, Quaternion.identity);
                //if (go.TryGetComponent<IProjectileChild>(out var child))
                //{
                //    child.Inherit(this);
                //}
            }
        }

        private void DoAreaDamage()
        {
            var center = (Vector2)transform.position;
            foreach (var c in Physics2D.OverlapCircleAll(center, AreaDamageRange))
            {
                if (c.TryGetComponent<IHitTarget>(out var target) && target.CanBeHit(Fraction))
                {
                    target.OnAreaDamageHit(Damage, center);
                }
            }
        }

        private void Unpin()
        {
            _isPinned = false;
            if (DoNotDestroyWhilePinned)
            {
                _timer.TimeToTrigger -= 1f;
            }
        }

        private float FlyMathFunc(float x)
        {
            return 4f - 8f * x;
        }

        [System.Serializable]
        public struct Parameters
        {
            [field: SerializeField]
            public float Damage { get; set; }
            [field: SerializeField]
            public float Speed { get; set; }
            [field: SerializeField]
            public float Acceleration { get; set; }
            [field: SerializeField]
            public float AngularSpeed { get; set; }
            [field: SerializeField]
            public float Range { get; set; }
            [field: SerializeField]
            public bool GoesThrough { get; set; }
            [field: SerializeField]
            public float StunTime { get; set; }
            [field: SerializeField]
            public float AreaDamageRange { get; set; }
            [field: SerializeField]
            public float PinToWeapon { get; set; }
            [field: SerializeField]
            public bool DoNotDestroyWhilePinned { get; set; }
            [field: SerializeField]
            public float CenterRotationSpeed { get; set; }
            [field: SerializeField]
            public float FlyHeight { get; set; }
        }
    }
}
