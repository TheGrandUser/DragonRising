using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld
{
   static class LevelingPolicy
   {
      public static readonly int levelUpBase = 200;
      public static readonly int levelUpFactor = 150;

      public static int XpForNextLevel(int currentLevel)
      {
         return levelUpBase + levelUpFactor * currentLevel;
      }
   }
}
