using MindstormEV3;

// Create new robot instance
// Dispozes automatically when using statement ends
using Robot robot = new Robot("COM10");

robot
    .Forward(150)
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