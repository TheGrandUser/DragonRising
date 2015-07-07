using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tinkering
{
   class HObject
   {
      HObject parent;
      List<HObject> children = new List<HObject>();

      public HObject()
      {
      }

      public HObject(HObject parent)
      {
         this.parent = parent;
      }

      public HObject Parent => parent;
      public List<HObject> Children => children;



      public void AddChild(HObject child) => children.Add(child);
      public void RemoveChild(HObject child) => children.Remove(child);
      
      public void BlockSignals(bool block)
      {

      }

      public IDisposable Connect<T>(HObject sender, IObservable<T> signal, Action<T> method, ConnectionType type = ConnectionType.AutoConnection)
      {
         throw new NotImplementedException();
      }
      public IDisposable Connect<T>(HObject sender, IObservable<T> signal, HObject reciever, Action<T> method, ConnectionType type = ConnectionType.AutoConnection)
      {
         throw new NotImplementedException();
      }

      public bool Disconnect<T>(IObservable<T> signal, HObject reciever, Action<T> method)
      {
         throw new NotImplementedException();
      }
      public bool Disconnect<T>(HObject sender, IObservable<T> signal, HObject reciever, Action<T> method)
      {
         throw new NotImplementedException();
      }

      public bool Disconnect<T>(HObject reciever, IObserver<T> method)
      {
         throw new NotImplementedException();
      }

      public T FindChild<T>(string name = "", FindOptions options = FindOptions.FindChildrenRecursively)
         where T : HObject
      {
         if(options == FindOptions.FindDirectChildrenOnly)
         {
            return children.OfType<T>().FirstOrDefault(c => name == null || c.Name == name);
         }
         else
         {
            return FindChild<T>(name, FindOptions.FindDirectChildrenOnly) ??
               children.Select(c => c.FindChild<T>(name)).FirstOrDefault(c => c != null);
         }
      }

      public IEnumerable<T> FindChildren<T>(string name = "", FindOptions options = FindOptions.FindChildrenRecursively)
         where T : HObject
      {
         if (options == FindOptions.FindDirectChildrenOnly)
         {
            return children.OfType<T>().Where(c => name == null || c.Name == name);
         }
         else
         {
            return children.OfType<T>().Where(c => name == null || c.Name == name).Concat(
               children.SelectMany(c => c.FindChildren<T>(name)));
         }
      }

      public IEnumerable<T> FindChildren<T>(Regex re, FindOptions options = FindOptions.FindChildrenRecursively)
         where T : HObject
      {
         if (options == FindOptions.FindDirectChildrenOnly)
         {
            return children.OfType<T>().Where(c => re.IsMatch(c.Name));
         }
         else
         {
            return children.OfType<T>().Where(c => re.IsMatch(c.Name)).Concat(
               children.SelectMany(c => c.FindChildren<T>(re)));
         }
      }

      public string Name { get; set; }
      
      public void DeleteLater(HObject obj)
      {

      }

      public IObservable<HObject> Destroyed { get; private set; }
      public IObservable<string> NameChanged { get; private set; }
   }

   public class Connection
   {

   }

   enum FindOptions
   {
      FindDirectChildrenOnly,
      FindChildrenRecursively
   }

   class PyTest
   {
      static string script =
@"class MyClass:
   i = 12345
   def f(self):
      return ""hello world""

def Make():
   return MyClass()

x = Make()
x";

      public static void Testing()
      {
         var engine = Python.CreateEngine();
         var scope = engine.CreateScope();
         var source = engine.CreateScriptSourceFromString(script, SourceCodeKind.AutoDetect);
         var compiled = source.Compile();

         var result = compiled.Execute(scope);
         Verify(scope, result);
      }

      public static void Testing2()
      {
         var engine = Python.CreateEngine();
         var scope = engine.CreateScope();
         
         var result = engine.Execute(script, scope);
         Verify(scope, result);
      }

      private static void Verify(ScriptScope scope, dynamic result)
      {
         dynamic x;
         if (scope.TryGetVariable("x", out x))
         {
            Console.WriteLine("x: " + x);
            var type = x.GetType();
            Console.WriteLine("x Type: " + type);
            Console.WriteLine("x.i" + x.i);

            var i = x.i;

            Console.WriteLine(" i type " + i.GetType());

            var oldInstance = x as OldInstance;

            if(oldInstance != null)
            {
               try
               {
                  //var contains = oldInstance.__contains__(new CodeContext(), 0);
                  var cls = x.__class__;

                  Console.WriteLine("x class: " + cls);
               }
               catch(Exception e)
               {
                  Console.WriteLine("Errro getting class");
                  Console.WriteLine(e.ToString());
               }
            }
         }
         else
         {
            Console.WriteLine("No x");
         }

         if (result != null)
         {
            Console.WriteLine("Result: " + result);
            Console.WriteLine("Result Type: " + result.GetType());
         }
         else
         {
            Console.WriteLine("No result");
         }
      }
   }

   enum ConnectionType
   {
      AutoConnection,
      DirectConnection,
      QueuedConnection,
      BlockingQueuedConnection,
      UniqueConnection,
   }
}
