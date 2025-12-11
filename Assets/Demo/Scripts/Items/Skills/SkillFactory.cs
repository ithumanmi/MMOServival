using static GlobalEnum;
public class SkillFactory
{
    public static ISkill CreateSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Boomb:
                return new BoombSkill();
            default:
                return null;
        }
    }
}
