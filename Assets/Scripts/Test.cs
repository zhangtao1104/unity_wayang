using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;
using System;
using System.Text;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public string url = "ws://114.236.93.153:8083/iov/websocket/dual?topic=11";
    private WebSocket webSocket;
    private string tag = "websocket: ";
    public Button send;
    public Button connect;
    public Button close;
    public InputField inputMessage;
    public Text logText;
    public Application application;

    void Start()
    {
        Connect();
        SendMessage();
    }

    void Update()
    {
        if (webSocket != null)
        {
            
#if (!UNITY_WEBGL || UNITY_EDITOR)
        
#else
        int status = (int) websocket.GetConnectionStatus();
        Debug.Log("connection status  ==== " + status);
#endif
        }
        SendMessage();
    }

    private void Init(string id)
    {
        Uri uri= new Uri(url + id);
        DebugLog("Init host = " + uri.Host + " Scheme = " + uri.Scheme + " Authority = " + uri.Authority);
        webSocket = new WebSocket(uri);
        webSocket.StartPingThread = true;
        webSocket.OnBinary += OnWebSocketBinaryDelegate;
        webSocket.OnClosed += OnWebSocketClosedDelegate;
        webSocket.OnError += OnWebSocketErrorDelegate;
        webSocket.OnOpen += OnWebSocketOpenDelegate;
        webSocket.OnMessage += OnWebSocketMessageDelegate;
        webSocket.OnErrorDesc += OnWebSocketErrorDescriptionDelegate;
    }

    public void Connect()
    {
        Init("123");
        webSocket.Open();
    }

    public void DebugLog(string message)
    {
        //logText.text += message + "\r\n";
        Debug.Log(message);
    }

    public void Close()
    {
        webSocket.Close();
    }

    public void SendMessage()
    {
        DebugLog("sendMessage ");
        webSocket.Send(System.Text.Encoding.UTF8.GetBytes("2322"));
    }

    void OnWebSocketBinaryDelegate(WebSocket webSocket, byte[] data)
    {
        DebugLog(tag + "OnWebSocketBinaryDelegate  " + System.Text.Encoding.UTF8.GetString(data));
    }

    void OnWebSocketClosedDelegate(WebSocket webSocket, UInt16 code, string message)
    {
        DebugLog(tag + "OnWebSocketClosedDelegate  errorCode = " + code + message);
    }

    void OnWebSocketErrorDelegate(WebSocket webSocket, Exception ex)
    {
        DebugLog(tag + "OnWebSocketErrorDelegate " + ex);
        
    }

    void OnWebSocketErrorDescriptionDelegate(WebSocket webSocket, string reason)
    {
        DebugLog(tag + "OnWebSocketErrorDescriptionDelegate"  + "reason");
    }

    void OnWebSocketOpenDelegate(WebSocket webSocket)
    {
        DebugLog(tag + "OnWebSocketOpenDelegate " );
    }

    void OnWebSocketMessageDelegate(WebSocket webSocket, string message)
    {
        DebugLog(tag + "OnWebSocketMessageDelegate  " + message);
    }
    
}
