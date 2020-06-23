using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace agora
{
    namespace unity
    {          
        public class MainView : MonoBehaviour
        {
            private string tag {
                get{
                    return "[MainView]  ";
                }
            }

            public Canvas SettingsCanvas;
            public Canvas MainCavas;
            public Button ConnectionStatusBtn;
            public Button ConfirmButton;
            //public Button sendMessage;
            private Image connectStatusImage;
            public Image settingCanvasConnectStatusImage;
            public InputField serverTextInputField;
            public InputField deviceIdTextInputField;
            public Text appVersionInfo;
            public Text sdkVersionInfo;
            public Text dateInfo;

            public IMainViewPersenter MainViewPersenter
            {
                get;
                set;
            }
            
            void SetUnityDefaultSettings()
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            void Awake()
            {
                InitUI();
                SetVisibleCanvas(true);
                InitOnBtnClickListener();
                SetUnityDefaultSettings();
                MainViewPersenter = new MainViewPersenter(this);
                MainViewPersenter.OnAwake();  
                JSON.InitJsonMapper();
            }

            void Start()
            {
                MainViewPersenter.OnStart();
            }

            void Update()
            {
                if (MainViewPersenter == null)
                {
                    Debug.Log(" MainViewPersenter  == null ");
                }
                MainViewPersenter.OnUpdate();
            }

            void OnApplicationPause(bool pause)
            {
                MainViewPersenter.OnApplicationPause(pause);
            }

            void OnApplicationFocus(bool focus)
            {
                MainViewPersenter.OnApplicationFocus(focus);
            }

            void OnApplicationQuit()
            {
                MainViewPersenter.OnApplicationQuit();
            }

            void OnDestroy()
            {
                MainViewPersenter.OnDestroy();
            }

            public void OnUpdateConnectServerUI(bool connected)
            {
                connectStatusImage.sprite = loadImage(connected);
                settingCanvasConnectStatusImage.sprite = loadImage(connected);
            }

            private void InitUI() 
            {
                connectStatusImage = ConnectionStatusBtn.GetComponent<Image>();
            }

            private void InitOnBtnClickListener()
            {
                //Application.Logger.Info("MainView ", "InitOnBtnClickListener");
                ConnectionStatusBtn.onClick.AddListener(OnConnectionStatusButtonClick);
                ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
                //sendMessage.onClick.AddListener(OnSendMessageClick);
            }

            void OnConnectionStatusButtonClick()
            {
                //Application.Logger.Info("MainView ", "OnConnectionStatusButtonClick");
                UpdateSettingCanvasUI();
                SetVisibleCanvas(false);
            }

            void OnConfirmButtonClick()
            {
                UpdateApplicationInfo();
                SetVisibleCanvas(true);
                CancelInvokeDate();
                MainViewPersenter.ResetWebSocket();
            }

            void OnSendMessageClick()
            {
                MainViewPersenter.OnSendMessage();
            }

            private void UpdateApplicationInfo()
            {
                Application.WebSocket_Url = serverTextInputField.text;
                Application.DeviceID = deviceIdTextInputField.text;
            }

            private void UpdateSettingCanvasUI()
            {
                serverTextInputField.text = Application.WebSocket_Url;
                deviceIdTextInputField.text = Application.DeviceID;
                appVersionInfo.text = Application.AppVersion;
                sdkVersionInfo.text = MainViewPersenter.GetSDKVersion();
                StartInvokeDate();
            }

            private void UpdateDateUI()
            {
                dateInfo.text = Tools.GetDateTime();
            }

            private void StartInvokeDate()
            {
                InvokeRepeating("UpdateDateUI", 0, 1);
            }

            private void CancelInvokeDate()
            {
                CancelInvoke();
            }

            private void SetVisibleCanvas(bool isMainView)
            {
                if(isMainView)
                {
                    MainCavas.gameObject.SetActive(true);
                    SettingsCanvas.gameObject.SetActive(false);
                }
                else
                {
                    MainCavas.gameObject.SetActive(false);
                    SettingsCanvas.gameObject.SetActive(true);
                }
            }

            private Sprite loadImage(bool connected)
            {
                if (connected)
                {
                    return Resources.Load<Sprite>("Picture/icon_server_connected");
                }
                else
                {
                    return Resources.Load<Sprite>("Picture/icon_server_disconnected");
                }
            }
        }
    }
}