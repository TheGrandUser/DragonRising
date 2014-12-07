using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Visitors
{
   public abstract class BaseVisitor : IVisitor
   {
      public void VisitScene(Scene scene)
      {
         OnScene(scene);

         foreach(var entity in scene.EntityStore.AllEntities)
         {
            VisitEntity(entity);
         }

         for (int column = 0; column < scene.MapWidth; column++)
         {
            for(int row=0; row<scene.MapHeight; row++)
            {
               var tile = scene.Map[column, row];

               VisitTile(tile, column, row);
            }
         }
      }

      protected abstract void OnScene(Scene scene);


      public void VisitTile(Tile tile, int column, int row)
      {
         OnTile(tile, column, row);
      }

      protected abstract void OnTile(Tile Tiletile, int column, int row);


      public void VisitEntity(Entity entity)
      {
         OnEntity(entity);

         foreach(var component in entity.Components)
         {
            VisitEntityComponent(component);
         }
      }

      protected abstract void OnEntity(Entity entity);


      public void VisitEntityComponent(IComponent component)
      {
         OnEntityComponent(component);
      }

      protected abstract void OnEntityComponent(IComponent component);
   }
}
