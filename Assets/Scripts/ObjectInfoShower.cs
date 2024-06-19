using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectInfoShower : MonoBehaviour
{
      public TextMeshProUGUI nameText;
      public TextMeshProUGUI valueText;
      public GameObject infoPanel;

     public void ShowInfo(string nameObject, int valueObject)
     {
          infoPanel.SetActive(true);
          nameText.text = nameObject;
          valueText.text = valueObject.ToString();
     }
}
