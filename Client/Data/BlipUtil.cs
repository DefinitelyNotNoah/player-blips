using CitizenFX.Core;
using CitizenFX.FiveM.Native;

namespace Client.Data;

public class BlipUtil : BaseScript
{
    /// <summary>
    /// Adds a name to the blip on the pause menu map.
    /// </summary>
    public static void SetBlipName(int blip, string name)
    {
        Natives.AddTextEntry("playerblip", "~a~");
        Natives.BeginTextCommandSetBlipName("playerblip");
        Natives.AddTextComponentSubstringPlayerName(name);
        Natives.EndTextCommandSetBlipName(blip);
        Natives.SetBlipCategory(blip, 7);
        Natives.DisplayPlayerNameTagsOnBlips(true);
    }

    /// <summary>
    /// Updates the blip sprite, heading indicator, and blip name.
    /// </summary>
    public static void UpdateBlip(int blip, uint vehicleModel, string name)
    {
        Natives.SetBlipSprite(blip, BlipInfo.GetBlipSpriteForVehicle(vehicleModel));
        Natives.ShowHeadingIndicatorOnBlip(blip, vehicleModel == 0);
        SetBlipName(blip, name);
    }

    /// <summary>
    /// In the event that the coordinate or entity blip does not exist, this will get called and fully initialize
    /// a new blip. This does not create a new blip, it only calls the required natives.
    /// </summary>
    public static void InitializeBlip(int blip, uint vehicleModel, string name)
    {
        Natives.SetBlipAsShortRange(blip, true);
        Natives.SetBlipScale(blip, 1.30f);
        Natives.SetBlipSprite(blip, BlipInfo.GetBlipSpriteForVehicle(vehicleModel));
        Natives.ShowHeadingIndicatorOnBlip(blip, vehicleModel == 0);
        SetBlipName(blip, name);
    }
}