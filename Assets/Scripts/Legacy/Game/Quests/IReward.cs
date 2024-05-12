using System.Collections.Generic;
using System.Linq;
using Economy.Products;
using GameServices.Player;
using Zenject;

namespace GameModel
{
    namespace Quests
	{
		public interface IReward
		{
			IEnumerable<IProduct> Items { get; }
			IEnumerable<ExperienceData> Experience { get; }
            ExperienceData PlayerExperience { get; }
		}

		public static class RewardExtension
		{
			public static bool Any(this IReward reward)
			{
				return reward != null && (reward.Items.Any() || reward.Experience.Any());
			}

		    public static void Consume(this IReward reward, PlayerSkills playerSkills)
		    {
		        foreach (var item in reward.Items)
		            item.Consume();

		        foreach (var item in reward.Experience)
				{
					long exp = System.Math.Min(item.Ship.Experience - item.ExperienceBefore + item.ExperienceAfter, playerSkills.MaxShipExperience);
					long expBefore = item.ExperienceBefore;
					UnityEngine.Debug.LogError("capped exp: " + exp + " ship exp before: " +item.ExperienceBefore + "max exp: " + playerSkills.MaxShipExperience);
					if (exp == playerSkills.MaxShipExperience)
						item.Ship.Experience = expBefore;
					else
						item.Ship.Experience = exp;
				}
		            

		        playerSkills.Experience += reward.PlayerExperience.ExperienceAfter - reward.PlayerExperience.ExperienceBefore;
            }
        }
	}
}
