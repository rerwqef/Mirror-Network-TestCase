using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{

    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject mainMenuPanel;
    /*    [SerializeField] TMP_InputField userNameinputField;
        [SerializeField] TextMeshProUGUI userNametext;
        [SerializeField] GameObject userNamePannel;*/
  /*  private void Start()
    {
        if (!Application.isBatchMode)
        {
            Debug.Log("clint connected");
            networkManager.StartClient();
        }
        else
        {
            Debug.Log("server connected");
        }
    }*/
    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        mainMenuPanel.SetActive(false);
        networkManager.StartClient();
    }

    public void Host()
    {
        networkManager.networkAddress = "localhost"; // Set network address for hosting
        mainMenuPanel.SetActive(false);
        networkManager.StartHost();
    }
    public void Disconnect()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // This instance is a host and a client
            networkManager.StopHost();
            networkManager.StopClient();
        }
        else if (NetworkClient.isConnected)
        {
            // This instance is a client
            networkManager.StopClient();
        }

        // Show main menu panel again
        mainMenuPanel.SetActive(true);
    }
}