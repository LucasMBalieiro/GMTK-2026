using System;
using System.Collections.Generic; // Required for List<>
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity), typeof(PlayerController))]
    public class PlayerVisual : MonoBehaviour
    {
        private Entity player;
        private PlayerController controller;

        [SerializeField] private SpriteRenderer heartPrefab;
        [SerializeField] private SpriteRenderer bulletPrefab;

        [SerializeField] private Transform heartParent;
        [SerializeField] private Transform bulletParent;
        
        [SerializeField] private Sprite heartFull;
        [SerializeField] private Sprite heartEmpty;
        [SerializeField] private Sprite bulletFull;
        [SerializeField] private Sprite bulletEmpty;
        
        private List<SpriteRenderer> spawnedHearts = new List<SpriteRenderer>();
        private List<SpriteRenderer> spawnedBullets = new List<SpriteRenderer>();
        
        private EventBinding<ResetConditions> resetBinding;
        
        private void Awake()
        {
            player = GetComponent<Entity>();
            controller = GetComponent<PlayerController>(); 
        }

        private void OnEnable()
        {
            resetBinding = new EventBinding<ResetConditions>(RefreshUI); 
            EventBus<ResetConditions>.Register(resetBinding); 

            controller.UpdateVisual += InitialUI;
        }

        private void OnDisable()
        {
            EventBus<ResetConditions>.Deregister(resetBinding); 
            
            controller.UpdateVisual -= InitialUI; 
        }

        private void InitialUI()
        {
            spawnedHearts.Clear();
            spawnedBullets.Clear();

            for (int i = 0; i < player.data.maxHealth; i++)
            {
                SpriteRenderer heart = Instantiate(heartPrefab, heartParent);
                heart.transform.localPosition = new Vector3(23f + (i * 14f), 0f, 0f); 
                spawnedHearts.Add(heart);
            }

            for (int i = 0; i < player.data.maxAmmo; i++)
            {
                SpriteRenderer bullet = Instantiate(bulletPrefab, bulletParent);
                bullet.transform.localPosition = new Vector3(0f + (i * 14f), 0f, 0f); 
                spawnedBullets.Add(bullet);
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            for (int i = 0; i < spawnedHearts.Count; i++)
            {
                spawnedHearts[i].sprite = i < player.CurrentHealth ? heartFull : heartEmpty;
            }

            for (int i = 0; i < spawnedBullets.Count; i++)
            {
                spawnedBullets[i].sprite = i < player.CurrentAmmo ? bulletFull : bulletEmpty;
            }
        }
    }
}