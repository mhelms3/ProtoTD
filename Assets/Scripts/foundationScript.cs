using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class foundationScript : MonoBehaviour {

    StructureBehavior sb;
   
    void AddMaterialSpecials (string s)
    {

        //string[] ruinMaterial = { "Marble", "Granite", "Obsidian", "Limestone", "Basalt", "Brownstone", "Flagstone", "Quadratum" };
        switch (s)
        {
            case "Marble":
                sb.specials.Add("Marble: +1 defense");
                sb.specials.Add("Marble:+100 value");
                break;
            case "Granite":
                sb.specials.Add("Granite: +3 defense");
                sb.specials.Add("Granite: half damage from ALL physical");
                break;
            case "Obsidian":
                sb.specials.Add("Obsidian: +3 dark damage");
                sb.specials.Add("Obsidian: +3 magic defense");
                sb.specials.Add("Obsidian: blocks ethereal");
                break;
            case "Limestone":
                sb.specials.Add("Limestone: +2 defense");
                sb.specials.Add("Limestone: +3 acid damage");
                break;
            case "Basalt":
                sb.specials.Add("Basalt: +2 defense");
                sb.specials.Add("Basalt: +3 frost damage");
                break;
            case "BrownStone":
                sb.specials.Add("BrownStone: +2 defense");
                sb.specials.Add("BrownStone: +3 crushing damange");
                break;
            case "FlagStone":
                sb.specials.Add("FlagStone: +2 defense");
                sb.specials.Add("FlagStone: +1 crushing damange");
                break;
            case "Quadratum":
                sb.specials.Add("Quadratum: +2 defense");
                sb.specials.Add("Quadratum: +1 ALL damange");
                break;
        }
    }

	void RuinStart (string s)
    {
        
        sb.foundationMaterial = s;
        sb.structureName = "Ruin";
        sb.SendMessage("updateStructureName");
        
        sb.buildFlag = false;
        sb.integrity = 50f;
        sb.percentComplete = 50f;
        sb.SendMessage("updateStatusBars");
        sb.specials.Add("Ruins: no material cost");
        sb.specials.Add("Ruins: double labor cost");
        sb.specials.Add("Ruins: +5% for undead/level");
        sb.specials.Add("Ruins: +5 defense");
        sb.specials.Add("Ruins: +25% of item on explore");
        AddMaterialSpecials(s);
        
    }

    
    

	void Awake() {
        sb = (StructureBehavior)GetComponentInParent(typeof(StructureBehavior));
    }
}
