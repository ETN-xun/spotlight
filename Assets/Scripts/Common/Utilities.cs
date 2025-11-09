namespace Common
{
    public static class Utilities
    {
        public static string SkillNameToAnimationName(string skillName)
        {

            switch (skillName)
            {
                case "断点斩杀":
                    return "Attack";
                case "堆栈护盾":
                    return "Attack";
                case "强制迁移":
                    return "Attack";
                case "闪回位移":
                case "Flashback":
                    return "Attack"; // 修改为Attack动画
                default:
                    return "Attack";
            }
        }
    }
}
