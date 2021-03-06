﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using ReactiveUI;

namespace DraconicEngine.EntitySystem
{
   public abstract class ListIteratingSystemSync<TNode> : GameSystemSync
      where TNode : Node, new()
   {
      protected IReadOnlyReactiveList<TNode> nodeList;
      private CompositeDisposable subscriptions;

      public ListIteratingSystemSync()
      {
      }

      public override void AddToEngine(EntityEngine engine)
      {
         this.subscriptions = new CompositeDisposable();

         nodeList = engine.GetNodes<TNode>();
         foreach (var node in nodeList)
         {
            NodeAddedFunction(node);
         }
         this.subscriptions.Add(nodeList.ItemsAdded.Subscribe(NodeAddedFunction));
         this.subscriptions.Add(nodeList.ItemsRemoved.Subscribe(NodeRemovedFunction));
      }

      public override void RemoveFromEngine(EntityEngine engine)
      {
         this.subscriptions.Dispose();
         nodeList = null;
      }

      public override void Update(double time)
      {
         foreach (var node in this.nodeList)
         {
            NodeUpdateFunction(node, time);
         }
      }


      protected abstract void NodeUpdateFunction(TNode node, double time);
      protected virtual void NodeAddedFunction(TNode node) { }
      protected virtual void NodeRemovedFunction(TNode node) { }
   }

   public abstract class OrderedListIteratingSystemSync<TNode, TKey> : GameSystemSync
      where TNode : Node, new()
   {
      protected IReadOnlyReactiveList<TNode> nodeList;
      private CompositeDisposable subscriptions;

      public OrderedListIteratingSystemSync()
      {
      }

      public override void AddToEngine(EntityEngine engine)
      {
         this.subscriptions = new CompositeDisposable();

         nodeList = engine.GetNodes<TNode>();
         foreach (var node in nodeList)
         {
            NodeAddedFunction(node);
         }
         this.subscriptions.Add(nodeList.ItemsAdded.Subscribe(NodeAddedFunction));
         this.subscriptions.Add(nodeList.ItemsRemoved.Subscribe(NodeRemovedFunction));
      }

      public override void RemoveFromEngine(EntityEngine engine)
      {
         this.subscriptions.Dispose();
         nodeList = null;
      }

      public override void Update(double time)
      {
         foreach (var node in this.nodeList.OrderBy(n => this.Orderer(n)))
         {
            NodeUpdateFunction(node, time);
         }
      }

      protected abstract TKey Orderer(TNode n);

      protected abstract void NodeUpdateFunction(TNode node, double time);
      protected virtual void NodeAddedFunction(TNode node) { }
      protected virtual void NodeRemovedFunction(TNode node) { }
   }

   //public abstract class ListIteratingSystemAsync<TNode> : GameSystemAsync
   //   where TNode : Node, new()
   //{
   //   protected IReadOnlyReactiveList<TNode> nodeList;
   //   private CompositeDisposable subscriptions;

   //   public ListIteratingSystemAsync()
   //   {
   //   }

   //   public override void AddToEngine(EntityEngine engine)
   //   {
   //      this.subscriptions = new CompositeDisposable();

   //      nodeList = engine.GetNodes<TNode>();
   //      foreach (var node in nodeList)
   //      {
   //         NodeAddedFunction(node);
   //      }
   //      this.subscriptions.Add(nodeList.ItemsAdded.Subscribe(NodeAddedFunction));
   //      this.subscriptions.Add(nodeList.ItemsRemoved.Subscribe(NodeRemovedFunction));
   //   }

   //   public override void RemoveFromEngine(EntityEngine engine)
   //   {
   //      this.subscriptions.Dispose();
   //      nodeList = null;
   //   }

   //   public override async Task Update(double time)
   //   {
   //      foreach (var node in this.nodeList)
   //      {
   //         await NodeUpdateFunction(node, time);
   //      }
   //   }


   //   protected abstract Task NodeUpdateFunction(TNode node, double time);
   //   protected virtual void NodeAddedFunction(TNode node) { }
   //   protected virtual void NodeRemovedFunction(TNode node) { }
   //}
}
