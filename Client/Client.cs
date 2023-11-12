using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.FiveM;
using CitizenFX.FiveM.Native;
using Client.Data;

namespace Client;

public class ClientScript : BaseScript
{
    private readonly Dictionary<int, BlipData> _blipData = new();

    public ClientScript() => OnResourceStart();

    private static void SetBlipName(int blip, string name)
    {
        Natives.AddTextEntry("playerblip", "~a~");
        Natives.BeginTextCommandSetBlipName("playerblip");
        Natives.AddTextComponentSubstringPlayerName(name);
        Natives.EndTextCommandSetBlipName(blip);
        Natives.SetBlipCategory(blip, 7);
        Natives.DisplayPlayerNameTagsOnBlips(true);
    }

    private async void OnResourceStart()
    {
        // This is called in the constructor in a scenario that a player exists inside a client's scope on resource start.
        // Since the script heavily relies on scope event listeners, this method is required since scope events don't execute on resource start.

        // Loop through all players known to client. (Within a client's scope.)
        foreach (Player player in PlayerList.Players.ToList())
        {
            // If the current iterated player is equal to the client's handle, skip the current loop and continue.
            if (player.Handle == Game.Player.Handle)
            {
                continue;
            }

            // Get the handle of the iterated player, and attach a blip to them.
            int playerEntity = player.Character.Handle;
            int entityBlip = Natives.AddBlipForEntity(playerEntity);

            // Blip Customization
            Natives.SetBlipAsShortRange(entityBlip, true);
            Natives.SetBlipScale(entityBlip, 1.30f);
            Natives.SetBlipColour(entityBlip, (int)BlipColor.White);
            SetBlipName(entityBlip, player.Name);

            // Every player's coordinates on the server gets sent to the client every x amount of seconds.
            // We create a blip for each player and set that blip's position to the player's position, that updates every x amount of seconds.
            // This is necessary because entities that go out of scope are deallocated due to OneSync.
            // Which means the blips attached to the player entities will also get removed.

            // We'll wait 1 second to allow all players known to client to get added into _blipData list.
            await Wait(1000);

            BlipData data = _blipData[player.ServerId];
            data.EntityBlip = entityBlip;
        }
    }

