using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingTutorialTwoNew : MonoBehaviour {

    [SerializeField] private GameObject _goInHand;
    //[SerializeField] private GameObject _goOutHand;
    private int _timesClicked;
    private bool _handShown;
    private FridayTutorial _fridayTutorial;
    private bool _showedText;
    private bool _madeFood;

    private void Start()
    {
        _goInHand.SetActive(false);
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
    }

    private void Update()
    {
        if (_madeFood)
        {
            if (_goInHand != null) _goInHand.SetActive(true);
            _handShown = true;
            if (!_showedText)
            {
                _fridayTutorial.Next();
                _showedText = true;
            }
        }

        if (GameManager.Instance.Pet.FedInTutorial <= 0)
        {
            GameManager.Instance.Pet.HungerPerSecond = 3;
            //_fridayTutorial.Next();
            //_goOutHand.SetActive(true);
            Destroy(gameObject);
        }
    }

    public void MakeFood()
    {
        if (GameManager.Instance.SolarPanel.Dirt < 1 && !_madeFood)
        {
            _madeFood = true;
        }
    }

    public void GoBackIn()
    {
        if (_handShown)
        {
            Destroy(_goInHand);
        }
    }
}
