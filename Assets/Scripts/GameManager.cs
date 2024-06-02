using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] cardPrefabs, playerCardPosition, playerCardPositionSplit, dealerCardPosition;
    [SerializeField]
    private GameObject backCardPrefab;
    [SerializeField]
    private Button primaryBtn, secondaryBtn, resetBalanceBtn, splitBtn, redoubleBtn;
    [SerializeField]
    private Slider betSlider;
    [SerializeField]
    private Text textMoney, textBet, textPlayerPoints, textplayerPointsWithSplit, textDealerPoints, textPlaceYourBet, textSelectingBet, textWinner;
    [SerializeField]
    private Image resetImgBtn;

    private List<Card> playerCards;
    private List<Card> playerCardsSplit;
    private List<Card> dealerCards;
    private bool isPlaying, isFirstHand, haveSplit;
    private int playerPoints;
    private int playerPointsWithSplit;
    private int actualDealerPoints, displayDealerPoints;
    private int playerMoney;
    private int currentBet;
    private int playerCardPointer, playerCardPointerSplit = 1, dealerCardPointer;

    private Deck playingDeck;

    private void Start()
    {
        playerMoney = 1000;
        currentBet = 50;
        resetGame();

        primaryBtn.onClick.AddListener(delegate
        {
            if (isPlaying)
            {
                if (isFirstHand)
                {
                    playerDrawCard();
                }
                else
                {
                    playerDrawCardSplit();
                }
            }
            else
            {
                startGame();
            }
        });

        secondaryBtn.onClick.AddListener(delegate
        {
            playerEndTurn();
        });

        redoubleBtn.onClick.AddListener(delegate
        {
            DoubleDown();
        });

        splitBtn.onClick.AddListener(delegate
        {
            Split();
        });

        betSlider.onValueChanged.AddListener(delegate
        {
            updateCurrentBet();
        });

        resetBalanceBtn.onClick.AddListener(delegate
        {
            playerMoney = 1000;
            betSlider.maxValue = playerMoney;
        });
    }

    private void Update()
    {
        textMoney.text = "Баланс: $" + playerMoney.ToString();
    }

    public void startGame()
    {
        if (playerMoney > 0)
        {
            playerMoney -= currentBet;
            if (playerMoney < 0)
            {
                playerMoney += currentBet;
                betSlider.maxValue = playerMoney;
                return;
            }

            isPlaying = true;

            // обновим UI
            betSlider.gameObject.SetActive(false);
            textSelectingBet.gameObject.SetActive(false);
            textPlaceYourBet.gameObject.SetActive(false);
            primaryBtn.GetComponentInChildren<Text>().text = "Ещё карту";
            secondaryBtn.gameObject.SetActive(true);
            redoubleBtn.gameObject.SetActive(true);
            splitBtn.gameObject.SetActive(true);
            textBet.text = "Cтавка: $" + currentBet.ToString();
            resetBalanceBtn.gameObject.SetActive(false);
            isFirstHand = true;
            haveSplit = false;

            // assign the playing deck with 2 deck of cards
            playingDeck = new Deck(cardPrefabs, 2);
            // draw 2 cards for player
            playerDrawCard();
            playerDrawCard();
            updatePlayerPoints();
            // draw 2 cards for dealer
            dealerDrawCard();
            dealerDrawCard();
            updateDealerPoints(true);

            checkIfPlayerBlackjack();
        }
    }

    private void checkIfPlayerBlackjack()
    {
        if (playerPoints == 21)
        {
            playerBlackjack();
        }
    }

    public void endGame()
    {
        primaryBtn.gameObject.SetActive(false);
        secondaryBtn.gameObject.SetActive(false);
        betSlider.gameObject.SetActive(false);
        redoubleBtn.gameObject.SetActive(false);
        splitBtn.gameObject.SetActive(false);
        textPlaceYourBet.text = "";
        textSelectingBet.text = "";

        resetImgBtn.gameObject.SetActive(true);
        resetImgBtn.GetComponent<Button>().onClick.AddListener(delegate
        {
            resetGame();
        });
    }

    public void dealerDrawCard()
    {
        Card drawnCard = playingDeck.DrawRandomCard();
        GameObject prefab;
        dealerCards.Add(drawnCard);
        if (dealerCardPointer <= 0)
        {
            prefab = backCardPrefab;
        }
        else
        {
            prefab = drawnCard.Prefab;
        }
        Instantiate(prefab, dealerCardPosition[dealerCardPointer++].transform);
        updateDealerPoints(false);
    }

    public void playerDrawCard()
    {
        Card drawnCard = playingDeck.DrawRandomCard();
        playerCards.Add(drawnCard);
        Instantiate(drawnCard.Prefab, playerCardPosition[playerCardPointer++].transform);
        updatePlayerPoints();
        if (playerPoints > 21)
        {
            if (!haveSplit)
            {
                playerBusted();
            }
            else
            {
                //isFirstHand = false;
            }
        }
    }

    private void playerEndTurn()
    {
        if (isFirstHand)
        {
            isFirstHand = false;
        }
        else
        {

            if (!haveSplit)
            {
                endOneHand();
            }
            else
            {
                endTwoHand();
            }
        }
    }

    public void endOneHand()
    {
        revealDealersDownFacingCard();
        // dealer start drawing
        while (actualDealerPoints < 17 && actualDealerPoints < playerPoints)
        {
            dealerDrawCard();
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

    public void endTwoHand()
    {
        revealDealersDownFacingCard();
        // dealer start drawing
        while (actualDealerPoints < 17 && actualDealerPoints < playerPoints)
        {
            dealerDrawCard();
        }
        updateDealerPoints(false);
        actualDealerPoints = actualDealerPoints > 21 ? 0 : actualDealerPoints;
        playerPoints = playerPoints > 21 ? 0 : playerPoints;
        playerPointsWithSplit = playerPointsWithSplit > 21 ? 0 : playerPointsWithSplit;
        if (actualDealerPoints > playerPoints && actualDealerPoints > playerPointsWithSplit)
        {
            textWinner.text = "У Игрока перебор в обоих руках\nДилер выиграл !!!";
            endGame();
        }
        else if (actualDealerPoints < playerPoints && actualDealerPoints < playerPointsWithSplit)
        {
            textWinner.text = "У Игрока обе руки победили!!!";
            playerMoney += currentBet * 2;
            endGame();
        }
        else if (actualDealerPoints > playerPoints && actualDealerPoints < playerPointsWithSplit)
        {
            textWinner.text = "У Игрока одна рука победила!!!";
            playerMoney += currentBet;
            endGame();
        }
        else if (actualDealerPoints < playerPoints && actualDealerPoints > playerPointsWithSplit)
        {
            textWinner.text = "У Игрока одна рука победила!!!";
            playerMoney += currentBet;
            endGame();
        }
        else if (actualDealerPoints == playerPoints || actualDealerPoints == playerPointsWithSplit)
        {
            textWinner.text = "Ничья!!!";
            playerMoney += currentBet;
            endGame();
        }
        else
        {
            //Debug.Log("p = " + playerPoints);
            //Debug.Log("ps = " + playerPointsWithSplit);
        }
    }

    private void revealDealersDownFacingCard()
    {
        // reveal the dealer's down-facing card
        Destroy(dealerCardPosition[0].transform.GetChild(0).gameObject);
        Instantiate(dealerCards[0].Prefab, dealerCardPosition[0].transform);
    }

    private void updatePlayerPoints()
    {
        playerPoints = 0;
        foreach (Card c in playerCards)
        {
            playerPoints += c.Point;
        }

        // transform ace to 1 if there is any
        if (playerPoints > 21)
        {
            playerPoints = 0;
            foreach (Card c in playerCards)
            {
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

    private void updateCurrentBet()
    {
        currentBet = (int)betSlider.value;
        textSelectingBet.text = "$" + currentBet.ToString();
    }

    private void playerBusted()
    {
        dealerWin(true);
    }

    private void dealerBusted()
    {
        playerWin(true);
    }

    private void playerBlackjack()
    {
        textWinner.text = "Blackjack !!!";
        playerMoney += Convert.ToInt32(currentBet * 2.5);
        endGame();
    }

    private void playerWin(bool winByBust)
    {
        if (winByBust)
            textWinner.text = "У Дилера перебор\nИгрок выиграл !!!";
        else
            textWinner.text = "Игрок выиграл !!!";
        playerMoney += currentBet * 2;
        endGame();
    }

    private void dealerWin(bool winByBust)
    {
        if (winByBust)
            textWinner.text = "У Игрока перебор\nДилер выиграл !!!";
        else
            textWinner.text = "Дилер выиграл !!!";
        endGame();
    }

    private void gameDraw()
    {
        textWinner.text = "Ничья";
        playerMoney += currentBet;
        endGame();
    }

    private void resetGame()
    {
        isPlaying = false;

        // reset points
        playerPoints = 0;
        actualDealerPoints = 0;
        playerCardPointer = 0;
        playerCardPointerSplit = 1;
        dealerCardPointer = 0;

        // reset cards
        playingDeck = new Deck(cardPrefabs, 2);
        playerCards = new List<Card>();
        playerCardsSplit = new List<Card>();
        dealerCards = new List<Card>();

        // reset UI
        primaryBtn.gameObject.SetActive(true);
        primaryBtn.GetComponentInChildren<Text>().text = "Ставка";
        secondaryBtn.gameObject.SetActive(false);
        redoubleBtn.gameObject.SetActive(false);
        splitBtn.gameObject.SetActive(false);
        betSlider.gameObject.SetActive(true);
        betSlider.maxValue = playerMoney;
        textSelectingBet.gameObject.SetActive(true);
        textSelectingBet.text = "$" + currentBet.ToString();
        textPlaceYourBet.gameObject.SetActive(true);
        textPlayerPoints.text = "";
        textDealerPoints.text = "";
        textBet.text = "";
        textWinner.text = "";
        resetImgBtn.gameObject.SetActive(false);
        resetBalanceBtn.gameObject.SetActive(true);
        textplayerPointsWithSplit.gameObject.SetActive(false);
        textplayerPointsWithSplit.gameObject.SetActive(false);

        // clear cards on table
        clearCards();
    }

    private void clearCards()
    {
        foreach (GameObject g in playerCardPosition)
        {
            if (g.transform.childCount > 0)
                for (int i = 0; i < g.transform.childCount; i++)
                {
                    Destroy(g.transform.GetChild(i).gameObject);
                }
        }
        foreach (GameObject g in playerCardPositionSplit)
        {
            if (g.transform.childCount > 0)
                for (int i = 0; i < g.transform.childCount; i++)
                {
                    Destroy(g.transform.GetChild(i).gameObject);
                }
        }
        foreach (GameObject g in dealerCardPosition)
        {
            if (g.transform.childCount > 0)
                for (int i = 0; i < g.transform.childCount; i++)
                {
                    Destroy(g.transform.GetChild(i).gameObject);
                }
        }
    }
    private void clearCardsPlayer()
    {
        foreach (GameObject g in playerCardPosition)
        {
            if (g.transform.childCount > 0)
                for (int i = 0; i < g.transform.childCount; i++)
                {
                    Destroy(g.transform.GetChild(i).gameObject);
                }
        }
    }
    private void Split()
    {
        // Check if the player can split the cards (only if their values are equal)
        if (playerCards.Count == 2 && playerCards[0].Point == playerCards[1].Point && playerMoney >= currentBet)
        {
            textplayerPointsWithSplit.gameObject.SetActive(true);
            // Subtract the bet from the player's balance
            playerMoney -= currentBet;

            // Clear the old cards from the table
            clearCardsPlayer();

            // Move the second card to the new hand
            playerCardsSplit.Add(playerCards[1]);
            playerCards.RemoveAt(1);

            // Instantiate the cards
            Instantiate(playerCards[0].Prefab, playerCardPosition[0].transform);
            Instantiate(playerCardsSplit[0].Prefab, playerCardPositionSplit[0].transform);
            playerCardPointer--;
            playerDrawCard();
            playerDrawCardSplit();

            // Update the player's points
            updatePlayerPoints();
            updatePlayerPointsAfterSplit();

            // Create a new bet for the new hand
            currentBet *= 2;
            textBet.text = "Cтавка: $" + currentBet.ToString();

            // Disable the split button
            splitBtn.gameObject.SetActive(false);
            redoubleBtn.gameObject.SetActive(false);
            haveSplit = true;

        }

    }

    public void playerDrawCardSplit()
    {
        Card drawnCard = playingDeck.DrawRandomCard();
        playerCardsSplit.Add(drawnCard);
        Instantiate(drawnCard.Prefab, playerCardPositionSplit[playerCardPointerSplit++].transform);
        updatePlayerPointsAfterSplit();
        if (playerPointsWithSplit > 21)
        {
            endTwoHand();
        }

    }

    private void updatePlayerPointsAfterSplit()
    {
        playerPointsWithSplit = 0;
        foreach (Card c in playerCardsSplit)
        {
            playerPointsWithSplit += c.Point;
        }

        // преобразовать туз в 1, если он есть
        if (playerPointsWithSplit > 21)
        {
            playerPointsWithSplit = 0;
            foreach (Card c in playerCardsSplit)
            {
                if (c.Point == 11)
                    playerPointsWithSplit += 1;
                else
                    playerPointsWithSplit += c.Point;
            }
        }

        textplayerPointsWithSplit.text = playerPointsWithSplit.ToString();
    }

    private void DoubleDown()
    {
        // Проверяем, может ли игрок удвоить ставку (только если у него две карты)
        if (playerCards.Count == 2 && playerMoney >= currentBet)
        {
            // Удваиваем ставку
            playerMoney -= currentBet;
            currentBet *= 2;
            textBet.text = "Cтавка: $" + currentBet.ToString();

            // Добавляем одну карту
            playerDrawCard();

            // Обновляем очки игрока
            updatePlayerPoints();

            // Проверяем, не перебор ли у игрока
            if (playerPoints > 21)
            {
                playerBusted();
            }
            else
            {
                // Дилер начинает свой ход
                playerEndTurn();
                playerEndTurn();
            }
        }
    }

}
