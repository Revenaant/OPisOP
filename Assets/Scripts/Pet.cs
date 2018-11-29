using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Presets;

public enum PetState { Sleep, Eat, Play, Depression, Neutral };

public class Pet : MonoBehaviour, IHaveEvents
{
    [ReadOnly, SerializeField]
    private PetState _state;
    [SerializeField, ReadOnly]
    private float _happiness;
    [SerializeField, ReadOnly]
    private float _hunger;
    private float _sleepiness;
    private float _sleepCD;
    private bool _shouldSleep;
    [SerializeField, ReadOnly]
    private bool _thermalDiscomfort;
    private bool _justAte;

    [SerializeField]
    [Tooltip("How long the pet sleeps, the cooldown for the nap is twice as long")]
    private float SleepDuration;

    [SerializeField] private float StageOneHunger;
    [SerializeField] private float StageTwoHunger;
    [Space]
    [SerializeField] private float GetHungryPerSecond;
    [SerializeField] private float HappinessReducedWhenHungry;
    [SerializeField] private float HungerReducedWhenEating;
    [Space]
    [SerializeField] private float StatsChangedWhenWoke;
    [SerializeField] private float TooHot;
    [SerializeField] private float TooCold;
    [SerializeField] private float HungerCD;
    private float HungerTimer;
    [Space]
    [SerializeField] private float PolutionTolerance;
    [SerializeField] private float PolutionUnhappines;
    [Space]
    [SerializeField] private float _jumpForce;
    [Space]
    [SerializeField] private GameObject _young;
    [SerializeField] private GameObject _middleAged;
    [SerializeField] private GameObject _old;
    private GameObject _currentPet = null;
    [Space]
    [SerializeField] private CapsuleCollider _colliderYoung;
    [SerializeField] private CapsuleCollider _colliderMiddleAged;
    [SerializeField] private CapsuleCollider _colliderOld;
    [Space]
    [Header("Feeding Food")]
    [SerializeField] private Transform _smallHand;
    [SerializeField] private Transform _mediumHand;
    [SerializeField] private Transform _bigHand;
    private Transform _currentHand;
    [Space]
    [SerializeField] private GameObject _foodParticle;
    [SerializeField] private GameObject _heldfood;
    [SerializeField] private Vector3 _foodOffset;
    private int _fedForTutorial;
    private int _level;
    //[SerializeField] private GameObject _fuckingCuntStayInPlace;

    [ReadOnly, SerializeField] private float _timeHappy = 0;

    //public DisplayStats displayStats;

    private Rigidbody _rb;
    private Animator _anime;
    private RaycastHit _hit;
    public bool _grounded;
    private float _distanceFromGround;

    private Queue<int> feedQueue = new Queue<int>();

    public float TooHotTemperature
    {
        get { return TooHot; }
    }

    public float TooColdTemperature
    {
        get { return TooCold; }
    }

    public static Action<float> onHappinessChanged;
    public static Action<float> onHappiness100Changed;
    public static Action<float> onHungerChanged;

    public System.Action OnJump;
    public System.Action OnLand;
    public System.Action OnStartEat;

    private void Start()
    {
        Happiness = 100;
        //Hunger = 0;
        Sleepiness = SleepDuration;
        _sleepCD = SleepDuration * 2;
        _state = PetState.Neutral;

        GameManager.Instance.Pet = this;
        _justAte = true;
        FedInTutorial = 4;

        _young.SetActive(true);
        _middleAged.SetActive(false);
        _old.SetActive(false);
        _currentPet = _young;

        _currentHand = _smallHand;

        _colliderYoung.enabled = true;
        _colliderMiddleAged.enabled = false;
        _colliderOld.enabled = false;

        _rb = GetComponent<Rigidbody>();
        _anime = GetComponentInChildren<Animator>();
    }

    public void ForceEvents()
    {
        if (Application.isPlaying)
        {
            if (onHappinessChanged != null)
                onHappinessChanged(Happiness / 100);
            if (onHappiness100Changed != null)
                onHappiness100Changed(Happiness);
        }
    }

    private void Update()
    {
        if (Happiness > 0) _timeHappy += Time.deltaTime;
        if (_sleepCD < 0) _sleepCD = 0;
        //print(FedInTutorial);
        JumpCast();
        NaturalStates();
        Upgrade();
    }

