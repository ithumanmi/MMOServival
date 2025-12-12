using System.Collections.Generic;
using UnityEngine;

namespace Hawki.Backkey
{
    public interface IKeyBack
    {
        public bool OnKeyBack();
    }

    public class BackKeyManager : RuntimeSingleton<BackKeyManager>, IUpdateBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (this.keys.Count > 0)
                {
                    var list = new List<IKeyBack>(this.keys);
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].OnKeyBack())
                        {
                            return;
                        }
                    }
                }
            }
        }

        private List<IKeyBack> keys = new List<IKeyBack>();

        public void AddListener(IKeyBack key)
        {
            keys.Add(key);
        }

        public void RemoveListener(IKeyBack key)
        {
            keys.Remove(key);
        }
    }
}
