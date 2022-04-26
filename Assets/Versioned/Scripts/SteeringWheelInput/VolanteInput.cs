using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VehiclePhysics;

public class VolanteInput : MonoBehaviour
{

    [SerializeField] private VehicleBase vehiculoTrainee;

    // Use this for initialization
    void Start()
    {
        vehiculoTrainee = GetComponent<VehicleBase>();
        vehiculoTrainee.data.Set(Channel.Input, InputData.AutomaticGear, 0); //transmision manual
        vehiculoTrainee.data.Set(Channel.Settings, SettingsData.AutoShiftOverride, 2);
        vehiculoTrainee.data.Set(Channel.Input, InputData.Retarder, 0); //retarder brake apagado
        Debug.Log("SteeringInit:" + LogitechGSDK.LogiSteeringInitialize(false));
    }

    void OnApplicationQuit()
    {
        Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }

    int InputVolanteFormateada(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        float x = (rec.lX / 32768f); //normalizamos el valor
        return (int) (x * 10000); //lo llevamos a las unidades usadas por el VPP
    }

    int InputAceleradorFormateada(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        if (rec.lY >= 0)
        {
            return 0;
        }

        return (int) ((rec.lY / -32768f) * 10000);

    }

    int InputFrenoFormateada(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        if (rec.lRz >= 0)
        {
            return 0;
        }

        return (int)((rec.lRz / -32768f) * 10000);
    }

    int InputEmbragueFormateada(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        if (rec.rglSlider[1] >= 0)
        {
            return 0;
        }

        return (int)((rec.rglSlider[1] / -32768f) * 10000);
    }

    int InputCambiosFormateadosSecuencial(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        //El VPP toma como input una cantidad de cambios a subir/bajar. El volante tiene botones con distintos numeros
        //de cambio. Para traducir, conseguimos el cambio que el volante nos dice y devolvmeos cuantos cambios subir/bajar
        //en el VPP.
        int cambioActual = vehiculoTrainee.data.Get(Channel.Vehicle, VehicleData.GearboxGear);
        for (int i = 0; i < 128; i++)
        {
            if (rec.rgbButtons[i] == 128) //128 debe indicar el boton que este presionado (probablemente)
            {
                //Valores usados por el G29
                switch (i)
                {
                    case 8: //arrib
                        return 1;
                    case 9: //ABAJO
                        return -1;
                }
            }

        }

        return 0;
    }

    void Shift(int cantidadShifts)
    {
        while (cantidadShifts > 0)
        {
            vehiculoTrainee.data.Set(Channel.Input, InputData.GearShift, 1);
            cantidadShifts--;
        }
        while (cantidadShifts < 0)
        {
            vehiculoTrainee.data.Set(Channel.Input, InputData.GearShift, -1);
            cantidadShifts++;
        }
    }
    void SetCambiosFormateados(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        //El VPP toma como input una cantidad de cambios a subir/bajar. El volante tiene botones con distintos numeros
        //de cambio. Para traducir, conseguimos el cambio que el volante nos dice y devolvmeos cuantos cambios subir/bajar
        //en el VPP.
        int cambioActual = vehiculoTrainee.data.Get(Channel.Vehicle, VehicleData.GearboxGear);
        for (int i = 0; i < 128; i++)
        {
            if (rec.rgbButtons[i] == 128) //128 debe indicar el boton que este presionado (probablemente)
            {
                Debug.Log(i + " " + cambioActual);
                //Valores usados por el G29
                switch (i)
                {
                    case 8: //Primera
                        //Shift(1 - cambioActual);
                        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 1);
                        return;
                    case 9: //segunda
                        //Shift(2 - cambioActual);
                        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 2);
                        return;
                    case 10: //tercera
                        //Shift(3 - cambioActual);
                        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 3);
                        return;
                    case 11: //cuarta
                        //Shift(4 - cambioActual);
                        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 4);
                        return;
                    case 12: //quinta
                        //Shift(5 - cambioActual);
                        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 5);
                        return;
                    case 13: //sexta
                        //Shift(6 - cambioActual);
                        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 6);
                        return;
                    case 14: //reversa
                        //Shift(-1 - cambioActual);
                        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, -1);
                        return;
                    default: vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 0);
                       return;
                }
            }

        }
        vehiculoTrainee.data.Set(Channel.Input, InputData.ManualGear, 0);
    }

    void LlaveIgnicion(LogitechGSDK.DIJOYSTATE2ENGINES rec)
    {
        for (int i = 0; i < 128; i++)
        {
            if (rec.rgbButtons[i] == 128) //128 debe indicar el boton que este presionado (probablemente)
            {
                if (i == 7)
                {
                    if (vehiculoTrainee.data.Get(Channel.Vehicle, VehicleData.EngineWorking) == 0)
                    {
                        vehiculoTrainee.data.Set(Channel.Input, InputData.Key, 1);
                        return;
                    }
                   
                }
                if (i == 15)
                {
                    vehiculoTrainee.data.Set(Channel.Input, InputData.Key, -1);
                    return;
                }
                
            }
            vehiculoTrainee.data.Set(Channel.Input, InputData.Key, 0);
        }
            
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //All the test functions are called on the first device plugged in(index = 0)
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);
            vehiculoTrainee.data.Set(Channel.Input, InputData.Steer, InputVolanteFormateada(rec));
            vehiculoTrainee.data.Set(Channel.Input, InputData.Throttle, InputAceleradorFormateada(rec));
            vehiculoTrainee.data.Set(Channel.Input, InputData.Brake, InputFrenoFormateada(rec));
            vehiculoTrainee.data.Set(Channel.Input, InputData.Clutch, InputEmbragueFormateada(rec));
            SetCambiosFormateados(rec);
            
            LlaveIgnicion(rec);

        }
    }
}
