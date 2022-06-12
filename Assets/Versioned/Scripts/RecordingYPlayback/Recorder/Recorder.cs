using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Recorder: MonoBehaviour
{
    private static Recorder _instance;
    public static Recorder instancia { get { return _instance;} }
    
    public Dictionary<int, SnapshotVehiculo> capturasVehiculos;
    public Dictionary<int, SnapshotPeaton> capturasPeatones;
    public SnapshotTrainee capturaTrainee;
    
    public Dictionary<int, int> colorSemaforos;
    public int cantidadSemaforos;

    //public Dictionary<int, EstadoInicialInterseccion> estadosInicialesIntersecciones;

    [NonSerialized]public List<int> modelosVehiculos;
    [NonSerialized]public List<int> modelosPeatones;
    
    [SerializeField] private WeatherController instanciaClima;
    [SerializeField] private DayNightCicle instanciaCicloDia;

    [NonSerialized]public int indiceIntervalo;
    private Coroutine c1;
    private Coroutine c2;
    private bool estaGrabando;
    private int fps;

    public delegate void CaptureFrame();
    public event CaptureFrame OnCapture;

    private FileStream fsHeader;
    private BinaryWriter _writerHeader;
    private BinaryReader _readerHeader;
    
    private FileStream fs;
    private BinaryWriter _writer;
    private BinaryReader _reader;

    private FileStream fsClima;
    private BinaryWriter _writerClima;
    private BinaryReader _readerClima;
    public static String fileClimaName = "grabacionClimaCiclo.nfj";
    private static String pathClima;
    
    //public static String folder = @"C:\Users\Franco\Documents\";
    public static String folder = @"E:\Facultad\Ingeniería\Grabacion2\";
    public static String fileName = "grabacion.nfj";
    public static String fileHeaderName = "grabacionHeader.nfj";
    

    private static String path;
    private static String pathHeader;
    
    private void Awake()
    {
        modelosVehiculos = new List<int>();
        modelosPeatones = new List<int>();
        //estadosInicialesIntersecciones = new Dictionary<int, EstadoInicialInterseccion>();
        _instance = this;
        
        colorSemaforos = new Dictionary<int, int>();
        path = folder + fileName;
        if (File.Exists(path))
            File.Delete(path);
        fs = File.Create(path);
        _writer = new BinaryWriter(fs);
        _reader = new BinaryReader(fs);    
        
        pathHeader = folder + fileHeaderName;
        if (File.Exists(pathHeader))
            File.Delete(pathHeader);
        fsHeader = File.Create(pathHeader);
        _writerHeader = new BinaryWriter(fsHeader);
        _readerHeader = new BinaryReader(fsHeader);
        
        pathClima = folder + fileClimaName;
        if (File.Exists(pathClima))
            File.Delete(pathClima);
        fsClima = File.Create(pathClima);
        _writerClima = new BinaryWriter(fsClima);
        _readerClima = new BinaryReader(fsClima);
        
        indiceIntervalo = 0;
        capturasVehiculos = new Dictionary<int, SnapshotVehiculo>();
        capturasPeatones = new Dictionary<int, SnapshotPeaton>();
    }

    public void GrabacionManager(int fps, int fpsClima)
    {
        if(estaGrabando)
            _instance.PararDeGrabar();
        else
            _instance.Grabar(fps, fpsClima);
    }

    private void Grabar(int fps, int fpsClima)
    {
        Debug.Log("Comenzo la grabacion");
        estaGrabando = true;
        this.fps = fps;
        float time = (float)1/fps;
        c1 = StartCoroutine(CorutinaGrabacion(time));
        //InvokeRepeating(nameof(CapturarFrame), 0, time);
        float timeClima = (float)1/fpsClima;
        _writerClima.Write(fpsClima);
        c2 = StartCoroutine(CorutinaClima(timeClima));
        
    }

    private void PararDeGrabar()
    {
        estaGrabando = false;
        StopCoroutine(c1);
        StopCoroutine(c2);
        //CancelInvoke(nameof(CapturarFrame));
        escribirHeader();
        //leerArchivo();
        fsClima.Dispose();
        fsHeader.Dispose();
        fs.Dispose();
        Debug.Log("Se detuvo la grabacion");
    }

    private void leerArchivo()
    {
        _reader.BaseStream.Seek(0, SeekOrigin.Begin);
        _readerHeader.BaseStream.Seek(0, SeekOrigin.Begin);
        Debug.Log(leerHeader()); 
        for (int i = 0; i < indiceIntervalo; i++)
        {
            Debug.Log("Intervalo numero " + i);
            Debug.Log("      Ambulancia: ");
            Debug.Log("            Transform Ambulancia: " + leerTransform());
            Debug.Log("            Rueda FL: " + leerTransform());
            Debug.Log("            Rueda FR: " + leerTransform());
            Debug.Log("            Rueda RL: " + leerTransform());
            Debug.Log("            Rueda RR: " + leerTransform());

            int cantVehiculoInt = _reader.ReadInt32();
            Debug.Log("      Vehiculos: " + cantVehiculoInt);
            for (int j = 0; j < cantVehiculoInt; j++)
            {
                int idVehiculo = _reader.ReadInt32();
                Debug.Log("            Transform Vehiculo " + idVehiculo + ":" + leerTransform());
                Debug.Log("            Rueda FL: " + leerTransform());
                Debug.Log("            Rueda FR: " + leerTransform());
                Debug.Log("            Rueda RL: " + leerTransform());
                Debug.Log("            Rueda RR: " + leerTransform());
            }
            
            int cantPeatonesInt = _reader.ReadInt32();
            Debug.Log("      Peatones: " + cantPeatonesInt);
            for (int j = 0; j < cantPeatonesInt; j++)
            {
                int idPeaton = _reader.ReadInt32();
                Debug.Log("            Transform Peaton " + idPeaton + ":" + leerTransform());
                Debug.Log("            Velocidad: " + _reader.ReadSingle());
            }
        }
    }

    public String leerHeader()
    {
        String retorno;
        int cantVehiculos = _readerHeader.ReadInt32();
        retorno = "Cant vehiculos: " + cantVehiculos + '\n';
        for (int i = 0; i < cantVehiculos; i++)
        {
            retorno += "    Modelo del vehiculo " + i + ": " + _readerHeader.ReadInt32() + '\n';
        }
        
        int cantPeatones = _readerHeader.ReadInt32();
        retorno += "Cant peatones: " + cantPeatones + '\n';
        for (int i = 0; i < cantPeatones; i++)
        {
            retorno += "    Modelo del vehiculo " + i + ": " + _readerHeader.ReadInt32() + '\n';
        }
        
        retorno += "fps: " + _readerHeader.ReadInt32() + '\n';
        retorno += "cantidad intervalos: " + _readerHeader.ReadInt32() + '\n';

        return retorno;

    }
   
    public String leerTransform()
    {
        String retorno = "(" + _reader.ReadSingle() + ", " + _reader.ReadSingle() + ", " + _reader.ReadSingle() + ")   (" + _reader.ReadSingle() + ", " + _reader.ReadSingle() + ", " + _reader.ReadSingle() + ")";
        return retorno;
    }

    IEnumerator CorutinaGrabacion(float time)
    {
        while (true)
        {
            if(OnCapture != null)
                OnCapture();
            
            escribirArchivo();

            indiceIntervalo++;
            capturasVehiculos.Clear();
            capturasPeatones.Clear();
            colorSemaforos.Clear();
            yield return new WaitForSeconds(time);
        }
    }

    IEnumerator CorutinaClima(float time)
    {
        while (true)
        {
            String clima = instanciaClima.currentWeatherPreset;
            switch (clima)
            {
                case"sunny":
                    _writerClima.Write(0);
                    break;                
                case"rain":
                    _writerClima.Write(1);
                    break;                
                case"cloudy":
                    _writerClima.Write(2);
                    break;
            }

            float hora = instanciaCicloDia.timeOfDay;
            float velocidadOrbita = instanciaCicloDia.orbitSpeed;
            _writerClima.Write(hora);
            _writerClima.Write(velocidadOrbita);
            yield return new WaitForSeconds(time);
        }
    }
    
    void CapturarFrame()
    {
        if(OnCapture != null)
            OnCapture();
            
        escribirArchivo();

        indiceIntervalo++;
        capturasVehiculos.Clear();
        capturasPeatones.Clear();
    }
        
    private void escribirArchivo()
    {
        escribirTransform(capturaTrainee._transform);
        escribirTransform(capturaTrainee.ruedaFL);
        escribirTransform(capturaTrainee.ruedaFR);
        escribirTransform(capturaTrainee.ruedaRL);
        escribirTransform(capturaTrainee.ruedaRR);
        _writer.Write(capturaTrainee.estadoSirena);

        
        _writer.Write(capturasVehiculos.Count);
        foreach(KeyValuePair<int, SnapshotVehiculo> entry in capturasVehiculos)
        {
            _writer.Write(entry.Key);
            escribirTransform(entry.Value._transform);
            escribirTransform(entry.Value.ruedaFL);
            escribirTransform(entry.Value.ruedaFR);
            escribirTransform(entry.Value.ruedaRL);
            escribirTransform(entry.Value.ruedaRR);
        }
        _writer.Write(capturasPeatones.Count);
        foreach(KeyValuePair<int, SnapshotPeaton> entry in capturasPeatones)
        {
            _writer.Write(entry.Key);
            _writer.Write(entry.Value.estaMuerto);
            escribirTransform(entry.Value._transform);
            _writer.Write(entry.Value.velocidad);
        }
        
        foreach(var pair in colorSemaforos.OrderBy(p => p.Key)) {
            _writer.Write(pair.Value);
        }
        
    }

    private void escribirTransform(Transform transform)
    {
        var position = transform.position;
        _writer.Write(position.x);
        _writer.Write(position.y);
        _writer.Write(position.z);
        var rotation = transform.rotation;
        _writer.Write(rotation.eulerAngles.x);
        _writer.Write(rotation.eulerAngles.y);
        _writer.Write(rotation.eulerAngles.z);
    }

    private void escribirHeader()
    {
        _writerHeader.Write(modelosVehiculos.Count);
        foreach (int modelo in modelosVehiculos)
        {
            _writerHeader.Write(modelo);
        }

        _writerHeader.Write(modelosPeatones.Count);
        foreach (int modelo in modelosPeatones)
        {
            _writerHeader.Write(modelo);
        }
        
        /*
        _writerHeader.Write(estadosInicialesIntersecciones.Count);
        foreach (var entry in estadosInicialesIntersecciones)
        {
            _writerHeader.Write(entry.Key);
            _writerHeader.Write(entry.Value.segundosActivada);
            _writerHeader.Write(entry.Value.tiempoCadaSemaforoEnVerde);
        }
        */
        
        _writerHeader.Write(cantidadSemaforos);
        _writerHeader.Write(fps);
        _writerHeader.Write(indiceIntervalo);
    }
}
