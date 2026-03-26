using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlugController : MonoBehaviour
{
    public bool isConected = false;
    public UnityEvent OnWirePlugged;
    public Transform plugPosition;

    [SerializeField]
    public float disconnectTime = 1f;
    
    [HideInInspector]
    public Transform endAnchor;
    [HideInInspector]
    public Rigidbody endAnchorRB;
    [HideInInspector]
    public WireController wireController;

    private bool _isDisconnecting;
    private Coroutine _disconnectRoutine;
    
    public void OnPlugged()
    {
        OnWirePlugged.Invoke();
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (_isDisconnecting)
        {
            return;
        }
        
        if (other.gameObject == endAnchor.gameObject)
        {
            isConected = true;
            endAnchorRB.isKinematic = true;
            endAnchor.transform.position = plugPosition.position;
            endAnchor.transform.rotation = transform.rotation;


            OnPlugged();
        }
    }

    private void Update()
    {
        if (isConected)
        {
            endAnchorRB.isKinematic = true;
            endAnchor.transform.position = plugPosition.position;
            Vector3 eulerRotation = new Vector3(this.transform.eulerAngles.x + 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            endAnchor.transform.rotation = Quaternion.Euler(eulerRotation);
        }
    }

    public void StartDisconnect()
    {
        if (_disconnectRoutine != null)
        {
            StopCoroutine(_disconnectRoutine);
            _disconnectRoutine = null;
        }
        
        _disconnectRoutine = StartCoroutine(DisconnectRoutine(disconnectTime));
    }

    private IEnumerator DisconnectRoutine(float delayTime)
    {
        _isDisconnecting = true;
        
        yield return new WaitForSeconds(delayTime);
        
        _isDisconnecting = false;
    }
    
    public void SetDisconnectState(bool state = true)
    {
        _isDisconnecting = state;
    }
}
