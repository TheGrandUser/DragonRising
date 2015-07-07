using System;
using IronPython.Runtime.Types;
using Tinkering.Graph.Nodes;

namespace Tinkering.Graph.Datums
{
   internal class ShapeDatum : EvalDatum
   {
      public ShapeDatum(string name, Node parent)
         : base(name, parent)
      {

      }

      public static string TypeString { get; internal set; }

      public override DatumType DatumType
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override Type PyType
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string GetString()
      {
         throw new NotImplementedException();
      }

      protected override dynamic GetCurrentValue()
      {
         throw new NotImplementedException();
      }
   }
}