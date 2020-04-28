using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

	[SerializeField]
	private GameObject[] cardPrefabs, playerCardPosition, dealerCardPosition;
	[SerializeField]
	private GameObject backCardPrefab;
	[SerializeField]
	private Button primaryBtn, secondaryBtn, doubleDownBtn, resetBalanceBtn, exitBtn;
	[SerializeField]
	private Button bet50, bet100, bet500, allIn;
	[SerializeField]
	private TextMeshProUGUI textMoney, textBet, textPlayerPoints, textDealerPoints, textPlaceYourBet, textSelectingBet, textWinner;
	[SerializeField]
	private TextMeshProUGUI bet50text, bet100text, bet500text, allIntext;
	[SerializeField]
	private Image resetImgBtn, betImage, resetImg;
	[SerializeField]
	private GameObject blackChip, redChip, greenChip, blueChip, parentChip;
	[SerializeField]
	private AudioSource loses, wins, twentyone;
	[SerializeField]
	private GameObject coinParticleSystem, moneySound;

	private List<Card> playerCards;
	private List<Card> dealerCards;
	private bool isPlaying;
	private int playerPoints;
	private int actualDealerPoints, displayDealerPoints;
	private float playerMoney;
	private float currentBet;
	private int playerCardPointer, dealerCardPointer;



	private Deck playingDeck;
	
	private void Start() {
		//playerMoney = 1000;
		//textMoney.text = "$" + playerMoney.ToString();
		playerMoney = PlayerPrefs.GetFloat("Money", 2000f);
		textMoney.text = playerMoney.ToString();
		currentBet = 50;
		resetGame();

		primaryBtn.onClick.AddListener(delegate {
			if (isPlaying) {
				playerDrawCard();
				doubleDownBtn.gameObject.SetActive(false);
			} else {
				startGame();
			}
		});

		secondaryBtn.onClick.AddListener(delegate {
			playerEndTurn();
			primaryBtn.gameObject.SetActive(false);
			secondaryBtn.gameObject.SetActive(false);
			exitBtn.gameObject.SetActive(false);
			doubleDownBtn.gameObject.SetActive(false);
		});

		doubleDownBtn.onClick.AddListener(delegate {
			doubleDown();
			if (playerPoints < 21)
            {
				Invoke("doubleDownStand", 1);
			}
		});

		bet50.onClick.AddListener(delegate
		{
			updateCurrentBet50();
			moneySound.GetComponent<AudioSource>().Play();
		});

		bet100.onClick.AddListener(delegate
		{
			updateCurrentBet100();
			moneySound.GetComponent<AudioSource>().Play();
		});

		bet500.onClick.AddListener(delegate
		{
			updateCurrentBet500();
			moneySound.GetComponent<AudioSource>().Play();
		});

		allIn.onClick.AddListener(delegate
		{ 
			updateCurrentBetAllIn();
			moneySound.GetComponent<AudioSource>().Play();
		});


		resetBalanceBtn.onClick.AddListener(delegate
        {
			PlayerPrefs.SetFloat("Money", 2000);
			playerMoney = PlayerPrefs.GetFloat("Money");
			checkFunds();
			textSelectingBet.gameObject.SetActive(true);
			textPlaceYourBet.gameObject.SetActive(true);
        });
    }

	private void doubleDownStand()
    {
		secondaryBtn.onClick.Invoke();
    }
	
	 private void Update() {
		textMoney.text = playerMoney.ToString();
	}

	public void startGame() {

		if (playerMoney > 0)
		{
			playerMoney -= currentBet;
			//textMoney.text = "$" + playerMoney.ToString();
			if (playerMoney < 0) {
				playerMoney += currentBet;
				//textMoney.text = "Money $" + playerMoney.ToString();

				return;
			}

			isPlaying = true;

			// Update UI accordingly
			//bet50.gameObject.SetActive(false);
			//bet100.gameObject.SetActive(false);
			//bet500.gameObject.SetActive(false);
			//allIn.gameObject.SetActive(false);
			parentChip.SetActive(false);
			textSelectingBet.gameObject.SetActive(false);
			textPlaceYourBet.gameObject.SetActive(false);
			primaryBtn.gameObject.SetActive(false);
			exitBtn.gameObject.SetActive(false);
			textBet.text = currentBet.ToString();
			resetBalanceBtn.gameObject.SetActive(false);
			betImage.gameObject.SetActive(true);
			textBet.gameObject.SetActive(true);

			// assign the playing deck with 2 deck of cards
			playingDeck = new Deck(cardPrefabs, 4);
			// draw 2 cards for player
			Invoke("playerDrawCard", 1);
			Invoke("playerDrawCard", 2);
			updatePlayerPoints();
			// draw 2 cards for dealer
			dealerDrawCard();
			dealerDrawCard();
			//Invoke("dealerDrawCard", 2);
			updateDealerPoints(true);
			Invoke("beginDealing", 3f);

			Invoke("checkIfPlayerBlackjack", 3f);
			Invoke("checkIfDoubleDown", 3f);
		}
	}

	private void beginDealing()
    {
		primaryBtn.gameObject.SetActive(true);
		primaryBtn.gameObject.transform.localPosition = new Vector3(600, 0, 0);
		primaryBtn.GetComponentInChildren<TextMeshProUGUI>().text = "HIT";
		secondaryBtn.gameObject.SetActive(true);
	}

	private void checkIfPlayerBlackjack()
	{
		if (playerPoints == 21)
		{
			playerBlackjack();
		}
	}

	public void endGame() {
		bet50.gameObject.SetActive(false);
		bet100.gameObject.SetActive(false);
		bet500.gameObject.SetActive(false);
		allIn.gameObject.SetActive(false);
		textPlaceYourBet.text = "Place Your Bet";

		resetImg.gameObject.SetActive(true);
		//StartCoroutine(MoveToTargetReset());
		resetImgBtn.GetComponent<Button>().onClick.AddListener(delegate {
			//resetGame();
			//StartCoroutine(MoveToTargetResetDone());
			resetImg.GetComponent<Animator>().Play("BaseLayer.ResetButtonExit", -1, 0f);
			textMoney.GetComponent<Animator>().enabled = false;
			Invoke("resetGame", 1f);
		});
	}

	private void checkFunds()
    {
		if (int.Parse(bet50text.text) > playerMoney)
        {
			bet50.gameObject.SetActive(false);
			greenChip.SetActive(false);
			textSelectingBet.gameObject.SetActive(false);
			textPlaceYourBet.gameObject.SetActive(false);
		}
        else
        {
			bet50.gameObject.SetActive(true);
			greenChip.SetActive(true);
		}
		if (int.Parse(bet100text.text) > playerMoney)
		{
			bet100.gameObject.SetActive(false);
			redChip.SetActive(false);
			
		}
		else
		{
			bet100.gameObject.SetActive(true);
			redChip.SetActive(true);
		}
		if (int.Parse(bet500text.text) > playerMoney)
		{
			bet500.gameObject.SetActive(false);
			blueChip.SetActive(false);
			
		}
		else
		{
			bet500.gameObject.SetActive(true);
			blueChip.SetActive(true);
		}
		if (int.Parse(allIntext.text) > playerMoney)
		{
			allIn.gameObject.SetActive(false);
			blackChip.SetActive(false);
			
		}
		else
		{
			allIn.gameObject.SetActive(true);
			blackChip.SetActive(true);
		}
	}

	public void dealerDrawCard() {
		Card drawnCard = playingDeck.DrawRandomCard();
		GameObject prefab;
		dealerCards.Add(drawnCard);
		if (dealerCardPointer <= 0) {
			prefab = backCardPrefab;
		} else {
			prefab = drawnCard.Prefab;
		}

		Instantiate(prefab, dealerCardPosition[dealerCardPointer++].transform);
		updateDealerPoints(false);

	}

	public void playerDrawCard() {
		Card drawnCard = playingDeck.DrawRandomCard();
		playerCards.Add(drawnCard);
		Instantiate(drawnCard.Prefab, playerCardPosition[playerCardPointer++].transform);
		updatePlayerPoints();
		if (playerPoints > 21)
		playerBusted();
		else if (playerPoints == 21 && playerCards.Count > 2)
        {
			primaryBtn.gameObject.SetActive(false);
			secondaryBtn.gameObject.SetActive(false);
			Invoke("doubleDownStand", 1);
        }
	}

	private void playerEndTurn() {
		revealDealersDownFacingCard();
		updateDealerPoints(false);
		StartCoroutine(dealingCards());
    }

	private void revealDealersDownFacingCard() {
		// reveal the dealer's down-facing card
		Destroy(dealerCardPosition[0].transform.GetChild(0).gameObject);
		dealerCardPosition[0].transform.localPosition = new Vector3(dealerCardPosition[0].transform.localPosition.x, .1f, dealerCardPosition[0].transform.localPosition.z);
		Instantiate(dealerCards[0].Prefab, dealerCardPosition[0].transform);
	}

	private void updatePlayerPoints() {
		playerPoints = 0;
		foreach(Card c in playerCards) {
			playerPoints += c.Point;
		}

		// transform ace to 1 if there is any
		if (playerPoints > 21)
		{
			playerPoints = 0;
			foreach(Card c in playerCards) {
				if (c.Point == 11)
					playerPoints += 1;
				else
					playerPoints += c.Point;
			}
		}

		textPlayerPoints.text = playerPoints.ToString();
	}

    private void updateDealerPoints(bool hideFirstCard)
    {
        actualDealerPoints = 0;
        foreach (Card c in dealerCards)
        {
            actualDealerPoints += c.Point;
        }

        // transform ace to 1 if there is any
        if (actualDealerPoints > 21)
        {
            actualDealerPoints = 0;
            foreach (Card c in dealerCards)
            {
                if (c.Point == 11)
                    actualDealerPoints += 1;
                else
                    actualDealerPoints += c.Point;
            }
        }

        if (hideFirstCard)
            displayDealerPoints = dealerCards[1].Point;
        else
            displayDealerPoints = actualDealerPoints;
        textDealerPoints.text = displayDealerPoints.ToString();
    }

 


	private void updateCurrentBet50() {
		currentBet = int.Parse(bet50text.text);
		textSelectingBet.text = currentBet.ToString();
		textSelectingBet.GetComponent<Animator>().enabled = true;
		textSelectingBet.GetComponent<Animator>().Play("BaseLayer.ResetButtonPop", -1, 0f);
	}

	private void updateCurrentBet100()
	{
		currentBet = int.Parse(bet100text.text);
		textSelectingBet.text = currentBet.ToString();
		textSelectingBet.GetComponent<Animator>().enabled = true;
		textSelectingBet.GetComponent<Animator>().Play("BaseLayer.ResetButtonPop", -1, 0f);
	}

	private void updateCurrentBet500()
	{
		currentBet = int.Parse(bet500text.text);
		textSelectingBet.text = currentBet.ToString();
		textSelectingBet.GetComponent<Animator>().enabled = true;
		textSelectingBet.GetComponent<Animator>().Play("BaseLayer.ResetButtonPop", -1, 0f);
	}

	private void updateCurrentBetAllIn()
	{
		currentBet = int.Parse(allIntext.text);
		textSelectingBet.text = currentBet.ToString();
		textSelectingBet.GetComponent<Animator>().enabled = true;
		textSelectingBet.GetComponent<Animator>().Play("BaseLayer.ResetButtonPop", -1, 0f);
	}

	private void playerBusted() {
		dealerWin(true);
		secondaryBtn.gameObject.SetActive(false);
		primaryBtn.gameObject.SetActive(false);
		exitBtn.gameObject.SetActive(false);
	}

	private void dealerBusted() {
		playerWin(true);
	}

	private void checkIfDoubleDown()
    {
		if (playerPoints < 20 && currentBet <= playerMoney)
        {
			doubleDownBtn.gameObject.SetActive(true);
        }
        else
        {
			doubleDownBtn.gameObject.SetActive(false);
        }
    }

	private void doubleDown()
    {
		currentBet = currentBet * 2;
		playerMoney -= currentBet / 2;

		textBet.text = (currentBet).ToString();
		playerDrawCard();
		primaryBtn.gameObject.SetActive(false);
		doubleDownBtn.gameObject.SetActive(false);
	}

	private void coinsFalling()
    {
		coinParticleSystem.SetActive(true);
    }
	private void coinsFallingStop()
    {
		coinParticleSystem.SetActive(false);
    }

	private void playerBlackjack() {
		StartCoroutine(ScoreUpdaterBlackjack());
		//StartCoroutine(MoveToTarget());
		textMoney.GetComponent<Animator>().enabled = true;
		textMoney.GetComponent<Animator>().Play("BaseLayer.ResetButtonPop", -1, 0);
		secondaryBtn.gameObject.SetActive(false);
		primaryBtn.gameObject.SetActive(false);
		exitBtn.gameObject.SetActive(false);
		coinsFalling();
		Invoke("endGame", 1);
		Invoke("playerBlackjackText", 1);
		}

	private void playerBlackjackText()
	{
		textWinner.text = "Blackjack!\n$" + (currentBet * 2.5f).ToString();
		twentyone.Play();
	}

	private void playerWin(bool winByBust) {
		StartCoroutine(ScoreUpdaterWin());
		//StartCoroutine(MoveToTarget());
		textMoney.GetComponent<Animator>().enabled = true;
		textMoney.GetComponent<Animator>().Play("BaseLayer.ResetButtonPop", -1, 0);
		secondaryBtn.gameObject.SetActive(false);
		primaryBtn.gameObject.SetActive(false);
		exitBtn.gameObject.SetActive(false);
		coinsFalling();
		if (winByBust)
			Invoke("playerWinTextByBust", 1);
		else
			Invoke("playerWinTextByNonBust", 1);
		Invoke("endGame", 1);


	}

	private void playerWinTextByBust()
	{
		textWinner.text = "Dealer Busted!\nYou Win $" + (currentBet * 2).ToString();
		//textMoney.text = (currentBet * 2).ToString();
		wins.Play();
	}
	private void playerWinTextByNonBust()
	{
		textWinner.text = "You Win!\n$" + (currentBet * 2).ToString();
		wins.Play();
	}

	private void dealerWin(bool winByBust) {
		if (winByBust)
			Invoke("dealerWinTextByBust", 1);
		else
			Invoke("dealerWinTextByNonBust", 1);
		Invoke("endGame", 1);
	}
	private void dealerWinTextByBust()
	{
		textWinner.text = "Player Busted\nDealer Wins!";
		loses.Play();
	}
	private void dealerWinTextByNonBust()
	{
		textWinner.text = "Dealer Beats " + playerPoints.ToString() +"\nDealer Wins!";
		loses.Play();
	}

	private void gameDraw() {
		StartCoroutine(ScoreUpdaterDraw());
		//StartCoroutine(MoveToTarget());
		textMoney.GetComponent<Animator>().enabled = true;
		textMoney.GetComponent<Animator>().Play("BaseLayer.ResetButtonPop", -1, 0);
		secondaryBtn.gameObject.SetActive(false);
		primaryBtn.gameObject.SetActive(false);
		exitBtn.gameObject.SetActive(false);
		Invoke("endGame", 1);
		Invoke("gameDrawText", 1);
	}
	private void gameDrawText()
    {
		textWinner.text = "Push";
	}

	private IEnumerator ScoreUpdaterBlackjack()
	{
		float currentPlayerMoney = playerMoney;
		
		
		while (true)
		{
			if (playerMoney < currentPlayerMoney + (currentBet * 2.5f) && currentBet == 1000)
			{
				playerMoney += 50;
				
			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2.5f) && currentBet == 2000)
			{
				playerMoney += 50;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2.5f) && currentBet == 500)
			{
				playerMoney += 50;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2.5f) && currentBet == 200)
			{
				playerMoney += 25;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2.5f) && currentBet == 100)
			{
				playerMoney += 25;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2.5f) && currentBet == 50)
			{
				playerMoney += 5;

			}
			yield return new WaitForSeconds(0);// I used .2 secs but you can update it as fast as you want
		}
		
	}
	private IEnumerator ScoreUpdaterWin()
	{
		float currentPlayerMoney = playerMoney;
		

		while (true)
		{
			if (playerMoney < currentPlayerMoney + (currentBet * 2f) && currentBet == 1000)
			{
				playerMoney += 50;
				
			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2f) && currentBet == 2000)
			{
				playerMoney += 50;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2f) && currentBet == 500)
			{
				playerMoney += 50;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2f) && currentBet == 200)
			{
				playerMoney += 25;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2f) && currentBet == 100)
			{
				playerMoney += 25;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 2f) && currentBet == 50)
			{
				playerMoney += 5;

			}
			yield return new WaitForSeconds(0);// I used .2 secs but you can update it as fast as you want
		}
	}
	private IEnumerator ScoreUpdaterDraw()
	{
		float currentPlayerMoney = playerMoney;

		while (true)
		{
			if (playerMoney < currentPlayerMoney + (currentBet * 1) && currentBet == 1000)
			{
				playerMoney += 50;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 1) && currentBet == 2000)
			{
				playerMoney += 50;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 1f) && currentBet == 500)
			{
				playerMoney += 50;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 1) && currentBet == 200)
			{
				playerMoney += 25;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 1f) && currentBet == 100)
			{
				playerMoney += 25;

			}
			else if (playerMoney < currentPlayerMoney + (currentBet * 1f) && currentBet == 50)
			{
				playerMoney += 5;

			}
			yield return new WaitForSeconds(0f);// I used .2 secs but you can update it as fast as you want
		}
		
	}

	//IEnumerator MoveToTarget()
	//{
	//	//yield return new WaitForSeconds(.1f);
	//	float speed = 150f * Time.deltaTime;
	//	Vector3 newPos = new Vector3(-425, -100, 0);
	//	Vector3 oldPos = new Vector3(-450, -100, 0);
	//	RectTransform rectTransform = textMoney.gameObject.GetComponent<RectTransform>();
	//	while (rectTransform.localPosition != newPos)
	//	{
	//		rectTransform.localPosition = Vector3.MoveTowards(
	//			rectTransform.localPosition, newPos, speed);
	//		textMoney.fontSize += 3f;
	//		yield return null;
	//	}
	//	while (rectTransform.localPosition != oldPos)
	//	{
	//		rectTransform.localPosition = Vector3.MoveTowards(
	//			rectTransform.localPosition, oldPos, speed);
	//		textMoney.fontSize -= 3f;
	//		yield return null;
	//	}
	//}


	private void resetGame() {
		isPlaying = false;
		
		// reset points
		playerPoints = 0;
		actualDealerPoints = 0;
		playerCardPointer = 0;
		dealerCardPointer = 0;
		dealerCardPosition[0].transform.localPosition = new Vector3(dealerCardPosition[0].transform.localPosition.x, 0f, dealerCardPosition[0].transform.localPosition.z);

		// reset cards
		playingDeck = new Deck(cardPrefabs, 4);
		playerCards = new List<Card>();
		dealerCards = new List<Card>();
		StopAllCoroutines();
		textBet.text = "";
		textBet.gameObject.SetActive(false);


		int currentSelectBet = int.Parse(textSelectingBet.text);
		// reset UI
		if ((currentBet / 2) == currentSelectBet)
        {
			currentBet = currentBet / 2;
			textBet.text = "";

		}
		primaryBtn.gameObject.SetActive(true);
		primaryBtn.GetComponentInChildren<TextMeshProUGUI>().text = "DEAL";
		primaryBtn.gameObject.transform.localPosition = new Vector3(-25, -300, 0);
		secondaryBtn.gameObject.SetActive(false);
		coinsFallingStop();
		exitBtn.gameObject.SetActive(true);
		betImage.gameObject.SetActive(false);
		parentChip.SetActive(true);
		//bet50.gameObject.SetActive(true);
		//bet100.gameObject.SetActive(true);
		//bet500.gameObject.SetActive(true);
		//allIn.gameObject.SetActive(true);
		textSelectingBet.gameObject.SetActive(true);
		textSelectingBet.GetComponent<Animator>().enabled = false;
		textSelectingBet.text = currentBet.ToString();
		textPlaceYourBet.gameObject.SetActive(true);
		textPlayerPoints.text = "";
		textDealerPoints.text = "";
		textWinner.text = "";
		resetImg.gameObject.transform.localPosition = new Vector3(0, 0, 0);
		resetImg.gameObject.SetActive(false);
		resetBalanceBtn.gameObject.SetActive(true);
		PlayerPrefs.SetFloat("Money", float.Parse(textMoney.text));
		playerMoney = PlayerPrefs.GetFloat("Money");
		checkFunds();

		// clear cards on table
		clearCards();
	}

	private void clearCards() {
		foreach(GameObject g in playerCardPosition)
		{
			if (g.transform.childCount > 0)
				for (int i = 0; i < g.transform.childCount; i++)
				{
					Destroy(g.transform.GetChild(i).gameObject);
				}
		}
		foreach(GameObject g in dealerCardPosition)
		{
			if (g.transform.childCount > 0)
				for (int i = 0; i < g.transform.childCount; i++)
				{
					Destroy(g.transform.GetChild(i).gameObject);
				}
		}
	}


	IEnumerator dealingCards()
    {
		yield return new WaitForSeconds(1f);
		while (actualDealerPoints < 17)
		{
			dealerDrawCard();
			yield return new WaitForSeconds(1f);
		}
		updateDealerPoints(false);
		if (actualDealerPoints > 21)
			dealerBusted();
		else if (actualDealerPoints > playerPoints)
			dealerWin(false);
		else if (actualDealerPoints == playerPoints)
			gameDraw();
		else
			playerWin(false);

	}
}
