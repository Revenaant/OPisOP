using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Basket : MonoBehaviour
{
    private class Food
    {
        public GameObject go = null;
        public Rigidbody rb = null;
        public float timer = 1;

        public Food(GameObject go, float timer)
        {
            this.go = go;
            this.timer = timer;

            rb = go.GetComponent<Rigidbody>();
        }
    }

    //[SerializeField, ColorUsage(true, true)] private Color color;

    private List<Food> _movingGameObjects = new List<Food>();
    private List<Food> _notmovingGameObjects = new List<Food>();

    private string _tag = "Suppliment";

    public string Tag
    {
        get { return _tag; }
        set { _tag = value; }
    }

    [SerializeField] private int _maxFood = 5;
    [SerializeField, ReadOnly] private int _food = 0;
    [SerializeField] private float _stickTime = 0.1f;

    public System.Action<GameObject> onStick;
    public System.Action<GameObject> onUnstick;

    private void Update()
    {
        if (_movingGameObjects.Count == 0) return;

        for (int i = _movingGameObjects.Count - 1; i >= 0; i--)
        {
            Food food = _movingGameObjects[i];

            if (food.timer <= 0)
            {
                food.go.transform.SetParent(transform);
                //food.rb.isKinematic = true;
                //food.rb.useGravity = false;

                _movingGameObjects.RemoveAt(i);
                _notmovingGameObjects.Add(food);
                _food++;
                onStick(food.go);

                continue;
            }

            food.timer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Hit");
        if (_food >= _maxFood || !collider.gameObject.CompareTag(_tag)
                              || !collider.gameObject.GetComponent<Rigidbody>()) return;

        _movingGameObjects.Add(new Food(collider.gameObject, _stickTime));
        //_food++;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (!collider.gameObject.CompareTag(_tag)) return;

        for (int i = _movingGameObjects.Count - 1; i >= 0; i--)
        {
            Food food = _movingGameObjects[i];
            if (food.go != collider.gameObject) continue;
            _movingGameObjects.RemoveAt(i);
            return;
        }

        for (int i = _notmovingGameObjects.Count - 1; i >= 0; i--)
        {
            Food food = _notmovingGameObjects[i];
            if (food.go != collider.gameObject) continue;
            _notmovingGameObjects.RemoveAt(i);

            _food--;
            food.go.transform.SetParent(null);
            onUnstick(food.go);
            return;
        }
    }

    public void Reset()
    {
        _food = 0;

        foreach (Food food in _movingGameObjects)
            Destroy(food.go);
        foreach (Food food in _notmovingGameObjects)
            Destroy(food.go);

        _movingGameObjects.Clear();
        _notmovingGameObjects.Clear();
    }

    public int Result
    {
        get { return _food; }
    }
}
