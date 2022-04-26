using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using VehiclePhysics;
public class FeedbackFisico : MonoBehaviour
{
    public VehicleBase vehiculoTrainee;

    //en m/s
    private List<float> velocidadesRegistradas = new List<float>();
    private List<float> aceleracionesEnX = new List<float>();
    private List<float> aceleracionesEnY = new List<float>();
    private List<float> aceleracionesEnZ = new List<float>();
    public Socket soc;
    private UdpClient enviadorTelemetria;
    
    private void FixedUpdate()
    {
        velocidadesRegistradas.Add((vehiculoTrainee.data.Get(Channel.Vehicle, (VehicleData.Speed))/1000.0f * 3.6f)); //para que este en KM/h
        aceleracionesEnX.Add(vehiculoTrainee.localAcceleration.x/3.6f);
        aceleracionesEnY.Add(vehiculoTrainee.localAcceleration.y/3.6f);
        aceleracionesEnZ.Add(vehiculoTrainee.localAcceleration.z/3.6f);
    }

    float Moda(List<float> l)
    {
        float m = l.GroupBy(n => n).
            OrderByDescending(g => g.Count()).
            Select(g => g.Key).FirstOrDefault();
        return m;
    }
    
    //calculamos moda cada 1 segundo
    IEnumerator EnviarModasTCP()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
           
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Defino la ip de destino la cual va a ser mi localhost
            string server = "127.0.0.1";
            IPAddress ipAdd = IPAddress.Parse(server);

            //Apuntar a donde se enviara el mensaje
            IPEndPoint remoteEP = new IPEndPoint(ipAdd, 5000);

            //realizo la conexion
            soc.Connect(remoteEP);

            //creo el mensaje
            string mensaje = "" + "velocidad: " + Moda(velocidadesRegistradas) + ", " 
                             + "aceleracionEnX " + Moda(aceleracionesEnX)
                             + ", " + "aceleracionEnY " + Moda(aceleracionesEnY) 
                             + ", " +  "aceleracionEnZ " + Moda(aceleracionesEnZ) +  "\n";
            byte[] msg = System.Text.Encoding.UTF8.GetBytes(mensaje);
            velocidadesRegistradas.Clear();
            aceleracionesEnX.Clear();
            aceleracionesEnY.Clear();
            aceleracionesEnZ.Clear();
            
            soc.Send(msg);

            //cierro la conexion
            soc.Close();

        }
    }

    IEnumerator EnviarModasUDP()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); 
            //UdpClient enviadorTelemetria = new UdpClient(5000);
            try
            {
                //enviadorTelemetria.Connect(IPAddress.Parse("127.0.0.1"), 5000);
                string mensaje = "" + "velocidad: " + Moda(velocidadesRegistradas) + "\n" 
                                 + "aceleracionEnX " + Moda(aceleracionesEnX)
                                 + "\n" + "aceleracionEnY " + Moda(aceleracionesEnY) 
                                 + "\n" +  "aceleracionEnZ " + Moda(aceleracionesEnZ);
                byte[] msg = System.Text.Encoding.UTF8.GetBytes(mensaje);
                soc.Send(msg);
                velocidadesRegistradas.Clear();
                aceleracionesEnX.Clear();
                aceleracionesEnY.Clear();
                aceleracionesEnZ.Clear();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                enviadorTelemetria.Close();
            }
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        //soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //soc.Connect(IPAddress.Parse("127.0.0.1"), 5000);
        vehiculoTrainee = GetComponent<VehicleBase>();
        StartCoroutine(EnviarModasTCP());
        //StartCoroutine(EnviarModasUDP());
    }
    
}
