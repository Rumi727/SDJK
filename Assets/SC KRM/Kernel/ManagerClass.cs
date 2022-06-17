using UnityEngine;

namespace SCKRM
{
    public class Manager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance { get; private set; }



        /// <summary>
        /// 싱글톤을 초기화 합니다
        /// </summary>
        /// <param name="manager">
        /// 초기화 할 오브젝트
        /// </param>
        /// <returns></returns>
        protected static bool SingletonCheck(T manager)
        {
            if (instance != null && instance != manager)
            {
                DestroyImmediate(manager.gameObject);
                return false;
            }

            return (instance = manager) == manager;
        }
    }
}
