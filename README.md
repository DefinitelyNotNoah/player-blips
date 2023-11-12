# player-blips
* Dynamically switches between coordinate blips and entity blips.
* Makes use of scope events to maximize efficiency.
* Features vehicle blip sprites.

# Install
* Drop `player-blips` into your resource directory.
* Add `ensure player-blips` to your server.cfg file.

# Usage
To change the update rate for coordinate blips, change the following:
```cs
        [Tick]
        public async Coroutine OnTick()
        {
            SendPlayerData();
            await Wait(500); // <-- Change
        }
```
