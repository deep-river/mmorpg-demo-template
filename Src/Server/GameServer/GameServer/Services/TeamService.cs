using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            Log.InfoFormat("OnTeamInviteRequest::FromId:{0} FromName:{1} ToID:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);

            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if (target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "玩家当前不在线";
                sender.SendResponse();
                return;
            }

            if (target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "玩家已在其他队伍中";
                sender.SendResponse();
                return;
            }

            // 向目标角色客户端发送teamInviteReq请求
            Log.InfoFormat("Forward TeamInviteRequest::FromId:{0} FromName:{1} ToId: {2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteResponse::character:{0} Result:{1} FromId:{2} ToId:{3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);

            sender.Session.Response.teamInviteRes = response;
            if (response.Result == Result.Success)
            {
                // sender:被邀请玩家
                // inviter:发起组队玩家
                var inviter = SessionManager.Instance.GetSession(response.Request.FromId);
                if (inviter == null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "邀请者已离线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(inviter.Session.Character, character);
                    inviter.Session.Response.teamInviteRes = response;
                    inviter.SendResponse();
                }
            }
            sender.SendResponse();
        }

        void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamLeave::character:{0} TeamId:{1}:{2}", character.Id, request.TeamId, request.CharacterId);

            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.characterId = request.CharacterId;
            sender.Session.Response.teamLeave.Result = Result.Success;

            character.Team.Leave(character);
            sender.SendResponse();
        }
    }
}
