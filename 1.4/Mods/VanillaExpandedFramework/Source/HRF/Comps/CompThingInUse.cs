using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Verse.AI.ReservationManager;

namespace HRF
{
    public class CompProperties_ThingInUse : CompProperties
    {
        public List<UseProps> useProperties;
        public CompProperties_ThingInUse()
        {
            this.compClass = typeof(CompThingInUse);
        }
    }
    public class CompThingInUse : ThingComp, IAdjustResource
    {
        public static Dictionary<Thing, CompThingInUse> things = new Dictionary<Thing, CompThingInUse>();

        public static HashSet<StatDef> statsWithBoosters = new HashSet<StatDef> { };

        public CompPowerTrader compPower;
        public CompGlower compParentGlower;

        public bool powerIsOn;
        public bool UseIsEnabled(UseProps useProps)
        {
            var ind = this.Props.useProperties.IndexOf(useProps);
            if (useProps.toggleResourceUse && resourceUseToggleStates != null && resourceUseToggleStates.TryGetValue(ind, out bool state) && !state)
            {
                return false;
            }
            return true;
        }

        public virtual bool PawnCanUseIt(Pawn pawn, UseProps useProps)
        {
            return true;
        }

        public override string CompInspectStringExtra()
        {
            var sb = new StringBuilder(base.CompInspectStringExtra());
            var useProps = this.Props.useProperties;
            foreach (var useProp in useProps)
            {
                if (useProp.hediffRequired)
                {
                    sb.AppendLine("ART.RequiresResource".Translate(useProp.hediff.label));
                }
            }
            return sb.ToString().TrimEndNewlines();
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                resourceUseToggleStates = new Dictionary<int, bool>();
                foreach (var useProps in Props.useProperties)
                {
                    var ind = Props.useProperties.IndexOf(useProps);
                    resourceUseToggleStates[ind] = useProps.defaultToggleState;
                }
            }
            things[this.parent] = this;
            foreach (var useProps in Props.useProperties)
            {
                if (useProps.statOffsets != null)
                {
                    foreach (var stat in useProps.statOffsets)
                    {
                        statsWithBoosters.Add(stat.stat);
                    }
                }

                if (useProps.statFactors != null)
                {
                    foreach (var stat in useProps.statFactors)
                    {
                        statsWithBoosters.Add(stat.stat);
                    }
                }
            }
            Register();
            var gameComp = HRFManager.Instance;
            gameComp.UpdateAdjuster(this);
            this.compPower = this.parent.TryGetComp<CompPowerTrader>();
            this.compParentGlower = this.parent.TryGetComp<CompGlower>();
            gameComp.RegisterFacilityInUse(this);
            boolValueCache = new BoolPawnsValueCache(InUseInt(out IEnumerable<Pawn> claimants), claimants);
        }

