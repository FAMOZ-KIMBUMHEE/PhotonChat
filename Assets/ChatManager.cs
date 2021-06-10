using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    string userName;
    string currentChannelName;

    public InputField inputField;
    public InputField nameinputField;
    public InputField privateinputField;
    public Text outputText;

    string gameVersion = "1.0";

    public GameObject userNameBox;
    public GameObject chatBox;

    [SerializeField] BanManager banManager;

    void Start()
    {
        SettingInit();
    }

    //초기셋팅
    void SettingInit()
    {
        Application.runInBackground = true;
        currentChannelName = "002";
        chatClient = new ChatClient(this);
    }

    void Update()
    {
        chatClient.Service();
    }

    //닉네임 설정 후 서버 접속
    public void SetUserName()
    {
        userName = nameinputField.text;
        PhotonNetwork.LocalPlayer.NickName = userName;

        NextChat();

        ClientConnect();
    }

    void NextChat()
    {
        userNameBox.SetActive(false);
        chatBox.SetActive(true);
    }

    //서버연결시도
    void ClientConnect()
    {
        //new AuthenticationValues(userName) => userName을 userID로 전환
        chatClient.Connect(ChatSettings.Instance.AppId, gameVersion, new AuthenticationValues(userName));
        AddLine(string.Format("연결시도", userName));
    }

    //프로그램이 종료되면 연결해제
    private void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }
    //서버연결성공
    public void OnConnected()
    {
        AddLine("서버에 연결되었습니다.");
        //currentChannelName 이름의 채널 구독
        chatClient.Subscribe(new string[] { currentChannelName}, 10);


        //자신의 상태 설정
        chatClient.SetOnlineStatus(ChatUserStatus.Online, null);
    }
    //서버연결실패
    public void OnDisconnected()
    {
        AddLine("서버에 연결이 끊어졌습니다.");
    }

    //채널구독성공
    public void OnSubscribed(string[] channels, bool[] results)
    {
        //string.Join = 문자마다 추가문자를 넣음
        AddLine(string.Format("채널 입장 ({0})", string.Join(",", channels)));
    }

    //채널구독끊어짐
    public void OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));

    }

    //메시지 보내기
    public void Input_OnEndEdit(string text)
    {
        if (chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            string selectUserName = privateinputField.text;
            //채널로 원하는 메시지를 전송함 -> OnGetMessages에서 받은 메시지를 처리함
            if (selectUserName == "")
            {
                chatClient.PublishMessage(currentChannelName, inputField.text);
            }
            else
            {
                string _text = string.Format("가 {0} 에게 귓속말 : {1}", selectUserName, inputField.text);
                chatClient.SendPrivateMessage(selectUserName, _text);

            }
            inputField.text = "";
        }
    }

    //받은 메세지를 라인에 추가
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //senders = 사용유저
        //messages = 내용
        for (int i = 0; i < messages.Length; i++)
        {
            if (senders[i] != banManager.GetBanPlayer().NickName)
            {
                AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));

            }
            else
            {
                AddLine("채팅이 차단되었습니다.");
            }

        }
    }

    //채팅
    public void AddLine(string lineString)
    {
        outputText.text += lineString + "\r\n";
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        AddLine(string.Format("{0}{1}", sender, message.ToString()));
    }

    #region unUseCallback
    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);
    }

    public void OnUserSubscribed(string channel, string user)
    {
        AddLine(string.Format("채널 입장 ({0})", channel));

    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        AddLine(string.Format("채널 퇴장 ({0})", channel));
    }
    #endregion
}
