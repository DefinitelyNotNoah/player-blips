[https://forum.cfx.re/t/real-time-player-blips-on-map/5184529](https://forum.cfx.re/t/real-time-player-blips-on-map/5184529)

# player-blips
* Fully event driven.
* Dynamically switches between coordinate blips and entity blips.
* Makes use of scope events to maximize efficiency.
* Features vehicle blip sprites.
* Clients only update players in the same routing bucket.

# Install
* Drop `playerblips` into your resource directory.
* Add `ensure playerblips` to your server.cfg file.

# Usage
To change the update rate for coordinate blips, change the following in `config.json`
```json
{
    "Delay": 500
}
```
