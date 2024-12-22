using UnityEngine;

//Add as a component to GameObject to know which pool they belong to
public class GameUnit : MonoBehaviour
{
    public Transform transform;
    //If Despawn then object know which pool to push this object to
    public Pool pool;
}
