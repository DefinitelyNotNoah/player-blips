using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.FiveM;
using CitizenFX.FiveM.Native;

namespace Client.Data;

public abstract class BlipInfo : BaseScript
{
    private static readonly Dictionary<string, int> Sprites = new()
    {
        { "CARNOTFOUND", 1 },
        { "TAXI", 56 },
        { "RHINO", 421 },
        { "LAZER", 424 },
        { "BESRA", 424 },
        { "HYDRA", 424 },
        { "INSURGENT", 426 },
        { "INSURGENT2", 426 },
        { "INSURGENT3", 426 },
        { "LIMO2", 460 },
        { "BLAZER5", 512 },
        { "PHANTOM2", 528 },
        { "BOXVILLE5", 529 },
        { "RUINER2", 530 },
        { "DUNE4", 531 },
        { "DUNE5", 531 },
        { "WASTELANDER", 532 },
        { "VOLTIC2", 533 },
        { "TECHNICAL2", 534 },
        { "TECHNICAL3", 534 },
        { "TECHNICAL", 534 },
        { "APC", 558 },
        { "OPPRESSOR", 559 },
        { "OPPRESSOR2", 639 },
        { "HALFTRACK", 560 },
        { "DUNE3", 561 },
        { "TAMPA3", 562 },
        { "TRAILERSMALL2", 563 },
        { "ALPHAZ1", 572 },
        { "BOMBUSHKA", 573 },
        { "HAVOK", 574 },
        { "HOWARD", 575 },
        { "HUNTER", 576 },
        { "MICROLIGHT", 577 },
        { "MOGUL", 578 },
        { "MOLOTOK", 579 },
        { "NOKOTA", 580 },
        { "PYRO", 581 },
        { "ROGUE", 582 },
        { "STARLING", 583 },
        { "SEABREEZE", 584 },
        { "TULA", 585 },
        { "AVENGER", 589 },
        { "STROMBERG", 595 },
        { "DELUXO", 596 },
        { "THRUSTER", 597 },
        { "KHANJALI", 598 },
        { "RIOT2", 599 },
        { "VOLATOL", 600 },
        { "BARRAGE", 601 },
        { "AKULA", 602 },
        { "CHERNOBOG", 603 },
    };

    public static int GetBlipSpriteForVehicle(uint vehicleModel)
    {
        string displayName = Natives.GetDisplayNameFromVehicleModel(vehicleModel);
        if (Sprites.TryGetValue(displayName, out int blipSprite))
        {
            return blipSprite;
        }

        if (Natives.IsThisModelABike(vehicleModel))
        {
            return 348;
        }
        
        if (Natives.IsThisModelABoat(vehicleModel))
        {
            return 427;
        }
        
        if (Natives.IsThisModelAHeli(vehicleModel))
        {
            return 422;
        }
        
        return Natives.IsThisModelAPlane(vehicleModel) ? 423 : 225;
    }
}