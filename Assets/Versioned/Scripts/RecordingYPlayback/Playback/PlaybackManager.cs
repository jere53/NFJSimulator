using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlaybackManager : MonoBehaviour
{
    //public PlaybackInterseccion[] intersecciones;

    public PlaybackSemaforo[] semaforos;
    
    public GameObject[] prefabsVehiculos;

    public GameObject[] prefabsPeatones;

    public GameObject prefabTrainee;

    private List<GameObject> vehiculos = new List<GameObject>();
    private List<GameObject> peatones = new List<GameObject>();
    private GameObject trainee;

    public string pathToRecording;
    public string pathToHeader;
    public string pathToEvalRecording;
    public string pathToWeatherAndToDRecording;
    public WeatherController weatherController;
    public DayNightCicle dayNightCicle;
    

    [SerializeField] private EstructuraGrabacion _estructuraGrabacion;

    public bool play;
    
    int recordingFps;
    int cantidadIntervalos;

    private int cantidadSemaforos;

    public void BeginPlayback(string pathToRecordingFolder, string recordingName)
    {
        pathToRecording = pathToRecordingFolder + recordingName;
        pathToHeader = pathToRecordingFolder + Path.GetFileNameWithoutExtension(recordingName) + "Header.nfj";
        pathToWeatherAndToDRecording = pathToRecordingFolder + Path.GetFileNameWithoutExtension(recordingName) + "WeatherAndToD.nfj";
        pathToEvalRecording = pathToRecordingFolder + Path.GetFileNameWithoutExtension(recordingName) +
                              "Evals.json";
        
        SpawnearEIndexarGrabados();
        Debug.Log("Spawned");
        using (BinaryReader reader = new BinaryReader(File.Open(pathToRecording, FileMode.Open)))
        {
            CargarGrabacion(0, cantidadIntervalos, _estructuraGrabacion.grabacion, reader);
        }

        Debug.Log("loaded");
        
        CargarEvals();

        CargarGrabacionClima();
        
        StartCoroutine(Play());
        StartCoroutine(PlayClimaAndToD());

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) play = true; 
        if (play)
        {
            SpawnearEIndexarGrabados();
            Debug.Log("Spawned");
            using (BinaryReader reader = new BinaryReader(File.Open(pathToRecording, FileMode.Open)))
            {
                CargarGrabacion(0, cantidadIntervalos, _estructuraGrabacion.grabacion, reader);
            }

            Debug.Log("loaded");
            
            CargarEvals();
            
            CargarGrabacionClima();
            play = false;
            /*
            foreach (var interseccion in intersecciones)
            {
                if(_estructuraGrabacion.estadosInicialesIntersecciones.TryGetValue(interseccion.id, out var e))
                    interseccion.InicializarInterseccion(e);
            }
            */
            StartCoroutine(Play());
            StartCoroutine(PlayClimaAndToD());
        }
    }


    /*
    public void InicializarIntersecciones(BinaryReader reader)
    {
        int cantidadIntersecciones = reader.ReadInt32();
        for (int i = 0; i < cantidadIntersecciones; i++)
        {
            int id = reader.ReadInt32();
            float segundosActivada = reader.ReadSingle();
            float tiempoCadaSemaforoEnVerde = reader.ReadSingle();
            _estructuraGrabacion.estadosInicialesIntersecciones.Add(id, new EstadoInicialInterseccion
                {
                    segundosActivada = segundosActivada,
                    tiempoCadaSemaforoEnVerde = tiempoCadaSemaforoEnVerde
                }
            );
        }
        
    }
    */
    public void SpawnearEIndexarGrabados()
    {
        int cantidadVehiculos;
        int cantidadPeatones;
        using (BinaryReader reader = new BinaryReader(File.Open(pathToHeader, FileMode.Open)))
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
                p.GetComponent<PlaybackPeaton>().id = idPeaton;
                p.GetComponent<PlaybackPeaton>().ComenzarAEscuchar();
                //peatones.Add(p);
            }

            trainee = Instantiate(prefabTrainee);
            trainee.SetActive(false);
            trainee.GetComponent<PlaybackTrainee>().estructuraGrabacion = _estructuraGrabacion;
            trainee.GetComponent<PlaybackTrainee>().ComenzarAEscuchar();
            weatherController.rainParticleSystem = trainee.GetComponent<PlaybackTrainee>().rainParticleSystem;
            
            //InicializarIntersecciones(reader);

            cantidadSemaforos = reader.ReadInt32();
            
            recordingFps = reader.ReadInt32();
            cantidadIntervalos = reader.ReadInt32();

            Debug.Log(cantidadPeatones+ " " + cantidadVehiculos + " " + cantidadIntervalos);

        }
    }

    public void CargarEvals()
    {
        string recording = File.ReadAllText(pathToEvalRecording);
        List<DatosEvaluacion> evals = JsonConvert.DeserializeObject<List<DatosEvaluacion>>(recording);
        _estructuraGrabacion.evals = evals;
    }
    
    public void CargarGrabacion(int intervaloComienzo, int intervaloFinal, 
        Queue<EstructuraGrabacion.IntervaloGrabacion> destino, BinaryReader reader)
    {
        
            for (int intervalo = intervaloComienzo; intervalo < intervaloFinal; intervalo++)
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

                /*
                Debug.Log("POS");
    
                Debug.Log(snapshotTrainee.posYrot.position);
                Debug.Log(snapshotTrainee.posYrot.eulerAngles);
    
                Debug.Log("RUEDA FL");
                Debug.Log(snapshotTrainee.posYrotRuedaFL.position);
                Debug.Log(snapshotTrainee.posYrotRuedaFL.eulerAngles);
    
                Debug.Log("RUEDA FR");
    
                Debug.Log(snapshotTrainee.posYrotRuedaFR.position);
                Debug.Log(snapshotTrainee.posYrotRuedaFR.eulerAngles);
                Debug.Log("RUEDA RR");
    
                Debug.Log(snapshotTrainee.posYrotRuedaRR.position);
                Debug.Log(snapshotTrainee.posYrotRuedaRR.eulerAngles);
                Debug.Log("RUEDA RL");
    
                Debug.Log(snapshotTrainee.posYrotRuedaRL.position);
                Debug.Log(snapshotTrainee.posYrotRuedaRL.eulerAngles);
                */

                _estructuraGrabacion.snapshotTraineeIntervalo = snapshotTrainee;

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

                for (int s = 0; s < cantidadSemaforos; s++)
                {
                    int snapshotSemaforo = reader.ReadInt32();
                    semaforos[s].estructuraGrabacion = _estructuraGrabacion;
                    semaforos[s].ComenzarAEscuchar();
                    intervaloGrabacion.snapshotSemaforo.Add(s, snapshotSemaforo);
                }

                destino.Enqueue(intervaloGrabacion);
            }
    }

    private Queue<EstructuraGrabacion.IntervaloGrabacion> actualChunkGrabacion =
        new Queue<EstructuraGrabacion.IntervaloGrabacion>();
    private Queue<EstructuraGrabacion.IntervaloGrabacion> proximoChunkGrabacion =
        new Queue<EstructuraGrabacion.IntervaloGrabacion>();
    

    public IEnumerator Play()
    {
        float deltaIntervalos = 1f/recordingFps;
        Debug.Log("Intervalos: " + cantidadIntervalos+ "delta Intervalos" + deltaIntervalos);

        for (int intervalo = 0; intervalo < cantidadIntervalos; intervalo++)
        {
            _estructuraGrabacion.SiguienteIntervalo();
            
            //Como capturamos un intervalo Eval cada vez que un IntervaloSnapshot,
            //la cuenta de intervalos va a ser la misma!
            _estructuraGrabacion.PlayIntervaloEval(intervalo);
            
            yield return new WaitForSeconds(deltaIntervalos);
        }
        yield return null;
    }

    private int climaYToDFPS;
    private int cantidadIntervalosClimaYToD;
    public void CargarGrabacionClima()
    {
        using (BinaryReader reader = new BinaryReader(File.Open(pathToWeatherAndToDRecording, FileMode.Open)))
        {
            long fileLength = reader.BaseStream.Length;
            _estructuraGrabacion.dayNightCicle = dayNightCicle;
            _estructuraGrabacion.weatherController = weatherController;
            //climaYToDFPS = reader.ReadInt32();
            climaYToDFPS = recordingFps;
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
    
    public IEnumerator PlayClimaAndToD()
    {
        float deltaIntervalos = 1f/climaYToDFPS;
        Debug.Log(deltaIntervalos + "   intervalos Clima" + cantidadIntervalosClimaYToD);
        for (int intervalo = 0; intervalo < cantidadIntervalosClimaYToD; intervalo++)
        {
            //_estructuraGrabacion.PlayIntervaloClimaToD(); //CAMBIADO
            Debug.Log("Tiempo deltaIntervalos: " + deltaIntervalos);
            yield return new WaitForSeconds(deltaIntervalos);
        }
        yield return null;
    }
}
