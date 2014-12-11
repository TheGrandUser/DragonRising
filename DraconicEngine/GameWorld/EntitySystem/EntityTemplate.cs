using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   public abstract class EntityTemplate
   {
      public string Name { get; set; }

      List<ComponentTemplate> componentTemplatess = new List<ComponentTemplate>();

      public void AddComponent(ComponentTemplate component)
      {
         this.componentTemplatess.Add(component);
      }

      protected abstract Entity CreateEntityCore();

      public Entity Create()
      {
         var entity = CreateEntityCore();

         foreach (var template in this.componentTemplatess)
         {
            var type = template.ComponentType;

            Component component = template.CreateComponent();

            entity.AddComponent(type, component);
         }

         return entity;
      }
   }
}
