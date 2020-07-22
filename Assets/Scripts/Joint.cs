using UnityEngine;

[DisallowMultipleComponent]
public sealed class Joint : MonoBehaviour
{
        [SerializeField] private Vector3 _axis = Vector3.right;
        [SerializeField, Range(-180f, 180f)] private float _minAngle = -180f;
        [SerializeField, Range(-180f, 180f)] private float _maxAngle = 180f;
        
        public Vector3 Axis => _axis;
        public Vector3 StartOffset { get; private set; }
        public Vector3 Position => transform.position;
        
        public float MinAngle => _minAngle;

        public float MaxAngle => _maxAngle;

        private void Awake()
        {
                StartOffset = transform.localPosition;
        }

        private void OnValidate()
        {
                if (_minAngle > _maxAngle)
                        _minAngle = _maxAngle;
        }
}