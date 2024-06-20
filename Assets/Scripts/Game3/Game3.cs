using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Game3 : MonoBehaviour{
    [SerializeField] private Match3Skin match3Skin;

    Vector3 dragStart;

    bool isDragging;


    private void Awake(){
        match3Skin.StartNewGame();
    }

    private void Update(){
        if (match3Skin.IsPlaying){
            if (!match3Skin.IsBusy){
                HandleInput();
            }
            match3Skin.DoWork();
        }
        else if (Input.GetKeyDown(KeyCode.Space)){
            match3Skin.StartNewGame();
        }
    }

    void HandleInput(){
        if (!isDragging && Input.GetMouseButtonDown(0)){
            dragStart = Input.mousePosition;
            isDragging = true;
        }
        else if (isDragging && Input.GetMouseButton(0)){
            isDragging = match3Skin.EvaluateDrag(dragStart, Input.mousePosition);
        }
        else{
            isDragging = false;
        }
    }
}