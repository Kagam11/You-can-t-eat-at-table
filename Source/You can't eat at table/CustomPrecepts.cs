using RimWorld;

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
    }
}
