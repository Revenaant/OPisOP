using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePieceUI : MonoBehaviour
{
    public GameObject lockedGameObject = null;
    public GameObject buttonGameObject = null;
    public GameObject upgradedGameObject = null;

    public void Upgrade()
    {
        if (lockedGameObject != null)
            lockedGameObject.SetActive(false);
        if (buttonGameObject != null)
            buttonGameObject.SetActive(false);
        if (upgradedGameObject != null)
            upgradedGameObject.SetActive(true);
    }

    public void Unlock()
    {
        if (lockedGameObject != null)
            lockedGameObject.SetActive(false);
        if (buttonGameObject != null)
            buttonGameObject.SetActive(true);
        if (upgradedGameObject != null)
            upgradedGameObject.SetActive(false);
    }

    public void Lock()
    {
        if (lockedGameObject != null)
            lockedGameObject.SetActive(true);
        if (buttonGameObject != null)
            buttonGameObject.SetActive(false);
        if (upgradedGameObject != null)
            upgradedGameObject.SetActive(false);
    }
}
