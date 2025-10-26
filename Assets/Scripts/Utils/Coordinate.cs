using UnityEngine;

namespace Utils
{
    public static class Coordinate
    {
        public static void Transform(ref Vector2Int coord)
        {
            if (coord.x < 1 || coord.x > 8 || coord.y < 1 || coord.y > 8)
            {
                Debug.LogError($"Coordinate.Transform: input out of expected range [1..8]: {coord}");
            }

            coord = new Vector2Int(1 - coord.x, 1 - coord.y);
        }
    }
}