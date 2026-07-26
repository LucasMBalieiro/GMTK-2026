using Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Entity/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    public EntityData data;
    
    public Sprite idle1;
    public Sprite idle2;
    public Sprite attack;
    public Sprite defend;
    public Sprite reload;
    public Sprite death;
}
