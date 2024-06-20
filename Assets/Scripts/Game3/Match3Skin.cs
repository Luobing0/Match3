using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class Match3Skin : MonoBehaviour{
    [SerializeField] Game3Tile[] tilePrefabs;
    [SerializeField] Match3Game game;
    [SerializeField, Range(0.1f, 1f)] float dragThreshold = 0.5f;
    Game3Grid<Game3Tile> tiles;
    [SerializeField] TileSwapper tileSwapper;
    [SerializeField, Range(0.1f, 20f)]
    float dropSpeed = 8f;
    [SerializeField, Range(0f, 10f)]
    float newDropOffset = 2f;

    float busyDuration;

    float2 tileOffset;
    public bool IsPlaying => true;

    public bool IsBusy => busyDuration > 0f;

    public void StartNewGame(){
        busyDuration = 0f;
        game.StartNewGame();
        tileOffset = -0.5f * (float2)(game.Size - 1);
        if (tiles.IsUndefined){
            tiles = new(game.Size);
        }
        else{
            //���ͼ��
            for (int y = 0; y < tiles.SizeY; y++){
                for (int x = 0; x < tiles.SizeX; x++){
                    tiles[x, y].Despawn();
                    tiles[x, y] = null;
                }
            }
        }

        for (int y = 0; y < tiles.SizeY; y++){
            for (int x = 0; x < tiles.SizeX; x++){
                tiles[x, y] = SpawnTile(game[x, y], x, y);
            }
        }
    }

    Game3Tile SpawnTile(TileState t, float x, float y) =>
        tilePrefabs[(int)t - 1].Spawn(new Vector3(x + tileOffset.x, y + tileOffset.y));

    public void DoWork(){
        if (busyDuration > 0f){
            tileSwapper.Update();
            busyDuration -= Time.deltaTime;
            if (busyDuration > 0f){
                return;
            }
        }
        if (game.HasMatches){
            ProcessMatches();
        }
        else if (game.NeedsFilling){
            DropTiles();
        }
    }

    void DropTiles(){
        game.DropTiles();
        for (int i = 0; i < game.DroppedTiles.Count; i++){
            TileDrop drop = game.DroppedTiles[i];
            Game3Tile tile;
            if (drop.fromY < tiles.SizeY){
                tile = tiles[drop.coordinates.x, drop.fromY];
                // tile.transform.localPosition =
                //     new Vector3(drop.coordinates.x + tileOffset.x, drop.coordinates.y + tileOffset.y);
            }
            else{
                tile = SpawnTile(
                    game[drop.coordinates], drop.coordinates.x, drop.fromY + newDropOffset
                );
            }
            tiles[drop.coordinates] = tile;
            busyDuration = Mathf.Max(
                tile.Fall(drop.coordinates.y + tileOffset.y, dropSpeed), busyDuration
            );
        }
    }

    void ProcessMatches(){
        game.ProcessMatches();

        for (int i = 0; i < game.ClearedTileCoordinates.Count; i++){
            int2 c = game.ClearedTileCoordinates[i];
            tiles[c].Despawn();
            busyDuration = Mathf.Max(tiles[c].Disappear(), busyDuration);
            tiles[c] = null;
        }
    }

    //预测拖拽
    public bool EvaluateDrag(Vector3 start, Vector3 end){
        float2 a = ScreenToTileSpace(start), b = ScreenToTileSpace(end);
        var move = new Game3Move(
            (int2)math.floor(a), (b - a) switch{
                var d when d.x > dragThreshold => MoveDirection.Right,
                var d when d.x < -dragThreshold => MoveDirection.Left,
                var d when d.y > dragThreshold => MoveDirection.Up,
                var d when d.y < -dragThreshold => MoveDirection.Down,
                _ => MoveDirection.None
            }
        );
        if (
            move.IsValid &&
            tiles.AreValidCoordinates(move.From) && tiles.AreValidCoordinates(move.To)
        ){
            DoMove(move);
            return false;
        }

        return true;
    }


    private void DoMove(Game3Move move){
        // if (game.TryMove(move)){
        //     (tiles[move.From].transform.localPosition, tiles[move.To].transform.localPosition) =
        //         (tiles[move.To].transform.localPosition, tiles[move.From].transform.localPosition);
        //     tiles.Swap(move.From, move.To);
        // }
        bool success = game.TryMove(move);
        Game3Tile a = tiles[move.From], b = tiles[move.To];
        busyDuration = tileSwapper.Swap(a, b, !success);
        if (success)
        {
            tiles[move.From] = b;
            tiles[move.To] = a;
        }
    }

    /// <summary>
    /// 判断点到了Tile的位置坐标
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    float2 ScreenToTileSpace(Vector3 screenPosition){
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
        return new float2(p.x - tileOffset.x + 0.5f, p.y - tileOffset.y + 0.5f);
    }
}