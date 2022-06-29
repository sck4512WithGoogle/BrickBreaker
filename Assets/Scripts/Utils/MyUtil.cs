
namespace MJ
{
    using UnityEngine;
    public class MyUtil
    {
        public static Vector3 GetReflectionVector(Vector3 _IncidenceVector, Vector3 _Normal)
        {
            var dot = Vector3.Dot(_IncidenceVector, _Normal);
            return _IncidenceVector - 2 * _Normal * dot;
        }
    }
}