using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentTest : MonoBehaviour
{
    public List<NavMeshAgent> agents;
    // Start is called before the first frame update
    void Start()
    {
        foreach (NavMeshAgent agent in agents)
        {
            agent.SetDestination(transform.position);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
