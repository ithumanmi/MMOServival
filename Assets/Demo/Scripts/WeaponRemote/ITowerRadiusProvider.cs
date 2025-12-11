using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITowerRadiusProvider
{
    float effectRadius { get; }
    Color effectColor { get; }
    Targetter targetter { get; }
}
