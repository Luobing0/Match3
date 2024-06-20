using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeetCodeTest : MonoBehaviour{
    public int[] _nums = new[]{ 1,2,3,5,2,6};
    public int K;
    private void Start(){
        Rotate(_nums,K);
    }
    
    //实现将数组向右旋转 k 步
    public void Rotate(int[] nums, int k){
        //向右移动K步
        // k = k % nums.Length;
        // for (int i = 0; i < k; i++){
        //     int nextIndex = nums.Length - k + i;
        //     (nums[nextIndex], nums[i]) = (nums[i], nums[nextIndex]);
        // }
        //
        // nums.ToList().ForEach(num => Debug.Log($"{num} "));
        k %= nums.Length;
        //先翻转所有数组
        Reverse(nums,0,nums.Length-1);
        //再翻转0-k个
        Reverse(nums,0,k-1);
        //最后k-Length
        Reverse(nums,k,nums.Length-1);
        nums.ToList().ForEach(num => Debug.Log($"{num} "));
    }

    private void Reverse(int[] nums, int start, int end){
        while (start < end){
            (nums[start], nums[end]) = (nums[end], nums[start]);
            start++;
            end--;
        }
    }
}
