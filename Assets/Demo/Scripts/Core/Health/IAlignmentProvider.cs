using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlignmentProvider : ISerializableInterface
{
    bool CanHarm(IAlignmentProvider other);
}

[Serializable]
public class SerializableIAlignmentProvider : SerializableInterface<IAlignmentProvider>
{
}

