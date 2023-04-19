using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        public void GetQuestsInfo(List<NQuestInfo> list)
        {
            foreach (var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        public NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }

        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;

            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbQuest = DBService.Instance.Entities.CharacterQuests.Create();
                dbQuest.QuestID = questId;
                if (quest.Target1 == QuestTarget.None)
                {
                    // 没有任务目标直接完成
                    dbQuest.Status = (int)QuestStatus.Completed;
                }
                else
                {
                    dbQuest.Status = (int)QuestStatus.InProgress;
                }

                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbQuest);
                character.Data.Quests.Add(dbQuest);
                DBService.Instance.Save();
                Log.InfoFormat("Quest accepted::Character:{0}:QuestId:{1}", character.Id, questId);
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                Log.InfoFormat("Quest acceptance failed::Character:{0}:QuestId:{1}", character.Id, questId);
                return Result.Failed;
            }
        }

        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbQuest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (dbQuest != null)
                {
                    if (dbQuest.Status != (int)QuestStatus.Completed)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    dbQuest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbQuest);
                    DBService.Instance.Save();
                    // 处理任务奖励
                    if (quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if (quest.RewardExp > 0)
                    {
                    }
                    if (quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }
                    DBService.Instance.Save();
                    Log.InfoFormat("Quest submitted::Character:{0}:QuestId:{1}", character.Id, questId);
                    return Result.Success;
                }
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[2]";
                Log.InfoFormat("Quest submission failed::Character:{0}:QuestId:{1}", character.Id, questId);
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[1]";
                Log.InfoFormat("Quest submission failed::Character:{0}:QuestId:{1}", character.Id, questId);
                return Result.Failed;
            }
        }
    }
}
