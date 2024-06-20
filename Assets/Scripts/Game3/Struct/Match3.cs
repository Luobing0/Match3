using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
/// <summary>
/// ��ͬԪ�ص�ƥ�����һ����ͬ��Ԫ�أ�
/// </summary>
[System.Serializable]
public struct Match3{
   public int2 coordinates;
   public int length;
   public bool isHorizontal;

   public Match3(int x, int y, int length, bool isHorizontal){
      coordinates.x = x;
      coordinates.y = y;
      this.length = length;
      this.isHorizontal = isHorizontal;   
   }
}
