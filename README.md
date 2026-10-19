# CS464 Assignment 02 · Muhammad Hamza · BSSE23001

- **Video:** https://drive.google.com/file/d/1n48I84EVXIcvum7u2LUfEa8rXRGC3FgB/view?usp=sharing. (NOTE because I have low constraint laptop with unity running on hdd so I could not make a video for 1 min only, Please accept the video length)
- **Input:** Legacy Input with Active Input Handling set to Both
- **Ground check:** User ground layer added and assigned to appropriate planes. Gizmos ground check function is also added as instructed in manual.
- **Camera:** Exact smoothing `1 − e^(−k·dt)` with k = 5, plus velocity look-ahead of 0.35 s is configured according to the manual.
- **Juice:** Squash on landing with ease outback is added.

## Tuning

| Setting          | Value       | Why this number                                                                                                        |
| ---------------- | ----------- | ---------------------------------------------------------------------------------------------------------------------- |
| Speed            | 5 m/s       | Crosses a 3 m gap's run-up in 0.6 s.                                                                                   |
| Jump height      | 1.8 m       | Clears the 1.2 m ledges with 0.6 m to spare. Calculated launch speed from in manual formula is approximately 5.94 m/s. |
| Time to apex     | 0.61 s      | Calculated from v and g: v/g = 5.94 / 9.81.                                                                            |
| Fall multiplier  | 2.2×        | Fall now takes 0.41 s, so the jump feels real while the apex stays at 1.8 m.                                           |
| Coyote time      | 0.1 s       | Forgiving while remaining real the jumping at 5 m/s that is 0.5 m past the edge.                                       |
| Jump buffer      | 0.12 s      | A jump pressed 0.6 m before landing is still held.                                                                     |
| Camera smoothing | sharpness 5 | Steady-state is preserved with lag = speed / k = 5/5 = 1 m.                                                            |
| Look-ahead       | 0.35 s      | 1.75 m of view ahead, more than the 1 m lag, so the player always remain in the front half of the screen.              |

## Four answers

1. **The jump, in numbers.** v = 5.94 m/s (calculated from lecture formula), so with mass 1 the impulse (jumpForce) is 5.94. Time to apex = 0.61 s; fall time with 2.2× gravity is 0.41 s; total air time is 1.02 s. At 5 m/s that carries me about 5.1 m, so it clears my widest gap of 3 m with room to spare.
2. **Update and FixedUpdate.** Update reads input, builds the relative move vector for camera, does the ground layer check, the coyote and buffer timestamps, queues the jump and starts the squash. FixedUpdate sets linearVelocity (keeping y), applies the queued jump once, and adds fall gravity.
3. **Vector3.Dot.** `away = Vector3.Dot(move, fwd);` in `PlayerController.Update`, where `fwd` is the camera's flattened forward. Positive means the player is moving away from the camera (W), negative means toward it (S), zero means sideways. I draw it green and red in the gizmo.
4. **The juice.** Landing squash is configured where scale.y goes from 0.7 to 1 through Ease.OutBack. Duration 0.25 seconds = 15 frames at 60 fps.

## Levels

| Level   | What the controller must do there                                                                                                                     | What I changed for this jump                                                                                                 |
| ------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Level01 | The player must follow the candy trail, climb the staircase to the platform, cross the bridge and reach the goal.                                     | Nothing was changed, every jump was successfully crossed. There is one more gap that is intentionally made to not crossable. |
| Level02 | Here the player is supposed to walk through the forest following the smoke, jump over rocks and take cover behind crates and can also jump over them. | No particular jump was added in this level but we can check jumping over the crates.                                         |
| Level03 | Here we have to reach the enemy base end point to initiate c3 bomb. There is no particular jump here also but some raised platforms are there.        | Nothing was changed.                                                                                                         |
| Level04 | This is an escape mission with different covers and routes to tackle the enemy as well as hide, then run out to the car at the exit.                  | Nothing was changed as there is no particular jump here.                                                                     |
| Level05 | This is a mountains mission where we climb upward to the cloud-covered bridge to kill the enemies and reach the goal.                                 | Nothing was changed, every jump hight was perfect.                                                                           |

## Other changings from Jump related

- **Level05 cliffs:** the cliffs in level 5 had no colliders, so the player fell through them. I added Mesh Colliders to the cliffs.
- **Camera orbit (Levels 04 and 05):** these two levels needed the view to be turned, so I added right-mouse-button orbit to CameraFollow script where we can change the camera view by holding right click and moving the camera.
