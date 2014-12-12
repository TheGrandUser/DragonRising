using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   [Serializable]
   public class EntityTemplate
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public Character Character { get; set; }

      public bool Blocks { get; set; }

      public EntityTemplate()
      {
      }

      public EntityTemplate(string name, Character character, params ComponentTemplate[] componentTemplates)
      {
         this.Name = name;
         this.Character = character;
         this.componentTemplates = componentTemplates.ToList();
      }

      public EntityTemplate(string name, Glyph glyph, RogueColor color, params ComponentTemplate[] componentTemplates)
      {
         this.Name = name;
         this.Character = new Character(glyph, color);
         this.componentTemplates = componentTemplates.ToList();
      }

      List<ComponentTemplate> componentTemplates = new List<ComponentTemplate>();

      public void AddComponent(ComponentTemplate component)
      {
         this.componentTemplates.Add(component);
      }

      public Entity Create()
      {
         var entity = new Entity(this.Name, this);

         foreach (var template in this.componentTemplates)
         {
            var type = template.ComponentType;

            Component component = template.CreateComponent();

            entity.AddComponent(type, component);
         }

         return entity;
      }
   }
}
