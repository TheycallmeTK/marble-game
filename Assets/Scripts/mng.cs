using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mng : MonoBehaviour
{
    public List<GameObject> checkpoints = new List<GameObject>();
    public int startingCheckpoint;
    public GameObject player;
    // Start is called before the first frame update

    void Awake()
    {
        
        if(startingCheckpoint != 0)
        {
            Vector3 checPos = checkpoints[startingCheckpoint - 1].transform.position;
            player.transform.position = new Vector3(checPos.x, checPos.y+5, checPos.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
