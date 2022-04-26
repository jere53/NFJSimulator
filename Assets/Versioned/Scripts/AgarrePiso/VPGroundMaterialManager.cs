using UnityEngine;

namespace VehiclePhysics
{
    [AddComponentMenu("Vehicle Physics/Ground Materials/Ground Material Manager")]
    public class VPGroundMaterialManager : GroundMaterialManagerBase
    {
        public WeatherController weatherController; //referencia al Subject que es observado para el cambio de clima.
        public GroundMaterial[] groundMaterials = new GroundMaterial[0];
        public GroundWeatherInteraction[] groundWeatherInteractions = new GroundWeatherInteraction[0]; 
        public GroundMaterial fallback = new GroundMaterial();
        //public GroundWeatherInteraction fallbackGWI = new GroundWeatherInteraction();

        void AssociateGroundMatsToGWIs(){
            int c = 0;

            if (groundMaterials.Length <= groundWeatherInteractions.Length)
                c = groundMaterials.Length;
            else
                c = groundWeatherInteractions.Length;

            for (int i = 0; i < c; i++){ //asociar los GWIs a cada GM, posicionalmente
                //groundMaterials[i].customData = groundWeatherInteractions[i];
                groundWeatherInteractions[i].SetGroundMaterial(groundMaterials[i]);
                groundWeatherInteractions[i].BuildGADFDictionary();
                groundWeatherInteractions[i].SetWeatherController(weatherController);
                weatherController.Attach(groundWeatherInteractions[i]);
            }

            //si hay mas GWIs que GMs, no quedaron GMs sin asociar, retornamos
            //if (groundWeatherInteractions.Length >= groundMaterials.Length) return; 

            //si quedaron GroundMaterials sin asociar, asociarles el fallbackGWI
            //for (int i = groundWeatherInteractions.Length; i < groundMaterials.Length; i++){
                //groundMaterials[i].customData = fallbackGWI;
            //}
        }

        private void Awake() {
            AssociateGroundMatsToGWIs(); //asociamos los GMs a los GWIs
        }

        // Returns a GroundMaterial from the list based on the given PhysicMaterial.
        // null is also valid as physic material (colliders with no physic material assigned).

        //  vehicle     VehicleBase object which is querying the material
        //  groundHit   Contact information (physic material, collider, position)
        ///
        //  returns     A GroundMaterial from the list, or the fallback material if no matching
        //              PhysicMaterial is found

        public override GroundMaterial GetGroundMaterial (VehicleBase vehicle, GroundMaterialHit groundHit)
        {
            for (int i=0, c=groundMaterials.Length; i<c; i++)
            {
                if (groundMaterials[i].physicMaterial == groundHit.physicMaterial) {
                    return groundMaterials[i];
                }
            }
            return fallback;
        }

        public override void GetGroundMaterialCached (VehicleBase vehicle, GroundMaterialHit groundHit,
            ref GroundMaterialHit cachedGroundHit, ref GroundMaterial groundMaterial)
        {
            // Query the ground material (typically slow, table look-up) only when the physic material changes.
            // Otherwise do not change actual ground material reference.
            //
            // NOTE: This default implementation verifies the physic material only. collider and hitPoint
            // are ignored. This method must be overridden with a proper implementation if the
            // GetGroundMaterial implementation uses collider and/or hitPoint.


            if (groundHit.physicMaterial != cachedGroundHit.physicMaterial)
            {
                cachedGroundHit = groundHit;
                groundMaterial = GetGroundMaterial(vehicle, groundHit);
            }
        }
    }
}