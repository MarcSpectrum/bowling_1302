# Bowling 1302

A colorful single-player ten-pin bowling game for Windows, built with Unity 6. The game includes official scoring, a complete ten-frame match loop, arcade presentation, menus, persistent display/audio settings, pause controls, and automatic replay.

## Controls

- `A` / `D` or left/right arrows — position the ball
- `Q` / `E` — adjust aim
- Hold and release `Space` — charge and launch
- `Escape` — pause or resume

Position and aim lock as soon as charging begins. A roll ends at the rear dead zone, after the ball is nearly stationary for one second, or at the 12-second safety timeout. Pins then receive two seconds to settle.

## Rules and match flow

Bowling 1302 follows official ten-pin scoring: strikes and spares receive bonuses, open frames total their pins, and the tenth frame awards bonus rolls when appropriate. The HUD shows frame marks and cumulative totals. Fallen pins are removed for a frame's second roll; completed frames and strike bonus situations receive a fresh rack. After the final score is displayed, a new match begins automatically after five seconds without reloading the scene.

## Scenes

- `Assets/Scenes/MainMenu.unity` — build index 0; Play, Settings, Credits, and Quit
- `Assets/Scenes/BowlingGame.unity` — build index 1; lane, ten-pin rack, gameplay HUD, pause and results
- `Assets/Scenes/SampleScene.unity` — retained as an unused reference asset

The source-of-truth scene generator is available at **Bowling 1302 → Rebuild Game Scenes** in the Unity Editor.

## Windows build

1. Open the project in Unity `6000.3.22f1` or a compatible Unity 6 editor.
2. Run **Bowling 1302 → Rebuild Game Scenes** if scene regeneration is needed.
3. Open **File → Build Profiles**, select Windows, and confirm MainMenu and BowlingGame are the only enabled scenes in that order.
4. Choose an output folder and select **Build** (or **Build and Run**).

Scoring Edit Mode tests are under `Assets/Tests/EditMode` and can be run from **Window → General → Test Runner**.
