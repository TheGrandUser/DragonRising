using System;
using IronPython.Runtime.Types;
using Tinkering.Graph.Nodes;

namespace Tinkering.Graph.Datums
{
   internal class FloatOutputDatum : OutputDatum
   {
      public FloatOutputDatum(string name, Node parent)
         : base(name, parent)
      {

      }

      public static string TypeString { get; internal set; }

      public override DatumType DatumType => DatumType.FLOAT_OUTPUT;

      public override Type PyType => typeof(double);

      public override string GetString() => valid ? value.Tostring() : "Invalid output";
   }
}