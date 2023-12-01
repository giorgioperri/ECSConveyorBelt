using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameParameterManager : MonoBehaviour
{
    [SerializeField] private GameObject _HUD;

    [SerializeField] private TextMeshProUGUI _spawnAmt;
    [SerializeField] private TextMeshProUGUI _speedAmt;
    [SerializeField] private TextMeshProUGUI _impulseAmt;
    [SerializeField] private TextMeshProUGUI _correctAmt;
    [SerializeField] private TextMeshProUGUI _wrongAmt;

    public int RepulsionForce = 1;
    public int SpawnRate = 1;
    public int BeltSpeed = 1;
    
    public int Correct = 0;
    public int Wrong = 0;
    
    //create a singleton
    private static GameParameterManager _instance;
    public static GameParameterManager Instance => _instance;
    
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

    private void Start()
    {
        SetValues();
    }
    
    public void SetValues()
    {
        _spawnAmt.text = SpawnRate.ToString();
        _speedAmt.text = BeltSpeed.ToString();
        _impulseAmt.text = RepulsionForce.ToString();
        _correctAmt.text = Correct.ToString();
        _wrongAmt.text = Wrong.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _HUD.SetActive(Input.GetKey(KeyCode.Tab));

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetValues();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            RepulsionForce--;
            SetValues();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            RepulsionForce++;
            SetValues();
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            BeltSpeed--;
            SetValues();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            BeltSpeed++;
            SetValues();
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnRate--;
            SetValues();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SpawnRate++;
            SetValues();
        }
    }
}
