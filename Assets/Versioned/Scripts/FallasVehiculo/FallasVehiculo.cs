using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VehiclePhysics;

public class FallasVehiculo : NetworkBehaviour
{
    //Componente que contiene la instancia de vehicle
    public GameObject myVehicle;
    
    //Componentes
    [Header("Wheels")]
    public VPWheelCollider flCollider;
    public VPWheelCollider frCollider;
    public VPWheelCollider rlCollider;
    public VPWheelCollider rrCollider;
    private VPVehicleController vc;
    private VPTelemetry tel;
    private float breakTorque;
    private float handBreakTorque;


    [Header("Tarnished glass")]
    public GameObject vidrio;
    private MeshRenderer _renderer;
    public float tarnishedTime;
    [Range(0.0f, 1f)]
    public float tarnishedIntensity;
    private float minTarnished = 0.6f;
    private float maxTarnished = 1f;
    private float tarnishedValue = 1f;
    private Coroutine c1;
    private Coroutine c2;
    public float fadeValue = 0.2f; //valor en segundos del tiempo que pasa entre cada iteracion de la corrutina
    private bool defoggerActivated = false;
    public float defoggerTime;

    private void Start()
    {
        vc = myVehicle.GetComponent<VPVehicleController>();
        tel = myVehicle.GetComponent<VPTelemetry>();
        breakTorque = vc.brakes.maxBrakeTorque;
        handBreakTorque = vc.brakes.handbrakeTorque;
        _renderer = vidrio.GetComponent<MeshRenderer>();
    }

    public void Frenos()
    {
        Debug.Log("Frenos cortados");
        if (vc.brakes.maxBrakeTorque != 0)
            vc.brakes.maxBrakeTorque = 0;
        else
            vc.brakes.maxBrakeTorque = breakTorque;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void CortarFrenosServerRpc()
    {
        Frenos();
    }


    public void FrenoDeManos()
    {
        Debug.Log("Frenos de manos cortados");
        if (vc.brakes.handbrakeTorque != 0)
            vc.brakes.handbrakeTorque = 0;
        else
            vc.brakes.handbrakeTorque = handBreakTorque;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void CortarFrenoDeManosServerRpc()
    {
        FrenoDeManos();
    }
    
    public void PincharRueda(int wheelIndex)
    {
        float radius = 0.33f; //0.33 es el valor default del radio de la rueda
        switch (wheelIndex)
        {
            case 0:
                radius = flCollider.radius;
                break;
            case 1:
                radius = frCollider.radius; 
                break;
            case 2:
                radius = rlCollider.radius; 
                break;
            case 3:
                radius = rrCollider.radius; 
                break;
        }
        Debug.Log("Rueda pinchada: " + wheelIndex);
        tel.vehicle.SetWheelRadius (wheelIndex, radius * 0.95f); //reduzco el radio del collider de la rueda un 5%
        //tel.vehicle.SetWheelTireFrictionMultiplier (wheelIndex, 2.25f); //aumento la friccion de la rueda un 125%
        tel.vehicle.SetWheelTireFrictionMultiplier (wheelIndex, 1.22f); //aumento la friccion de la rueda 
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void PincharRuedaServerRpc(int wheelIndex)
    {
        PincharRueda(wheelIndex);
    }
    
    public void ReventarRueda(int wheelIndex)
    {
        float radius = 0.33f; //0.33 es el valor default del radio de la rueda
        switch (wheelIndex)
        {
            case 0:
                radius = flCollider.radius;
                break;
            case 1:
                radius = frCollider.radius; 
                break;
            case 2:
                radius = rlCollider.radius; 
                break;
            case 3:
                radius = rrCollider.radius; 
                break;
        }
        Debug.Log("Rueda reventada: " + wheelIndex);
        tel.vehicle.SetWheelRadius (wheelIndex, radius * 0.75f); //reduzco el radio del collider de la rueda 
        tel.vehicle.SetWheelTireFrictionMultiplier (wheelIndex, 0.2f);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ReventarRuedaServerRpc(int wheelIndex)
    {
        ReventarRueda(wheelIndex);
    }
    
    public void EmpaniarVidrio()
    {
        if (!defoggerActivated)
        {
            //valor del empaniamiento maximo
            tarnishedValue = (maxTarnished - minTarnished) * Math.Abs(1 - tarnishedIntensity) + minTarnished; 
            c1 = StartCoroutine(CorutinaEmpaniar(tarnishedTime));
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void EmpaniarVidrioServerRpc()
    {
        EmpaniarVidrio();
    }

    IEnumerator CorutinaEmpaniar(float time)
    {
        Debug.Log("Empezo el empaniador");
        _renderer.material.shader = Shader.Find("HDRP/Lit");
        float incremento = (fadeValue / time) * (_renderer.material.GetFloat("_SmoothnessRemapMax") - tarnishedValue);
        Debug.Log(defoggerActivated);
        Debug.Log(_renderer.material.GetFloat("_SmoothnessRemapMax"));
        
        while (_renderer.material.GetFloat("_SmoothnessRemapMax") > tarnishedValue && !defoggerActivated)
        {
            float valorActual = _renderer.material.GetFloat("_SmoothnessRemapMax");
            _renderer.material.SetFloat("_SmoothnessRemapMax", valorActual - incremento);
            yield return new WaitForSeconds(fadeValue);
        }
        Debug.Log("Termino el empaniador");
        Debug.Log(_renderer.material.GetFloat("_SmoothnessRemapMax"));
    }
    
    
    public void DesempaniarVidrio()
    {
        if (!defoggerActivated)
        {
            defoggerActivated = true;
            c2 = StartCoroutine(CorutinaDesempaniar());
        }
        else
        {
            defoggerActivated = false;
        }
    }
    
    IEnumerator CorutinaDesempaniar()
    {
        Debug.Log("Empezo el defogger");
        _renderer.material.shader = Shader.Find("HDRP/Lit");
        float incremento = (fadeValue / defoggerTime) * (1 - _renderer.material.GetFloat("_SmoothnessRemapMax"));
        while (_renderer.material.GetFloat("_SmoothnessRemapMax") < 1)
        {
            float valorActual = _renderer.material.GetFloat("_SmoothnessRemapMax");
            _renderer.material.SetFloat("_SmoothnessRemapMax", valorActual + incremento);
            yield return new WaitForSeconds(fadeValue);
        }
        Debug.Log("Terminó el defogger");
    }
    
    




}
