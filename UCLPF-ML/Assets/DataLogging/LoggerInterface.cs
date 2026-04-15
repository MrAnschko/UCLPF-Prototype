using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface LoggerInterface
{
    //Must provide a method to call for automatic Data Addition
    public void AddData();
    // Must Provide a method to call for saving all Data.
    public void Save();
}
