namespace Common
{
    public static class Utilities
    {
        public static string SkillNameToAnimationName(string skillName)
        {

            switch (skillName)
            {
                case "断点斩杀":
                    return "BreakpointKill";
                case "堆栈护盾":
                    return "Shield";
                case "强制迁移":
                    return "ForceMigration";
                default:
                    return "Attack";
            }
        }
    }
}