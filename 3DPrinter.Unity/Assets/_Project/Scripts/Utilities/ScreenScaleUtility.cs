using UnityEngine;

namespace _Project.Scripts.Utilities
{
    public static class ScreenScaleUtility
    {
        private static readonly Vector2 ReferenceResolution = new Vector2(1920, 1080);
        
        private const float MatchWidthOrHeight = 0.5f;
        
        public static Vector2 Adapt(Vector2 originalPosition)
        {
            var scaleX = Screen.width / ReferenceResolution.x;
            var scaleY = Screen.height / ReferenceResolution.y;

            var scale = Mathf.Pow(scaleX, 1f - MatchWidthOrHeight) * Mathf.Pow(scaleY, MatchWidthOrHeight);

            return originalPosition * scale;
        }

        public static Vector2 Adapted(this Vector2 originalPosition)
        {
            return Adapt(originalPosition);
        }
    }
}