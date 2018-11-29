using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeable
{
    int MaxUpgradeLevel { get; set; }
    int CurrentUpgradeLevel { get; set; }

    void Upgrade(int byHowManyLevels);
    void UpgradeTo(int setLevel);
}
