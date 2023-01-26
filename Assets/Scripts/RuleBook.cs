using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBook : MonoBehaviour
{
    // Start is called before the first frame update
    public Result GetResult(Battler player,Battler enemy)
    {
        // check type
        if ((player.SubmitCard.Base.Type.Equals(CardType.Rock) && enemy.SubmitCard.Base.Type.Equals(CardType.Scissors))
            || (player.SubmitCard.Base.Type.Equals(CardType.Scissors) && enemy.SubmitCard.Base.Type.Equals(CardType.Paper))
            || (player.SubmitCard.Base.Type.Equals(CardType.Paper) && enemy.SubmitCard.Base.Type.Equals(CardType.Rock))
            || (player.SubmitCard.Base.Type.Equals(CardType.Joker)))
        {
            return Result.TurnWin;
        }
        else if ((player.SubmitCard.Base.Type.Equals(CardType.Rock) && enemy.SubmitCard.Base.Type.Equals(CardType.Paper))
            || (player.SubmitCard.Base.Type.Equals(CardType.Scissors) && enemy.SubmitCard.Base.Type.Equals(CardType.Rock))
            || (player.SubmitCard.Base.Type.Equals(CardType.Paper) && enemy.SubmitCard.Base.Type.Equals(CardType.Scissors))
            || (enemy.SubmitCard.Base.Type.Equals(CardType.Joker)))
        {
            return Result.TurnLose;
        }

        // check level
        if(player.SubmitCard.Base.Level > enemy.SubmitCard.Base.Level)
        {
            return Result.TurnWin;
        }
        else if(player.SubmitCard.Base.Level < enemy.SubmitCard.Base.Level)
        {
            return Result.TurnLose;
        }
        return Result.TurnDraw;
    }

}

public enum Result
{
    TurnWin,
    TurnLose,
    TurnDraw,
    TurnWin2,
    TurnLose2,
    GameWin,
    GameLose,
    GameDraw
}