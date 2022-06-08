using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CriterioRespetarSemaforos : MonoBehaviour, ICriterio
{
    private bool _evaluando;
    private bool _semaforoYaFueReportado;
    private Vector3 _posicionSemaforo;
    private Transform _centroVehiculo;
    private Collider _colliderUltimoSemaforo;
    private float _tiempoDesdeComienzo = 0;
    private List<float> _momentosInfracciones = new List<float>();
    private VPVehicleController _vehicleController;

    private bool isRecording;
    private CapturadorErrores _capturadorErrores;

    private void Awake()
    {
        _centroVehiculo = transform.Find("LimitesSemaforo");
        
        if (!_centroVehiculo)
        {
            Debug.LogError("No se encontro la transform con el centro del vehiculo en el GameObject correspondiente al vehiculo del Trainee.");
        }

        _vehicleController = GetComponent<VPVehicleController>();

        if (!_vehicleController)
        {
            Debug.LogError("No se encontro un VPVehicleController en el GameObject correspondiente al vehiculo del Trainee.");
        }
    }

    private void Update()
    {
        if (_evaluando)
        {
            _tiempoDesdeComienzo += Time.deltaTime;

            if (_colliderUltimoSemaforo)
            {
                //Existe la posibilidad de que solo haya un semaforo en la sesion y que tenga que pasar muchas veces por el sin tocar
                //otro semaforo. Debido al workaround usado en el OnTriggerEnter, si pasa mas de una vez por el mismo semaforo sin pasar
                //por otro, no se considerara. Debido a esto, mientras tengamos asignado un colliderUltimoSemaforo revisamos si la ambulancia
                //se alejo de el mas de 10 metros, o si el semaforo se puso en verde. De ser asi, la ambulancia ya no se considera en ese semaforo.
                
                if (_colliderUltimoSemaforo.enabled) //si se desactivado, no es necesario calcular una distancia
                {
                    if (Vector3.Distance(_centroVehiculo.position, _colliderUltimoSemaforo.bounds.center) > 10f)
                    {
                        _colliderUltimoSemaforo = null;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_evaluando) return;

        if (other.CompareTag("Semaforo"))
        {
            
            //Para que no reporte muchas infracciones cuando distintos colliders de la ambulancia interactuan con el semaforo.
            //mantenemos el registro del ultimo semaforo que se toco. Si se dispara el OnTriggerEnter mas de una vez con el mismo
            //semaforo, se ignorara.
            if (!_colliderUltimoSemaforo || !_colliderUltimoSemaforo.Equals(other))
            {
                _colliderUltimoSemaforo = other;

                if (_vehicleController.data.Get(Channel.Vehicle, VehicleData.Speed) / 1000f * 3.6 < 10f
                    && _vehicleController.data.Get(Channel.Input, InputData.Throttle) / 10000f < 50f)
                {
                    //Si se mueve a menos de 10 kmh y no esta acelerando, probablemente solo se adelanto un poco
                    Debug.Log("Se esta adelantando demasiado");
                }
                else
                {
                    _momentosInfracciones.Add(_tiempoDesdeComienzo);

                    if (isRecording)
                    {
                        _capturadorErrores.AddCapturaSemaforo(_tiempoDesdeComienzo);
                    }
                    
                    Debug.Log("SEMAFORO EN ROJO");

                }
            }
        }
    }
    

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datosEvaluacion)
    {
        datosEvaluacion.DatosCriterioRespetarSemaforos = _momentosInfracciones;
    }

    public void ComenzarEvaluacion()
    {
        _colliderUltimoSemaforo = null;
        _momentosInfracciones.Clear();
        _tiempoDesdeComienzo = 0;
        _evaluando = true;
        enabled = true;
    }

    public void ConcluirEvaluacion()
    {
        _evaluando = false;
        enabled = false;
    }
    
    public void Remover()
    {
        Destroy(this);
    }
    
    public void EnableRecording(CapturadorErrores capturadorErrores)
    {
        if(!capturadorErrores) return;

        _capturadorErrores = capturadorErrores;
        
        isRecording = true;
    }

    public void DisableRecording()
    {
        isRecording = false;
        _capturadorErrores = null;
    }



}
