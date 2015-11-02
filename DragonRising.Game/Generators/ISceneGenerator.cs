using DraconicEngine;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Generators;
using DragonRising.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Generators
{
   public interface ISceneGenerator
   {
      Scene GenerateNewScene(World world);
   }

   class SceneGenerator : ISceneGenerator
   {
      public Scene GenerateNewScene(World world)
      {
         world.PopScene();

         var greenskins = new GreenskinsGenerator();
         //var generator = new DungeonGenerator(greenskins, new StandardItemGenerator());
         var generator = new TwoRoomsOneOrcGenerator(greenskins);

         Scene newScene = new Scene(world.MapWidth, world.MapHeight, TileLibrary.Current.BasicWallId, world.EntityEngine.CreateChildStore());
         newScene.EntityStore.AddEntity(world.Player);
         newScene.FocusEntity = world.Player;
         newScene.Level = world.DungeonLevel;

         Debug.Assert(newScene.EntityStore.Entities.Contains(world.Player));

         var startPoint = generator.MakeMap(newScene);
         world.Player.SetLocation(startPoint);

         world.PushScene(newScene);

         return newScene;
      }
   }
}
