using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource winAudio;
    public AudioSource loseAudio;
    public RabbitRoulette rabbitRoulette;
    public BettingStart bettingStart;
    public NewRound newRound;
    public PlaceBet bet;

    [System.Serializable]
    public class BettingStart
    {
        public string roomId;
        public long roundNumber;
        public int bettingTime;
    }


    [System.Serializable]
    public class PlaceBet
    {
        public string userId;
        public string roomId;
        public int number;
        public int amount;
        public long roundNumber;
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
        bettingStart = result;

    }
    public void BetPlaced(string resultJson)
    {
        Debug.Log("Bet Placed" + resultJson);
        var result = JsonUtility.FromJson<PlaceBet>(resultJson);
        if (result.roomId == bettingStart.roomId)
        {
            bet = result;
        }
        else
        {
            bet = new PlaceBet();
            bet.number = -1;
        }

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
        rabbitRoulette.StartFinalMove(result.winningNumber,bet.number);

        if (result.userId == bet.userId && result.roomId == bet.roomId && result.roundNumber == bet.roundNumber)
        {
            StartCoroutine(PlayAnimation(result.isWinner));
        }    

      
    }

    IEnumerator PlayAnimation(bool isWinner)
    {

        
        yield return new WaitForSecondsRealtime(2f);
        if (isWinner)
        {
            //    winAnimator.gameObject.SetActive(true);
            Debug.Log("Play win Animation");
        //winAnimator.SetTrigger("Win");
            winAudio.Play();
        }
        else
        {
            //loseAnimator.gameObject.SetActive(true);
            Debug.Log("Play loss Animation");
            //loseAnimator.SetTrigger("Lose");
            loseAudio.Play();
        }
        yield return new WaitForSecondsRealtime(5f);
        winAudio.Stop();
        loseAudio.Stop();
    }

}
