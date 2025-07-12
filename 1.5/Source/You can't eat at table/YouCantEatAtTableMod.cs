using UnityEngine;
using Verse;

namespace YouCantEatAtTable
{
    public class YouCantEatAtTableMod : Mod
    {
        public static YouCantEatAtTableModSettings settings;

        public YouCantEatAtTableMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<YouCantEatAtTableModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "YouCantEatAtTable".Translate();
        }
    }
}
