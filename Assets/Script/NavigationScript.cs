using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

using NavMeshSurface = NavMeshPlus.Components.NavMeshSurface;
using UnityEngine.SceneManagement;

public class NavigationScript : MonoBehaviour
{
    public Transform player;
    public Transform navMesh2D;
    private bool playerCreated;
    private bool destinationSet;
    NavMeshAgent navMeshAgent;
    public new Camera camera;



    // Start is called before the first frame update
    void Start()
    {
        playerCreated = false;
        destinationSet= false;
        this.BakeNavMesh();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!playerCreated)
            {
                Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                clickPosition.z = 0f;
                CreatePlayer(clickPosition);
            }
            else
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);     // vector de movimiento al punto donde se dio el click
                NavMeshHit navMeshHit;      // guarda información del punto donde se clickeo

                if (NavMesh.SamplePosition(ray.origin, out navMeshHit, 500, NavMesh.AllAreas)){ // si el área está dentro del navmesh
                    Vector3 destination = navMeshHit.position;  // punto donde se ha clickeado
                    
                    if (IsReachable(destination)){
                        destinationSet = true;
                        navMeshAgent.SetDestination(destination);
                    }
                    else{
                        UnityEngine.Debug.Log("Posición inalcanzable debido a obstáculos");
                    }
                    
                }
                
                else{
                    UnityEngine.Debug.Log("POSICIÓN INALCANZABLE");
                    UnityEngine.Debug.Log(navMeshHit.position);
                    UnityEngine.Debug.Log("Mouse Position: " + Input.mousePosition);
                }   
            }
        }
        CompprobarFinished();
    }


    private void BakeNavMesh(){
        NavMeshSurface navMeshSurface = navMesh2D.GetComponent<NavMeshSurface>();
        if(navMeshSurface != null)    
            navMeshSurface.BuildNavMesh();
        else
            UnityEngine.Debug.Log("navmesh no encontrado");
    }

    private void CreatePlayer(Vector3 clickPosition){
        // Instantiate(player, clickPosition, Quaternion.identity);
        player.transform.position = clickPosition;
        navMeshAgent = player.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.enabled = true;    // recuerda: disable el navmesh agent del jugador en unity de antemano para que funcione 
            playerCreated = true;
        }
        else
        {
            UnityEngine.Debug.LogError("El componente NavMeshAgent no se encontró en el jugador.");
        }
    }

    private void CompprobarFinished(){
        if (playerCreated && navMeshAgent != null && destinationSet
        && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
        && (navMeshAgent.hasPath ||navMeshAgent.velocity.sqrMagnitude == 0f))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    private bool IsReachable(Vector3 destination){

      NavMeshPath path = new NavMeshPath();
      navMeshAgent.CalculatePath(destination, path);
      return path.status == NavMeshPathStatus.PathComplete;
    }
}
