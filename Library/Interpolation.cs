// using Godot;

// namespace Dose.Library
// {
//     public static class Interpolation
//     {
//         public static class RotationUtils
//         {
//             static Quaternion EulerToQuat(Vector3 euler)
//             {
//                 Basis b = new Basis();
//                 b = b.Rotated(Vector3.Right, euler.X);
//                 b = b.Rotated(Vector3.Up, euler.y);
//                 b = b.Rotated(Vector3.Forward, euler.z);
//                 return new Quaternion(b);
//             }
    
//             public static Vector3 InterpolateRotation(Vector3 sourceRotation, Vector3 targetRotation, float maxAngle)
//             {
//                 // Convert Euler angles to Basis
//                 Basis sourceBasis = new Basis())
//                 Basis targetBasis = new Basis(targetRotation);

//                 // Convert Basis to Quaternion
//                 Quaternion sourceQuat = new Quaternion(sourceBasis);
//                 Quaternion targetQuat = new Quaternion(targetBasis);

//                 // Compute angle between quaternions
//                 float angle = sourceQuat.AngleTo(targetQuat);

//                 if (angle <= maxAngle || Mathf.IsZeroApprox(angle))
//                     return targetRotation;

//                 // Slerp by fraction
//                 float t = maxAngle / angle;
//                 Quaternion resultQuat = sourceQuat.Slerp(targetQuat, t);

//                 // Convert back to Euler angles
//                 Basis resultBasis = resultQuat.Basis();
//                 return resultBasis.GetEuler();
//             }
//         }
//     }
// }