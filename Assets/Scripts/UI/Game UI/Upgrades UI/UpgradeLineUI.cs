using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeLineUI : MonoBehaviour
{
    private readonly UpgradePieceUI[] _upgradePieces = new UpgradePieceUI[3];

    public UpgradePieceUI upgrade1 = null;
    public UpgradePieceUI upgrade2 = null;
    public UpgradePieceUI upgrade3 = null;

    public int upgradeNumber = 0;

    private void OnValidate()
    {
        //UpgradeTo(upgradeNumber);
    }

    private void Start()
    {
        _upgradePieces[0] = upgrade1;
        _upgradePieces[1] = upgrade2;
        _upgradePieces[2] = upgrade3;
    }

    public void Upgrade()
    {
        UpgradeTo(upgradeNumber + 1);
    }

    public void UpgradeTo(int number)
    {
        upgradeNumber = number;
        if (number - 1 >= 0 && number - 1 < _upgradePieces.Length - 1) _upgradePieces[number - 1].Upgrade();
        if (number < _upgradePieces.Length) _upgradePieces[number].Unlock();
        //for (int i = number + 1; i < _upgradePieces.Length; i++)
        //    _upgradePieces[number].Lock();
    }
}
