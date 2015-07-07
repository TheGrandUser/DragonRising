using System;
using System.Diagnostics;
using Tinkering.Graph.Nodes;

namespace Tinkering.Graph.Datums
{
   abstract internal class OutputDatum : Datum
   {
      dynamic newValue;

      protected OutputDatum(string name, Node parent)
         : base(name, parent)
      {

      }

      protected override dynamic GetCurrentValue() => newValue;
      public void SetNewValue(dynamic p)
      {
         Debug.Assert((bool)(p != null));

         this.newValue = p;
         Update();
      }

      public override string GetString() => "Shape";

      public override bool CanEdit => false;
   }
}