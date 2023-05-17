using EnemyAI;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GameLoader : MonoBehaviour 
{
    public static Game instance;
    
    private void Awake()
    {
        instance=this;
    }
    
    // Define dependency variables here...
}
