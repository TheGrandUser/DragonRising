module ActionTypes

open GameTypes
open Microsoft.Xna.Framework
open SadConsole

type ConfirmedMove = ConfirmedMove of Loc3
type ConfirmedAttack = ConfirmedAttack of EntityId
type DirectionInteraction =
   | Blocked
   | MoveTo of ConfirmedMove
   | AttackEntity of ConfirmedAttack

type LogMessage = LogMessage of ColoredString
