﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thermo_Features : MonoBehaviour
{
    // Temperature of the atmosphere (Kelvin)
    public float Tatm = 300f;

    // Time in between each heat exchange
    public float deltaT = 0.15f;

    // specific heat capacity kJ/(K*kg)
    public const float C = 0.5f;

    // thermal diffusivity
    public const float D = 0.00001f;

    public float Dboost = 1f;

}
