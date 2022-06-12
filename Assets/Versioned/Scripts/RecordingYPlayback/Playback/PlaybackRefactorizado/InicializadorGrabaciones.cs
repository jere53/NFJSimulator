
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;


public class InicializadorGrabaciones : MonoBehaviour
{
    public PlaybackSemaforo[] semaforos;
    public GameObject[] prefabsVehiculos;
    public GameObject[] prefabsPeatones;
    public GameObject prefabTrainee;
    private GameObject trainee;
    public WeatherController weatherController;
    public DayNightCicle dayNightCicle;

    private int cantidadSemaforos;
    [HideInInspector]
    public int recordingFps;
    [HideInInspector]
    public int cantidadIntervalos;
    [HideInInspector]
    public int climaYToDFPS;
    [HideInInspector]
    public int cantidadIntervalosClimaYToD;
    

    public void InicializarGrabacion(string pathHeader, EstructuraGrabacion _estructuraGrabacion, Reproductor reproductor)
    {
        int cantidadVehiculos;
        int cantidadPeatones;
        reproductor.weatherController = weatherController;
        reproductor.dayNightCicle = dayNightCicle;
        using (BinaryReader reader = new BinaryReader(File.Open(pathHeader, FileMode.Open)))
        {
            cantidadVehiculos = reader.ReadInt32(); //el primer byte es el entero con la cant vehiculos...
            //leemos los modelos usados por cada uno de los vehiculos, para saber que instanciar
            for (int i = 0; i < cantidadVehiculos; i++)
            {
                //instanciamos los vehiculos
                int modeloVehiculo = reader.ReadInt32();
                int idVehiculo = i;
                GameObject v = Instantiate(prefabsVehiculos[modeloVehiculo]);
                v.SetActive(false);
                v.GetComponent<PlaybackVehiculo>().estructuraGrabacion = _estructuraGrabacion;
                v.GetComponent<PlaybackVehiculo>().reproductor = reproductor;
                v.GetComponent<PlaybackVehiculo>().id = idVehiculo;
                v.GetComponent<PlaybackVehiculo>().ComenzarAEscuchar();
                //vehiculos.Add(v);
            }

            cantidadPeatones = reader.ReadInt32();
            for (int i = 0; i < cantidadPeatones; i++)
            {
                //instanciamos los vehiculos
                int modeloPeaton = reader.ReadInt32();
                int idPeaton = i;
                GameObject p = Instantiate(prefabsPeatones[modeloPeaton]);
                p.SetActive(false);
                p.GetComponent<PlaybackPeaton>().estructuraGrabacion = _estructuraGrabacion;
                p.GetComponent<PlaybackPeaton>().reproductor = reproductor;
                p.GetComponent<PlaybackPeaton>().id = idPeaton;
                p.GetComponent<PlaybackPeaton>().ComenzarAEscuchar();
                //peatones.Add(p);
            }

            trainee = Instantiate(prefabTrainee);
            trainee.SetActive(false);
            trainee.GetComponent<PlaybackTrainee>().estructuraGrabacion = _estructuraGrabacion;
            trainee.GetComponent<PlaybackTrainee>().reproductor = reproductor;
            trainee.GetComponent<PlaybackTrainee>().ComenzarAEscuchar();
            //trainee.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            weatherController.rainParticleSystem = trainee.GetComponent<PlaybackTrainee>().rainParticleSystem;
            
            //InicializarIntersecciones(reader);

            cantidadSemaforos = reader.ReadInt32();
            for (int s = 0; s < cantidadSemaforos; s++)   
            {
                semaforos[s].estructuraGrabacion = _estructuraGrabacion;
                semaforos[s].reproductor = reproductor;
                semaforos[s].ComenzarAEscuchar();
            }
            
            recordingFps = reader.ReadInt32();
            cantidadIntervalos = reader.ReadInt32();
            

        }        
    }

    public void CargarEvals(string pathToEvalRecording, EstructuraGrabacion estructuraGrabacion)
    {
        string recording = File.ReadAllText(pathToEvalRecording);
        List<DatosEvaluacion> evals = JsonConvert.DeserializeObject<List<DatosEvaluacion>>(recording);
        estructuraGrabacion.evals = evals;
    }
    
