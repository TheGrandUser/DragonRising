using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Behaviors;
using DragonRising.Storage;

namespace DragonRising.Libraries
{
   class SimpleBehaviorLibrary : IBehaviorLibrary
   {
      Dictionary<string, Behavior> templates = new Dictionary<string, Behavior>();
      public Dictionary<string, Behavior> Templates => templates;

      public SimpleBehaviorLibrary()
      {
         Add(new BasicMonsterBehavior());
         Add(new ConfusedBehavior());
         Add(new JustPassBehavior());
         Add(new ExternallyControlledBehavior());
      }

      public void Add(Behavior template)
      {
         this.templates.Add(template.Name, template);
      }

      public Behavior Get(string templateName)
      {
         return templates[templateName];
      }

      public bool Contains(string name)
      {
         return templates.ContainsKey(name);
      }
   }
}