        private BoolPawnsValueCache boolValueCache;
        public bool InUse(out IEnumerable<Pawn> claimants)
        {
            if (boolValueCache is null || Find.TickManager.TicksGame + 60 > boolValueCache.updateTick)
            {
                boolValueCache = new BoolPawnsValueCache(InUseInt(out IEnumerable<Pawn> pawns), pawns);
            }
            claimants = boolValueCache.pawns;
            return boolValueCache.value;
        }
        private bool InUseInt(out IEnumerable<Pawn> claimants)
        {
            claimants = Claimants;
            if (claimants != null)
            {
                if (this.parent is Frame)
                {
                    foreach (var claimant in claimants)
                    {
                        if (!claimant.pather.MovingNow && claimant.CurJobDef == JobDefOf.FinishFrame
                            && claimant.CurJob.targetA.Thing == this.parent
                            && this.parent.OccupiedRect().Cells.Any(x => x.DistanceTo(claimant.Position) <= 1.5f))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (var claimant in claimants)
                    {
                        var pawnPosition = claimant.Position;
                        if (pawnPosition == this.parent.Position || pawnPosition == this.parent.InteractionCell)
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }
        public IEnumerable<Pawn> GetActualUsers(IEnumerable<Pawn> claimants)
        {
            if (this.parent is Frame)
            {
                foreach (var claimant in claimants)
                {
                    if (!claimant.pather.MovingNow && claimant.CurJobDef == JobDefOf.FinishFrame
                        && claimant.CurJob.targetA.Thing == this.parent
                        && this.parent.OccupiedRect().Cells.Any(x => x.DistanceTo(claimant.Position) <= 1.5f))
                    {
                        yield return claimant;
                    }
                }
            }
            else
            {
                foreach (var claimant in claimants)
                {
                    var pawnPosition = claimant.Position;
                    if (pawnPosition == this.parent.Position || pawnPosition == this.parent.InteractionCell)
                    {
                        yield return claimant;
                    }
                }
            }
        }
        private IEnumerable<Reservation> Reservations => this.parent.Map != null 
            ? this.parent.Map.reservationManager.ReservationsReadOnly.Where(x => x.Target.Thing == this.parent)
            : Enumerable.Empty<Reservation>();

        private IEnumerable<Pawn> cachedClaimants;

        private int lastClaimantCacheTick;
        private IEnumerable<Pawn> Claimants
        {
            get
            {
                var curTicks = Find.TickManager.TicksGame;
                if (curTicks > lastClaimantCacheTick + 60 || cachedClaimants is null)
                {
                    cachedClaimants = Reservations.Select(x => x.Claimant);
                    lastClaimantCacheTick = curTicks;
                }
                return cachedClaimants;
            }
        }

        public CompProperties_ThingInUse Props => (CompProperties_ThingInUse)this.props;
        public List<ResourceProperties> ResourceSettings => null;
        public Dictionary<HediffResource, HediffResouceDisable> PostUseDelayTicks => null;
        public string DisablePostUse => null;
        public Thing Parent => this.parent;
        public Pawn PawnHost => null;
        public void ResourceTick()
        {
            bool inUse = InUse(out var claimaints);
            if (inUse)
            {
                var users = GetActualUsers(claimaints);
                foreach (var user in users)
                {
                    foreach (var useProps in Props.useProperties)
                    {
                        if (useProps.resourcePerSecond != -1f && this.UseIsEnabled(useProps))
                        {
                            float num = useProps.resourcePerSecond;
                            if (useProps.qualityScalesResourcePerSecond && this.parent.TryGetQuality(out QualityCategory qc))
                            {
                                num *= Utils.GetQualityMultiplierInverted(qc);
                            }
                            Utils.AdjustResourceAmount(user, useProps.hediff, num, useProps.addHediffIfMissing, null, useProps.applyToPart);
                        }

                    }
                }
            }
        }

        public Dictionary<int, bool> resourceUseToggleStates = new Dictionary<int, bool>();
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var g in base.CompGetGizmosExtra())
            {
                yield return g;
            }
            foreach (var useProps in Props.useProperties)
            {
                if (useProps.toggleResourceUse)
                {
                    var ind = Props.useProperties.IndexOf(useProps);
                    var toggle = new Command_Toggle();
                    toggle.defaultLabel = useProps.toggleResourceLabel;
                    toggle.defaultDesc = useProps.toggleResourceDesc;
                    toggle.icon = ContentFinder<Texture2D>.Get(useProps.toggleResourceGizmoTexPath);
                    toggle.toggleAction = delegate ()
                    {
                        if (resourceUseToggleStates.ContainsKey(ind))
                        {
                            resourceUseToggleStates[ind] = !resourceUseToggleStates[ind];
                        }
                        else
                        {
                            resourceUseToggleStates[ind] = false;
                        }
                        UpdateGraphics();
                    };
                    toggle.isActive = (() => resourceUseToggleStates is null || resourceUseToggleStates.ContainsKey(ind) ? resourceUseToggleStates[ind] : true);
                    yield return toggle;
                }
            }
        }

        public void UpdateGraphics()
        {
            bool changedGraphics = false;
            bool changedGlower = false;
            if (base.parent.Map != null)
            {
                if (resourceUseToggleStates is null)
                {
                    foreach (var useProps in Props.useProperties)
                    {
                        if (!changedGraphics && !useProps.texPathToggledOn.NullOrEmpty())
                        {
                            ChangeGraphic(useProps.texPathToggledOn);
                            changedGraphics = true;
                        }

                        if (!changedGlower && useProps.glowerOptions != null)
                        {
                            if (!useProps.glowOnlyPowered || (this.parent.TryGetComp<CompPowerTrader>()?.PowerOn ?? false))
                            {
                                UpdateGlower(useProps.glowerOptions);
                                changedGlower = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var useProps in Props.useProperties)
                    {
                        if (UseIsEnabled(useProps))
                        {
                            var ind = Props.useProperties.IndexOf(useProps);
                            if (!changedGraphics && !useProps.texPathToggledOn.NullOrEmpty() && resourceUseToggleStates.ContainsKey(ind) && resourceUseToggleStates[ind])
                            {
                                ChangeGraphic(useProps.texPathToggledOn);
                                changedGraphics = true;
                            }

                            if (!changedGlower && useProps.glowerOptions != null)
                            {
                                if (useProps.glowOnlyPowered && (this.parent.TryGetComp<CompPowerTrader>()?.PowerOn ?? false))
                                {
                                    UpdateGlower(useProps.glowerOptions);
                                    changedGlower = true;
                                }
                            }
                        }
                    }
                }

                if (!changedGraphics && (!parent.def.graphicData?.texPath.NullOrEmpty() ?? false))
                {
                    ChangeGraphic(parent.def.graphicData.texPath);
                }

                if (!changedGlower)
                {
                    if (this.compGlower != null)
                    {
                        base.parent.Map.glowGrid.DeRegisterGlower(this.compGlower);
                        this.compGlower = null;
                    }

                    if (compParentGlower != null)
                    {
                        base.parent.Map.glowGrid.RegisterGlower(compParentGlower);
                    }
                }
            }
        }

        public void ChangeGraphic(string texPath)
        {
            var graphicData = new GraphicData();
            graphicData.graphicClass = this.parent.def.graphicData.graphicClass;
            graphicData.texPath = texPath;
            graphicData.shaderType = this.parent.def.graphicData.shaderType;
            graphicData.drawSize = this.parent.def.graphicData.drawSize;
            graphicData.color = this.parent.def.graphicData.color;
            graphicData.colorTwo = this.parent.def.graphicData.colorTwo;
            var newGraphic = graphicData.GraphicColoredFor(this.parent);
            Traverse.Create(this.parent).Field("graphicInt").SetValue(newGraphic);
            base.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
        }

        public CompGlower compGlower;
        public void RemoveGlower()
        {
            if (this.compGlower != null)
            {
                base.parent.Map.glowGrid.DeRegisterGlower(this.compGlower);
                this.compGlower = null;
            }
            var parentGlower = this.parent.TryGetComp<CompGlower>();
            if (parentGlower != null)
            {
                base.parent.Map.glowGrid.DeRegisterGlower(parentGlower);
            }
        }
        public void UpdateGlower(GlowerOptions glowerOptions)
        {
            RemoveGlower();
            this.compGlower = new CompGlower();
            this.compGlower.parent = this.parent;
            this.compGlower.Initialize(new CompProperties_Glower()
            {
                glowColor = glowerOptions.glowColor,
                glowRadius = glowerOptions.glowRadius,
                overlightRadius = glowerOptions.overlightRadius
            });
            base.parent.Map.mapDrawer.MapMeshDirty(base.parent.Position, MapMeshFlag.Things);
            base.parent.Map.glowGrid.RegisterGlower(this.compGlower);
        }

        public void Drop()
        {

        }

        public void Notify_Removed()
        {

        }

        public bool TryGetQuality(out QualityCategory qc)
        {
            return this.parent.TryGetQuality(out qc);
        }

        public void Register()
        {
            HRFManager.Instance.RegisterAdjuster(this);
        }

        public void Deregister()
        {
            HRFManager.Instance.DeregisterAdjuster(this);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            Deregister();
            base.PostDestroy(mode, previousMap);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref resourceUseToggleStates, "resourceUseStates", LookMode.Value, LookMode.Value, ref intKeys, ref boolValues);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Register();
            }
        }

        private List<int> intKeys;
        private List<bool> boolValues;
        public override void PostPostMake()
        {
            base.PostPostMake();
            Register();
        }

        public void Update()
        {
            UpdateGraphics();
        }

        public ThingDef GetStuff()
        {
            return this.parent.Stuff;
        }

        public bool IsStorageFor(ResourceProperties resourceProperties, out ResourceStorage resourceStorages)
        {
            resourceStorages = null;
            return false;
        }
    }
}