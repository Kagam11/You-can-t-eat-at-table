using UnityEngine;
using Verse;
using RimWorld;

namespace YouCantEatAtTable
{
    public class YouCantEatAtTableModSettings : ModSettings
    {
        public int CanSlavesAndPrisonersEatAtTable = 2;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref CanSlavesAndPrisonersEatAtTable, "CanSlavesAndPrisonersEatAtTable", 2);
        }
        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("CanSlavesAndPrisonersEatAtTable".Translate());
            listing.Label("Explains".Translate());
            CanSlavesAndPrisonersEatAtTable = (int)listing.Slider(CanSlavesAndPrisonersEatAtTable, 0, 2);
            listing.End();
            base.Write();
        }
    }
}
