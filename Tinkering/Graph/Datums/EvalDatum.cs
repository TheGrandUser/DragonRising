using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkering.Graph.Nodes;

namespace Tinkering.Graph.Datums
{
   abstract class EvalDatum : Datum
   {
      protected string expr;
      protected int errorLineNo;
      protected string errorTraceBack;

      public EvalDatum(string name, Node parent)
         : base(name, parent)
      {

      }


      public string Expr
      {
         get { return expr; }
         set
         {
            if (value != expr || !postInitCalled)
            {
               expr = value;
               Update();
            }
         }
      }

      public int GetErrorLine() => errorLineNo;
      public string GetErrorTraceback() => errorTraceBack;


      protected virtual string PrepareExpr(string s) => s;
      protected override dynamic GetCurrentValue()
      {
         errorLineNo = -1;
         errorTraceBack = "";

         var e = PrepareExpr(expr);
         dynamic newValue = null;
         if (ValidateExpr(e))
         {
            var globals = Root.ProxyDict(this);
            var engine = Python.CreateEngine();
            ModifyGlobalsDict(globals);

            newValue = engine.Execute(e, globals); // PyRunString(e, GetStartToken(), globals, globals);

            newValue = ValidatePyObject(ValidateType(newValue));
         }
         return newValue;
      }
      public override string GetString() => HasInputValue ? inputHandler.GetString() : expr;

      protected virtual dynamic ValidatePyObject(dynamic v) => v;
      protected virtual bool ValidateExpr(string e) => true;
      protected virtual dynamic ValidateType(dynamic v)
      {
         if (v == null)
         {
            return null;
         }
         else if (v.GetType() == this.PyType)
         {
            return v;
         }
         else
         {

            //attempt to cast to proper python type
         }
         throw new NotImplementedException();
      }

      //protected virtual int GetStartToken()
      //{
      //   //return Py_eval_input;
      //   throw new NotImplementedException();
      //}
      protected virtual void ModifyGlobalsDict(ScriptScope g)
      {
         // do nothing
      }
      //protected virtual void OnPyError()
      //{
      //   throw new NotImplementedException();
      //}


   }
}
