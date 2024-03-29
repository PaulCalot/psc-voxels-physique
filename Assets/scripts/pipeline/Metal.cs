﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Metal : MonoBehaviour
{

    // Stores the Script Ref of MetalUpdater
    private MetalUpdater _refMetalUpdater;


    // Stores the Script Ref of Voxel
    Voxel referenceVoxel;


    private Toggle _toggle;

    // _visual
    private MeshRenderer _visual;

    // Thermal properties of a voxel
    private ThermoBody _thermals;

    // Indicates if we are showing temp variations through mesh, true means we are.
    public bool isThermDisplayed;

    // volume
    private float _v;

    // density
    private float _rho;

    //metal specific thermal a/o thermochemical properties

    // Probability of being a solvant
    public float p_solv = 0.5f;

    // chemical caracteristics
        // metal concentration (mole per cubic meter)
        public float c_m = 500.0f;

        // amount of metal (mole)
        private float _nm;
    
        // current amount of oxidised metal (mole); <= _nm
        private float _nox;

    // mecanical constants
        private float raideurInitial;
        private float raideurOx;    
        private float distanceInitialCoef;
        private float distanceOxCoef;

    // thermodynamic constants (some will be replaced by an abstract class)
        // atmospheric temperature (kelvin)
        
        
        // temperature (kelvin)
        private float _temp ;

        // amount of oxidised metal at thermodynamic balance (mole); <= _nm
        private float _neq;

    // cinetic constants
        // range of the evolution of cinetic
        private float _T1;
        private float _T2;
        private float _T3;
        
        // duration between two steps of the reaction (second)
        private float _DeltaT;

        // The time of the last reaction
        private float _lastReaction = 0f;

        // Frac and Fracmin enable to compute the reaction progress
        // prog is the percentage of progress at each step
        private float _prog; // prog is the percentage of progress at each step
        private float _P1;
        private float _P2;
        private float _P3;
        private float _R1;
        private float _R2;

        private float Fracmin; // Fracmin is the minimum part of _nm oxidised at each step 

    
    private void Awake() 
    {
        // Getting the meshrenderer of this gameobject
        _visual = this.GetComponent<MeshRenderer>();
        
        
        // Getting the thermal properties of the voxel
        _thermals = GetComponent<ThermoBody>();

        // tagging the metal
        tag = "Metal";
        
        // loading textures in C# file
        Material Iron = Resources.Load("Iron", typeof(Material)) as Material;
        _visual.material = Iron;
    }

    void Start()
    {
        _toggle = GameObject.FindWithTag("Toggle").GetComponent<Toggle>();

        // Gets the script Voxel
        this.referenceVoxel = this.GetComponent<Voxel>();

        // To find the platform that stores the lists of metals
        _refMetalUpdater = GameObject.FindWithTag("Platform").GetComponent<MetalUpdater>();
        
        Metal_Features mf = GetComponent<Metal_Features>();
        
        _rho = mf.rho;
        c_m = mf.GetC_m();
        p_solv = mf.p_solv;
        Fracmin = mf.Fracmin;
        _DeltaT = mf.DeltaT;

        raideurInitial = mf.raideurInitial;
        raideurOx = mf.raideurOx;
        distanceInitialCoef = mf.distanceInitialCoef;
        distanceOxCoef = mf.distanceOxCoef;

        
        _P1 = mf.P1;
        _P2 = mf.P2;
        _P3 = mf.P3;
        _T1 = mf.T1;
        _T2 = mf.T2;
        _T3 = mf.T3;
        
        _R1 = (_P2 - _P1)/(_T2 - _T1);
        _R2 = (_P3 - _P2)/(_T3 - _T2);

        // volume (cubic meter) and amount of metal
        _v = this.transform.localScale.x * this.transform.localScale.y * this.transform.localScale.z ;
        _nm = c_m*_v;

        // 0 atoms have been eroded initially
        _nox = 0f;
        
        // 0 acid initially
        _neq = 0f;

        // temp = Tatm
        _temp = _thermals.GetT();

        // initialising prog
        UpdateProg();

        // initialising isThermDisplayed
        if(GameObject.FindWithTag("Toggle").GetComponent<Toggle>().isOn == true)
        {
            isThermDisplayed = true;
            UpdateTextureThermals();
        }

    }

    public void UpdateProg() 
    {
        if(_temp <= _T1)
        {
            _prog = _P1;
        }
        else if (_temp <= _T2)
        {
            _prog = _P1 + _R1*(_temp - _T1);
        }
        else if (_temp <= _T3)
        {
            _prog = _P2 + _R2*(_temp - _T2);
        }
        else
        {
            _prog = _P3;
        }
    }

    // Updates the texture to show the corrosion
    private void UpdateTexture()
    {
        //Get the Renderer component from the new cube
        var cubeRenderer = _visual;

        //Get the proportion of the non-eroded atoms.
        float prop = (float)_nox/_nm;
        
        //for prop = 1, the triplet (R, G, B) defines the color Dark Brown
        float R = 1 - prop * (1.0f - 0.296f);
        float G = 1 - prop * (1.0f - 0.163f);
        float B = 1 - prop * (1.0f - 0.029f);

        //Call SetColor using the shader property name "_Color" and setting the color to (R, G, B)
        cubeRenderer.material.SetColor("_Color", new Color(R, G, B, prop/2));
 
    }

    private void DestroyMetal()
    {
        //Random Draw that decides if the metal is destroyed or passivated.    
        if(IsCompletelyCorroded())
        {
            //Uniform distribution
            float p =  Random.Range(0.0f, 1.0f);
            if(p <= p_solv)
            {
                // When destoryed it remove itself from the list of metals in Platform
                _refMetalUpdater.removeMetal(this.GetComponent<Metal>());

                // Removes the Mesh
                _visual.enabled = false;

                // Destroys the game object
                Destroy(gameObject);
            }
        }  
    }

    // Updates the texture to show the corrosion
    public void UpdateTextureThermals()
    {
        float _T = _thermals.GetT();

        //Get the percentage of heat considering that 
        float prop = ( Mathf.Min( Mathf.Max(_T, _T1), _T3) - _T1) / (_T3 - _T1);
        // the min and max are here to insure that prop won't get out of the [0, 1] interval.

        //for prop = 1, the triplet (R, G, B) defines the color Dark Brown
        float R = (1.0f - prop) * 0.0f   + prop * (1.0f);
        float G = (1.0f - prop) * 0.5f   + prop * (0.0f);
        float B = (1.0f - prop) * 1.0f   + prop * (0.0f);

        _visual.material.SetColor("_Color", new Color(R, G, B, prop/2));
    }


    public void UpdateDistance()
    {// Ronan ne change plus cette équation stp, (distanceOxCoef - distanceInitialCoef) est faux ça donne des résultats négatifs !
        this.referenceVoxel.breakingDistanceCoef = (_nox * distanceOxCoef + (_nm - _nox) * distanceInitialCoef) / _nm;
    }

    public void UpdateRaideur()
    {
        this.referenceVoxel.k = (_nox * raideurOx + (_nm - _nox) * raideurInitial) / _nm;
    }

    public void UpdateNox() {
        // recording that a new reaction starts
        _lastReaction = Time.time;
        
        // step is the reaction progress made at each time step
        // Fracmin*_nm ensures that the reaction ends in a finite time
        float step = Mathf.Max(_prog * ( _neq - _nox), Fracmin*_nm);
        
        // ensures that _nox <= eq
        _nox = Mathf.Min(_nox + step, _neq);

        //thermochemical consequence : heat release onto the voxel
        Debug.Log("Rho "+_rho);
        Debug.Log("Volume : "+_v);
        _thermals.ChangeT(_thermals.GetT() - step * (_thermals.Getdeltar_H0() / (_thermals.Getc() * _rho * _v)));
    }

    public void UpdateNeq(float delta) {
        _neq = Mathf.Min(_neq + delta, _nm);
    }

    public void UpdateTemp() 
    {
        _temp = _thermals.GetT();
    }

    public void UpdateMetal()
    {
        if(Time.time >= _lastReaction + _DeltaT && !this.EqReached())
        {   
            // Updating the cinetic according to temperature
            UpdateTemp();
            UpdateProg();
            
            // Trigger a reaction
            UpdateNox();

            // Updates the values of the parameters of the metal
            UpdateDistance();
            UpdateRaideur();

            // Updating the texture of the metal
            if(!isThermDisplayed)UpdateTexture();
            else UpdateTextureThermals();

            // Check If the MEtal is corroded, if it is it destroys it
            DestroyMetal();
        }
    }

    public float GetNm()
    {
        return _nm;
    }

    public float GetNeq() 
    {
        return _neq;
    }

    public bool EqReached()
    {
        return _nox >= _neq;
    }

    public bool IsCompletelyCorroded()
    {
        return _nox >= _nm;
    }

    void FixedUpdate(){
        if(_toggle.isOn == true && isThermDisplayed == false)
        {
           isThermDisplayed = true;
            UpdateTextureThermals();
        }
        else if(_toggle.isOn == false && isThermDisplayed == true)
        {
            Debug.Log("Coucou");
            isThermDisplayed = false;
            UpdateTexture();
        }
    }

}
