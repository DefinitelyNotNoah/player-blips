using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace Server;

public class ServerScript : BaseScript
{
    private static readonly string Config =
        File.ReadAllText(API.GetResourcePath(API.GetCurrentResourceName()) + "/config.json");

    private readonly dynamic _json = JsonConvert.DeserializeObject<dynamic>(Config);

    private void SendPlayerData()
    {
        IDictionary<string, object> playerData = new Dictionary<string, object>();

        // Loop through all the players known to the server, where their ped entity exists.
        foreach (Player player in Players.ToList().Where(player => player.Character != null))
        {
            // Check if player is in vehicle.
            int vehicle = API.GetVehiclePedIsIn(player.Character.Handle, false);
            bool isInVehicle = vehicle != 0;
            
            playerData[player.Handle] = new List<object>
            {
                player.Character.Position,
                player.Character.Heading,
                player.Name,
                API.GetEntityModel(vehicle),
                isInVehicle,
                API.GetPlayerRoutingBucket(player.Handle),
            };

            // Update player state bags to make routing buckets known client-side.
            player.State["playerblips-bucket"] = API.GetPlayerRoutingBucket(player.Handle);
        }

        TriggerClientEvent("PlayerBlips-Client:ReceivePlayerData", playerData);
        
    }

    [EventHandler("playerEnteredScope")]
    public void PlayerEnteredScopeServerEvent(IDictionary<string, object> data)
    {
        TriggerClientEvent("PlayerBlips-Client:PlayerEnteredScope", data["for"], data["player"]);
    }

    [EventHandler("playerLeftScope")]
    public void PlayerLeftScopeServerEvent(IDictionary<string, object> data)
    {
        TriggerClientEvent("PlayerBlips-Client:PlayerLeftScope", data["for"], data["player"]);
    }

    [EventHandler("playerDropped")]
    public void PlayerDroppedServerEvent([FromSource] Player source, string reason)
    {
        TriggerClientEvent("PlayerBlips-Client:PlayerDropped", source.Handle);
    }

    [Tick]
    public async Task OnTick()
    {
        SendPlayerData();
        await Delay((int)_json["Delay"]);
    }
}