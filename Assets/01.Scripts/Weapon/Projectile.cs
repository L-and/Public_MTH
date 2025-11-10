using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace _01.Scripts.Weapon
{	
	public class Projectile : MonoBehaviour
	{
		[Range(5, 100)] [Tooltip("After how long time should the bullet prefab be destroyed?")]
		public float destroyAfter;

		[Tooltip("If enabled the bullet destroys on impact")]
		public bool destroyOnImpact = false;

		[Tooltip("Minimum time after impact that the bullet is destroyed")]
		public float minDestroyTime;

		[Tooltip("Maximum time after impact that the bullet is destroyed")]
		public float maxDestroyTime;

		[Header("Impact Effect Prefabs")]
		public Transform[] bloodImpactPrefabs;
		public Transform[] metalImpactPrefabs;
		public Transform[] dirtImpactPrefabs;
		public Transform[] concreteImpactPrefabs;
		public Transform[] enemyImpactPrefabs;
		public Transform[] bossImpactPrefabs;
		
		private void Start()
		{
			// //Grab the game mode service, we need it to access the player character!
			// var gameModeService = ServiceLocator.Current.Get<IGameModeService>();
			// //Ignore the main player character's collision. A little hacky, but it should work.
			// Physics.IgnoreCollision(gameModeService.GetPlayerCharacter().GetComponent<Collider>(),
			// 	GetComponent<Collider>());

			//Start destroy timer
			StartCoroutine(DestroyAfter());
		}

		//If the bullet collides with anything
		private void OnCollisionEnter(Collision collision)
		{			
			//Ignore collisions with other projectiles.
			if (collision.gameObject.GetComponent<Projectile>() != null)
				return;

			// //Ignore collision if bullet collides with "Player" tag
			// if (collision.gameObject.CompareTag("Player")) 
			// {
			// 	//Physics.IgnoreCollision (collision.collider);
			// 	Debug.LogWarning("Collides with player");
			// 	//Physics.IgnoreCollision(GetComponent<Collider>(), GetComponent<Collider>());
			//
			// 	//Ignore player character collision, otherwise this moves it, which is quite odd, and other weird stuff happens!
			// 	Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
			//
			// 	//Return, otherwise we will destroy with this hit, which we don't want!
			// 	return;
			// }
			//
			//If destroy on impact is false, start 
			//coroutine with random destroy timer
			
			if (!destroyOnImpact)
			{
				StartCoroutine(DestroyTimer());
			}
			//Otherwise, destroy bullet on impact
			else
			{
				Destroy(gameObject);
			}

			// 적에게 명중 시 피격 이펙트
			if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				Instantiate(enemyImpactPrefabs[Random.Range
						(0, enemyImpactPrefabs.Length)], transform.position,
					Quaternion.LookRotation(collision.contacts[0].normal));
				Destroy(gameObject);
			}
			
			// 보스에게 명중 시 피격 이펙트
			if(collision.transform.tag == "Boss")
            {
				Instantiate(bossImpactPrefabs[Random.Range
						(0, bossImpactPrefabs.Length)], transform.position,
					Quaternion.LookRotation(collision.contacts[0].normal));
				Destroy(gameObject);
            }

			// [Untagged] 기본 명중이팩트 
			if (collision.transform.tag == "Untagged")
			{
				//Instantiate random impact prefab from array
				Instantiate(concreteImpactPrefabs[Random.Range
						(0, concreteImpactPrefabs.Length)], transform.position,
					Quaternion.LookRotation(collision.contacts[0].normal));
				//Destroy bullet object
				Destroy(gameObject);
			}
		}

		private IEnumerator DestroyTimer()
		{
			//Wait random time based on min and max values
			yield return new WaitForSeconds
				(Random.Range(minDestroyTime, maxDestroyTime));
			//Destroy bullet object
			Destroy(gameObject);
		}

		private IEnumerator DestroyAfter()
		{
			//Wait for set amount of time
			yield return new WaitForSeconds(destroyAfter);
			//Destroy bullet object
			Destroy(gameObject);
		}
	}
}