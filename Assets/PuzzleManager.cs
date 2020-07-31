using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamejam;

public class PuzzleManager : MonoBehaviour
{   // 15  20  30     28  26  24  22  20  18  16  14  12  10
    [SerializeField] public GameObject CardPrefab;
    [SerializeField] public GameObject GameOverPanel;
    [SerializeField] public GameObject PausePanel;
    [SerializeField] public GameObject WinPanel;
    [SerializeField] public bool GamePaused = false;
    [SerializeField] public bool GameOver = false;
    [SerializeField] public bool ActingBreak = false;
    [SerializeField] public Camera mainCam;
    [SerializeField] public GameObject TopCardOfDeck;
    CardFace _twoBackCardFaces = CardFace.NEUTRAL;
    CardFace _previousCardFace = CardFace.NEUTRAL;

    public float timeLeft = 0f;
    GameTop gametop;
    Card[,] cardGrid;
    LastPlayedUI lastPlayedUI;
    int _boardSize = 2;
    [SerializeField] public UI_VictoryScoreController uiVictoryScoreController;
    [SerializeField] public UI_GameOverScoreController uiGameOverScoreController;

    void FixedUpdate() {
        if (!GamePaused && !GameOver && !ActingBreak) {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f) {
                timeLeft = 0f;
                LoseBoard();
            }
        }
    }

    void Update() {
        CheckHotkeyClicks();
    }

    void Start() {
         gametop = Object.FindObjectOfType<GameTop>();
         lastPlayedUI = Object.FindObjectOfType<LastPlayedUI>();
         StartGame();
    }

    public bool ReadyToChoose() {
        return !GamePaused && !GameOver && !ActingBreak;
    }

    public void StartGame() {
        if (gametop.BoardSize == 3) {
            gametop.PlayMusic("YouGetTheBlues");
        } 
        if (gametop.BoardSize == 4) {
            gametop.PlayMusic("SpeedyDelta");
        } 
        if (gametop.BoardSize == 5) {
            gametop.PlayMusic("Glueworm");
        }
        GamePaused = false;
        _boardSize = gametop.BoardSize;
        ResetGameTimer();
        PlaceCardsToGrid();
        AdjustCameraPosition();
        Object.FindObjectOfType<UI_SlidingStartText>().GoGoStartText("READY", "GO!!");
    }

    public void RequestPause() {
        GamePaused = true;
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RequestUnpause() {
        GamePaused = false;
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
        
    }

    private void ResetGameTimer() {
        timeLeft = gametop.SecondsPerBoard;
    }

    private void grayOutCardFace(Card card1) {
        card1.AssignFace(card1.CurrentFace, true);
    }

    public void CardRevealed(Card revealedCard) {
        GamePaused = true;

        int visibleCardCount = QueryableSet().FindAll(o => o.visible).Count;
        if (visibleCardCount == 1 && revealedCard.CurrentFace == CardFace.BOMB) {
            HotSwapTheBomb(revealedCard);
        }

        lastPlayedUI.SetIcon(revealedCard.CurrentFace);
        _twoBackCardFaces = _previousCardFace;
        _previousCardFace = revealedCard.CurrentFace;

        switch(revealedCard.CurrentFace) {
            case CardFace.SHUFFLE :
                lastPlayedUI.SetName("Shuffle", CardType.BAD);
                lastPlayedUI.SetDescription("Shuffle all face down cards");
                if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.BOMB :
                lastPlayedUI.SetName("Bomb", CardType.BAD);
                lastPlayedUI.SetDescription("Boom!");
                if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.ADDTIME :
                lastPlayedUI.SetName("Add Time", CardType.GOOD);
                lastPlayedUI.SetDescription("Gain a little extra time");
                if (_twoBackCardFaces == CardFace.NULLBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.FREEFLIP :
                lastPlayedUI.SetName("Freebie", CardType.GOOD);
                lastPlayedUI.SetDescription("Reveals a non-Bomb card, does NOT trigger its effect");
                if (_twoBackCardFaces == CardFace.NULLBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.HIGHLIGHTSAFE :
                lastPlayedUI.SetName("Safe Highlight", CardType.GOOD);
                lastPlayedUI.SetDescription("Highlight 1 non-Bomb card");
                if (_twoBackCardFaces == CardFace.NULLBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.HIGHLIGHTCHANCE :
                lastPlayedUI.SetName("Chance Highlight", CardType.GOOD);
                lastPlayedUI.SetDescription("Highlight 4 cards, at least 1 is a Bomb");
                if (_twoBackCardFaces == CardFace.NULLBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.LOSETIME :
                lastPlayedUI.SetName("Lose Time", CardType.BAD);
                lastPlayedUI.SetDescription("Lose a little time");
                if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.REVEAL :
                lastPlayedUI.SetName("Reveal", CardType.GOOD);
                lastPlayedUI.SetDescription("See 2 random cards");
                if (_twoBackCardFaces == CardFace.NULLBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.REPLACE :
                lastPlayedUI.SetName("Mystery Replace", CardType.BAD);
                lastPlayedUI.SetDescription("Replace a face up card with a random non-Bomb card");
                if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.NULLBUFF :
                lastPlayedUI.SetName("Null Field", CardType.BAD);
                lastPlayedUI.SetDescription("Next card will not trigger if its a PERK");
                if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.NULLDEBUFF :
                lastPlayedUI.SetName("Shield Up", CardType.GOOD);
                lastPlayedUI.SetDescription("Next card will not trigger if its a PITFALL or BOMB");
                if (_twoBackCardFaces == CardFace.NULLBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.DOUBLEBOMB :
                lastPlayedUI.SetName("Double Bomb", CardType.BAD);
                lastPlayedUI.SetDescription("Convert a random facedown card to a Bomb");
                if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
                    grayOutCardFace(revealedCard);
                }
                break;
            case CardFace.NEUTRAL :
                lastPlayedUI.SetName("Neutral Card", CardType.NEUTRAL);
                lastPlayedUI.SetDescription("No effect");
                break;
            default:
                Debug.Log("UNIDENTIFIED FACE");
                break;
        }
        GamePaused = false;
        StartCoroutine(ActingBreak_LetCardsTurn(revealedCard));
    }

    IEnumerator ActingBreak_LetCardsTurn(Card revealedCard) {
        ActingBreak = true;
        yield return new WaitForSeconds(0.8f);
        ActingBreak = false;

        int hiddenCardCount = QueryableSet().FindAll(o => !o.visible).Count;
        bool skipSecondLast = revealedCard.CurrentFace != CardFace.BOMB && hiddenCardCount == 1;

        if (!skipSecondLast) {
            switch(revealedCard.CurrentFace) {
                case CardFace.SHUFFLE :
                    yield return StartCoroutine(ActingBreak_Shuffle(revealedCard));
                    break;
                case CardFace.BOMB :
                    yield return StartCoroutine(ActingBreak_BombCard(revealedCard));
                    break;
                case CardFace.REPLACE :
                    yield return StartCoroutine(ActingBreak_ReplaceCard(revealedCard));
                    break;
                case CardFace.NULLBUFF :
                    yield return StartCoroutine(ActingBreak_NullBuff(revealedCard));
                    break;
                case CardFace.NULLDEBUFF :
                    yield return StartCoroutine(ActingBreak_NullDeBuff(revealedCard));
                    break;
                case CardFace.DOUBLEBOMB :
                    yield return StartCoroutine(ActingBreak_DoubleBomb(revealedCard));
                    break;
                case CardFace.FREEFLIP :
                    yield return StartCoroutine(ActingBreak_Freeflip(revealedCard));
                    break;
                case CardFace.HIGHLIGHTSAFE :
                    yield return StartCoroutine(ActingBreak_HighlightSafe(revealedCard));
                    break;
                case CardFace.HIGHLIGHTCHANCE :
                    yield return StartCoroutine(ActingBreak_HighlightChance(revealedCard));
                    break;
                case CardFace.REVEAL :
                    yield return StartCoroutine(ActingBreak_Reveal(revealedCard));
                    break;
                case CardFace.ADDTIME :
                    yield return StartCoroutine(ActingBreak_AddTime(revealedCard));
                    break;
                case CardFace.LOSETIME :
                    yield return StartCoroutine(ActingBreak_LoseTime(revealedCard));
                    break;
                default:
                    break;
            }
        }
        

        if (PlayerMetAWinCondition()) {
            WinBoard();
        }
    }

    IEnumerator ActingBreak_Reveal(Card revealedCard) {
        int COUNT_MAX_REVEAL_CARDS = 2;
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullBuffTriggered());
            ActingBreak = false;
            yield break;
        }
        List<Card> faceDownList = QueryableSet().FindAll(o => !o.visible);
        List<Card> victims = new List<Card>();
        for(var i=0; i < COUNT_MAX_REVEAL_CARDS; i++) {
            if (faceDownList.Count > 0) {
                int randomIndex = Random.Range(0, faceDownList.Count);
                victims.Add(faceDownList[randomIndex]);
                faceDownList.RemoveAt(randomIndex);
            }
        }

        foreach(Card victim in victims) {
            victim.ChangeVisibility(true);
        }
        gametop.PlaySound("Reveal");
        yield return new WaitForSeconds(0.4f);
        foreach(Card victim in victims) {
            victim.ChangeVisibility(false);
        }
        yield return new WaitForSeconds(0.2f);
        ActingBreak = false;
    }
    IEnumerator ActingBreak_HighlightChance(Card revealedCard) {
        int COUNT_MAX_NON_BOMB_HIGHLIGHTED_CARDS = 3;
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullBuffTriggered());
            ActingBreak = false;
            yield break;
        }

        List<Card> bombsFaceDownList = QueryableSet().FindAll(o => !o.visible && o.CurrentFace == CardFace.BOMB);
        Card bombVictim = bombsFaceDownList[Random.Range(0, bombsFaceDownList.Count)];
        List<Card> victims = new List<Card>();
        victims.Add(bombVictim);

        List<Card> otherFaceDownEligible = QueryableSet().FindAll(o => !o.visible && o != bombVictim);
        for(var i=0; i < COUNT_MAX_NON_BOMB_HIGHLIGHTED_CARDS; i++) {
            if (otherFaceDownEligible.Count > 0) {
                int randomIndex = Random.Range(0, otherFaceDownEligible.Count);
                victims.Add(otherFaceDownEligible[randomIndex]);
                otherFaceDownEligible.RemoveAt(randomIndex);
            }
        }
        foreach(Card victim in victims) {
            victim.ToggleDangerBacklight(true);
        }
        gametop.PlaySound("Highlight");
        yield return new WaitForSeconds(1.2f);

        foreach(Card victim in victims) {
            victim.ToggleDangerBacklight(false);
        }
        ActingBreak = false;
    }

    IEnumerator ActingBreak_HighlightSafe(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullBuffTriggered());
            ActingBreak = false;
            yield break;
        }

        List<Card> eligibles = QueryableSet().FindAll(o => !o.visible && o.CurrentFace != CardFace.BOMB);
        Card victim = eligibles[Random.Range(0, eligibles.Count)];
        victim.ToggleDangerBacklight(true);
        gametop.PlaySound("Highlight");
        yield return new WaitForSeconds(0.6f);

        victim.ToggleDangerBacklight(false);
        ActingBreak = false;
       
    }

    IEnumerator ActingBreak_Freeflip(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullBuffTriggered());
            ActingBreak = false;
            yield break;
        }

        List<Card> eligible = QueryableSet().FindAll(o => !o.visible && o.CurrentFace != CardFace.BOMB);
        Card victim = eligible[Random.Range(0, eligible.Count)];

        victim.ToggleDangerBacklight(true);
        grayOutCardFace(victim);
        gametop.PlaySound("Highlight");
        yield return new WaitForSeconds(.4f);

        victim.ChangeVisibility(true);
        victim.ToggleDangerBacklight(false);

        yield return new WaitForSeconds(0.4f);
        ActingBreak = false;
    }

    IEnumerator ActingBreak_LoseTime(Card revealedCard) {
        float BUFFER_TIME_ALLOWED_FOR_USER = 1f;
        float SECONDS_PENALTY = 2f;
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullDebuffTriggered());
            ActingBreak = false;
            yield break;
        }
        if (timeLeft - SECONDS_PENALTY < BUFFER_TIME_ALLOWED_FOR_USER) {
            timeLeft = BUFFER_TIME_ALLOWED_FOR_USER;
        } else if (timeLeft - SECONDS_PENALTY >= BUFFER_TIME_ALLOWED_FOR_USER) {
            timeLeft = timeLeft - SECONDS_PENALTY;
        }
        gametop.PlaySound("Lose_Time");

        yield return null;

        ActingBreak = false;
    }

    IEnumerator ActingBreak_AddTime(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullBuffTriggered());
            ActingBreak = false;
            yield break;
        }
        gametop.PlaySound("Gain_Time");
        timeLeft = Mathf.Clamp(timeLeft + 5f, 0f, gametop.SecondsPerBoard);
        yield return null;

        ActingBreak = false;
    }

    IEnumerator ActingBreak_DoubleBomb(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullDebuffTriggered());
            ActingBreak = false;
            yield break;
        }

        List<Card> allFacedownCards = QueryableSet().FindAll(o => !o.visible);
        List<Card> facedownNonBombs = QueryableSet().FindAll(o => o.CurrentFace != CardFace.BOMB && !o.visible);

        foreach(Card card in allFacedownCards) {
            card.ToggleDangerBacklight(true);
        }
        gametop.PlaySound("Ticking2");
        yield return new WaitForSeconds(1f);

        Card victimCard = facedownNonBombs[Random.Range(0, facedownNonBombs.Count)];
        victimCard.AssignFace(CardFace.BOMB);

        foreach(Card card in allFacedownCards) {
            card.ToggleDangerBacklight(false);
        }

        yield return new WaitForSeconds(0.5f);

        ActingBreak = false;
    }

    IEnumerator ActingSubroutine_NullDebuffTriggered() {
        Debug.Log("NULL DEBUFF TRIGGERED");
        yield return null;
    }

    IEnumerator ActingSubroutine_NullBuffTriggered() {
        Debug.Log("NULL BUFF TRIGGERED");
        yield return null;
    }

    IEnumerator ActingBreak_NullBuff(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullDebuffTriggered());
            ActingBreak = false;
            yield break;
        }
        gametop.PlaySound("Null_Buff");
        yield return null;
        ActingBreak = false;
    }

    IEnumerator ActingBreak_NullDeBuff(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullBuffTriggered());
            ActingBreak = false;
            yield break;
        }
        gametop.PlaySound("Null_Debuff");
        yield return null;
        ActingBreak = false;
    }

    IEnumerator ActingBreak_ReplaceCard(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullDebuffTriggered());
            ActingBreak = false;
            yield break;
        }
    
        List<Card> potentialCards = QueryableSet().FindAll(o => o.visible && o.CurrentFace != CardFace.BOMB);

        int victimId = Random.Range(0, potentialCards.Count);
        Card victim = potentialCards[victimId];

        if (!victim.visible) {
            victim.ChangeVisibility(true);
            yield return new WaitForSeconds(1.5f);
        }

        victim.ToggleDangerBacklight(true);
        victim.ChangeVisibility(false);
        gametop.PlaySound("Replace");
        yield return new WaitForSeconds(1f);

        Vector3 startingPosition = victim.transform.position;
        DiscardCard(victim);

        yield return new WaitForSeconds(0.4f);

        victim.ToggleDangerBacklight(false);
        Vector2 swapSpot = FindGridPositionOfCard(victim);
        Card newlyDrawnCard = DrawCardToPosition(GetPosForGridCard((int) swapSpot.x, (int) swapSpot.y), randomCardReplacement());
        cardGrid[(int) swapSpot.x, (int) swapSpot.y] = newlyDrawnCard;

        yield return new WaitForSeconds(0.5f);
        ActingBreak = false;
    }

    IEnumerator ActingBreak_Shuffle(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullDebuffTriggered());
            ActingBreak = false;
            yield break;
        }

        List<Card> facedownCards = QueryableSet().FindAll(o => !o.visible);

        foreach(Card c in facedownCards) {
            c.ToggleDangerBacklight(true);
        }
        yield return new WaitForSeconds(0.4f);
        foreach(Card c in facedownCards) {
            c.ChangeVisibility(true, true);
        }

        yield return new WaitForSeconds(1.1f);

        foreach(Card c in facedownCards) {
            c.ToggleDangerBacklight(false);
            c.SetDestination(getOffscreenPosition());
        }

        yield return new WaitForSeconds(.7f);
        List<CardFace> faceList = new List<CardFace>();
        foreach(Card c in facedownCards) {
            c.ChangeVisibility(false, true);
            faceList.Add(c.CurrentFace);
        }

        foreach(Card c in facedownCards) {
            int randomId = Random.Range(0, faceList.Count);
            CardFace faceChosen = faceList[randomId];
            faceList.RemoveAt(randomId);
            c.AssignFace(faceChosen);
        }

        yield return new WaitForSeconds(.6f);
        
        foreach(Card c in facedownCards) {
            c.ReturnToPreviousDestination();
        }

        yield return new WaitForSeconds(0.5f);
        ActingBreak = false;
    }

    IEnumerator ActingBreak_BombCard(Card revealedCard) {
        ActingBreak = true;
        if (_twoBackCardFaces == CardFace.NULLDEBUFF) {
            _previousCardFace = CardFace.NEUTRAL;
            yield return StartCoroutine(ActingSubroutine_NullDebuffTriggered());
            ActingBreak = false;
            yield break;
        }
        gametop.PlaySound("Explosion");
        LoseBoard();
        ActingBreak = false;
    }

    private void UpdateScoreBoards(
        int cleared,
        int timeRemaining,
        int boardMultiplier,
        int boardTotal,
        int starting,
        int gameTotal
    ) {
        uiVictoryScoreController.UpdateScores(
            cleared,
            timeRemaining,
            boardMultiplier,
            boardTotal,
            starting,
            gameTotal
        );

        uiGameOverScoreController.UpdateScores(
            cleared,
            gameTotal
        );
    }

    private void WinBoard() {
        StartCoroutine(ActingBreak_YouWin());
    }

    IEnumerator ActingBreak_YouWin(){
        GameOver = true;
        ActingBreak = true;
        float timeLeftAtEndOfLevel = timeLeft;
        Object.FindObjectOfType<UI_SlidingStartText>().GoGoStartText("CLEAR!", "CLEAR!");
        yield return new WaitForSeconds(0.4f);

        // flip over the bomb cards
        List<Card> bombCards = QueryableSet().FindAll(o => !o.visible && o.CurrentFace == CardFace.BOMB);
        foreach(Card c in bombCards) {
            c.ChangeVisibility(true);
        }
        
        yield return new WaitForSeconds(1.5f);
        foreach(Card c in bombCards) {
            DiscardCard(c);
            c.DANGER_SetVisibleRotation(new Vector3(0f, 180f, 180f));
        }
        yield return new WaitForSeconds(1f);

        int boardMultiplier = gametop.ConsecutiveWins + 1;
        int boardCompletionScore = boardMultiplier * 1000;
        float rawTimeBonus = (timeLeftAtEndOfLevel * 20) * boardMultiplier;
        int roundedTimeBonus = (int) rawTimeBonus;
        int boardTotal = boardCompletionScore + roundedTimeBonus;
        int gameTotal = gametop.GameScore + boardTotal;
        
        int cleared = gametop.ConsecutiveWins + 1;
        int timeRemaining = (int) timeLeftAtEndOfLevel;
        int starting = gametop.GameScore;

        UpdateScoreBoards(
            cleared,
            timeRemaining,
            boardMultiplier,
            boardTotal,
            starting,
            gameTotal
        );
        ActingBreak = false;
        WinPanel.SetActive(true);
        gametop.ReportBoardWin(gameTotal);
    }

    private void LoseBoard() {
        int cleared = gametop.ConsecutiveWins;
        int timeRemaining = 0;
        int boardMultiplier = gametop.ConsecutiveWins;
        int boardTotal = 0;
        int starting = gametop.GameScore;
        int gameTotal = gametop.GameScore;
        UpdateScoreBoards(
            cleared,
            timeRemaining,
            boardMultiplier,
            boardTotal,
            starting,
            gameTotal
        );
        GameOverPanel.SetActive(true);
        GameOver = true;
        gametop.ReportBoardLoss(gameTotal);
    }

    private bool PlayerMetAWinCondition() {
        if (GameOver) return false;

        int hiddenCardCount = QueryableSet().FindAll(o => !o.visible).Count;
        int hiddenBombCardCount = QueryableSet().FindAll(o => o.CurrentFace == CardFace.BOMB && !o.visible).Count;
    
        bool allCardsLeftAreBombs = hiddenCardCount == hiddenBombCardCount;
        bool noBombsLeft = hiddenBombCardCount == 0;

        return allCardsLeftAreBombs || noBombsLeft;
    }
    // private Card GetRandomCard() {
    //     int randomX = Random.Range(0, _boardSize);
    //     int randomY = Random.Range(0, _boardSize);

    //     return cardGrid[randomX, randomY];
    // }

    private Card RandomCard_HiddenNonBomb() {
        var eligibles = QueryableSet().FindAll(o => !o.visible && o.CurrentFace != CardFace.BOMB);
        int selectIndex = Random.Range(0, eligibles.Count);
        return eligibles[selectIndex];
    }

    private List<Card> QueryableSet() {
        List<Card> lc = new List<Card>();

        foreach(Card c in cardGrid) {
            lc.Add(c);
        }
        return lc;
    }

    private Vector2 FindGridPositionOfCard(Card card) {
        for(int x=0; x<_boardSize; x++) {
            for(int y=0; y<_boardSize; y++) {
                if (cardGrid[x, y] == card) {
                    return new Vector2(x, y);
                }
            }
        }
        return Vector2.zero;
    }

    private void HotSwapTheBomb(Card clickedCard) {
        Card swapToCard = RandomCard_HiddenNonBomb();
        clickedCard.AssignFace(swapToCard.CurrentFace);
        swapToCard.AssignFace(CardFace.BOMB);
    }

    private void AdjustCameraPosition() {
        // if (_boardSize == 2) {
        //     mainCam.transform.position = new Vector3(1.25f, 0.8f, -2.44f);
        //     mainCam.orthographicSize = 1.75f;
        // }
        if (_boardSize == 3) {
            mainCam.transform.position = new Vector3(2.15f, 1.4f, -2.44f);
            mainCam.orthographicSize = 2.4f;
        }
        if (_boardSize == 4) {
            mainCam.transform.position = new Vector3(3f, 2f, -2.44f);
            mainCam.orthographicSize = 3.1f;
        }
        if (_boardSize == 5) {
            mainCam.transform.position = new Vector3(3.9f, 2.65f, -2.44f);
            mainCam.orthographicSize = 3.75f;
        }
        if (_boardSize == 6) {
            mainCam.transform.position = new Vector3(5.15f, 3.31f, -2.44f);
            mainCam.orthographicSize = 4.5f;
        }
    }

    private Card DrawCardToPosition(Vector3 position, CardFace face) {
        GameObject go = GameObject.Instantiate(CardPrefab, TopCardOfDeck.transform.position, TopCardOfDeck.transform.rotation);
        Card thisCard = go.GetComponent<Card>();
        thisCard.SetDestination(position);
        thisCard.AssignFace(face);
        return go.GetComponent<Card>();
    }

    private void PlaceCardsToGrid() {
        cardGrid = new Card[_boardSize, _boardSize];

        List<CardFace> faces = new List<CardFace>();
        
        // if (_boardSize == 2) {
        //     faces = new List<CardFace> {
        //         CardFace.HIGHLIGHTSAFE, CardFace.NEUTRAL,
        //         CardFace.BOMB, CardFace.NULLDEBUFF
        //     };
        // }

        if (_boardSize == 3) {
            faces = new List<CardFace> {
                CardFace.SHUFFLE, CardFace.REPLACE, CardFace.NULLBUFF,
                CardFace.BOMB, CardFace.REVEAL, CardFace.NULLDEBUFF,
                CardFace.HIGHLIGHTCHANCE, CardFace.FREEFLIP, CardFace.HIGHLIGHTSAFE
            };
        }

        if (_boardSize == 4) {
            faces = new List<CardFace> {
                CardFace.SHUFFLE, CardFace.REPLACE, CardFace.NULLBUFF, CardFace.NULLDEBUFF,
                CardFace.BOMB, CardFace.REVEAL, CardFace.LOSETIME, CardFace.NULLDEBUFF,
                CardFace.FREEFLIP, CardFace.ADDTIME, CardFace.DOUBLEBOMB, CardFace.REVEAL,
                CardFace.REVEAL, CardFace.HIGHLIGHTSAFE, CardFace.NULLBUFF, CardFace.HIGHLIGHTCHANCE
            };
        }

        if (_boardSize == 5) {
            faces = new List<CardFace> {
                CardFace.SHUFFLE, CardFace.REPLACE, CardFace.NULLBUFF, CardFace.NULLDEBUFF, CardFace.HIGHLIGHTCHANCE,
                CardFace.BOMB, CardFace.REPLACE, CardFace.FREEFLIP, CardFace.NULLDEBUFF, CardFace.ADDTIME,
                CardFace.FREEFLIP, CardFace.REVEAL, CardFace.DOUBLEBOMB, CardFace.REPLACE, CardFace.HIGHLIGHTSAFE,
                CardFace.SHUFFLE, CardFace.LOSETIME, CardFace.ADDTIME, CardFace.NULLDEBUFF, CardFace.FREEFLIP,
                CardFace.REVEAL, CardFace.REPLACE, CardFace.NULLBUFF, CardFace.NULLDEBUFF, CardFace.REVEAL
            };
        }

        if (_boardSize == 6) {
            faces = new List<CardFace> {
                CardFace.HIGHLIGHTCHANCE, CardFace.SHUFFLE, CardFace.REPLACE, CardFace.NULLBUFF, CardFace.NULLDEBUFF, CardFace.REVEAL,
                CardFace.FREEFLIP, CardFace.BOMB, CardFace.REPLACE, CardFace.REVEAL, CardFace.NULLDEBUFF, CardFace.LOSETIME,
                CardFace.HIGHLIGHTSAFE, CardFace.FREEFLIP, CardFace.NULLBUFF, CardFace.DOUBLEBOMB, CardFace.REPLACE, CardFace.ADDTIME,
                CardFace.FREEFLIP, CardFace.SHUFFLE, CardFace.HIGHLIGHTSAFE, CardFace.ADDTIME, CardFace.REVEAL, CardFace.REVEAL,
                CardFace.HIGHLIGHTCHANCE, CardFace.ADDTIME, CardFace.REPLACE, CardFace.NULLBUFF, CardFace.NULLDEBUFF, CardFace.HIGHLIGHTCHANCE,
                CardFace.FREEFLIP, CardFace.SHUFFLE, CardFace.REPLACE, CardFace.ADDTIME, CardFace.NULLDEBUFF, CardFace.REVEAL
            };
        }

        StartCoroutine(DealCards(faces));
    }

    IEnumerator DealCards(List<CardFace> faces) {
        ActingBreak = true;
        gametop.PlaySound("DealCards");
        for(int x=0; x<_boardSize; x++) {
            for(int y=0; y<_boardSize; y++) {
                yield return new WaitForSeconds(.05f);
                int randomNumber = Random.Range(0, faces.Count);
                CardFace faceChosen = faces[randomNumber];
                Card thisCard = DrawCardToPosition(GetPosForGridCard(x, y), faceChosen);
                cardGrid[x, y] = thisCard;
                faces.RemoveAt(randomNumber);
            }
        }
        gametop.StopSound();
        yield return new WaitForSeconds(1.25f);
        ActingBreak = false;
    }

    private Vector3 GetPosForGridCard(int x, int y) {
        return new Vector3(1.2f * x, 1.2f * y, 0f);
    }

    private void CheckHotkeyClicks() {
         if(!GameOver && Input.GetKeyDown(KeyCode.Escape)) {
            if (GamePaused) {
                RequestUnpause();
            } else {
                RequestPause();
            }
         }
    }


    private Vector3 getOffscreenPosition() {
            if (_boardSize == 2) {
                return new Vector3(0.6f, -2.4f, 0f);
            }
            if (_boardSize == 3) {
                return new Vector3(1.3f, -2.4f, 0f);
            }
            return new Vector3(2f, -2.4f, 0f);
    }

    private void DiscardCard(Card card) {
        card.ChangeVisibility(true);
        if (_boardSize == 3) {
            card.SetDestination(new Vector3(8f, 1.75f, -0.67f));
        }
        if (_boardSize == 4) {
            card.SetDestination(new Vector3(10f, 1.75f, -0.67f));
        }
        if (_boardSize == 5) {
            card.SetDestination(new Vector3(12f, 1.75f, -0.67f));
        }
        if (_boardSize == 6) {
            card.SetDestination(new Vector3(15f, 1.75f, -0.67f));
        }
    }

    private CardFace randomCardReplacement() {
        int rando = Random.Range(0, GetPotentialReplacements().Count);
        return GetPotentialReplacements()[rando];
    }

    List<CardFace> GetPotentialReplacements() {
        List<CardFace> theList = new List<CardFace>();
        theList.AddRange(potentialReplacementCards);
        if (_boardSize > 3) {
            theList.Add(CardFace.DOUBLEBOMB);
        }
        return theList;
    }

    List<CardFace> potentialReplacementCards = new List<CardFace> {
        CardFace.NULLBUFF,
        CardFace.NULLDEBUFF,
        CardFace.SHUFFLE,
        CardFace.ADDTIME,
        CardFace.FREEFLIP,
        CardFace.HIGHLIGHTSAFE,
        CardFace.HIGHLIGHTCHANCE,
        CardFace.REVEAL,
        CardFace.NEUTRAL
    };

    public int GetCurrentBombCount() {
        return QueryableSet().FindAll(o => !o.visible && o.CurrentFace == CardFace.BOMB).Count;
    }
}
