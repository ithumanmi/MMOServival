#if ANTADA_FALCON

using Antada.Libs;
using Hawki;
using Hawki.MyCoroutine;
using System.Collections;

namespace Hawki_Antada_Falcon.Firebase
{
    public class FalconFirebaseManager : RuntimeSingleton<FalconFirebaseManager>, IStartBehaviour
    {
        public void Start()
        {
            CoroutineManager.Instance.Start(TryInitFirebase());
        }

        IEnumerator TryInitFirebase()
        {
            while (true)
            {
                var firebaseManager = FirebaseManager.I;

                if (firebaseManager != null)
                {
                    firebaseManager.Init();
                    break;
                };

                yield return null;
            }
        }

    }
}

#endif