    [EventHandler("PlayerBlips-Client:ReceivePlayerData")]
    public void ReceivePlayerDataClientEvent(ExpandoObject playerData)
    {
        // Loop through all the positions that are being sent to the client every x amount of seconds.
        foreach (var entry in playerData)
        {
            object[]? entryData = (object[])entry.Value;
            
            if (Game.Player.State["bucket"] != Convert.ToInt32(entryData[4]))
            {
                // In the event that the player's routing bucket changes while they exist in _blipData,
                // we'll search for them and remove them.
                if (_blipData.TryGetValue(Convert.ToInt32(entry.Key), out BlipData player))
                {
                    int coordBlip = player.CoordBlip;
                    Natives.RemoveBlip(ref coordBlip);
                    _blipData.Remove(Convert.ToInt32(entry.Key));
                }
                continue;
            }
            
            // If the current iterated player is equal to the client's handle, skip the current loop and continue.
            // We will also ensure that we're only checking for players inside the client's routing bucket.
            if (Convert.ToInt32(entry.Key) == Game.Player.ServerId)
            {
                continue;
            }
            
            Vector3 position = (Vector3)entryData[0];
            float heading = (float)entryData[1];
            string name = (string)entryData[2];
            uint vehicleModel = Convert.ToUInt32(entryData[3]);
                
            // Check for key existence
            if (!_blipData.TryGetValue(Convert.ToInt32(entry.Key), out BlipData data))
            {
                int coordBlip = Natives.AddBlipForCoord(position.X, position.Y, position.Z);
                    
                // Customize Blip
                Natives.SetBlipAsShortRange(coordBlip, true);
                Natives.SetBlipScale(coordBlip, 1.30f);
                    
                data = new BlipData(Convert.ToInt32(entry.Key), coordBlip, vehicleModel);
                _blipData[Convert.ToInt32(entry.Key)] = data;
            }
            else
            {
                // Continuously update the player's current vehicle model. This is used to vehicle blip checks.
                data.VehicleModel = vehicleModel;
                
                // If entity blip exists (meaning that the player exists in client scope)
                // To ensure this only executes once, we'll use data.EntityExists.
                if (data.EntityBlip != 0 && !data.EntityExists)
                {
                    Natives.SetBlipDisplay(data.CoordBlip, 0);
                    Natives.SetBlipScale(data.EntityBlip, 1.30f);
                    Natives.ShowHeadingIndicatorOnBlip(data.EntityBlip, data.VehicleModel == 0);
                    SetBlipName(data.EntityBlip, name);
                    data.EntityExists = true;
                }
                
                // If entity blip doesn't exist, use the coordinate blip instead.
                // We don't need to manipulate the entity blip since it's disposed of at this point.
                if (data.EntityBlip == 0)
                {
                    Natives.SetBlipDisplay(data.CoordBlip, 2);
                    Natives.SetBlipSprite(data.CoordBlip, BlipInfo.GetBlipSpriteForVehicle(data.VehicleModel));
                    Natives.ShowHeadingIndicatorOnBlip(data.CoordBlip, data.VehicleModel == 0);
                    Natives.SetBlipCoords(data.CoordBlip, position.X, position.Y, position.Z);
                    SetBlipName(data.CoordBlip, name);
                    if (data.VehicleModel == 0)
                    {
                        Natives.SetBlipRotation(data.CoordBlip, (int)heading);
                    }
                }
                // To prevent using this native every x amount of seconds, we'll only check if blip sprite has changed.
                else if (Natives.GetBlipSprite(data.EntityBlip) != BlipInfo.GetBlipSpriteForVehicle(data.VehicleModel))
                {
                    Natives.SetBlipSprite(data.EntityBlip, BlipInfo.GetBlipSpriteForVehicle(data.VehicleModel));
                    Natives.ShowHeadingIndicatorOnBlip(data.EntityBlip, data.VehicleModel == 0);
                    SetBlipName(data.EntityBlip, name);
                }
            }
        }
    }

    [EventHandler("PlayerBlips-Client:PlayerEnteredScope")]
    public async void PlayerEnteredScopeClientEvent(int playerInside, int playerEntering)
    {
        // If the player inside scope is equal to the client's server ID, return.
        if (Game.Player.ServerId != playerInside || !_blipData.TryGetValue(playerEntering, out BlipData data))
        {
            return;
        }

        // Now we can add an entity blip (this will automatically dispose after entity leaves scope.)
        int player = Natives.GetPlayerFromServerId(playerEntering);
        int playerPed = 0;

        // Sometimes the GetPlayerPed native will take too long to get, leaving it with a value of 0.
        // We'll infinitely loop while it's 0 to make sure it gets the entity.
        while (playerPed == 0)
        {
            playerPed = Natives.GetPlayerPed(player);
            await Wait();
        }

        int entityBlip = Natives.AddBlipForEntity(playerPed);

        // Customize Blip
        Natives.SetBlipAsShortRange(entityBlip, true);
        Natives.SetBlipScale(entityBlip, 1.30f);
        Natives.SetBlipColour(entityBlip, (int)BlipColor.White);

        data.EntityBlip = entityBlip;
    }

    [EventHandler("PlayerBlips-Client:PlayerLeftScope")]
    public void PlayerLeftScopeClientEvent(int playerInside, int playerLeaving)
    {
        // If the player inside scope is equal to the client's server ID, return.
        if (Game.Player.ServerId != playerInside || !_blipData.TryGetValue(playerLeaving, out BlipData data))
        {
            return;
        }

        // Since the entity no longer exists within the client's scope, we'll set data.EntityBlip to 0.
        data.EntityBlip = 0;
        data.EntityExists = false;
    }

    [EventHandler("PlayerBlips-Client:PlayerDropped")]
    public void PlayerDroppedClientEvent(int serverId)
    {
        if (!_blipData.TryGetValue(serverId, out BlipData data))
        {
            return;
        }

        int blip = data.CoordBlip;

        // Remove the blip from client, and _blipData list whenever the player leaves.
        Natives.RemoveBlip(ref blip);
        _blipData.Remove(serverId);
    }
}