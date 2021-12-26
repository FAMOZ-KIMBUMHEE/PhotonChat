using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BanManager : MonoBehaviour
{
    [SerializeField] Dropdown dropdown;
    int beforePlayerListLength;

    // Update is called once per frame
    void Update()
    {
        if(beforePlayerListLength != PhotonNetwork.PlayerList.Length)
        {
            beforePlayerListLength = PhotonNetwork.PlayerList.Length;
            List<string> playerNameList = new List<string>();
            playerNameList.Add("All");
            foreach (var player in PhotonNetwork.PlayerList)
            {
                playerNameList.Add(player.ActorNumber.ToString());
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(playerNameList);
        }
    }

    public Player GetBanPlayer()
    {
        //All
        if(dropdown.value == 0)
        {
            return null;
        }
        else
        {
            return PhotonNetwork.PlayerList[dropdown.value-1];
        }
    }
}
