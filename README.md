# Knect

[![Unity](https://img.shields.io/badge/Unity_2022.3-%23000000.svg?logo=unity&logoColor=white)](#)
![Kinect](https://img.shields.io/badge/Microsoft-Kinect-green)
[![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&logoColor=white)](#)

## About

**Knect** is a demo game consisting of minigames meant to be **played with a Kinect sensor**. Alternatively, the game is fully **playable with a keyboard**.

## Getting started

1. Unity Engine is required. Download unity [here](https://unity.com/download). The project was built with version 2022.3.50f1.
2. Clone this repository.
3. Import the project from Unity Hub.

## Showcase

At the moment, only one minigame has been implemented - **Breakout** - inspired by _Kinect Adventures Rallyball_.

### Breakout



![](./Docs/Screenshot1.png)
Logo screen at the start of the game.

![](./Docs/Screenshot2.png)
Title screen.

If a Microsoft Kinect sensor is detected, a hand cursor will show up and is used to interact with the user interface. Otherwise, you may use _the arrow keys and enter_.

![](./Docs/Screenshot3.png)
Main menu. The second planned game is not selectable yet.

![](./Docs/Screenshot4.png)
Submenu for the Breakout game.

The left side of the screen shows your high score along with bonuses that for completing score-based milestones. For each completed milestone, the square light will light up indicating that the bonus is applied on each subsequent game.

The right side of hte screen shows a slideshow of screenshots. If you are playing with Microsoft Kinect, you can select the _Use Kinect_ button. Otherwise, the is player with a keyboard.

![](./Docs/Screenshot5.png)
The tutorial is shown at the start of Breakout while the camera pans through the game scene.

![](./Docs/Screenshot6.png)
Gameplay screenshot. Hitting 3+ bricks in a row counts as a combo and grants you bonus points. The audience reacts to your combos.

![](./Docs/Screenshot7.png)
Greater combos give you more points.

![](./Docs/Screenshot8.png)
Once the time runs out, you gain bonus points for each ball and the audience celebrates.

![](./Docs/Screenshot9.png)
At the end of the level, your results are tallied.