using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connectable : MonoBehaviour
{
    [Header("Energy Storage")]
    [SerializeField] protected float _energy;
    [SerializeField] protected float _capacity = 10;

    private int _maximumConnections = 1000;
    [ReadOnly, SerializeField] protected List<Connectable> _connections;

    /// <summary>
    /// Sends a request for x amount of energy, coming from the issuing object [Use 'this']
    /// </summary>
    public System.Action<Connectable, float> SendRequest;

    /// <summary>
    /// Adds energy clamped to the capacity of the connectable, if the energy is less than capacity.
    /// </summary>
    /// <param name="value"></param>
    public virtual void ReceiveEnergy(float value)
    {
        if (_energy < _capacity)
        {
            _energy += Mathf.Clamp(value, 0, _capacity - _energy);
        }
    }

    /// <summary>
    /// Checks if an energy pull request can be fulfilled, otherwise sends a new request to the connected objects
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="value"></param>
    public void RelayRequest(Connectable connection, float value)
    {
        Storage storage = this.GetComponent<Storage>();
        //GasTank gasTank = this.GetComponent<GasTank>();

        // If this is the storage, answer the request
        if (storage != null)
        {
            storage.PushTo(connection, value);
            return;
        }
        //// If this is a gasTank, answer the request
        //else if (gasTank != null && gasTank.isActive)
        //{
        //    gasTank.PushTo(connection, value);
        //    return;
        //}

        // If this object can't answer, send a new request to the connected nodes
        if (SendRequest != null)
            SendRequest.Invoke(this, value);
    }

    /// <summary>
    /// Gets a connection by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Connectable GetConnection(int index)
    {
        return _connections[index];
    }

    /// <summary>
    /// Adds a connectable to the list. Connectables in the list are unique.
    /// </summary>
    /// <param name="pConnectable"></param>
    private void AddConnection(Connectable pConnectable)
    {
        if (_connections.Contains(pConnectable) || _connections.Count > _maximumConnections) return;
        _connections.Add(pConnectable);
    }

    /// <summary>
    /// Removes a connection from the list
    /// </summary>
    /// <param name="pConnectable"></param>
    private void RemoveConnection(Connectable pConnectable)
    {
        _connections.Remove(pConnectable);
    }

    /// <summary>
    /// Updates the internal lists of the Connectables
    /// </summary>
    protected void UpdateConnections(ref Connectable current, ref Connectable previous)
    {
        if (current != previous && current != null)
        {
            // Remove
            if (previous != null)
            {
                previous.RemoveConnection(this);
                RemoveConnection(previous);
            }

            // Add
            previous = current;
            current.AddConnection(this);
            AddConnection(current);
        }
    }

    /// <summary>
    /// Sends a request for <paramref name="energy"/>, coming from this issuing object.
    /// </summary>
    /// <param name="energy">The amound of requested energy.</param>
    protected void RequestEnergy(float energy)
    {
        SendRequest(this, energy);
    }
}