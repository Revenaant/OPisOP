using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : Connectable, IPush, IPull
{
    [Header("Connection properties")]
    [Tooltip("The start endpoint, energy goes one-way")]
    [SerializeField] private Connectable _start;

    [Tooltip("The end endpoint, energy goes one-way")]
    [SerializeField] private Connectable _end;

    [Tooltip("Speed of the transfer")]
#pragma warning disable 0414
    [SerializeField] private float _flowRate = 1f;
#pragma warning restore 0414

    [ReadOnly, SerializeField] private float _travelDistance;

    [Header("Debug")]
    [SerializeField] private UIFillBar _bar;
    private Connectable lastStart = null;
    private Connectable lastEnd = null;
    private MeshRenderer _meshRenderer = null;

    public System.Action<float> OnStartTransfer;
    public System.Action<float> OnEndTransfer;

    private Queue<float> transfers = new Queue<float>();

    // Use this for initialization
    private void Start()
    {
        _meshRenderer = transform.GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;

        // Requests are received and sent to the next node if needed
        _end.SendRequest += RelayRequest;
        SendRequest += _start.RelayRequest;

        // The end receives the answered request OnEndTransfer
        OnEndTransfer += _end.ReceiveEnergy;

        IPull puller = _end as IPull;
        OnEndTransfer += puller.Pull;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw lines with arrows
        if (_start != null && _end != null)
            DrawArrow.DrawGizmoLine(new Vector3(_start.transform.position.x, transform.position.y + 1, _start.transform.position.z),
                new Vector3(_end.transform.position.x, transform.position.y + 1, _end.transform.position.z), Color.yellow, 0.75f, 30f);
    }

    private void OnValidate()
    {
        // Update lists when the Start node has changed
        UpdateConnections(ref _start, ref lastStart);

        // Update the lists when the End node has changed
        UpdateConnections(ref _end, ref lastEnd);
    }

    /// <summary>
    /// IPush implementation. Force pushes an amount of energy
    /// </summary>
    /// <param name="value"></param>
    public void Push(float value)
    {
        if(OnEndTransfer != null)
            OnEndTransfer.Invoke(value);
    }

    /// <summary>
    /// IPull implementation. Starts transfer process.
    /// </summary>
    /// <param name="value"></param>
    public void Pull(float value)
    {
        StartCoroutine(transfer(value));
        transfers.Enqueue(value);
    }

    //float n = 0;

    private IEnumerator transfer(float amount)
    {
        _meshRenderer.enabled = true;

        if (OnStartTransfer != null)
            OnStartTransfer(amount);

        StartCoroutine(testTimer(1));

        if (amount > 0)
        {
            // Update Debug bar
            while (progress == false)
            {
                yield return null;
                //_bar.Value = n++ / 60;
            }
            //n = 0;
            //_bar.Value = n;
        }
        else
        {
            yield return new WaitForSeconds(1);
        }
        
        if(OnEndTransfer != null)
            OnEndTransfer.Invoke(amount);

        transfers.Dequeue();

        progress = false;

        if(transfers.Count <= 0)
            _meshRenderer.enabled = false;
    }

    // Only used for Debug testing
    bool progress = false;
    private IEnumerator testTimer(float time)
    {
        yield return new WaitForSeconds(time);
        progress = true;
    }

}