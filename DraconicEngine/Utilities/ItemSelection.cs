using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace DraconicEngine.Utilities
{
   public static class ItemSelection
   {
      //public static T Choose<T>(IReadOnlyCollection<T> items)
      //{
      //   var index = RogueGame.Current.GameRandom.Next(items.Count);

      //   return items.ElementAt(index);
      //}

      public static T RandomChoice<T>(IReadOnlyDictionary<T, double> items)
      {
         var total = items.Sum(item => item.Value);

         var choice = RogueGame.Current.GameRandom.NextDouble() * total;

         return items.Scan(Tuple(0.0, default(T)),
            (last, current) => Tuple(current.Value + last.Item1, current.Key))
            .First(place => choice <= place.Item1)
            .Item2;
      }

      public static T RandomChoice<T>(int level, IReadOnlyDictionary<T, Either<double, IEnumerable<Tuple<double, int>>>> items)
      {
         var finalizedWeights = items
            .Select(kvp => new
            {
               Item = kvp.Key,
               Weight = kvp.Value.Match<double>(
                  Left: weight => weight,
                  Right: byLevel => FromDungeonLevel(level, byLevel)
                  .Match(
                     Some: val => val,
                     None: () => 0))
            })
            .Where(kvp => kvp.Weight > 0)
            .ToDictionary(kvp => kvp.Item, kvp => kvp.Weight);

         var total = finalizedWeights.Sum(item => item.Value);

         var choice = RogueGame.Current.GameRandom.NextDouble() * total;

         return finalizedWeights.Scan(Tuple(0.0, default(T)),
            (last, current) => Tuple(current.Value + last.Item1, current.Key))
            .First(place => choice <= place.Item1)
            .Item2;
      }

      //public static T Choose<T>(IEnumerable<Entry<T>> items)
      //{
      //   var total = items.Sum(item => item.Weight);

      //   var choice = RogueGame.Current.GameRandom.NextDouble() * total;

      //   return items.Scan(tuple(0.0, default(T)),
      //      (current, last) => tuple(current.Weight + last.Item1, current.Item))
      //      .First(place => choice <= place.Item1)
      //      .Item2;
      //}

      public static int RandomChoice(IEnumerable<double> items)
      {
         var total = items.Sum();

         var choice = RogueGame.Current.GameRandom.NextDouble() * total;

         return items.Scan(Tuple(0.0, 0),
            (last, current) => Tuple(current + last.Item1, last.Item2 + 1))
            .First(place => choice <= place.Item1)
            .Item2;
      }

      public static Option<T> FromDungeonLevel<T>(int level, IEnumerable<Tuple<T, int>> valuesPerLevel)
      {
         var selected = valuesPerLevel.Reverse().FirstOrDefault(t => level >= t.Item2);

         if (selected != null)
         {
            return selected.Item1;
         }
         return None;
      }

      public static Either<double, IEnumerable<Tuple<double, int>>> Make(double item)
      {
         return item;
      }

      public static Either<double, IEnumerable<Tuple<double, int>>> Make(params Tuple<double, int>[] items)
      {
         return items;
      }

   }

   //public struct Entry<T>
   //{
   //   public T Item { get; }
   //   public double Weight { get; }

   //   public Entry(T item, double chance)
   //   {
   //      this.Item = item;
   //      this.Weight = chance;
   //   }
   //}
}
