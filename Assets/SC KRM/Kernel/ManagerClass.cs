using UnityEngine;

namespace SCKRM
{
    [WikiDescription(
@"싱글톤을 자동화하는 클래스 입니다

예시 코드:  
```C#
void Awake()
{
    if (SingletonCheck(this))
    {
        //your code...
    }
}
```"
)]
    public class Manager<T> : MonoBehaviour where T : Manager<T>
    {
        [WikiDescription("싱글톤의 인스턴스를 가져옵니다")]
        public static T instance { get; private set; }



        /// <summary>
        /// 싱글톤을 초기화 합니다
        /// </summary>
        /// <param name="manager">
        /// 초기화 할 오브젝트
        /// </param>
        /// <returns></returns>
        [WikiDescription("싱글톤을 초기화 합니다")]
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
