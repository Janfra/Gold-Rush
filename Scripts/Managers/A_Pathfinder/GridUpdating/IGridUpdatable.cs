using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridUpdatable
{
    public int GridUpdateIndex { get; set; }

    // Use: GridUpdateManager.Instance.GetIndex()
}
