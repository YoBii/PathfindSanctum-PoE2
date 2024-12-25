using ExileCore2;
using ExileCore2.Shared.Enums;
using System.Collections.Generic;
using System.Text;

namespace PathfindSanctum;

public class WeightCalculator(GameController gameController, PathfindSanctumSettings settings)
{
    private readonly GameController gameController = gameController;
    private readonly PathfindSanctumSettings settings = settings;
    private readonly StringBuilder debugText = new();

    public (double weight, string debug) CalculateRoomWeight(RoomState room)
    {
        if (room == null) return (0, string.Empty);

        debugText.Clear();
        var profile = settings.Profiles[settings.CurrentProfile];
        double weight = 1000000; // Base weight so we can go up or down
        debugText.AppendLine($"Base: {weight}");

        weight += CalculateRoomTypeWeight(room, profile);
        weight += CalculateAfflictionWeights(room, profile);
        weight += CalculateRewardWeights(room, profile);
        weight += CalculateConnectivityBonus(room);

        return (weight, debugText.ToString());
    }

    private double CalculateRoomTypeWeight(RoomState room, ProfileContent profile)
    {
        var roomType = room.RoomType;
        if (profile.RoomTypeWeights.TryGetValue(roomType, out float typeWeight))
        {
            debugText.AppendLine($"Room Type ({roomType}): {typeWeight}");
            return typeWeight;
        }
        return 0;
    }

    private double CalculateAfflictionWeights(RoomState room, ProfileContent profile)
    {
        double totalWeight = 0;

        var affliction = room.Affliction;
        var afflictionName = affliction.ToString();
        var (dynamicWeight, explanation) = CalculateDynamicAfflictionWeight(afflictionName);
        if (explanation != string.Empty)
        {
            totalWeight += dynamicWeight;
            debugText.AppendLine($"Affliction ({afflictionName}): {explanation}");
        } else if (profile.AfflictionWeights.TryGetValue(afflictionName, out float afflictionWeight))
        {
            totalWeight += afflictionWeight;
            debugText.AppendLine($"Affliction ({afflictionName}): {afflictionWeight}");
        }

        return totalWeight;
    }

    private (float weight, string explanation) CalculateDynamicAfflictionWeight(string afflictionName)
    {
        return afflictionName switch
        {
            "Iron Manacles" => CalculateIronManaclesWeight(),
            // Future dynamic affliction weights can be added here
            _ => ((float weight, string explanation))(0, string.Empty), // No dynamic modification
        };
    }

    private (float weight, string explanation) CalculateIronManaclesWeight()
    {
        var playerEvasion = gameController.Player.Stats.GetValueOrDefault(GameStat.EvasionRating, 0);
        
        if (playerEvasion > 6000)
        {
            return (-5000, "(High Evasion Build: -5000)");
        }

        return (0, null);
    }

    private double CalculateRewardWeights(RoomState room, ProfileContent profile)
    {
        double weight = 0;

        var reward = room.Reward;
        var rewardName = reward.ToString();
        if (profile.RewardWeights.TryGetValue(rewardName, out float rewardWeight))
        {
            weight += rewardWeight;
            debugText.AppendLine($"Reward ({rewardName}): {rewardWeight}");
        }

        return weight;
    }

    private double CalculateConnectivityBonus(RoomState room)
    {
        var connectionBonus = room.Connections * 100;
        debugText.AppendLine($"Connections: +{connectionBonus}");
        return connectionBonus;
    }
} 
