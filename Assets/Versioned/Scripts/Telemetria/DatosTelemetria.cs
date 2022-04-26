using System;
using System.Net;
using System.Net.Sockets;
using EdyCommonTools;
using UnityEngine;
using VehiclePhysics;

public class DatosTelemetria : MonoBehaviour
{
    public VPTelemetry telemetria;

    public Socket soc;
    // Start is called before the first frame update
    void Start()
    {
        telemetria = GameObject.FindWithTag("Auto").GetComponent<VPTelemetry>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //Creo el socket
        soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   

        //Defino la ip de destino la cual va a ser mi localhost
        string server = "127.0.0.1";
        IPAddress ipAdd = IPAddress.Parse(server);
        
        //Apuntar a donde se enviara el mensaje
        IPEndPoint remoteEP = new IPEndPoint(ipAdd, 3456);
    
        //realizo la conexion
        soc.Connect(remoteEP);

        //creo el mensaje
        string mensaje = "velocidad(k/h): " + Math.Truncate(Math.Abs(telemetria.vehicle.data.Get(Channel.Vehicle, VehicleData.Speed) * 3.6f / 1000.0f));
        byte[] msg = System.Text.Encoding.UTF8.GetBytes(mensaje);
        //msg[msg.Length] = 's';

        //envio el mensaje
        soc.Send(msg);

        //cierro la conexion
        soc.Close();
    }

    private void crearConexion()
    {

    }
    
    private void imprimirDatos()
    {
        Debug.Log("velocidad(k/h): " + telemetria.vehicle.data.Get(Channel.Vehicle, VehicleData.Speed)*3.6f/1000.0f);
        Debug.Log("RPM motor: " + telemetria.vehicle.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 1000.0f);
        Debug.Log("Carga del motor: " + telemetria.vehicle.data.Get(Channel.Vehicle, VehicleData.EngineLoad) / 1000.0f);
        Debug.Log("Cambio: " + telemetria.vehicle.data.Get(Channel.Vehicle, VehicleData.GearboxGear));
        
        // fuerzas G aplicadas al vehiculo en cada eje. 1 unidad G es igual a la aceleracion de la gravedad (9.8m/s)
        Debug.Log("fuerza G longitudinal: " + telemetria.vehicle.localAcceleration.z / Gravity.reference);
        Debug.Log("fuerza G lateral: " + telemetria.vehicle.localAcceleration.x / Gravity.reference);
        Debug.Log("fuerza G vertical: " + telemetria.vehicle.localAcceleration.y / Gravity.reference);
        
        // pitch es la rotacion del vehiculo en el eje transversal (cuanto se inclina hacia adelante o atras)
        // tiene un rango entre -180 y 180
        Debug.Log("Pitch: " + MathUtility.ClampAngle(telemetria.vehicle.cachedTransform.eulerAngles.x));
        Debug.Log("velocidad Pitch(radianes por segundo): " + telemetria.vehicle.cachedRigidbody.angularVelocity.x);
        
        // roll es la rotacion del vehiculo en el eje longitudinal (caunto se inclina hacia los costados)
        // tiene un rango entre -180 y 180
        Debug.Log("Roll: " + MathUtility.ClampAngle(telemetria.vehicle.cachedTransform.eulerAngles.z));
        Debug.Log("velocidad Roll(radianes por segundo): " + telemetria.vehicle.cachedRigidbody.angularVelocity.z);
        
        // Yaw es la rotacion del vehiculo en el eje vertical
        // tiene un rango entre 0 y 360
        Debug.Log("Yaw: " + MathUtility.ClampAngle(telemetria.vehicle.cachedTransform.eulerAngles.x));
        Debug.Log("velocidad Yaw(radianes por segundo): " + telemetria.vehicle.cachedTransform.eulerAngles.y); // revisar
        
        Debug.Log("Embrague: " + telemetria.vehicle.data.Get(Channel.Vehicle, VehicleData.ClutchTorque));
    }
}
