# Unity Game Client for Sprout 2024 C++ Course

This is the source code repo for the custom version of Kitchen Chaos game. Some major modifications are as follows:

- Support Round-based game, using 1800 rounds instead of 60 seconds.
- Support Map-based game, where players can only move along the coordinates.
- 3 different kinds of recipe mode: salad only, salad & cheeseburger, and all recipe.
- Designed and implemented an API to support gameplay with a stateful server instead of human player.

## Reference

Game Assets are mostly from the tutorial of [Code Monkey](https://www.youtube.com/watch?v=AmGSEH7QcDg). Also, many of the C# code follows the tutorial (for the WebGL build), while the code has been modified by me to enable the gameplay with stateful C++ server.




