using UnityEngine;
using System.Runtime.CompilerServices;

namespace TechnoDemo.Extensions
{
    public static class VectorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 X0Y(this Vector3 self) => new Vector3(self.x, 0.0f, self.y);
    }
}