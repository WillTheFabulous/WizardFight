

using MLAPI;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    bool gameStarted = false;
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
            gameStarted = true;
        }

        GUILayout.EndArea();
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    void FixedUpdate()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
            out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<WizardMovement>();
            if (player)
            {
                Vector3 newPosition = new Vector3(0, 0, 0);
                Vector3 newEulerAngles = new Vector3(0, 0, 0);
                player.GetNewPosition(Time.deltaTime, out newPosition, out newEulerAngles);

                if (!NetworkManager.Singleton.IsServer)
                {
                    player.SubmitPositionRequestServerRpc(newPosition);
                    player.SubmitLocalEulerAnglesRequestServerRpc(newEulerAngles);
                }
                else
                {
                    player.Position.Value = newPosition;
                    player.LocalEulerAngles.Value = newEulerAngles;
                }
            }
        }
    }

    void Update()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
            out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<WizardMovement>();
            if (player)
            {
                if (Input.GetButton("Fire1") && Time.time > player.nextFire)
                {
                    player.nextFire = Time.time + player.fireRate;
                    //AudioSource.PlayClipAtPoint(fireSoundEffect, gameObject.transform.position);
                    player.SubmitShootBulletRequestServerRpc();
                }
            }
        }
    }
}