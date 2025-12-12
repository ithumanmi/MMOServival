using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.Config
{
    public abstract class ConfigDataBase
    {
        public virtual void OnLoad()
        {

        }
    }

    public abstract class ConfigDataBase<T> where T : ConfigDataBase
    {

    }
}