    public float Happiness
    {
        get { return _happiness; }
        set
        {
            float newValue = Mathf.Clamp(value, 0, 100);
            if (Math.Abs(_happiness - newValue) > Mathf.Epsilon)
            {
                if (onHappinessChanged != null)
                    onHappinessChanged(newValue / 100.0f);
                if (onHappiness100Changed != null)
                    onHappiness100Changed(newValue);
            }
            _happiness = newValue;
        }
    }

    public float Hunger
    {
        get { return _hunger; }
        set
        {
            if (Math.Abs(_hunger - value) > Mathf.Epsilon
                && onHungerChanged != null) onHungerChanged(value);
            _hunger = Mathf.Clamp(value, 0, 100);
        }
    }

    public float HungerPerSecond
    {
        get { return GetHungryPerSecond; }
        set { GetHungryPerSecond = value; }
    }


    public float Sleepiness
    {
        get { return _sleepiness; }
        set { _sleepiness = value; }
    }

    public float HappyTime
    {
        get { return _timeHappy; }
        set { _timeHappy = value; }
    }

    public float JumpForce
    {
        get { return _jumpForce; }
        set { _jumpForce = value; }
    }

    public PetState StateOfPet
    {
        get { return _state; }
    }

    public bool OnTheGround
    {
        get { return _grounded; }
    }

    public int FedInTutorial
    {
        get { return _fedForTutorial; }
        set { _fedForTutorial = value; }
    }

    public void Clicked()
    {
        _state = PetState.Play;
    }

