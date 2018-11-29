using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FoodMachine : Connectable, IPull, IPausable
{
    // Connectable has _energy, which can be used for the starting energy the machine has

    [Tooltip("Has enough energy to be running")]
    [ReadOnly, SerializeField] private bool _energized = false;

    private bool _wasOn = false;
    private bool _awaitingAnswer = false;
    [SerializeField] private bool _isOn = true;

    [Header("Consumption Properties")]

    [Tooltip("Energy that the machine asks for")]
    [SerializeField] private float _energyPull;
    [Tooltip("Energy that the machine consumes every tick")]
    [SerializeField] private float _consumption;
    [Tooltip("Rate at which the consumption triggers")]
    [SerializeField] private float _consumptionRate;

    [Header("Supply Production")]
    [SerializeField] private GameObject _suppliments;
    [SerializeField] private float _productionTime = 1f;
    [SerializeField] private int _unitsProduced = 1;
    [SerializeField] private GameObject _foodSpawnSpot;

    [Header("Debug")]
    public DisplayStats displayStats;

    //private Rigidbody rb;
    private float timer = 0;
    public System.Action<bool> OnPowered; // On and Off

    //For the tutorial
    [Header("Tutorial")]
    [SerializeField] private List<GameObject> _tutorialSuppliments;

    private Animator _anime;

    public System.Action OnActivate;

    // Use this for initialization
    void Start()
    {
        GameManager.Instance.FoodMachine = this;

        _anime = GetComponentInChildren<Animator>();

        // Initialization
        _wasOn = _isOn;
        timer = _consumptionRate;
        OnPowered += Power;

        //// Send an energy request
        //MyCoroutines.WaitOneFrame(() =>
        //{
        //    if (SendRequest != null)
        //        SendRequest.Invoke(this, _energyPull);
        //});
    }

    // Update is called once per frame
    void Update()
    {
        // IsOn dirty check
        if (_wasOn != _isOn)
        {
            //if (OnPowered != null) OnPowered.Invoke(_isOn);
            _wasOn = _isOn;
        }

        // Checks if there's enough energy to power the machine
        _energized = _energy >= _consumption;

        if (_energized)
        {
            Produce(_unitsProduced);
            _energy -= _consumption;

            //timer -= Time.deltaTime;
            //if (timer < 0)
            //{
            //    _energy -= _consumption;
            //    timer = _consumptionRate;
            //}
        }

        #region Debug
        // Debug
        if (displayStats.texts.Count == 0) return;
        foreach (var text in displayStats.texts)
            if (text == null)
                return;
        displayStats.texts[0].gameObject.transform.parent.gameObject.SetActive(GameManager.Instance.DisplayDebug);

        displayStats.texts[0].text = "Energy: " + _energy;
        displayStats.texts[3].text = "Is On: " + _isOn;
        displayStats.texts[2].text = "enough energy?: " + _energized;
        displayStats.texts[1].text = "Consumption: " + _consumption;

        #endregion
    }

    /// <summary>
    /// Produces x units of food and water
    /// </summary>
    /// <param name="units"></param>
    private void Produce(int units)
    {
        // TODO add particles and animation

        Debug.Log("startProduce");

        // Waits the production time, then generates the supply
        StartCoroutine(MyCoroutines.Wait(_productionTime, () =>
        {

            for (int i = 0; i < units; i++)
            {
                /*GameObject s =*/
                Instantiate(_suppliments, _foodSpawnSpot.transform.position + new Vector3(Random.Range(0, 100), Random.Range(0, 100)) / 100, _suppliments.transform.rotation);
                //_foodForTutorial--;
            }

            // Consumes energy
            _anime.SetTrigger("FoodMachineTap");
            _isOn = false;
            Debug.Log("Poop!");
        }));

    }

    /// <summary>
    /// IPull implementation, receives energy and sends a new request
    /// </summary>
    /// <param name="value"></param>
    public void Pull(float value)
    {
        if (!_isOn) return;

        //Debug.Log("Receiving request");
        ReceiveEnergy(value);
        _awaitingAnswer = false;

        //if (value == 0)
        //{
        //    StartCoroutine(MyCoroutines.WaitOneFrame(() => { _isOn = false; }));
        //}

        //// SendRequest wrapped in a 1 frame delay
        //MyCoroutines.WaitOneFrame(() =>
        //{
        //    if (SendRequest != null)
        //        SendRequest.Invoke(this, _energyPull);
        //});
    }

    /// <summary>
    /// if Powered On, send energy pull request
    /// </summary>
    /// <param name="value"></param>
    private void Power(bool value)
    {
        if (value == true)
        {
            // Send a request
            if (_awaitingAnswer == false && SendRequest != null)
            {
                //Debug.Log("Sending request");
                SendRequest.Invoke(this, _energyPull);
                _awaitingAnswer = true;
            }
        }
    }

    public void Activate()
    {
        //_isOn = true;z
        if (GameManager.Instance.Storage.Energy >= _consumption)
        {
            Debug.LogWarning("REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
            GameManager.Instance.Storage.Energy -= _consumption;
            _energy = _consumption;

            if (OnActivate != null)
                OnActivate.Invoke();
        }
    }

    public void ToggleOnOff()
    {
        _isOn = !_isOn;
    }

    public void TogglePause()
    {
        ToggleOnOff();
    }

    public bool IsPaused
    {
        get { return _isOn; }
        set { _isOn = value; }
    }
}
