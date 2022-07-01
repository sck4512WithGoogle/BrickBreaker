namespace MJ.Manager
{
    using UnityEngine;
    public static class UICanvasCachinger
    {
        public static Canvas CurrentCanvas => currentCanvas;
        private static Canvas currentCanvas;

        public static void SetCanvas(Canvas _Canvas)
        {
            currentCanvas = _Canvas;
        }
    }

}
