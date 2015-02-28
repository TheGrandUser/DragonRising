using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld;
using DraconicEngine;

namespace DragonRising.GameWorld.Components
{
   public class GameActionsComponent : Component
   {
      Dictionary<Entity, RogueAction> actionsToDo = new Dictionary<Entity, RogueAction>();

      public GameActionsComponent()
      {
      }

      protected GameActionsComponent(GameActionsComponent original, bool fresh)
         : base(original, fresh)
      {
         if (!fresh)
         {
            foreach(var kvp in original.actionsToDo)
            {
               this.actionsToDo.Add(kvp.Key, kvp.Value);
            }
         }
      }

      public void AddAction(Entity executer, RogueAction action)
      {
         this.actionsToDo.Add(executer, action);
      }

      public List<Tuple<Entity, RogueAction>> GetAndClearActions()
      {
         var stuff = actionsToDo.Select(kvp => tuple(kvp.Key, kvp.Value)).ToList();
         actionsToDo.Clear();
         return stuff;
      }

      protected override Component CloneCore(bool fresh)
      {
         return new GameActionsComponent(this, fresh);
      }
   }
}
