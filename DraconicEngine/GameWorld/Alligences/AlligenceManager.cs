using LanguageExt;
using LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Alligences
{
   public interface IAlligenceManager
   {
      Alligence Neutral { get; }

      void Add(Some<Alligence> alligence);
      Option<Alligence> GetByName(string name);
      Some<Alligence> GetOrAddByName(string name);
      bool AreEnemies(Some<Alligence> alligence1, Some<Alligence> alligence2);
      Relationship GetRelationship(Some<Alligence> alligence1, Some<Alligence> alligence2);
      void SetRelationship(Some<Alligence> alligence1, Some<Alligence> alligence2, Relationship relationship);
   }

   public static class AlligenceManager
   {
      static IAlligenceManager currentService;

      public static IAlligenceManager Current { get { return currentService; } }

      public static void SetAlligenceManager(IAlligenceManager alligenceManager)
      {
         currentService = alligenceManager;
      }
   }

   [Serializable]
   public class SimpleAlligenceManager : IAlligenceManager
   {
      Dictionary<string, Alligence> alligences = new Dictionary<string, Alligence>();

      static readonly Alligence neutral = new Alligence() { Name = "Neutral" };

      public Alligence Neutral => neutral;

      public void Add(Some<Alligence> alligence)
      {
         this.alligences.Add(alligence.Value.Name, alligence.Value);
      }
      public Option<Alligence> GetByName(string name)
      {
         if (alligences.ContainsKey(name))
         {
            return alligences[name];
         }

         return null;
      }

      public bool AreEnemies(Some<Alligence> alligence1, Some<Alligence> alligence2)
      {
         if (alligence1.Value == Neutral || alligence2.Value == Neutral)
         {
            return false;
         }

         return GetRelationship(alligence1, alligence2) == Relationship.Enemy;
      }

      public Relationship GetRelationship(Some<Alligence> alligence1, Some<Alligence> alligence2)
      {
         return Relationship.Enemy;
      }

      public void SetRelationship(Some<Alligence> alligence1, Some<Alligence> alligence2, Relationship relationship)
      {

      }

      public Some<Alligence> GetOrAddByName(string name)
      {
         if (alligences.ContainsKey(name))
         {
            return alligences[name];
         }

         var a = new Alligence() { Name = name };

         this.alligences[name] = a;

         return a;
      }
   }
}
