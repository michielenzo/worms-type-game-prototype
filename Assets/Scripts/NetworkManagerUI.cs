using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() => {
            bool success = NetworkManager.Singleton.StartServer();
            if (success) {
                serverBtn.GetComponent<Image>().color = Color.yellow;
                hostBtn.GetComponent<Image>().color = Color.white;
                clientBtn.GetComponent<Image>().color = Color.white;
            }
        });
        hostBtn.onClick.AddListener(() => {
            bool success = NetworkManager.Singleton.StartHost();
            if (success) {
                serverBtn.GetComponent<Image>().color = Color.white;
                hostBtn.GetComponent<Image>().color = Color.yellow;
                clientBtn.GetComponent<Image>().color = Color.white;
            }
        });
        clientBtn.onClick.AddListener(() => {
            bool success = NetworkManager.Singleton.StartClient();
            if (success) {
                serverBtn.GetComponent<Image>().color = Color.white;
                hostBtn.GetComponent<Image>().color = Color.white;
                clientBtn.GetComponent<Image>().color = Color.yellow;
            }
        });
    }
}
