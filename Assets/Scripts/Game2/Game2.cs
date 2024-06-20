using System;
using TMPro;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

public class Game2 : MonoBehaviour
{
    [SerializeField]
    MazeVisualization visualization;

    [SerializeField]
    int2 mazeSize = int2(20, 20);

    Maze maze;

    void Awake ()
    {
        maze = new Maze(mazeSize);
        visualization.Visualize(maze);
    }

    private void OnDestroy(){
        maze.Dispose();
    }
}