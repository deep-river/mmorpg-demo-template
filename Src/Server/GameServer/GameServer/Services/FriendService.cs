using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
    {
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }

        public void Init()
        {

        }

        void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRequest::FromId:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);

            if (request.ToId == 0)
            {
                // 如果没有传入id，则使用用户名查找
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if (cha.Value.Data.Name == request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }

            NetConnection<NetSession> friend = null;
            if (request.ToId > 0)
            {
                // 检查是否在好友列表中
                if (character.FriendManager.GetFriendInfo(request.ToId) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                    sender.SendResponse();
                    return;
                }
                friend = SessionManager.Instance.GetSession(request.ToId); // 获取待添加好友玩家的session
            }

            if (friend == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "好友不存在或不在线";
                sender.SendResponse();
                return;
            }

            // 直接调用好友的session，转发request
            Log.InfoFormat("ForwardRequest::FromId:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            friend.Session.Response.friendAddReq = request;
            friend.SendResponse();
        }

        void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse::Character:{0} Result:{1} FromId:{2} ToId:{3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);

            sender.Session.Response.friendAddRes = response;
            if (response.Result == Result.Success)
            {
                var requestSession = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requestSession == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "玩家不在线";
                }
                else
                {
                    // 角色互加好友
                    character.FriendManager.AddFriend(requestSession.Session.Character);
                    requestSession.Session.Character.FriendManager.AddFriend(character);
                    DBService.Instance.Save();

                    requestSession.Session.Response.friendAddRes = response;
                    requestSession.Session.Response.friendAddRes.Result = Result.Success;
                    requestSession.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    requestSession.SendResponse();
                }
            }

            sender.SendResponse();
        }

        void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove::Character:{0} FriendRelationID:{1}", character.Id, request.Id);

            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            // 删除请求发出者的好友
            if (character.FriendManager.RemoveFriendByID(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;

                // 删除对方好友列表中的自己
                var friend = SessionManager.Instance.GetSession(request.friendId);
                if (friend != null)
                {
                    // 好友在线，从内存中删除好友
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id);
                }
                else
                {
                    // 好友不在线，则从数据库中删除
                    this.RemoveFriend(request.friendId, character.Id);
                }
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;

            DBService.Instance.Save();
            sender.SendResponse();
        }

        void RemoveFriend(int characterId, int friendId)
        {
            var removeItem = DBService.Instance.Entities.TCharacterFriends.FirstOrDefault(v => v.CharacterID == characterId && v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
        }
    }
}
