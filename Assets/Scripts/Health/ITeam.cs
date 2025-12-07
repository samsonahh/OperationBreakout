public enum Team
{
    Player,
    Enemy,
    Neutral,
}

public interface ITeam
{
    Team Team { get; set; }
}