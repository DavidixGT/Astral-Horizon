using System.Linq;
using Economy.Products;
using UnityEngine;
using UnityEngine.UI;
using GameModel.Quests;
using GameServices.Player;
using Services.ObjectPool;
using Services.Resources;
using ViewModel.Common;
using Zenject;

namespace ViewModel
{
	namespace Quests
	{
	    public interface IItemDescription
	    {
	        string Name { get; }
	        Color Color { get; }
	    }

		public class ItemsPanel : MonoBehaviour
		{
		    [Inject] private readonly PlayerSkills _playerSkills;
			[Inject] private readonly IGameObjectFactory _factory;
		    [Inject] private readonly IResourceLocator _resourceLocator;

			public LayoutGroup LayoutGroup;
			public ScrollRect ScrollRect;
			public Text DescriptionText;
		    public PlayerExperienceItem PlayerExperience;

			public void Initialize(IReward reward)
			{
				if (reward != null && reward.Any())
				{
					gameObject.SetActive(true);
					LayoutGroup.transform.InitializeElements<Common.RewardItem, IProduct>(reward.Items, UpdateItem);
				    var experience = reward.Experience.Where(item => item.ExperienceBefore < _playerSkills.MaxShipExperience);
					//var experience = reward.Experience;
					//Debug.LogError(experience + "  h22222exp");
					foreach (var item in reward.Experience)
						Debug.LogError(item.ExperienceBefore + "  hahahahaha exp" + _playerSkills.MaxShipExperience + " :supposed reward" + item.ExperienceAfter);
					
                    LayoutGroup.transform.InitializeElements<Common.ShipExperienceItem, GameModel.ExperienceData>(experience, UpdateExperience, _factory);
					DescriptionText.text = string.Empty;
					ScrollRect.content.anchoredPosition = Vector2.zero;
                    PlayerExperience.Initialize(reward.PlayerExperience);
                }
				else
				{
					gameObject.SetActive(false);
				}
			}

			public void OnItemSelected(GameObject item)
			{
			    var description = item.GetComponent<IItemDescription>();

				DescriptionText.text = description.Name;
			    DescriptionText.color = description.Color;
			}

			public void OnItemDeselected()
			{
				DescriptionText.text = string.Empty;
            }
            
			private void UpdateItem(Common.RewardItem item, IProduct product)
			{
				item.Initialize(product, _resourceLocator);
				item.GetComponent<Toggle>().isOn = false;
			}

			private void UpdateExperience(Common.ShipExperienceItem item, GameModel.ExperienceData data)
			{
				item.Initialize(data);
				item.GetComponent<Toggle>().isOn = false;
			}

			private void Update()
			{
				ScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(ScrollRect.horizontalNormalizedPosition + Time.deltaTime*0.01f);
			}
		}
	}
}
