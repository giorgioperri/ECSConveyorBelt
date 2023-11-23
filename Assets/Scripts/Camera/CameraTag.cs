using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTag : MonoBehaviour
{
    private static CameraTag _instance;

    public static CameraTag Instance { get { return _instance; } }

    public Quaternion Rotation;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Update()
    {
        Rotation = transform.rotation;
    }
}
