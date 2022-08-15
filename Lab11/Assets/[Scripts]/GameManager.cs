using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public GameObject localCamera;
    public GameObject canvas;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            // show start buttons
            StartButtons();
        }
        else
        {
            // show the status labels
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    public void StartButtons()
    {
        if (GUILayout.Button("Host"))
        {
            NetworkManager.Singleton.StartHost();
            ToggleCamera();
        }

        if (GUILayout.Button("Client"))
        {
            NetworkManager.Singleton.StartClient();
            ToggleCamera();
        }
    }

    public void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Mode: " + mode);
    }

    public void ToggleCamera()
    {
        localCamera.SetActive(!localCamera.activeInHierarchy);
        canvas.SetActive(!canvas.activeInHierarchy);

    }


}