using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class buttonBehaviorScript : MonoBehaviour {

    public Text helpText;

    void showHelpText()
    {
        helpText.enabled = true;
    }
    void hideHelpText()
    {
        helpText.enabled = false;
    }

   void Start()
    {
        Text[] texts = gameObject.GetComponentsInChildren<Text>();
        foreach (Text t in texts)
        {
            if (t.tag == "HelpText")
            {
                helpText = t;
                print(helpText.text);
                helpText.enabled = false;

            }
        }
    }
}
