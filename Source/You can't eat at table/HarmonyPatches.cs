﻿using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace YouCantEatAtTable
{
    [StaticConstructorOnStartup]
    internal static class InitialiseHarmony
    {
        static InitialiseHarmony()
        {
            var harmony = new Harmony("kagami.YouCantEatAtTable");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Toils_Ingest), nameof(Toils_Ingest.CarryIngestibleToChewSpot))]
    public static class PatchCarryIngestibleToChewSpot
    {

        public static bool Prefix(Pawn pawn, TargetIndex ingestibleInd, ref Toil __result)
        {
            #region Slaves
            if (pawn.IsPrisoner || pawn.IsSlave)
            {
                var canSlaves = YouCantEatAtTableMod.settings.CanSlavesAndPrisonersEatAtTable;
                if (canSlaves < 0 || canSlaves > 2) canSlaves = 2;
                //Log.Warning($"pawn:{pawn.Name}, isSlave:{pawn.IsSlave}, isPrisoner:{pawn.IsPrisoner}");
                if (pawn.IsSlave || pawn.IsPrisoner)
                {
                    if (canSlaves == 0)
                    {
                        //Log.Warning($"pawn:{pawn.Name}, isSlave:{pawn.IsSlave}, isPrisoner:{pawn.IsPrisoner} can eat");
                        return true;
                    }
                    else if (canSlaves == 2)
                    {
                        //Log.Warning($"pawn:{pawn.Name}, isSlave:{pawn.IsSlave}, isPrisoner:{pawn.IsPrisoner} cannot eat");
                        __result = MakeToil(pawn, ingestibleInd);
                        return false;
                    }
                }
            }
            #endregion

            if (pawn.IsColonist && pawn.Ideo != null && pawn.Ideo.PreceptsListForReading.Any(x => x.def.ToString().StartsWith("YouCantEatAtTable")))
            {
                var canEatAtTable = false;

                #region Gender
                var gender = GenderCheck(pawn);
                canEatAtTable = gender || canEatAtTable;
                #endregion

                #region Role
                var role = RoleCheck(pawn);
                canEatAtTable = role || canEatAtTable;
                #endregion

                #region Title
                var title = TitleCheck(pawn);
                canEatAtTable = title || canEatAtTable;
                #endregion

                #region Carry
                //Log.Warning($"{pawn.Name} can eat at table?{canEatAtTable}");
                if (canEatAtTable)
                {
                    return true;
                }
                else
                {
                    __result = MakeToil(pawn, ingestibleInd);
                    return false;
                }
                #endregion
            }
            else return true;
        }

        private static Toil MakeToil(Pawn pawn, TargetIndex ingestibleInd)
        {
            Toil toil = ToilMaker.MakeToil("CarryIngestibleToChewSpot");
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                IntVec3 cell = IntVec3.Invalid;
                Thing thing = null;
                Thing thing2 = actor.CurJob.GetTarget(ingestibleInd).Thing;
                if (thing2.def.ingestible.chairSearchRadius > 0f)
                {
                    thing = null;
                }
                if (thing == null)
                {
                    cell = RCellFinder.SpotToChewStandingNear(actor, actor.CurJob.GetTarget(ingestibleInd).Thing, (IntVec3 c) => actor.CanReserveSittableOrSpot(c));
                    Danger chewSpotDanger = cell.GetDangerFor(pawn, actor.Map);
                    if (chewSpotDanger != Danger.None)
                    {
                        thing = GenClosest.ClosestThingReachable(actor.Position, actor.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell, TraverseParms.For(actor), thing2.def.ingestible.chairSearchRadius, (Thing t) => BaseChairValidator(t) && (int)t.Position.GetDangerFor(pawn, t.Map) <= (int)chewSpotDanger);
                    }
                }
                if (thing != null && !Toils_Ingest.TryFindFreeSittingSpotOnThing(thing, actor, out cell))
                {
                    Log.Error("Could not find sitting spot on chewing chair! This is not supposed to happen - we looked for a free spot in a previous check!");
                }
                actor.ReserveSittableOrSpot(cell, actor.CurJob);
                actor.Map.pawnDestinationReservationManager.Reserve(actor, actor.CurJob, cell);
                actor.pather.StartPath(cell, PathEndMode.OnCell);
                bool BaseChairValidator(Thing t)
                {
                    if (t.def.building == null || t.def.building.isSittable)
                    {
                        return false;
                    }
                    if (!Toils_Ingest.TryFindFreeSittingSpotOnThing(t, actor, out var cell2))
                    {
                        return false;
                    }
                    if (t.IsForbidden(pawn))
                    {
                        return false;
                    }
                    if (actor.IsColonist && t.Position.Fogged(t.Map))
                    {
                        return false;
                    }
                    if (!actor.CanReserve(t))
                    {
                        return false;
                    }
                    if (!t.IsSociallyProper(actor))
                    {
                        return false;
                    }
                    if (t.IsBurning())
                    {
                        return false;
                    }
                    if (t.HostileTo(pawn))
                    {
                        return false;
                    }
                    bool flag = false;
                    for (int i = 0; i < 4; i++)
                    {
                        Building edifice = (cell2 + GenAdj.CardinalDirections[i]).GetEdifice(t.Map);
                        if (edifice != null && edifice.def.surfaceType == SurfaceType.Eat)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                    return true;
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        private static bool GenderCheck(Pawn pawn)
        {
            bool canEatAtTable = false;
            var genderPrecept = pawn.Ideo.PreceptsListForReading.Find(x => x.def.ToString().StartsWith("YouCantEatAtTable_Gender"));
            if (genderPrecept != null)
            {
                var gender = pawn.gender;
                //Log.Warning($"pawn:{pawn.Name}, gender:{gender}, precept:{nameof(genderPrecept.def)}");
                if (genderPrecept.def.ToString() == nameof(CustomPrecepts.YouCantEatAtTable_Gender_Male))
                {
                    if (gender == Gender.Male)
                    {
                        canEatAtTable = true;
                    }
                }
                else if (genderPrecept.def.ToString() == nameof(CustomPrecepts.YouCantEatAtTable_Gender_Female))
                {
                    if (gender == Gender.Female)
                    {
                        canEatAtTable = true;
                    }
                }
            }
            return canEatAtTable;
        }

        private static bool RoleCheck(Pawn pawn)
        {
            bool canEatAtTable = false;
            var rolePrecept = pawn.Ideo.PreceptsListForReading.Find(x => x.def.ToString().StartsWith("YouCantEatAtTable_Role"));
            //Log.Warning($"pawn:{pawn.Name}, isleader:{isLeader}, ismg:{isMoralGuide}, isrole:{isRole}");
            if (rolePrecept != null)
            {
                var role = pawn.Ideo.GetRole(pawn);
                //Log.Warning($"pawn:{pawn.Name}, role:{role?.def.ToString() ?? "null"}");
                bool isLeader;
                bool isMoralGuide;
                bool isRole;
                if (role is null)
                {
                    isLeader = false;
                    isMoralGuide = false;
                    isRole = false;
                }
                else
                {
                    isLeader = role.def.ToString() == nameof(PreceptDefOf.IdeoRole_Leader);
                    isMoralGuide = role.def.ToString() == nameof(PreceptDefOf.IdeoRole_Moralist);
                    isRole = role.def.ToString().EndsWith("Specialist");
                }

                if (rolePrecept.def.ToString() == nameof(CustomPrecepts.YouCantEatAtTable_Role_Leader))
                {
                    if (isLeader)
                    {
                        canEatAtTable = true;
                    }
                }
                else if (rolePrecept.def.ToString() == nameof(CustomPrecepts.YouCantEatAtTable_Role_LeaderAndMoralGuide))
                {
                    if (isLeader || isMoralGuide)
                    {
                        canEatAtTable = true;
                    }
                }
                else if (rolePrecept.def.ToString() == nameof(CustomPrecepts.YouCantEatAtTable_Role_Roles))
                {
                    if (isRole || isLeader || isMoralGuide)
                    {
                        canEatAtTable = true;
                    }
                }
            }
            return canEatAtTable;
        }

        private static bool TitleCheck(Pawn pawn)
        {
            //Log.Warning("Checking Title");
            bool canEatAtTable = false;
            var titlePrecept = pawn.Ideo.PreceptsListForReading.Find(x => x.def.ToString().StartsWith("YouCantEatAtTable_RoyaltyTitle"));
            if (titlePrecept != null)
            {
                var title = pawn.GetCurrentTitleIn(Faction.OfEmpire);
                if (title == null) return false;
                var requiredTitle = Utils.Titles[titlePrecept.def.ToString().Split('_').Last()];
                var pawnTitle = Utils.Titles[title.defName];
                canEatAtTable = pawnTitle >= requiredTitle;
                //Log.Warning($"pawn:{pawn.Name}, title:{title.defName ?? "null"}, caneat:{canEatAtTable}");
            }
            return canEatAtTable;
        }
    }
}
