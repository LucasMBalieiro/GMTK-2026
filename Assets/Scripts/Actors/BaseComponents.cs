using System;
using UnityEngine;

namespace Actors
{
    [Serializable]
    public class BaseComponents
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxBullets;
        [SerializeField] private int currentBullets;
        
    }
}