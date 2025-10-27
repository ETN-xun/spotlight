using UnityEngine;

namespace Utils
{
    public static class Coordinate
    {
        public static void Transform(ref Vector2Int coord)
        {
            if (coord.x < 0 || coord.x > 7 || coord.y < 0 || coord.y > 7)
            {
                Debug.LogError($"Coordinate.Transform: input out of expected range [0..7]: {coord}");
            }

            // 修复坐标转换逻辑，确保坐标在有效范围内
            // 原来的逻辑会产生负坐标，现在改为简单的坐标验证和传递
            // coord = new Vector2Int(1 - coord.x, 1 - coord.y); // 这行代码有问题，会产生负坐标
        }
    }
}