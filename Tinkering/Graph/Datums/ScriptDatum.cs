using System;
using IronPython.Runtime.Types;
using Tinkering.Graph.Nodes;

namespace Tinkering.Graph.Datums
{
   internal class ScriptDatum : EvalDatum
   {
      public ScriptDatum(string name, Node parent)
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

      public void MakeInput(string name, Type ype, string value = "")
      {

      }

      public void MakeOutput(string name, out dynamic @out)
      {
         @out = null;
      }
   }
}