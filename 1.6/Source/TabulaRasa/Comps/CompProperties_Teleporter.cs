using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class CompProperties_Teleporter : CompProperties
    {
        public CompProperties_Teleporter()
        {
            this.compClass = typeof(Comp_Teleporter);
        }

        public TeleporterType teleporterType = TeleporterType.world;

        public List<string> networkTags = new List<string>();

        public TeleporterDirection direction = TeleporterDirection.both;

        public bool needsPower = false;

        public int energyCost = 0;

        public bool usesFuel = false;

        public float fuelCost;

        public bool receiverMustBeActive = false;

        public int useDuration = 20;

        public bool isPad = false;

        public bool canSendNonPawns = false;

        public IntVec2 teleportArea = new IntVec2();

        public SoundDef sound;
    }

    public enum TeleporterType
    {
        local,
        world
    }

    public enum TeleporterDirection
    {
        transmitter,
        receiver,
        both
    }
}
