using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill 
{
    private ISkill currentSkill;

    public Skill(ISkill skill)
    {
        currentSkill = skill;
    }

    // Thay đổi kỹ năng
    public void SetSkill(ISkill skill)
    {
        currentSkill = skill;
    }

    // Thực thi kỹ năng
    public void UseSkill()
    {
        currentSkill.Execute();
    }
}
