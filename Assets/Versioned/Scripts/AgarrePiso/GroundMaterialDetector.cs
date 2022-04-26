using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehiclePhysics{
    public class GroundMaterialDetector : MonoBehaviour
    {
        VPGroundMaterialManager groundMaterialManager;
        VehicleBase vehicle;
        GroundMaterial[] groundMaterialForWheel;
        GroundMaterialHit[] cachedGroundHits;
        GroundMaterialHit[] currentGroundHits;

        // Start is called before the first frame update
        void Start()
        {
            cachedGroundHits = new GroundMaterialHit[4]; //siempre hay 4 ruedas
            currentGroundHits = new GroundMaterialHit[4]; //para no tener que instanciar 4 GMHs en cada FixedUpdate
            groundMaterialForWheel = new GroundMaterial[4]; //el material sobre el cual esta cada rueda

            groundMaterialManager = GameObject.FindObjectOfType<VPGroundMaterialManager>(); //conseguimos el VPGMM

            vehicle = GetComponent<VehicleBase>(); //conseguimos el vehiculo que contiene a este script 
        }

        void FixedUpdate()
        {
            for (int i = 0; i < 4; i++){ //para cada una de las 4 ruedas...
                var groundCollider = vehicle.wheelState[i].hit.collider; //vemos contra que esta chocando la rueda

                if (groundCollider == null) return;
        
                //armamos un struct GroundMaterialHit que requiere la implementacion del VPGroundMaterialManager, para cada rueda
                //solamente nos interesa el PhysicMaterial que la rueda esta tocando, para buscarlo en la tabla del VPGMM.
                currentGroundHits[i].physicMaterial = groundCollider.sharedMaterial;

                //Si el material que cada rueda esta tocando (esta dentro de currentGroundHits) es distinto del que estaba tocando
                //(cachedGroundHits), entonces actualizamos el material que esa rueda esta tocando (groundMaterialOnWheel)
                //con el GroundMaterial asociado al PhysicMaterial del GameObject que esta tocando la rueda (esta asociacion esta
                //definida en el VPGroundMaterialManager). 
                groundMaterialManager.GetGroundMaterialCached(vehicle, currentGroundHits[i], ref cachedGroundHits[i], 
                                                                ref groundMaterialForWheel[i]);

                //Aplicamos el material a la rueda. 
                vehicle.wheelState[i].groundMaterial = groundMaterialForWheel[i];
                //Otra posibilidad seria cambiar la curva de friccion. usando vehicle.SetTireFriction()
            }

        }
    }
}
