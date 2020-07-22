using System;
using JetBrains.Annotations;
using UnityEngine;

public static class Kinematics
{
    public static void EvaluateInverse(Vector3 target, JointWithRotation[] joints, float learningRate, float distanceThreshold = 0f)
    {
        if (DistanceFromTarget(target, joints) < distanceThreshold) return;
        
        for (var index = 0; index < joints.Length; index++)
        {
            var gradient = PartialGradient(target, joints, index);
            
            var (joint, angle) = joints[index];
            var newAngle = angle - learningRate * gradient;
            newAngle = ClampAngle(newAngle, joint.MinAngle, joint.MaxAngle);
            joints[index].Angle = newAngle;
            
            if (DistanceFromTarget(target, joints) < distanceThreshold) return;
        }
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        return Mathf.Clamp(angle, min, max);
    }

    private static float NormalizeAngle(float angle)
    {
        var newAngle = angle;
        while (newAngle < -180f) newAngle += 360f;
        while (newAngle > 180f) newAngle -= 360f;
        return newAngle;
    }
    
    private static float PartialGradient(Vector3 target, JointWithRotation[] joints, int index)
    {
        var (_, initialAngle) = joints[index];

        var distance = DistanceFromTarget(target, joints);

        joints[index].Angle += Step;

        var nextDistance = DistanceFromTarget(target, joints);
        var gradient = (nextDistance - distance) / Step;

        joints[index].Angle = initialAngle;

        return gradient;        
    }

    private const float Step = 0.1f;

    private static float DistanceFromTarget(Vector3 target, JointWithRotation[] joints)
    {
        var point = EvaluateForward(joints);
        return Vector3.Distance(point, target);
    }    
    
    public static Vector3 EvaluateForward([NotNull] JointWithRotation[] jointsWithRotations)
    {
            if (jointsWithRotations == null) throw new ArgumentNullException(nameof(jointsWithRotations));
            if (jointsWithRotations.Length == 0) throw new ArgumentException("List of joints cannot be empty.");

            var previousPoint = jointsWithRotations[0].Joint.Position;
            var rotation = Quaternion.identity;

            for (var index = 1; index < jointsWithRotations.Length; index++)
            { 
                var (joint, angle) = jointsWithRotations[index];
                rotation *= Quaternion.AngleAxis(angle, joint.Axis);
                var nextPoint = previousPoint + rotation * joint.StartOffset;
                
                previousPoint = nextPoint;
            }

            return previousPoint;
    } 
}