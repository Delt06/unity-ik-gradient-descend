using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IkController : MonoBehaviour
{
    [SerializeField] private Transform _target = default;
    [SerializeField, Min(0f)] private float _learningRate = 1f;
    [SerializeField, Min(0f)] private float _distanceThreshold = 0.1f;
    
    private void Update()
    {
        Kinematics.EvaluateInverse(_target.position, _jointsWithRotations, _learningRate, _distanceThreshold);

        foreach (var jointWithRotation in _jointsWithRotations)
        {
            var (joint, angle) = jointWithRotation;
            joint.transform.localRotation = Quaternion.AngleAxis(angle, joint.Axis);
        }
    }

    private void Awake()
    {
        _joints = SearchForJointsRecursive(transform).ToArray();
        _jointsWithRotations = _joints
            .Select(j => new JointWithRotation(j))
            .ToArray();
    }

    private static IEnumerable<Joint> SearchForJointsRecursive(Transform root)
    {
        if (root.TryGetComponent(out Joint joint)) yield return joint;

        foreach (Transform child in root)
        {
            foreach (var childJoint in SearchForJointsRecursive(child))
            {
                yield return childJoint;
            }
        }
    }

    private Joint[] _joints = default;
    private JointWithRotation[] _jointsWithRotations;
}