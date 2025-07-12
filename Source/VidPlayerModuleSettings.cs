namespace Celeste.Mod.VidPlayer;

public class VidPlayerModuleSettings : EverestModuleSettings {

    [SettingName("MODOPTIONS_VIDPLAYER_DISABLEFNAPATCH")]
    [SettingSubText("MODOPTIONS_VIDPLAYER_DISABLEFNAPATCH_DESC")]
    public bool DisableFNAPatch { get; set; } = false;
}