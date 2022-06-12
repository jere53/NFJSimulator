using UnityEngine;

public class InputReader : MonoBehaviour
{

    public FallasVehiculo FallasVehiculo;
    public Sirena Sirena;
    

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        //Sirena
        if (Input.GetKeyDown(KeyCode.Home))
        {
            Sirena.ToggleSirena();
        }

        if (Input.GetKeyDown(KeyCode.ScrollLock))
        {
            FallasVehiculo.DesempaniarVidrio();
        }
        
    }
}
