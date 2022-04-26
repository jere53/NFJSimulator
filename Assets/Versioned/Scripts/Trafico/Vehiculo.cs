using System;
using UnityEngine;

namespace Trafico
{
    public class Vehiculo : MonoBehaviour
    {

        private IAVehiculo _iaVehiculo;

        private SpawnManager _spawnManager;

        [SerializeField] private float maximaFuerzaMotor;
        [SerializeField] private float velocidadMaxima;
        [SerializeField] private float maximaFuerzaFrenado;
        [SerializeField] private float velicidadMaximaDobando;
        
        [SerializeField] private WheelCollider flWheelCollider;
        [SerializeField] public WheelCollider frWheelCollider;
        [SerializeField] public WheelCollider rlWheelCollider;
        [SerializeField] public WheelCollider rrWheelCollider;
        
        [SerializeField] public Transform flWheelColliderTransform;
        [SerializeField] public Transform frWheelColliderTransform;
        [SerializeField] public Transform rlWheelColliderTransform;
        [SerializeField] public Transform rrWheelColliderTransform;
        
        private Rigidbody rb;


        private void Awake()
        {
            _iaVehiculo = GetComponent<IAVehiculo>();
            rb = GetComponent<Rigidbody>();
        }


        public void Spawn(Vector3 origen, Waypoint destino, SpawnManager spawnManager)
        {
            transform.position = origen;
            transform.LookAt(destino.transform);
            _spawnManager = spawnManager;
            _spawnManager.vehiculosHabilitados.Add(this);
            _iaVehiculo.SetDestino(destino);
            gameObject.SetActive(true);
            _iaVehiculo.HabilitarDetecionObstaculos();
        }

        public void DeSpawn()
        {
            _spawnManager.ReSpawnVehiculo(this);
        }

        public void aplicarMovimiento(float aceleracion, float frenado, float doblaje)
        {
            flWheelCollider.motorTorque = aceleracion * maximaFuerzaMotor;
            frWheelCollider.motorTorque = aceleracion * maximaFuerzaMotor;
            
            frWheelCollider.brakeTorque = frenado * maximaFuerzaFrenado;
            flWheelCollider.brakeTorque = frenado * maximaFuerzaFrenado;
            rrWheelCollider.brakeTorque = frenado * maximaFuerzaFrenado;
            rlWheelCollider.brakeTorque = frenado * maximaFuerzaFrenado;

            flWheelCollider.steerAngle = doblaje;
            frWheelCollider.steerAngle = doblaje;
            
            // actualizo la posicion de las ruedas a partir de sus wheel colliders
            actualizarRueda(flWheelCollider, flWheelColliderTransform);
            actualizarRueda(frWheelCollider, frWheelColliderTransform);
            actualizarRueda(rlWheelCollider, rlWheelColliderTransform);
            actualizarRueda(rrWheelCollider, rrWheelColliderTransform);
            
            
            float velocidadMetrosSegundos = velocidadMaxima / 3.6f;
            float s = rb.velocity.magnitude;

            float anguloNormalizado = Math.Abs(doblaje) / 70f;  // el doblaje maximo se da cuando el angulo para doblar es mayor o igual a 70 grados
            if (anguloNormalizado >= 1)
                anguloNormalizado = 1;

            float velocidadRestante = (velocidadMaxima - velicidadMaximaDobando)/ 3.6f;

            velocidadMetrosSegundos -= velocidadRestante * anguloNormalizado;
            
            if(s > velocidadMetrosSegundos) rb.velocity = velocidadMetrosSegundos * rb.velocity.normalized;
        }

        private void actualizarRueda(WheelCollider wheelCollider, Transform wheelTransform)
        {
            Vector3 pos;
            Quaternion rot;
            wheelCollider.GetWorldPose(out pos, out rot);
            wheelTransform.rotation = rot;
            wheelTransform.position = pos;
        }

    }

}