    //private void OnMouseDown()
    //{
    //    //Happiness += 1;
    //}

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<Supplements>() != null)
        {
            Destroy(col.gameObject);
            _heldfood.SetActive(true);
            feedQueue.Enqueue(1);

            _state = PetState.Eat;
            _anime.SetTrigger("Eating");

            GetComponentInChildren<ParticleSystem>().Play();

            if (!_justAte) FedInTutorial--;
            if (OnStartEat != null)
                OnStartEat.Invoke();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Supplements>() != null)
        {
            _state = PetState.Eat;
            col.gameObject.GetComponent<Supplements>().SpawnParticles();
            col.transform.position = transform.position + transform.forward;

            if (OnStartEat != null)
                OnStartEat.Invoke();
        }
    }

    private void NaturalStates()
    {
        Happiness = Happiness;
        //print(Happiness);
        switch (_state)
        {
            case PetState.Sleep:
                Sleepiness -= Time.deltaTime;
                if (Sleepiness <= 0)
                {
                    if (GetComponent<CapsuleCollider>().enabled) GetComponent<CapsuleCollider>().enabled = false;
                    Happiness -= StatsChangedWhenWoke;
                    Hunger += StatsChangedWhenWoke;
                    Sleepiness = SleepDuration;
                    _sleepCD = SleepDuration * 2;
                    _state = PetState.Neutral;
                    _shouldSleep = false;
                }
                break;
            case PetState.Eat:
                if (_heldfood.activeInHierarchy) _heldfood.transform.position = _currentHand.position + _foodOffset;
                if (_anime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    SpawnParticles(_heldfood.transform.position);
                    feedQueue.Dequeue();
                    Hunger -= HungerReducedWhenEating;

                    if (feedQueue.Count <= 0)
                    {
                        _heldfood.SetActive(false);
                        _justAte = true;
                        _state = PetState.Neutral;
                    }
                }
                break;
            case PetState.Play:
                if (_grounded && !_rb.isKinematic)
                {
                    _rb.AddForce((Vector3.up + -transform.forward) * _rb.mass * _jumpForce, ForceMode.Impulse);
                    _anime.SetTrigger("Jump");

                    if (OnJump != null) OnJump.Invoke();
                }
                _state = PetState.Neutral;
                break;
            case PetState.Depression:
                break;
            case PetState.Neutral:

                if (!GetComponent<CapsuleCollider>().enabled && !_justAte) GetComponent<CapsuleCollider>().enabled = true;

                if (_justAte) GetComponent<CapsuleCollider>().enabled = false;
                else GetComponent<CapsuleCollider>().enabled = true;

                _sleepCD -= Time.deltaTime;
                if (_sleepCD <= 0 && Math.Abs(Happiness - 100) < Mathf.Epsilon) _shouldSleep = true;
                if (_shouldSleep && GameManager.Instance != null && GameManager.Instance.GameRules != null
                    && GameManager.Instance.GameRules.Days > 1) _state = PetState.Sleep;

                if (_justAte)
                {
                    HungerTimer -= Time.deltaTime;
                    if (HungerTimer < 0)
                    {
                        HungerTimer = HungerCD;
                        _justAte = false;
                    }
                }

                if (!_justAte) Hunger += Time.deltaTime * GetHungryPerSecond;
                if (Hunger <= StageOneHunger) Happiness += Time.deltaTime * 2;
                else if (Hunger < StageTwoHunger && Hunger > StageOneHunger) Happiness -= Time.deltaTime * HappinessReducedWhenHungry * (_level * 0.5f);
                else if (Hunger > StageTwoHunger) Happiness -= Time.deltaTime * 2 * HappinessReducedWhenHungry * (_level * 0.5f);

                if (GameManager.Instance.Temperature > TooHot ||
                    GameManager.Instance.Temperature < TooCold) _thermalDiscomfort = true;
                else _thermalDiscomfort = false;
                if (_thermalDiscomfort) Happiness -= Time.deltaTime * (_level * 0.5f);
                else Happiness += Time.deltaTime;

                if (GameManager.Instance.PollutionManager != null)
                {
                    if (GameManager.Instance.PollutionManager.PollutionDamage > PolutionTolerance)
                        Happiness -= Time.deltaTime * PolutionUnhappines * (_level * 0.5f);
                }

                if (Happiness <= 0) print("deppressed af boiiiii");

                //for debug
                break;
            default:
                break;
        }
    }

    private bool wasGrounded = false;
    private void JumpCast()
    {
        _distanceFromGround = GetComponent<Collider>().bounds.size.y / 1.8f; //magic number bullshit
        if (Physics.Raycast(transform.position + Vector3.up / 2, Vector3.down, out _hit, _distanceFromGround))
        {
            _grounded = true;
        }
        else _grounded = false;

        // Check Landing
        if (wasGrounded != _grounded)
        {
            wasGrounded = _grounded;
            if (_grounded == true)
                if (OnLand != null) OnLand.Invoke();
        }

        Debug.DrawRay(transform.position + Vector3.up / 2, Vector3.down * _distanceFromGround, Color.green);

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (2 - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
    }

    public void Kill()
    {
        transform.position = transform.position - Vector3.down * 666666;
        GetComponentInChildren<Renderer>().enabled = false;
    }

    private void Upgrade()
    {
        if (GameManager.Instance == null || GameManager.Instance.CheckpointManager == null) return;

        if (GameManager.Instance.CheckpointManager.IsCheckpoint)
        {
            _level = GameManager.Instance.CheckpointManager.CheckpointsPassed;

            switch (_level)
            {
                case (1):
                    {
                        _colliderYoung.enabled = true;
                        _colliderMiddleAged.enabled = false;
                        _colliderOld.enabled = false;

                        _young.SetActive(true);
                        _middleAged.SetActive(false);
                        _old.SetActive(false);

                        _currentPet = _young;
                        _currentHand = _smallHand;

                        break;
                    }
                case (2):
                    {
                        _colliderYoung.enabled = false;
                        _colliderMiddleAged.enabled = true;
                        _colliderOld.enabled = false;

                        _young.SetActive(false);
                        _middleAged.SetActive(true);
                        _old.SetActive(false);

                        _currentPet = _middleAged;
                        _currentHand = _mediumHand;

                        break;
                    }
                case (3):
                    {
                        _colliderYoung.enabled = false;
                        _colliderMiddleAged.enabled = false;
                        _colliderOld.enabled = true;

                        _young.SetActive(false);
                        _middleAged.SetActive(false);
                        _old.SetActive(true);

                        _currentPet = _old;
                        _currentHand = _bigHand;

                        break;
                    }
            }
        }
    }

    public void SpawnParticles(Vector3 position)
    {
        Instantiate(_foodParticle, position, Quaternion.identity);
    }
}
