// Copyright 2021, Infima Games. All Rights Reserved.

using InfimaGames.LowPolyShooterPack;
using UnityEngine;

namespace _01.Scripts.PlayerControll.Animation
{
	/// <summary>
	/// Handles all the animation events that come from the character in the asset.
	/// </summary>
	public class CharacterAnimationEventHandler : MonoBehaviour
	{
		#region FIELDS

		/// <summary>
        /// Character Component Reference.
        /// </summary>
        private PlayerController _playerController;

		#endregion

		#region UNITY

		private void Awake()
		{
			//Grab a reference to the character component.
			_playerController = transform.root.GetComponent<PlayerController>();
		}

		#endregion

		#region ANIMATION

		/// <summary>
		/// Ejects a casing from the character's equipped weapon. This function is called from an Animation Event.
		/// </summary>
		private void OnEjectCasing()
		{
			//Notify the character.
			if(_playerController != null)
				_playerController.EjectCasing();
		}

		/// <summary>
		/// Fills the character's equipped weapon's ammunition by a certain amount, or fully if set to 0. This function is called
		/// from a Animation Event.
		/// </summary>
		private void OnAmmunitionFill(int amount = 0)
		{
			//Notify the character.
			if(_playerController != null)
				_playerController.FillAmmunition(amount);
		}
		/// <summary>
		/// Sets the character's knife active value. This function is called from an Animation Event.
		/// </summary>
		private void OnSetActiveKnife(int active)
		{
		}
		
		/// <summary>
		/// Spawns a grenade at the correct location. This function is called from an Animation Event.
		/// </summary>
		private void OnGrenade()
		{
		}
		/// <summary>
		/// Sets the equipped weapon's magazine to be active or inactive! This function is called from an Animation Event.
		/// </summary>
		private void OnSetActiveMagazine(int active)
		{
			//Notify the character.
			if (_playerController != null)
			{ 
				//_playerController.SetActiveMagazine(active);
			}
		}

		/// <summary>
		/// Bolt Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedBolt()
		{
		}
		/// <summary>
		/// Reload Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedReload()
		{
			//Notify the character.
			if (_playerController != null)
			{
				// _playerController.AnimationEndedReload();
			}
		}

		/// <summary>
		/// Grenade Throw Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedGrenadeThrow()
		{
		}
		/// <summary>
		/// Melee Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedMelee()
		{
		}

		/// <summary>
		/// Inspect Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedInspect()
		{
			//Notify the character.
			if (_playerController != null)
			{
				// _playerController.AnimationEndedInspect();
			}
		}
		/// <summary>
		/// Holster Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedHolster()
		{
			//Notify the character.
			if (_playerController != null)
			{
				// _playerController.AnimationEndedHolster();
			}
		}

		/// <summary>
		/// Sets the character's equipped weapon's slide back pose. This function is called from an Animation Event.
		/// </summary>
		private void OnSlideBack(int back)
		{
		}

		#endregion
	}   
}