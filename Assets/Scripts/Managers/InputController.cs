

namespace MJ.Manager
{
    using UnityEngine.InputSystem;
    public static class InputController
    {
        public static InputAction escInput => myInput.KeyInput.ESC;
        private static MyInput myInput;

        static InputController()
        {
            myInput = new MyInput();
        }
    }
}
