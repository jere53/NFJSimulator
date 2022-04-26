using System;

namespace Trafico
{
    public class Estado
    {                                                            
        public static int AVANZAR = 0;
        public static int REDUCIR_VELOCIDAD = 1;
        public static int FRENAR = 2;

        public float porcentajeAceleracion { get;  set; }
        public float porcentajeFrenado{ get; set; }

        private int estadoActual;

        public Estado(int estado)
        {
            cambiarEstado(estado);
        }

        public void cambiarEstado(int estado)
        {
            int estadoParcial = Math.Max(0, Math.Min(2, estado)); // si el parametro estado no esta en el rango de alguno de los estados definidos, entonces se setea al minimo o maximo
            estadoActual = estadoParcial;
            if (estadoActual == AVANZAR)
            {
                porcentajeAceleracion = 1;
                porcentajeFrenado = 0;
            } else if (estadoActual == REDUCIR_VELOCIDAD)
            {
                porcentajeAceleracion = (float) 0.5;
                porcentajeFrenado = 0;
            }
            else if (estadoActual == FRENAR)
            {
                porcentajeAceleracion = 0;
                porcentajeFrenado = 1;
            }
        }

        

    }
}