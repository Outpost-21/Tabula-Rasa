using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Hivemind
{
    public class Comp_HivemindPawn : ThingComp
    {
        public CompProperties_HivemindPawn Props => (CompProperties_HivemindPawn)this.props;

		public bool autoConnect = false;

		public Thing connectedCore = null;

		public override void PostExposeData()
		{
			base.PostExposeData();

			Scribe_Values.Look(ref autoConnect, "autoConnect");
			Scribe_Values.Look(ref connectedCore, "connectedCore");
		}

		public void Connect(Comp_HivemindCore hivemind)
        {
            Pawn pawn = (Pawn)parent;
            hivemind.connectedPawns.Add(pawn);
			connectedCore = hivemind.parent;
			hivemind.Notify_ConnectionChanged();
        }

        public void Disconnect(Comp_HivemindCore hivemind)
        {
            Pawn pawn = (Pawn)parent;
            hivemind.connectedPawns.Remove(pawn);
			connectedCore = null;
			hivemind.Notify_ConnectionChanged();
		}

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo c in base.CompGetGizmosExtra())
			{
				yield return c;
			}
			if (parent.Faction == Faction.OfPlayer)
			{
				yield return new Command_Toggle
				{
					toggleAction = delegate ()
					{
						autoConnect = !autoConnect;
					},
					defaultDesc = "Command_AutoConnect_Desc".Translate(),
					icon = ContentFinder<Texture2D>.Get(Props.autoConnectTexPath, true),
					defaultLabel = "Command_AutoConnect_Label".Translate()
				};
				
				if(connectedCore == null)
				{
					yield return new Command_Target
					{
						action = delegate (LocalTargetInfo target)
						{
							Comp_HivemindCore hiveComp = target.Pawn.TryGetComp<Comp_HivemindCore>();
							if(hiveComp != null)
							{
								Connect(hiveComp);
							}
							else
							{
								Messages.Message(new Message("Cannot Connect to " + target.Label + ", not viable.", MessageTypeDefOf.CautionInput));
							}
						},
						defaultDesc = "Command_Connect_Desc".Translate(),
						icon = ContentFinder<Texture2D>.Get(Props.connectTexPath, true),
						defaultLabel = "Command_Connect_Label".Translate()
					};
					
				}
				if(connectedCore != null)
				{
					yield return new Command_Action
					{
						action = delegate ()
						{
							Disconnect(connectedCore.TryGetComp<Comp_HivemindCore>());
						},
						defaultDesc = "Command_Disconnect_Desc".Translate(),
						icon = ContentFinder<Texture2D>.Get(Props.disconnectTexPath, true),
						defaultLabel = "Command_Disconnect_Label".Translate()
					};
				}
			}
			yield break;
		}
    }
}
