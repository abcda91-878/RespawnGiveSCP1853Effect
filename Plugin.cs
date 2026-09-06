using Exiled.API.Features;

public class Plugin : Plugin<Exiled.CreditTags.Config>
{
    public override string Name => "出生1853效果插件";
    public override string Author => "青枫社区服主";
    public override void OnEnabled()
    {
        Log.Info("加载成功！");
        base.OnEnabled();
    }
    public override void OnDisabled()
    {
        Log.Info("卸载成功！");
        base.OnDisabled();
    }
}
