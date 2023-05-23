using Models;
using Network;
using SkillBridge.Message;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Managers;

namespace Assets.Scripts.Services
{
    class ChatService : Singleton<ChatService>
    {
        public void Init()
        {

        }

        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChatResponse);
        }

        public void SendChat(ChatChannel channel, string content, int toId, string toName)
        {
            Debug.Log("SendChat");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage
            {
                Channel = channel,
                FromId = User.Instance.CurrentCharacter.Id,
                FromName = User.Instance.CurrentCharacter.Name,
                ToId = toId,
                ToName = toName,
                Message = content,
                Time = TimeUtil.timestamp
            };
            NetClient.Instance.SendMessage(message);
        }

        private void OnChatResponse(object sender, ChatResponse message)
        {
            if (message.Result == Result.Success)
            {
                ChatManager.Instance.AddMessages(ChatChannel.Private, message.privateMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Local, message.localMessages);
                ChatManager.Instance.AddMessages(ChatChannel.World, message.worldMessages);
                ChatManager.Instance.AddMessages(ChatChannel.System, message.systemMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Team, message.teamMessages);
            }
            else
                ChatManager.Instance.AddSystemMessage(string.Format("消息发送失败:{0}", message.Errormsg));
        }
    }
}
