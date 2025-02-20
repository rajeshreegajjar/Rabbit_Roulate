using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Animator winAnimator;
    public Animator loseAnimator;
    public RabbitRoulette rabbitRoulette;
    public BettingStart bettingStart;
    public NewRound newRound;
    public PlaceBet bet;

    [System.Serializable]
    public class BettingStart
    {
        public string userId;
        public long roundNumber;
    }


    [System.Serializable]
    public class PlaceBet
    {
        public string roomId;
        public long roundNumber;
        public int waitingTime;
    }

    [System.Serializable]
    public class NewRound
    {
        public string roomId;
        public long roundNumber;
        public int waitingTime;
    }


    [System.Serializable]
    public class RoundResult
    {
        public string userId;
        public string roomId;
        public long roundNumber;
        public bool isWinner;
        public int winnings;
        public object amount;
        public object number;
        public int winningNumber;
    }

    public void ResetAnimationState()
    {

    }
    public void BettingStarted(string resultJson)
    {
        Debug.Log("Betting Started" + resultJson);
        var result = JsonUtility.FromJson<BettingStart>(resultJson);

    }
    public void BetPlaced(string resultJson)
    {
        Debug.Log("Bet Placed" + resultJson);
        bet = JsonUtility.FromJson<PlaceBet>(resultJson);

    }
    public void NewRoundStarted(string resultJson)
    {
        Debug.Log("New Round Started" + resultJson);
        newRound = JsonUtility.FromJson<NewRound>(resultJson);
        rabbitRoulette.ResetRabitPosition();
        rabbitRoulette.isGameActive = true;
    }

    // Method to be called from JavaScript
    public void ReceiveRoundResults(string resultJson)
    {
        ShowRoundResults(resultJson);
    }

    public void ShowRoundResults(string resultJson)
    {
        Debug.Log("ShowRoundResults" + resultJson);
        var result = JsonUtility.FromJson<RoundResult>(resultJson);
        rabbitRoulette.StartFinalMove(result.winningNumber);

        if (result.isWinner)
        {
            Debug.Log("Play win Animation");
            //winAnimator.SetTrigger("Win");
        }
        else
        {
            Debug.Log("Play loss Animation");
            //loseAnimator.SetTrigger("Lose");
        }
    }

}

////using UnityEngine;

//public class GameManager : MonoBehaviour
//{
//    public Animator winAnimator;
//    public Animator loseAnimator;

//    [System.Serializable]
//    public class RoundResult
//    {
//        public bool isWinner;
//        public int winnings;
//        public int amount;
//        public int number;
//        public int winningNumber;
//    }

//    public void ShowRoundResults(string resultJson)
//    {

//        Debug.Log("ShowRoundResults" + resultJson);
//        var result = JsonUtility.FromJson<RoundResult>(resultJson);


//        //if (result.isWinner)
//        //{
//        //    winAnimator.SetTrigger("Win");
//        //}
//        //else
//        //{
//        //    loseAnimator.SetTrigger("Lose");
//        //}
//    }

//    // Method to be called from JavaScript
//    public void ReceiveRoundResults(string resultJson)
//    {
//        ShowRoundResults(resultJson);
//    }

//    public void ResetAnimationState()
//    {

//    }
//    public void BettingStarted(string resultJson)
//    {
//        Debug.Log("Betting Started" + resultJson);
//    }
//    public void BetPlaced(string resultJson)
//    {
//        Debug.Log("Bet Placed" + resultJson);
//    }
//    public void NewRoundStarted(string resultJson)
//    {
//        Debug.Log("New Round Started" + resultJson);
//    }

//}