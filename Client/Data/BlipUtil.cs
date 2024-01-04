using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client.Data;

public class BlipUtil : BaseScript
{
    /// <summary>
    /// Adds a name to the blip on the pause menu map.
    /// </summary>
    public static void SetBlipName(int blip, string name)
    {
        API.AddTextEntry("playerblip", "~a~");
        API.BeginTextCommandSetBlipName("playerblip");
        API.AddTextComponentSubstringPlayerName(name);
        API.EndTextCommandSetBlipName(blip);
        API.SetBlipCategory(blip, 7);
        API.DisplayPlayerNameTagsOnBlips(true);
    }

    /// <summary>
    /// Updates the blip sprite, heading indicator, and blip name.
    /// </summary>
    public static void UpdateBlip(int blip, uint vehicleModel, string name)
    {
        API.SetBlipSprite(blip, BlipInfo.GetBlipSpriteForVehicle(vehicleModel));
        API.ShowHeadingIndicatorOnBlip(blip, vehicleModel == 0);
        SetBlipName(blip, name);
    }

    /// <summary>
    /// In the event that the coordinate or entity blip does not exist, this will get called and fully initialize
    /// a new blip. This does not create a new blip, it only calls the required API.
    /// </summary>
    public static void InitializeBlip(int blip, uint vehicleModel, string name)
    {
        API.SetBlipAsShortRange(blip, true);
        API.SetBlipScale(blip, 1.30f);
        API.SetBlipSprite(blip, BlipInfo.GetBlipSpriteForVehicle(vehicleModel));
        API.ShowHeadingIndicatorOnBlip(blip, vehicleModel == 0);
        SetBlipName(blip, name);
    }
}