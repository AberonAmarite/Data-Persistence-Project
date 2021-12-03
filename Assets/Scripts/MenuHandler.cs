using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
   [SerializeField] TMP_InputField inputName;
    public static string username;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickStart() {
        SaveName();
        LoadMain();
    }
    private void LoadMain() {
        SceneManager.LoadScene(1);
    }
    private void SaveName() { 
        username = inputName.text;
    }

}
