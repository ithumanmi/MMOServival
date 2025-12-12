namespace Hawky.Stat
{
    public interface IGetStat
    {
        float Get(string statId);
    }

    public interface ISetStat
    {
        void Set(string statId, float value);
    }

    public interface IResetStat
    {
        void ResetAll();
    }

    public interface IModifyStatByPercent : IGetStat, IResetStat
    {
        void ModifyPercent(string statId, float value);
    }

    public interface IModifyStatByValue : IGetStat, IResetStat
    {
        void ModifyValue(string statId, float value);
    }

    public interface IStat : IGetStat, ISetStat, IResetStat, IModifyStatByPercent, IModifyStatByValue
    {

    }
}
