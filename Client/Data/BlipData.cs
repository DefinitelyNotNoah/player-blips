namespace Client.Data;

public class BlipData
{
    public BlipData(int serverId, int coordBlip, uint vehicleModel)
    {
        ServerId = serverId;
        CoordBlip = coordBlip;
        VehicleModel = vehicleModel;
    }

    public int ServerId { get; }

    public int CoordBlip { get; }

    public uint VehicleModel { get; set; }
    
    public int EntityBlip { get; set; }

    public bool EntityExists { get; set; }
}