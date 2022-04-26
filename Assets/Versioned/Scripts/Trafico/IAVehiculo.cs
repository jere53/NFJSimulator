using System;
using System.Collections;
using System.Collections.Generic;
using Trafico;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class IAVehiculo : MonoBehaviour
{
    [SerializeField] private Waypoint destinoActual;
    private Vehiculo cuerpo;
    public float distanciaLlegoDestino;
    
    public Estado estado;
    
    private float pasoPosRayo;
    public Transform radarTransform; //centro en donde se van a castear los rayos
    public int radarCantRayos; //cantidad de rayos que se van a castear
    public float radarAnchura; //ancho de la superficie donde salen los rayos
    public float umbralFrenado; // distancia maxima a un obstaculo en la cual se frena
    public float umbralRelentizar; // distancia maxima a un obtaculo en la cual se reduce la velocidad
    public float aperturaRayosAngulo;
    private void Awake()
    {
        CargarPosicionesRayos();
        estado = new Estado(Estado.AVANZAR);
        cuerpo = gameObject.GetComponent<Vehiculo>();
    }
    
    public void HabilitarDetecionObstaculos()
    {
       StartCoroutine(DetectarObstaculos(0.1f));
    }
    
    private void CargarPosicionesRayos()
    {
        pasoPosRayo = radarAnchura / (radarCantRayos - 1);
    }
    
    void Update()
    {
        if (distancia(destinoActual.transform) < distanciaLlegoDestino)
        {
            destinoActual = destinoActual.ObtenerSiguiente(Random.Range(0f, 1f));
        }

        if (!destinoActual)
        {
            cuerpo.DeSpawn();
            return;
        }

        MoverHaciaDestino();
    }
    
    public float distancia(Transform objeto)
    {
        Vector3 destino = objeto.transform.position;
        destino.y = 0;

        Vector3 posActual = transform.position;
        posActual.y = 0;

        return Vector3.Magnitude(destino - posActual);
    }
    
    public void MoverHaciaDestino()
    {
        cuerpo.aplicarMovimiento(estado.porcentajeAceleracion, estado.porcentajeFrenado, ObtenerAngulo(destinoActual.transform));
    }

    public float ObtenerAngulo(Transform objeto)
    {
        Vector3 relativePos = objeto.position - transform.position; // la x de relative pos me dice si esta a la derecha (positivo) o izquierda (negativo) y la z me dice si esta adelante (+) o atras (-)
        relativePos = relativePos / (Math.Abs(relativePos.x) + Math.Abs(relativePos.z));
        relativePos.y = 0;
        var tf = transform.forward;
        Vector3 forward = tf / (Math.Abs(tf.x)  + Math.Abs(tf.z));
        float angle = Vector3.SignedAngle(relativePos, forward, Vector3.up);
        return -angle/4;
    }

    public void SetDestino(Waypoint waypoint)
    {
        destinoActual = waypoint;
    }

    public void CambiarEstado(int estado)
    {
        this.estado.cambiarEstado(estado);
    }
    
    IEnumerator DetectarObstaculos(float time)
    {
        while (true)
        {
            int estado = 0;
            bool colisiono = false;
    
            Vector3 posicionLocalGuardado = radarTransform.localPosition;
            float rotacion = -aperturaRayosAngulo;
            float pasoRotacion = aperturaRayosAngulo * 2 / (radarCantRayos - 1);
    
            radarTransform.localPosition = new Vector3( - (pasoPosRayo + (radarCantRayos - 1) * pasoPosRayo/2), posicionLocalGuardado.y, posicionLocalGuardado.z) ;
            for(int i = 0; i < radarCantRayos; i++)
            {
                radarTransform.localPosition = new Vector3(radarTransform.localPosition.x + pasoPosRayo, posicionLocalGuardado.y, posicionLocalGuardado.z) ;
                RaycastHit hit;
                //DrawRay(radarTransform.position, Quaternion.Euler(0, rotacion, 0) * this.transform.forward * umbralRelentizar, Color.red, (float)0.1);
                if (Physics.Raycast(radarTransform.position, Quaternion.Euler(0, rotacion, 0) * transform.forward * umbralRelentizar, 
                    out hit, umbralRelentizar, 1 << 8 | 1 << 12 | 1 << 13 | 1 << 15, QueryTriggerInteraction.Collide))
                {
                    colisiono = true;
                    if (hit.distance <= umbralFrenado)
                    {
                        estado = Estado.FRENAR;
                        break;
                    }

                    estado = Estado.REDUCIR_VELOCIDAD;
                } else
                {
                    if(!colisiono)
                        estado = Estado.AVANZAR;
                }

                rotacion += pasoRotacion;
            }
    
            radarTransform.localPosition = posicionLocalGuardado;
            CambiarEstado(estado);
            yield return new WaitForSecondsRealtime(time);
        }
    }
    
}
