using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverPanelManager : PanelManager
{
    [SerializeField] Text messageText;


    public void SetMessage(string message)
    {
        messageText.text = message;
    }
    public void OnClickConfirm(Button button)
    {
        button.interactable = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
