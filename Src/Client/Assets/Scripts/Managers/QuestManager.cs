using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class QuestManager : Singleton<QuestManager>
    {
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
    }
}