    public void CargarFrames(string pathGrabacion, string pathClima, EstructuraGrabacion _estructuraGrabacion)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(pathGrabacion, FileMode.Open)))
        {
            CargarElementos(_estructuraGrabacion, reader);
        }
        using (BinaryReader reader = new BinaryReader(File.Open(pathClima, FileMode.Open)))
        {
            CargarClima(_estructuraGrabacion, reader);
        }
    }


    private void CargarElementos(EstructuraGrabacion _estructuraGrabacion, BinaryReader reader)
    {
        for (int intervalo = 0; intervalo < cantidadIntervalos; intervalo++)
        {
            EstructuraGrabacion.IntervaloGrabacion
                intervaloGrabacion = new EstructuraGrabacion.IntervaloGrabacion();
            
            intervaloGrabacion.snapshotVehiculos = new Dictionary<int, SnapshotVehiculo>();
            intervaloGrabacion.snapshotPeatones = new Dictionary<int, SnapshotPeaton>();
            intervaloGrabacion.snapshotSemaforo = new Dictionary<int, int>();

            //-------------------trainee------------------
            SnapshotTrainee snapshotTrainee = new SnapshotTrainee
            {
                posYrot =
                {
                    position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                },
                posYrotRuedaFL =
                {
                    position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                },
                posYrotRuedaFR =
                {
                    position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                },
                posYrotRuedaRL =
                {
                    position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                },
                posYrotRuedaRR =
                {
                    position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                },
                estadoSirena = reader.ReadBoolean()
            };
            

            _estructuraGrabacion.snapshotTraineeIntervalo = snapshotTrainee; //REVISAR. Para que actualiza la snapshot actual del trainee aca?

            intervaloGrabacion.snapshotTrainee = snapshotTrainee;
            //------------------------------------------


            //------------------Vehiculos---------------
            int cantVehiculosEnIntervalo = reader.ReadInt32();
            for (int v = 0; v < cantVehiculosEnIntervalo; v++)
            {
                int idVehiculo = reader.ReadInt32();
                //Debug.Log(idVehiculo);
                SnapshotVehiculo snapshotVehiculo = new SnapshotVehiculo
                {
                    posYrot =
                    {
                        position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    },
                    posYrotRuedaFL =
                    {
                        position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    },
                    posYrotRuedaFR =
                    {
                        position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    },
                    posYrotRuedaRL =
                    {
                        position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    },
                    posYrotRuedaRR =
                    {
                        position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    }
                };

                intervaloGrabacion.snapshotVehiculos.Add(idVehiculo, snapshotVehiculo);
            }
            //------------------------------------------


            //------------------Peatones----------------
            int cantPeatonesEnIntervalo = reader.ReadInt32();
            for (int p = 0; p < cantPeatonesEnIntervalo; p++)
            {
                int idPeaton = reader.ReadInt32();
                SnapshotPeaton snapshotPeaton = new SnapshotPeaton
                {
                    estaMuerto = reader.ReadBoolean(),
                    posYrot =
                    {
                        position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        eulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    },
                    velocidad = reader.ReadSingle()
                };

                intervaloGrabacion.snapshotPeatones.Add(idPeaton, snapshotPeaton);
            }
            //------------------------------------------

            for (int s = 0; s < cantidadSemaforos; s++)    //REVISAR. Por que se setea la estructura y comienza a escuchar aca y no cuando instancia los objetos???
            {
                int snapshotSemaforo = reader.ReadInt32();
                // semaforos[s].estructuraGrabacion = _estructuraGrabacion;
                // semaforos[s].ComenzarAEscuchar();
                intervaloGrabacion.snapshotSemaforo.Add(s, snapshotSemaforo);
            }

            _estructuraGrabacion.grabacion.Enqueue(intervaloGrabacion);
        }
        
    }
    

    private void CargarClima(EstructuraGrabacion _estructuraGrabacion, BinaryReader reader)
    {
        long fileLength = reader.BaseStream.Length;
        // _estructuraGrabacion.dayNightCicle = dayNightCicle;
        // _estructuraGrabacion.weatherController = weatherController;
        //Debug.Log(reader.ReadInt32());
        //climaYToDFPS = reader.ReadInt32();
        //fileLength -= 4; //los FPS
        while (fileLength > 0)
        {
            try
            {
                EstructuraGrabacion.IntervaloClimaToD intervaloClimaToD = new EstructuraGrabacion.IntervaloClimaToD
                {
                    clima = reader.ReadInt32(),
                    hora = reader.ReadSingle(),
                    velocidadOrbita = reader.ReadSingle(),
                };
                //Debug.Log(intervaloClimaToD.clima + " - " + intervaloClimaToD.hora + " - " + intervaloClimaToD.velocidadOrbita);
                fileLength -= 4;
                fileLength -= 4;
                fileLength -= 4;
                _estructuraGrabacion.grabacionClimaToD.Enqueue(intervaloClimaToD);
                cantidadIntervalosClimaYToD++;
            }
            catch (IOException e)
            {
                Debug.LogException(e);
                fileLength = 0;
            }
        }
    }
    
    
}
