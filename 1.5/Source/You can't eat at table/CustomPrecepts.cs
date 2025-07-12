using RimWorld;

#pragma warning disable CS0649
namespace YouCantEatAtTable
{
    internal class CustomPrecepts
    {
        #region Gender
        [MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_Gender_Male;

        [MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_Gender_Female;
        #endregion
        #region Role
        [MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_Role_Leader;

        [MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_Role_LeaderAndMoralGuide;

        [MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_Role_Roles;
        #endregion
        #region Title
        [MayRequireRoyalty, MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_RoyaltyTitle_Freeholder;

        [MayRequireRoyalty, MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_RoyaltyTitle_Novice;

        [MayRequireRoyalty, MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_RoyaltyTitle_Acolyte;

        [MayRequireRoyalty, MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_RoyaltyTitle_Knight;

        [MayRequireRoyalty, MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_RoyaltyTitle_Praetor;

        [MayRequireRoyalty, MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_RoyaltyTitle_Baron;

        [MayRequireRoyalty, MayRequireIdeology]
        public static PreceptDef YouCantEatAtTable_RoyaltyTitle_Archon;
        #endregion
    }
}
