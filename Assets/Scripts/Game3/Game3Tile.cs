using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SKCell;

public enum TileState
{
	None, A, B, C, D, E, F, G
}
public enum MoveDirection
{
    None, Up, Right, Down, Left
}
public class Game3Tile : MonoBehaviour{
    [System.Serializable]
    struct FallingState
    {
        public float fromY, toY, duration, progress;
    }

    FallingState falling;
    [SerializeField, Range(0f, 1f)]
    float disappearDuration = 0.25f;
    float disappearProgress;
    public Game3Tile Spawn(Vector3 position){
        GameObject instance = SKPoolManager.instance.spawnObject(this.gameObject);
        Game3Tile g3 = instance.GetComponent<Game3Tile>();
        g3.transform.localPosition = position;
        g3.transform.localScale = Vector3.one;
        g3.disappearProgress = -1f;
        g3.falling.progress = -1f;
        g3.enabled = false;
        return g3;
    }

    public float Fall(float toY, float speed){
        falling.fromY = transform.localPosition.y;
        falling.toY = toY;
        falling.duration = (falling.fromY - toY) / speed;
        falling.progress = 0;
        enabled = true;
        return falling.duration;
    }
    
    public float Disappear ()
    {
        disappearProgress = 0f;
        enabled = true;
        return disappearDuration;
    }
    void Update ()
    {
        Debug.Log(disappearProgress);
        if (disappearProgress >= 0f)
        {
            disappearProgress += Time.deltaTime;
            if (disappearProgress >= disappearDuration)
            {
                Despawn();
                return;
            }
            Debug.Log(1111);
            transform.localScale =
                Vector3.one * (1f - disappearProgress / disappearDuration);
        }

        if (falling.duration >= 0f){
            Vector3 position = transform.localPosition;
            falling.progress += Time.deltaTime;
            if (falling.progress >= falling.duration){
                falling.progress = -1;
                position.y = falling.toY;
                enabled = disappearProgress >= 0f;
            }
            else{
                position.y = Mathf.Lerp(falling.fromY, falling.toY, falling.progress / falling.duration);
            }

            transform.localPosition = position;
            
        }
    }

    public void Despawn() => Destroy(gameObject);
}
