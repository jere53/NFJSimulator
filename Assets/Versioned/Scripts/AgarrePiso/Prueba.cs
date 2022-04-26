using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehiclePhysics{
public class Prueba : MonoBehaviour
{
    public VPGroundMaterialManager gmm;
    public VPVehicleController vehicle;
    // Start is called before the first frame update
    void Start()
    {
        //vehicle = GetComponent<VPVehicleController>();
        var ws = vehicle.wheelState[1];
        ws.groundMaterial = gmm.groundMaterials[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (vehicle.wheelState[1].groundMaterial == null) return;

        vehicle.wheelState[0].groundMaterial = gmm.groundMaterials[0];
        vehicle.wheelState[1].groundMaterial = gmm.groundMaterials[0];
        vehicle.wheelState[2].groundMaterial = gmm.groundMaterials[0];
        vehicle.wheelState[3].groundMaterial = gmm.groundMaterials[0];
    }
}

}