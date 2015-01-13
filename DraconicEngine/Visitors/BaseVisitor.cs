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

         foreach (var entity in scene.EntityStore.AllEntities)
         {
            VisitEntity(entity);
         }

         for (int row = 0; row < scene.MapHeight; row++)
         {
            var stride = row * scene.MapWidth;
            for (int col = 0; col < scene.MapWidth; col++)
            {
               var tile = scene.Map[col + stride];

               VisitTile(tile, col, row);
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

         foreach (var component in entity.Components)
         {
            VisitEntityComponent(component);
         }
      }

      protected abstract void OnEntity(Entity entity);


      public void VisitEntityComponent(Component component)
      {
         OnEntityComponent(component);
      }

      protected abstract void OnEntityComponent(Component component);
   }
}
