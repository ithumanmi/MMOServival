namespace Hawky.GameFlow
{
    public partial class ResourcesLink
    {
        public const string ICON = "Images/Icons";
        public const string SHOPICON = "Images/ShopIcons";
    }

    public interface IGameFlowService
    {
        int MaxStage();
    }
}