using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPausable
{
    void TogglePause();
    bool IsPaused { get; set; }
}
