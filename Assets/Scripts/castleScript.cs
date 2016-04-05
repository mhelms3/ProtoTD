using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class castleScript : MonoBehaviour {

    
    public int castleLevel = 1;
    StructureBehavior sb;
 
    void Start()
    {
        
        sb = (StructureBehavior)GetComponentInParent(typeof(StructureBehavior));
        sb.integrity = 99.9f;
        sb.percentComplete = 99.9f;
        sb.buildFlag = true;
        sb.workerSpeed = 10;  
        sb.structureName = "Player's Castle";
        //sb.SendMessage("UpdateStatusBars");
    }
  
}
