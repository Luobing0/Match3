using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;

public class Match3Game : MonoBehaviour{
    [SerializeField] int2 size = 8;
    Game3Grid<Game3Tile> tiles;

    float2 tileOffset;
    Game3Grid<TileState> grid;
    List<Match3> matches;
    public bool HasMatches => matches.Count > 0;
    public List<int2> ClearedTileCoordinates{ get; private set; }
    public bool NeedsFilling{ get; private set; }
    public List<TileDrop> DroppedTiles{ get; private set; }
    public TileState this[int x, int y] => grid[x, y];

    public TileState this[int2 c] => grid[c];

    public int2 Size => size;


    public void StartNewGame(){
        if (grid.IsUndefined){
            grid = new(size);
            matches = new();
            ClearedTileCoordinates = new();
            DroppedTiles = new();
        }

        FillGrid();
    }

    /// <summary>
    /// ������
    /// </summary>
    void FillGrid(){
        for (int y = 0; y < size.y; y++){
            for (int x = 0; x < size.x; x++){
                //TOTHINK:Ϊʲô�����Ͳ�����������һ����Ԫ���أ�
                TileState a = TileState.None, b = TileState.None;
                //Ǳ�ڼ���
                int potentialMatchCount = 0;
                if (x > 1){
                    a = grid[x - 1, y];
                    if (a == grid[x - 2, y]){
                        potentialMatchCount = 1;
                    }
                }

                if (y > 1){
                    b = grid[x, y - 1];
                    if (b == grid[x, y - 2]){
                        potentialMatchCount += 1;
                        if (potentialMatchCount == 1){
                            a = b;
                        }
                        else if (b < a){
                            (a, b) = (b, a);
                        }
                    }
                }

                TileState t = (TileState)Random.Range(1, 8 - potentialMatchCount);
                if (potentialMatchCount > 0 && t >= a){
                    t += 1;
                }

                if (potentialMatchCount == 2 && t >= b){
                    t += 1;
                }
                // TileState t = (TileState)Random.Range(1, 8);

                grid[x, y] = t;
            }
        }
    }

    public bool TryMove(Game3Move move){
        grid.Swap(move.From, move.To);
        if (FindMatches()){
            return true;
        }

        grid.Swap(move.From, move.To);
        return false;
    }

    /// <summary>
    /// �ҵ�ƥ����
    /// </summary>
    /// <returns></returns>
    bool FindMatches(){
        for (int y = 0; y < size.y; y++){
            // ��ÿһ���У��ӵ�һ�п�ʼ�����������ڵ�Ԫ���Ƿ���ͬ
            TileState start = grid[0, y];
            int length = 1;
            for (int x = 1; x < size.x; x++){
                TileState t = grid[x, y];
                if (t == start){
                    //�ҵ���ͬԪ��
                    length += 1;
                }
                else{
                    if (length >= 3){
                        matches.Add(new Match3(x - length, y, length, true));
                    }

                    start = t;
                    length = 1;
                }
            }

            // ���һ�н���ʱ��ƥ�����
            if (length >= 3){
                matches.Add(new Match3(size.x - length, y, length, true));
            }
        }

        for (int x = 0; x < size.x; x++){
            TileState start = grid[x, 0];
            int length = 1;
            for (int y = 1; y < size.y; y++){
                TileState t = grid[x, y];
                if (t == start){
                    length += 1;
                }
                else{
                    if (length >= 3){
                        matches.Add(new Match3(x, y - length, length, false));
                    }

                    start = t;
                    length = 1;
                }
            }

            if (length >= 3){
                matches.Add(new Match3(x, size.y - length, length, false));
            }
        }

        return HasMatches;
    }

    /// <summary>
    /// ����ѱ����ƥ���ͼ���б�
    /// </summary>
    public void ProcessMatches(){
        ClearedTileCoordinates.Clear();
        for (int m = 0; m < matches.Count; m++){
            Match3 match = matches[m];
            int2 step = match.isHorizontal ? int2(1, 0) : int2(0, 1);
            int2 c = match.coordinates;
            for (int i = 0; i < match.length; c += step, i++){
                if (grid[c] != TileState.None){
                    grid[c] = TileState.None;
                    ClearedTileCoordinates.Add(c);
                }
            }
        }

        matches.Clear();
        NeedsFilling = true;
    }

    public void DropTiles(){
        DroppedTiles.Clear();
        for (int x = 0; x < size.x; x++){
            int holeCount = 0;
            for (int y = 0; y < size.y; y++){
                if (grid[x, y] == TileState.None){
                    holeCount += 1;
                }
                else if (holeCount > 0){
                    grid[x, y - holeCount] = grid[x, y];
                    DroppedTiles.Add(new TileDrop(x, y - holeCount, holeCount));
                }
            }
            //���Ϸ���tile������ɵ���
            for (int h = 1; h <= holeCount; h++)
            {
                grid[x, size.y - h] = (TileState)Random.Range(1, 8);
                DroppedTiles.Add(new TileDrop(x, size.y - h, holeCount));
            }
        }

        NeedsFilling = false;
        //��������ƥ��
        FindMatches();
    }
}