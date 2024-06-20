using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[System.Serializable]
public struct Game3Grid<T>{
   private T[] cells;
   private int2 size;
   public int2 Size => size;
   public bool IsUndefined => cells == null || cells.Length == 0;
   public int SizeX => size.x;
   public int SizeY => size.y;

   public T this[int x, int y]{
      get => cells[y * size.x + x];
      set => cells[y * size.x + x] = value;
   }
   public T this[int2 c]
   {
      get => cells[c.y * size.x + c.x];
      set => cells[c.y * size.x + c.x] = value;
   }
   
   //ÅÐ¶ÏÊÇ·ñºÏ·¨
   public bool AreValidCoordinates (int2 c) =>
      0 <= c.x && c.x < size.x && 0 <= c.y && c.y < size.y;
   
   public void Swap (int2 a, int2 b) => (this[a], this[b]) = (this[b], this[a]);
   
   public Game3Grid(int2 size){
      this.size = size;
      cells = new T[this.size.x * size.y];
   }
}
