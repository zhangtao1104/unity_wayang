using System.Collections.Generic;

namespace agora
{
    namespace unity
    {
        public enum MessageType 
        {
            SERVER = 0,
            CLIENT = 1
        }

        public class MessageComand 
        {
            private static string TAG = "MessageCommand";
            private Queue<ServerMessage> messageFromServerQueue = new Queue<ServerMessage>();
            private Queue<ServerMessage> messageToServerQueue = new Queue<ServerMessage>();

            public void SendJsonToMessageQueue(MessageType type, string json)
            {
                
                ServerMessage message = JSON.JsonToObject<ServerMessage>(json);
                if ((message.type == (int)TYPE.CMD_MESSAGE || message.type == 2) && message.device == Application.DeviceID)
                {
                    //Application.Logger.Info(TAG, "JsonFromServer send message type is " + (int)message.type);
                    Application.Logger.Info(TAG, "JsonFromServer json = " + json);
                    SendMessageToQueue(MessageType.SERVER, message);
                }
                else if (message.type != (int)TYPE.CMD_MESSAGE)
                {
                    //Application.Logger.Info(TAG, "JsonFromServer  abandon message  type is " + (int)message.type);
                }
                else if (message.device != Application.DeviceID)
                {
                    //Application.Logger.Info(TAG, "JsonFromServer abandon message my deviceId is : " + Application.DeviceID + "  , receive message deviceId is :" + message.device);
                }
                else
                {
                    //Application.Logger.Info(TAG, "JsonFromServer  abandon message  type is json: " + json);
                }
            }

            public void SendMessageToQueue(MessageType type, ServerMessage message)
            {
                if (message == null)
                {
                    Application.Logger.Error(TAG, "message is null----");
                }
                switch(type)
                {
                    case MessageType.SERVER:
                        lock(messageFromServerQueue)
                        {
                            //Application.Logger.Info(TAG, " SendMessageToQueue type = " + (int)type + "  message cmd = " + message.cmd);
                            if (messageFromServerQueue.Count >= 100)
                            {
                                messageFromServerQueue.Dequeue();
                            }
                            messageFromServerQueue.Enqueue(message);
                        }
                        break;

                    case MessageType.CLIENT:
                        lock(messageToServerQueue)
                        {
                            //Application.Logger.Info(TAG, " SendMessageToQueue type = " + (int)type + "  message cmd = " + message.cmd);
                            if (messageToServerQueue.Count >= 100)
                            {
                                messageToServerQueue.Dequeue();
                            }
                            messageToServerQueue.Enqueue(message);
                        }
                        break;
                    default:
                        break;
                }
            }

            public ServerMessage GetMessageFromQueue(MessageType type)
            {
                ServerMessage message = null;
                switch(type)
                {
                    case MessageType.SERVER:
                        lock(messageFromServerQueue)
                        {
                            if (messageFromServerQueue.Count > 0)
                            {
                                message = messageFromServerQueue.Dequeue();
                                //Application.Logger.Info(TAG, " GetMessageFromQueue type = " + (int)type + "  message cmd = " + message.cmd);
                            }
                        }
                        break;

                    case MessageType.CLIENT:
                        lock(messageToServerQueue)
                        {
                            if (messageToServerQueue.Count > 0)
                            {
                                message = messageToServerQueue.Dequeue();
                                //Application.Logger.Info(TAG, " GetMessageFromQueue type = " + (int)type + "  message cmd = " + message.cmd);
                            }
                        }
                        break;

                    default:
                        break;
                }
                return message;
            }

            public string ServerMessageToJson(ServerMessage message)
            {
                string json = JSON.ObjectToJson<ServerMessage>(message);
                return json;
            }

            public void UploadMessageToServer(ServerMessage message)
            {
                SendMessageToQueue(MessageType.CLIENT, message);
            }

            public void Release()
            {
                lock(messageFromServerQueue)
                {
                    messageFromServerQueue.Clear();
                }

                lock(messageToServerQueue)
                {
                    messageToServerQueue.Clear();
                }
            }
        }
    }
}
