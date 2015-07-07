using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.EntitySystem;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using DragonRising.Rules;

namespace DragonRising.GameWorld.Actions
{
   public class PickUpItemAction : ActionTaken
   {
      public Entity Picker { get; }
      public Entity ItemToPick { get; }
      
      public PickUpItemAction (Entity picker, Entity itemToPick)
      {
         Picker = picker;
         ItemToPick = itemToPick;
      }
   }

}
