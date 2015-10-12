using System;
using IronPython.Runtime.Types;
using Tinkering.Graph.Nodes;
using System.Text.RegularExpressions;

namespace Tinkering.Graph.Datums
{
   internal class FloatDatum : EvalDatum
   {
      public FloatDatum(string name, Node parent)
         : base(name, parent)
      {
         this.inputHandler = new SingleInputHandler(this);
      }

      public FloatDatum(string name, string expr, Node parent)
         : this(name, parent)
      {
         Expr = expr;
      }

      public static string TypeString => "float";
      public override Type PyType => typeof(double);
      public override DatumType DatumType => DatumType.FLOAT;

      public bool DragValue(double delta)
      {
         string s = Expr;
         double v;
         if (double.TryParse(s, out v))
         {
            Expr = (v + delta).ToString();
            return true;
         }

         var regex = new Regex(@"(.*[+\-]\s*)(\d*(\.\d*|)(e\d+(\.\d*|)|))");

         var match = regex.Match(s);
         if (match.Success)
         {
            v = double.Parse(match.Captures[2].Value);
            Expr = (v + delta).ToString();
            return true;
         }
         return false;
      }

   }
}