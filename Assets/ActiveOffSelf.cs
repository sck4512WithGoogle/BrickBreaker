using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOffSelf : MonoBehaviour
{
    [SerializeField] private float waitTime = 0.4f;

    private void OnEnable()
    {
        Invoke("ActiveOff", waitTime);
    }

    private void ActiveOff()
    {
        gameObject.SetActive(false);
    }
}
