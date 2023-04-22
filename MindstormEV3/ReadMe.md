
# Lego Mindstorm EV3

This is a demo project for **Lego Mindstorm EV3** controlled via **.Net Core** application. The project is based on [Lego-Mindstorms](https://github.com/seesharprun/Lego-Mindstorms). It has one main class **Robot** which contains all needed methods to controll **EV3RSTORM**.

## Needed hardware

1. Lego Mindstorm EV3 brick
2. Two Lego Mindstorm EV3 large motors
3. One Lego Mindstorm EV3 motor

## Prerequisites

Build [EV3RSTORM](https://www.lego.com/cdn/product-assets/product.bi.additional.extra.pdf/31313_X_EV3RSTORM.pdf)

## Getting Started

```c#

using MindstormEV3;

// Create new robot instance
// Dispozes automatically when using statement ends
using Robot robot = new Robot("COM11");

robot.Forward(150)
    .TurnRight()
    .Forward(20)
    .Backward(30)
    .TurnLeft()
    .Forward(10)
    .TurnRight()
    .TurnRight()
    .Shoot()
    .Sing()
    .Shoot();

```
