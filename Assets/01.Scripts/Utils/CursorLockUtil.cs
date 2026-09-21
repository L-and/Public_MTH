using UnityEngine;

namespace _01.Scripts.Utils
{
    public static class CursorLockUtil
    {
        /// <summary>
        /// 마우스커서를 잠굼
        /// </summary>
        public static void Lock()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// 마우스커서의 잠금을 해제
        /// </summary>
        public static void Unlock()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}